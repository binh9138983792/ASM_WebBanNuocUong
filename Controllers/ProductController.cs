using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASM_WebBanNuocUong.Data;
using ASM_WebBanNuocUong.Models;

namespace ASM_WebBanNuocUong.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Product 
        public async Task<IActionResult> Index()
        {
            var sanPhams = await _context.SanPhams
                .Include(sp => sp.DanhMuc)
                .Where(sp => sp.TrangThai)
                .ToListAsync();
            return View(sanPhams);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

    var sanPham = await _context.SanPhams
        .FirstOrDefaultAsync(m => m.MaSanPham == id);
            
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewBag.DanhMucList = _context.DanhMucs
                .Where(dm => dm.TrangThai)
                .ToList();
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenSanPham,Gia,HinhAnh,MoTa,ChuDe,SoLuongTon,MaDanhMuc")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                sanPham.MaSanPham = Guid.NewGuid();
                sanPham.NgayTao = DateTime.Now;
                sanPham.TrangThai = true;
                
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.DanhMucList = _context.DanhMucs
                .Where(dm => dm.TrangThai)
                .ToList();
            return View(sanPham);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            
            ViewBag.DanhMucList = _context.DanhMucs
                .Where(dm => dm.TrangThai)
                .ToList();
            return View(sanPham);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MaSanPham,TenSanPham,Gia,HinhAnh,MoTa,ChuDe,TrangThai,SoLuongTon,MaDanhMuc,NgayTao")] SanPham sanPham)
        {
            if (id != sanPham.MaSanPham)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    sanPham.NgayCapNhat = DateTime.Now;
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSanPham))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.DanhMucList = _context.DanhMucs
                .Where(dm => dm.TrangThai)
                .ToList();
            return View(sanPham);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

    var sanPham = await _context.SanPhams
        .FirstOrDefaultAsync(m => m.MaSanPham == id);
            
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                // Soft delete: chỉ đổi trạng thái
                sanPham.TrangThai = false;
                sanPham.NgayCapNhat = DateTime.Now;
                _context.Update(sanPham);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(Guid id)
        {
            return _context.SanPhams.Any(e => e.MaSanPham == id);
        }
    }
}