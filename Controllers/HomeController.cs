using Microsoft.AspNetCore.Mvc;
using ASM_WebBanNuocUong.Data;
using ASM_WebBanNuocUong.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_WebBanNuocUong.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.SanPhamMoi = await _context.SanPhams
                .Where(sp => sp.TrangThai)
                .OrderByDescending(sp => sp.NgayTao)
                .Take(8)
                .ToListAsync();

            ViewBag.ComboNoiBat = await _context.Combos
                .Where(c => c.TrangThai)
                .Take(4)
                .ToListAsync();

            ViewBag.DanhMuc = await _context.DanhMucs
                .Where(dm => dm.TrangThai)
                .Include(dm => dm.DanhSachSanPham)
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {
            return View();
        }

        public async Task<IActionResult> Contact()
        {
            return View();
        }
    }
}