using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASM_WebBanNuocUong.Data;
using ASM_WebBanNuocUong.Models;

namespace ASM_WebBanNuocUong.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            return View(await _context.DanhMucs.ToListAsync());
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucs
                .FirstOrDefaultAsync(m => m.MaDanhMuc == id);
            
            if (danhMuc == null)
            {
                return NotFound();
            }

            return View(danhMuc);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDanhMuc")] DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên danh mục đã tồn tại chưa
                var existingCategory = await _context.DanhMucs
                    .FirstOrDefaultAsync(dm => dm.TenDanhMuc.ToLower() == danhMuc.TenDanhMuc.ToLower());
                
                if (existingCategory != null)
                {
                    ModelState.AddModelError("TenDanhMuc", "Tên danh mục đã tồn tại!");
                    return View(danhMuc);
                }

                danhMuc.MaDanhMuc = Guid.NewGuid();
                danhMuc.NgayTao = DateTime.Now;
                danhMuc.TrangThai = true;
                
                _context.Add(danhMuc);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Thêm danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(danhMuc);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            return View(danhMuc);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MaDanhMuc,TenDanhMuc,TrangThai,NgayTao")] DanhMuc danhMuc)
        {
            if (id != danhMuc.MaDanhMuc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra tên danh mục đã tồn tại chưa (trừ danh mục hiện tại)
                    var existingCategory = await _context.DanhMucs
                        .FirstOrDefaultAsync(dm => 
                            dm.TenDanhMuc.ToLower() == danhMuc.TenDanhMuc.ToLower() && 
                            dm.MaDanhMuc != danhMuc.MaDanhMuc);
                    
                    if (existingCategory != null)
                    {
                        ModelState.AddModelError("TenDanhMuc", "Tên danh mục đã tồn tại!");
                        return View(danhMuc);
                    }

                    danhMuc.NgayCapNhat = DateTime.Now;
                    _context.Update(danhMuc);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhMucExists(danhMuc.MaDanhMuc))
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
            return View(danhMuc);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucs
                .Include(dm => dm.DanhSachSanPham)
                .FirstOrDefaultAsync(m => m.MaDanhMuc == id);
            
            if (danhMuc == null)
            {
                return NotFound();
            }

            // Kiểm tra số lượng sản phẩm trong danh mục
            var productCount = danhMuc.DanhSachSanPham?.Count ?? 0;
            ViewBag.ProductCount = productCount;
            ViewBag.HasProducts = productCount > 0;

            return View(danhMuc);
        }

        // POST: Category/Delete/5 - XÓA VĨNH VIỄN
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var danhMuc = await _context.DanhMucs
                .Include(dm => dm.DanhSachSanPham)
                .FirstOrDefaultAsync(m => m.MaDanhMuc == id);
            
            if (danhMuc == null)
            {
                TempData["ErrorMessage"] = "Danh mục không tồn tại!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra nếu danh mục có sản phẩm
            if (danhMuc.DanhSachSanPham != null && danhMuc.DanhSachSanPham.Any())
            {
                TempData["ErrorMessage"] = $"Không thể xóa danh mục '{danhMuc.TenDanhMuc}' vì còn {danhMuc.DanhSachSanPham.Count} sản phẩm thuộc danh mục này!";
                return RedirectToAction(nameof(Delete), new { id });
            }

            try
            {
                // XÓA VĨNH VIỄN
                _context.DanhMucs.Remove(danhMuc);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Đã xóa vĩnh viễn danh mục '{danhMuc.TenDanhMuc}'!";
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa danh mục: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
            
            return RedirectToAction(nameof(Index));
        }

        // POST: Category/Deactivate/5 - Vô hiệu hóa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc != null)
            {
                danhMuc.TrangThai = false;
                danhMuc.NgayCapNhat = DateTime.Now;
                _context.Update(danhMuc);
                
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã vô hiệu hóa danh mục '{danhMuc.TenDanhMuc}'!";
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool DanhMucExists(Guid id)
        {
            return _context.DanhMucs.Any(e => e.MaDanhMuc == id);
        }
    }
}