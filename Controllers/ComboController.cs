using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASM_WebBanNuocUong.Data;
using ASM_WebBanNuocUong.Models;

namespace ASM_WebBanNuocUong.Controllers
{
    public class ComboController : Controller
    {
        private readonly AppDbContext _context;

        public ComboController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Combo
        public async Task<IActionResult> Index()
        {
            var combos = await _context.Combos
                .Include(c => c.DanhSachChiTietCombo)
                    .ThenInclude(ct => ct.SanPham)
                .Where(c => c.TrangThai)
                .ToListAsync();
            
            return View(combos);
        }

        // GET: Combo/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combo = await _context.Combos
                .Include(c => c.DanhSachChiTietCombo)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(m => m.MaCombo == id);
            
            if (combo == null)
            {
                return NotFound();
            }

            return View(combo);
        }

        // GET: Combo/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.SanPhamList = await _context.SanPhams
                .Where(sp => sp.TrangThai)
                .ToListAsync();
            
            return View();
        }

        // POST: Combo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Combo combo, List<Guid> selectedProducts, List<int> quantities)
        {
            if (ModelState.IsValid && selectedProducts != null && selectedProducts.Count > 0)
            {
                combo.MaCombo = Guid.NewGuid();
                combo.NgayTao = DateTime.Now;
                combo.TrangThai = true;
                
                _context.Add(combo);
                await _context.SaveChangesAsync();

                // Thêm chi tiết combo
                for (int i = 0; i < selectedProducts.Count; i++)
                {
                    var chiTiet = new ChiTietCombo
                    {
                        MaChiTietCombo = Guid.NewGuid(),
                        MaCombo = combo.MaCombo,
                        MaSanPham = selectedProducts[i],
                        SoLuong = quantities != null && i < quantities.Count ? quantities[i] : 1
                    };
                    _context.ChiTietCombos.Add(chiTiet);
                }
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SanPhamList = await _context.SanPhams
                .Where(sp => sp.TrangThai)
                .ToListAsync();
            
            return View(combo);
        }

        // GET: Combo/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combo = await _context.Combos
                .Include(c => c.DanhSachChiTietCombo)
                .FirstOrDefaultAsync(m => m.MaCombo == id);
            
            if (combo == null)
            {
                return NotFound();
            }

            ViewBag.SanPhamList = await _context.SanPhams
                .Where(sp => sp.TrangThai)
                .ToListAsync();
            
            return View(combo);
        }

        // POST: Combo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Combo combo, List<Guid> selectedProducts, List<int> quantities)
        {
            if (id != combo.MaCombo)
            {
                return NotFound();
            }

            if (ModelState.IsValid && selectedProducts != null && selectedProducts.Count > 0)
            {
                try
                {
                    combo.NgayCapNhat = DateTime.Now;
                    _context.Update(combo);

                    // Xóa chi tiết cũ
                    var chiTietCu = await _context.ChiTietCombos
                        .Where(ct => ct.MaCombo == id)
                        .ToListAsync();
                    
                    _context.ChiTietCombos.RemoveRange(chiTietCu);

                    // Thêm chi tiết mới
                    for (int i = 0; i < selectedProducts.Count; i++)
                    {
                        var chiTiet = new ChiTietCombo
                        {
                            MaChiTietCombo = Guid.NewGuid(),
                            MaCombo = combo.MaCombo,
                            MaSanPham = selectedProducts[i],
                            SoLuong = quantities != null && i < quantities.Count ? quantities[i] : 1
                        };
                        _context.ChiTietCombos.Add(chiTiet);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComboExists(combo.MaCombo))
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

            ViewBag.SanPhamList = await _context.SanPhams
                .Where(sp => sp.TrangThai)
                .ToListAsync();
            
            return View(combo);
        }

        // GET: Combo/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combo = await _context.Combos
                .Include(c => c.DanhSachChiTietCombo)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(m => m.MaCombo == id);
            
            if (combo == null)
            {
                return NotFound();
            }

            return View(combo);
        }

        // POST: Combo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo != null)
            {
                // Soft delete
                combo.TrangThai = false;
                combo.NgayCapNhat = DateTime.Now;
                _context.Update(combo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComboExists(Guid id)
        {
            return _context.Combos.Any(e => e.MaCombo == id);
        }
    }
}