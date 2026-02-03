using ASM_WebBanNuocUong.Models;
using ASM_WebBanNuocUong.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ASM_WebBanNuocUong.Controllers;

public class CartController : Controller
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    // Class phụ để lưu giỏ hàng tạm
    public class CartItem
    {
        public Guid MaSanPham { get; set; }
        public string TenSanPham { get; set; } = null!;
        public decimal Gia { get; set; }
        public string? HinhAnh { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => SoLuong * Gia;
        public bool IsCombo { get; set; } = false;
        public Guid? MaCombo { get; set; }
    }

    // Lấy giỏ hàng từ Session
    List<CartItem> GetCart()
    {
        var sessionCart = HttpContext.Session.GetString("GioHang");
        if (sessionCart != null)
        {
            return JsonSerializer.Deserialize<List<CartItem>>(sessionCart) ?? new List<CartItem>();
        }
        return new List<CartItem>();
    }

    // Lưu giỏ hàng vào Session
    void SaveCart(List<CartItem> cart)
    {
        var sessionCart = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString("GioHang", sessionCart);
    }

    // --- 1. THÊM SẢN PHẨM VÀO GIỎ ---
    public IActionResult AddProductToCart(Guid id)
    {
        var sanPham = _context.SanPhams.Find(id);
        if (sanPham == null) return NotFound();

        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.MaSanPham == id && !c.IsCombo);
        
        if (item != null)
        {
            item.SoLuong++;
        }
        else
        {
            cart.Add(new CartItem 
            { 
                MaSanPham = sanPham.MaSanPham, 
                TenSanPham = sanPham.TenSanPham, 
                Gia = sanPham.Gia, 
                HinhAnh = sanPham.HinhAnh, 
                SoLuong = 1,
                IsCombo = false
            });
        }
        SaveCart(cart);
        return RedirectToAction("Index");
    }

    // --- 1B. THÊM COMBO VÀO GIỎ ---
    public IActionResult AddComboToCart(Guid id)
    {
        var combo = _context.Combos.Find(id);
        if (combo == null) return NotFound();

        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.MaCombo == id && c.IsCombo);
        
        if (item != null)
        {
            item.SoLuong++;
        }
        else
        {
            cart.Add(new CartItem 
            { 
                MaCombo = combo.MaCombo,
                MaSanPham = combo.MaCombo, // Dùng tạm để lưu
                TenSanPham = combo.TenCombo, 
                Gia = combo.Gia, 
                HinhAnh = combo.HinhAnh, 
                SoLuong = 1,
                IsCombo = true
            });
        }
        SaveCart(cart);
        return RedirectToAction("Index");
    }

    // --- 2. XEM GIỎ HÀNG ---
    public IActionResult Index()
    {
        var cart = GetCart();
        ViewBag.TongTien = cart.Sum(c => c.ThanhTien);
        return View(cart);
    }

    // --- 3. XÓA ITEM ---
    public IActionResult Remove(Guid id, bool isCombo = false)
    {
        var cart = GetCart();
        CartItem? item;
        
        if (isCombo)
        {
            item = cart.FirstOrDefault(c => c.MaCombo == id && c.IsCombo);
        }
        else
        {
            item = cart.FirstOrDefault(c => c.MaSanPham == id && !c.IsCombo);
        }
        
        if (item != null) cart.Remove(item);
        SaveCart(cart);
        return RedirectToAction("Index");
    }

    // --- 4. CẬP NHẬT SỐ LƯỢNG ---
    [HttpPost]
    public IActionResult UpdateQuantity(Guid id, int quantity, bool isCombo = false)
    {
        if (quantity < 1) return RedirectToAction("Index");

        var cart = GetCart();
        CartItem? item;
        
        if (isCombo)
        {
            item = cart.FirstOrDefault(c => c.MaCombo == id && c.IsCombo);
        }
        else
        {
            item = cart.FirstOrDefault(c => c.MaSanPham == id && !c.IsCombo);
        }
        
        if (item != null)
        {
            item.SoLuong = quantity;
        }
        
        SaveCart(cart);
        return RedirectToAction("Index");
    }

    // --- 5. THANH TOÁN (CHECKOUT) ---
    public IActionResult Checkout()
    {
        var userIdStr = HttpContext.Session.GetString("UserID");
        if (string.IsNullOrEmpty(userIdStr)) 
            return RedirectToAction("Login", "Account");

        if (!Guid.TryParse(userIdStr, out Guid userId))
            return RedirectToAction("Login", "Account");

        var cart = GetCart();
        if (cart.Count == 0) return RedirectToAction("Index");

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            // Tạo đơn hàng
            var donHang = new DonHang
            {
                MaDonHang = Guid.NewGuid(),
                MaNguoiDung = userId,
                NgayDat = DateTime.Now,
                TongTien = cart.Sum(c => c.ThanhTien),
                TrangThai = "Chờ xác nhận"
            };
            _context.DonHangs.Add(donHang);
            _context.SaveChanges();

            // Tạo chi tiết đơn hàng
            foreach (var item in cart)
            {
                var chiTiet = new ChiTietDonHang
                {
                    MaChiTietDonHang = Guid.NewGuid(),
                    MaDonHang = donHang.MaDonHang,
                    SoLuong = item.SoLuong,
                    Gia = item.Gia
                };

                if (item.IsCombo && item.MaCombo.HasValue)
                {
                    chiTiet.MaCombo = item.MaCombo.Value;
                    chiTiet.LoaiSanPham = "Combo";
                }
                else
                {
                    chiTiet.MaSanPham = item.MaSanPham;
                    chiTiet.LoaiSanPham = "SanPham";
                }

                _context.ChiTietDonHangs.Add(chiTiet);
            }
            
            _context.SaveChanges();
            transaction.Commit();

            // Xóa giỏ và báo thành công
            HttpContext.Session.Remove("GioHang");
            TempData["ThongBao"] = "Đặt hàng thành công!";
            return RedirectToAction("History");
        }
        catch (Exception)
        {
            transaction.Rollback();
            TempData["Loi"] = "Có lỗi xảy ra khi đặt hàng!";
            return RedirectToAction("Index");
        }
    }
    // --- 6. LỊCH SỬ ĐƠN HÀNG ---
    public IActionResult History()
    {
        var userIdStr = HttpContext.Session.GetString("UserID");
        if (string.IsNullOrEmpty(userIdStr)) 
            return RedirectToAction("Login", "Account");

        if (!Guid.TryParse(userIdStr, out Guid userId))
            return RedirectToAction("Login", "Account");

        var history = _context.DonHangs
            .Include(dh => dh.DanhSachChiTiet)
                .ThenInclude(ct => ct.SanPham)
            .Include(dh => dh.DanhSachChiTiet)
                .ThenInclude(ct => ct.Combo)
            .Where(d => d.MaNguoiDung == userId)
            .OrderByDescending(d => d.NgayDat)
            .ToList();

        return View(history);
    }
}