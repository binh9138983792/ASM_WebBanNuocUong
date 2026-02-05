using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASM_WebBanNuocUong.Data;
using ASM_WebBanNuocUong.Models;

namespace ASM_WebBanNuocUong.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _context.DonHangs
                    .Include(dh => dh.NguoiDung)
                    .OrderByDescending(dh => dh.NgayDat)
                    .ToListAsync();

                return View(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi load đơn hàng: {ex.Message}");
                return View(new List<DonHang>());
            }
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            try
            {
                var donHang = await _context.DonHangs
                    .Include(dh => dh.NguoiDung)
                    .FirstOrDefaultAsync(m => m.MaDonHang == id);
                
                if (donHang == null) return NotFound();

                // Load chi tiết đơn hàng
                var chiTiet = await _context.ChiTietDonHangs
                    .Include(ct => ct.SanPham)
                    .Include(ct => ct.Combo)
                    .Where(ct => ct.MaDonHang == id)
                    .ToListAsync();
                
                donHang.DanhSachChiTiet = chiTiet;

                return View(donHang);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi load chi tiết đơn hàng: {ex.Message}");
                return NotFound();
            }
        }

        public IActionResult Create()
        {
            try
            {
                // Lấy danh sách khách hàng có TrạngThái = true
                ViewBag.KhachHangList = _context.NguoiDungs
                    .Where(nd => nd.TrangThai && nd.VaiTro == "Customer")
                    .ToList();
                
                ViewBag.SanPhamList = _context.SanPhams
                    .Where(sp => sp.TrangThai && sp.SoLuongTon > 0)
                    .ToList();
                
                ViewBag.ComboList = _context.Combos
                    .Where(c => c.TrangThai)
                    .ToList();
                
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi load dữ liệu tạo đơn: {ex.Message}");
                ViewBag.KhachHangList = new List<NguoiDung>();
                ViewBag.SanPhamList = new List<SanPham>();
                ViewBag.ComboList = new List<Combo>();
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonHang donHang, 
            List<Guid> selectedProducts, List<Guid> selectedCombos,
            List<int> productQuantities, List<int> comboQuantities)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    donHang.MaDonHang = Guid.NewGuid();
                    donHang.NgayDat = DateTime.Now;
                    donHang.TrangThai = "Chờ xác nhận";
                    
                    decimal tongTien = 0;
                    var chiTietList = new List<ChiTietDonHang>();

                    // Thêm sản phẩm
                    if (selectedProducts != null)
                    {
                        for (int i = 0; i < selectedProducts.Count; i++)
                        {
                            var sp = await _context.SanPhams.FindAsync(selectedProducts[i]);
                            if (sp != null)
                            {
                                var quantity = productQuantities != null && i < productQuantities.Count ? 
                                             productQuantities[i] : 1;
                                
                                tongTien += sp.Gia * quantity;
                                
                                chiTietList.Add(new ChiTietDonHang
                                {
                                    MaChiTietDonHang = Guid.NewGuid(),
                                    MaDonHang = donHang.MaDonHang,
                                    MaSanPham = sp.MaSanPham,
                                    SoLuong = quantity,
                                    Gia = sp.Gia,
                                    LoaiSanPham = "SanPham"
                                });
                            }
                        }
                    }

                    // Thêm combo
                    if (selectedCombos != null)
                    {
                        for (int i = 0; i < selectedCombos.Count; i++)
                        {
                            var combo = await _context.Combos.FindAsync(selectedCombos[i]);
                            if (combo != null)
                            {
                                var quantity = comboQuantities != null && i < comboQuantities.Count ? 
                                             comboQuantities[i] : 1;
                                
                                tongTien += combo.Gia * quantity;
                                
                                chiTietList.Add(new ChiTietDonHang
                                {
                                    MaChiTietDonHang = Guid.NewGuid(),
                                    MaDonHang = donHang.MaDonHang,
                                    MaCombo = combo.MaCombo,
                                    SoLuong = quantity,
                                    Gia = combo.Gia,
                                    LoaiSanPham = "Combo"
                                });
                            }
                        }
                    }

                    donHang.TongTien = tongTien;
                    donHang.DanhSachChiTiet = chiTietList;

                    _context.DonHangs.Add(donHang);
                    await _context.SaveChangesAsync();
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi tạo đơn hàng: {ex.Message}");
                }
            }

            // Reload dữ liệu nếu có lỗi
            ViewBag.KhachHangList = _context.NguoiDungs
                .Where(nd => nd.TrangThai && nd.VaiTro == "Customer")
                .ToList();
            
            ViewBag.SanPhamList = _context.SanPhams
                .Where(sp => sp.TrangThai && sp.SoLuongTon > 0)
                .ToList();
            
            ViewBag.ComboList = _context.Combos
                .Where(c => c.TrangThai)
                .ToList();
            
            return View(donHang);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            try
            {
                var donHang = await _context.DonHangs
                    .Include(dh => dh.NguoiDung)
                    .FirstOrDefaultAsync(m => m.MaDonHang == id);
                
                if (donHang == null) return NotFound();

                // Load chi tiết đơn hàng
                var chiTiet = await _context.ChiTietDonHangs
                    .Include(ct => ct.SanPham)
                    .Include(ct => ct.Combo)
                    .Where(ct => ct.MaDonHang == id)
                    .ToListAsync();
                
                donHang.DanhSachChiTiet = chiTiet;

                ViewBag.KhachHangList = await _context.NguoiDungs
                    .Where(nd => nd.TrangThai && nd.VaiTro == "Customer")
                    .ToListAsync();
                
                return View(donHang);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi load đơn hàng để edit: {ex.Message}");
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DonHang donHang)
        {
            if (id != donHang.MaDonHang) return NotFound();

            // Kiểm tra nếu đơn hàng đã hủy thì không cho sửa
            var donHangCu = await _context.DonHangs.FindAsync(id);
            if (donHangCu != null && donHangCu.TrangThai == "Đã hủy")
            {
                TempData["ErrorMessage"] = "Không thể chỉnh sửa đơn hàng đã hủy";
                return RedirectToAction(nameof(Edit), new { id });
            }

            try
            {
                // Lấy đơn hàng từ database
                var donHangDb = await _context.DonHangs.FindAsync(id);
                if (donHangDb == null)
                {
                    return NotFound();
                }

                // Cập nhật chỉ các trường được phép
                donHangDb.MaNguoiDung = donHang.MaNguoiDung;
                donHangDb.TrangThai = donHang.TrangThai;
                donHangDb.NgayGiao = donHang.NgayGiao;
                donHangDb.GhiChu = donHang.GhiChu;
                
                // Nếu chuyển sang trạng thái "Đã giao" và chưa có ngày giao
                if (donHangDb.TrangThai == "Đã giao" && !donHangDb.NgayGiao.HasValue)
                {
                    donHangDb.NgayGiao = DateTime.Now;
                }

                _context.DonHangs.Update(donHangDb);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi cập nhật đơn hàng: {ex.Message}");
                
                ViewBag.KhachHangList = await _context.NguoiDungs
                    .Where(nd => nd.TrangThai && nd.VaiTro == "Customer")
                    .ToListAsync();
                
                return View(donHang);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var donHang = await _context.DonHangs.FindAsync(id);
                if (donHang != null)
                {
                    // Xóa chi tiết đơn hàng trước
                    var chiTiet = await _context.ChiTietDonHangs
                        .Where(ct => ct.MaDonHang == id)
                        .ToListAsync();
                    
                    if (chiTiet.Any())
                    {
                        _context.ChiTietDonHangs.RemoveRange(chiTiet);
                    }
                    
                    // Xóa đơn hàng
                    _context.DonHangs.Remove(donHang);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Xóa đơn hàng thành công";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa đơn hàng: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa đơn hàng";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                var donHang = await _context.DonHangs.FindAsync(id);
                if (donHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Chỉ được hủy đơn hàng đang chờ xác nhận
                if (donHang.TrangThai != "Chờ xác nhận")
                {
                    return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang chờ xác nhận" });
                }

                donHang.TrangThai = "Đã hủy";
                _context.DonHangs.Update(donHang);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Hủy đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, string trangThai)
        {
            try
            {
                var donHang = await _context.DonHangs.FindAsync(id);
                if (donHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                donHang.TrangThai = trangThai;
                if (trangThai == "Đã giao")
                {
                    donHang.NgayGiao = DateTime.Now;
                }

                _context.DonHangs.Update(donHang);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}