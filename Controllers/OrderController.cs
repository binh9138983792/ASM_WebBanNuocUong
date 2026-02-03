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

        // GET: Order (Admin)
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var orders = await _context.DonHangs
                .Include(dh => dh.NguoiDung)
                .Include(dh => dh.DanhSachChiTiet)
                .OrderByDescending(dh => dh.NgayDat)
                .ToListAsync();

            return View(orders);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(dh => dh.NguoiDung)
                .Include(dh => dh.DanhSachChiTiet)
                    .ThenInclude(ct => ct.SanPham)
                .Include(dh => dh.DanhSachChiTiet)
                    .ThenInclude(ct => ct.Combo)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);
            
            if (donHang == null)
            {
                return NotFound();
            }

            if (!IsAdmin() && donHang.MaNguoiDung.ToString() != HttpContext.Session.GetString("UserID"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(donHang);
        }

        // POST: Order/UpdateStatus/5
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, string trangThai)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "Không có quyền" });

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

            _context.Update(donHang);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
        }

        // GET: Order/History (User)
        public async Task<IActionResult> History()
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Account");

            if (!Guid.TryParse(userIdStr, out Guid userId))
                return RedirectToAction("Login", "Account");

            var history = await _context.DonHangs
                .Include(dh => dh.DanhSachChiTiet)
                    .ThenInclude(ct => ct.SanPham)
                .Include(dh => dh.DanhSachChiTiet)
                    .ThenInclude(ct => ct.Combo)
                .Where(d => d.MaNguoiDung == userId)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            return View(history);
        }

        // POST: Order/Cancel/5
        [HttpPost]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr))
                return Json(new { success = false, message = "Vui lòng đăng nhập" });

            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
            }

            // Kiểm tra quyền
            if (!IsAdmin() && donHang.MaNguoiDung.ToString() != userIdStr)
            {
                return Json(new { success = false, message = "Không có quyền hủy đơn" });
            }

            // Chỉ cho hủy khi đơn hàng đang chờ xác nhận
            if (donHang.TrangThai != "Chờ xác nhận")
            {
                return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang chờ xác nhận" });
            }

            donHang.TrangThai = "Đã hủy";
            _context.Update(donHang);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Hủy đơn hàng thành công" });
        }

        private bool IsAdmin()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            return vaiTro == "Admin";
        }
    }
}