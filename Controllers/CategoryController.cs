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
                danhMuc.MaDanhMuc = Guid.NewGuid();
                danhMuc.NgayTao = DateTime.Now;
                danhMuc.TrangThai = true;
                
                _context.Add(danhMuc);
                await _context.SaveChangesAsync();
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
                    danhMuc.NgayCapNhat = DateTime.Now;
                    _context.Update(danhMuc);
                    await _context.SaveChangesAsync();
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

            return View(danhMuc);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc != null)
            {
                // Soft delete
                danhMuc.TrangThai = false;
                danhMuc.NgayCapNhat = DateTime.Now;
                _context.Update(danhMuc);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DanhMucExists(Guid id)
        {
            return _context.DanhMucs.Any(e => e.MaDanhMuc == id);
        }
    }
}