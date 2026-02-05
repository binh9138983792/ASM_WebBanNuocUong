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
            try
            {
                // Chỉ lấy combo, không include chi tiết để tối ưu
                var combos = await _context.Combos
                    .Where(c => c.TrangThai)
                    .OrderByDescending(c => c.NgayTao)
                    .ToListAsync();
                
                return View(combos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi load combos: {ex.Message}");
                return View(new List<Combo>());
            }
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            try
            {
                var combo = await _context.Combos
                    .FirstOrDefaultAsync(m => m.MaCombo == id);
                
                if (combo == null) return NotFound();

                // Load chi tiết combo riêng và tính số lượng
                var chiTiet = await _context.ChiTietCombos
                    .Include(ct => ct.SanPham)
                    .Where(ct => ct.MaCombo == id)
                    .ToListAsync();
                
                combo.DanhSachChiTietCombo = chiTiet;

                return View(combo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi load chi tiết combo: {ex.Message}");
                return NotFound();
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.SanPhamList = await _context.SanPhams
                    .Where(sp => sp.TrangThai)
                    .Select(sp => new 
                    {
                        sp.MaSanPham,
                        sp.TenSanPham,
                        sp.Gia,
                        sp.SoLuongTon,
                        sp.HinhAnh
                    })
                    .ToListAsync();
                
                return View(new Combo());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi load sản phẩm: {ex.Message}");
                ViewBag.SanPhamList = new List<object>();
                return View(new Combo());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Combo combo, 
            List<Guid> selectedProducts, List<int> quantities)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    combo.MaCombo = Guid.NewGuid();
                    combo.NgayTao = DateTime.Now;
                    combo.TrangThai = true;
                    
                    // Tự tính giá combo nếu không nhập
                    if (combo.Gia == 0 && selectedProducts != null)
                    {
                        decimal tongGia = 0;
                        for (int i = 0; i < selectedProducts.Count; i++)
                        {
                            var sp = await _context.SanPhams.FindAsync(selectedProducts[i]);
                            if (sp != null)
                            {
                                var quantity = quantities != null && i < quantities.Count ? 
                                             quantities[i] : 1;
                                tongGia += sp.Gia * quantity;
                            }
                        }
                        // KHÔNG GIẢM GIÁ - Sử dụng nguyên giá
                        combo.Gia = tongGia;
                    }
                    
                    _context.Combos.Add(combo);
                    await _context.SaveChangesAsync();

                    // Thêm chi tiết combo
                    if (selectedProducts != null && selectedProducts.Count > 0)
                    {
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
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi tạo combo: {ex.Message}");
                }
            }

            // Reload sản phẩm nếu có lỗi
            ViewBag.SanPhamList = await _context.SanPhams
                .Where(sp => sp.TrangThai)
                .Select(sp => new 
                {
                    sp.MaSanPham,
                    sp.TenSanPham,
                    sp.Gia,
                    sp.SoLuongTon,
                    sp.HinhAnh
                })
                .ToListAsync();
            
            return View(combo);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            try
            {
                var combo = await _context.Combos
                    .FirstOrDefaultAsync(m => m.MaCombo == id);
                
                if (combo == null) return NotFound();

                // Load chi tiết combo
                var chiTiet = await _context.ChiTietCombos
                    .Include(ct => ct.SanPham)
                    .Where(ct => ct.MaCombo == id)
                    .ToListAsync();
                
                combo.DanhSachChiTietCombo = chiTiet;

                ViewBag.SanPhamList = await _context.SanPhams
                    .Where(sp => sp.TrangThai)
                    .Select(sp => new 
                    {
                        sp.MaSanPham,
                        sp.TenSanPham,
                        sp.Gia,
                        sp.SoLuongTon,
                        sp.HinhAnh
                    })
                    .ToListAsync();
                
                return View(combo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi load combo để edit: {ex.Message}");
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Combo combo, 
            List<Guid> selectedProducts, List<int> quantities)
        {
            if (id != combo.MaCombo) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    combo.NgayCapNhat = DateTime.Now;
                    
                    // Tự tính giá combo nếu không nhập
                    if (combo.Gia == 0 && selectedProducts != null)
                    {
                        decimal tongGia = 0;
                        for (int i = 0; i < selectedProducts.Count; i++)
                        {
                            var sp = await _context.SanPhams.FindAsync(selectedProducts[i]);
                            if (sp != null)
                            {
                                var quantity = quantities != null && i < quantities.Count ? 
                                             quantities[i] : 1;
                                tongGia += sp.Gia * quantity;
                            }
                        }
                        // KHÔNG GIẢM GIÁ - Sử dụng nguyên giá
                        combo.Gia = tongGia;
                    }
                    
                    _context.Combos.Update(combo);

                    // Xóa chi tiết cũ
                    var chiTietCu = await _context.ChiTietCombos
                        .Where(ct => ct.MaCombo == id)
                        .ToListAsync();
                    
                    _context.ChiTietCombos.RemoveRange(chiTietCu);

                    // Thêm chi tiết mới
                    if (selectedProducts != null && selectedProducts.Count > 0)
                    {
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
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi cập nhật combo: {ex.Message}");
                }
            }

            ViewBag.SanPhamList = await _context.SanPhams
                .Where(sp => sp.TrangThai)
                .Select(sp => new 
                {
                    sp.MaSanPham,
                    sp.TenSanPham,
                    sp.Gia,
                    sp.SoLuongTon,
                    sp.HinhAnh
                })
                .ToListAsync();
            
            return View(combo);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            try
            {
                var combo = await _context.Combos
                    .FirstOrDefaultAsync(m => m.MaCombo == id);
                
                if (combo == null) return NotFound();

                return View(combo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi load combo để xóa: {ex.Message}");
                return NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var combo = await _context.Combos.FindAsync(id);
                if (combo != null)
                {
                    // Soft delete
                    combo.TrangThai = false;
                    combo.NgayCapNhat = DateTime.Now;
                    _context.Combos.Update(combo);
                    
                    await _context.SaveChangesAsync();
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa combo: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // Hàm tính số lượng sản phẩm trong combo (nếu cần dùng)
        private async Task<int> TinhSoLuongSanPhamTrongCombo(Guid maCombo)
        {
            var chiTiet = await _context.ChiTietCombos
                .Where(ct => ct.MaCombo == maCombo)
                .ToListAsync();
            
            return chiTiet.Sum(ct => ct.SoLuong);
        }

        private bool ComboExists(Guid id)
        {
            return _context.Combos.Any(e => e.MaCombo == id);
        }
    }
}