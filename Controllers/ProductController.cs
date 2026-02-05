using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASM_WebBanNuocUong.Data;
using ASM_WebBanNuocUong.Models;

namespace ASM_WebBanNuocUong.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Product 
        public async Task<IActionResult> Index()
            {
                var sanPhams = await _context.SanPhams
                    .Include(sp => sp.DanhMuc)
                    .OrderByDescending(sp => sp.TrangThai) // Đang bán (true) trước, Ngừng bán (false) sau
                    .ThenBy(sp => sp.TenSanPham) // Sau đó sắp xếp theo tên
                    .ToListAsync();
                return View(sanPhams);
            }        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(sp => sp.DanhMuc)
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
            ViewBag.MaDanhMuc = new SelectList(
                _context.DanhMucs.Where(dm => dm.TrangThai), 
                "MaDanhMuc", 
                "TenDanhMuc"
            );
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenSanPham,Gia,MoTa,ChuDe,SoLuongTon,MaDanhMuc,TrangThai")] SanPham sanPham, IFormFile HinhAnhFile)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh
                if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                {
                    // Tạo thư mục img nếu chưa tồn tại
                    var imgFolder = Path.Combine(_environment.WebRootPath, "img");
                    if (!Directory.Exists(imgFolder))
                    {
                        Directory.CreateDirectory(imgFolder);
                    }

                    // Tạo tên file duy nhất
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhAnhFile.FileName);
                    var filePath = Path.Combine(imgFolder, fileName);

                    // Lưu file vào thư mục img
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnhFile.CopyToAsync(stream);
                    }

                    // Lưu tên file vào database
                    sanPham.HinhAnh = fileName;
                }
                else
                {
                    // Nếu không upload ảnh, sử dụng ảnh mặc định
                    sanPham.HinhAnh = "demo.png";
                }

                sanPham.MaSanPham = Guid.NewGuid();
                sanPham.NgayTao = DateTime.Now;
                
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.MaDanhMuc = new SelectList(
                _context.DanhMucs.Where(dm => dm.TrangThai), 
                "MaDanhMuc", 
                "TenDanhMuc", 
                sanPham.MaDanhMuc
            );
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
            
            ViewBag.MaDanhMuc = new SelectList(
                _context.DanhMucs.Where(dm => dm.TrangThai), 
                "MaDanhMuc", 
                "TenDanhMuc", 
                sanPham.MaDanhMuc
            );
            return View(sanPham);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MaSanPham,TenSanPham,Gia,HinhAnh,MoTa,ChuDe,TrangThai,SoLuongTon,MaDanhMuc,NgayTao")] SanPham sanPham, IFormFile HinhAnhFile)
        {
            if (id != sanPham.MaSanPham)
            {
                return NotFound();
            }

            // Lấy sản phẩm cũ từ database để giữ ảnh cũ
            var existingProduct = await _context.SanPhams.AsNoTracking()
                .FirstOrDefaultAsync(p => p.MaSanPham == id);
            
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Giữ ảnh cũ nếu không upload ảnh mới
            if (HinhAnhFile == null || HinhAnhFile.Length == 0)
            {
                sanPham.HinhAnh = existingProduct.HinhAnh;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý upload ảnh mới nếu có
                    if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                    {
                        // Xóa ảnh cũ nếu không phải ảnh mặc định
                        if (!string.IsNullOrEmpty(existingProduct.HinhAnh) && existingProduct.HinhAnh != "demo.png")
                        {
                            var oldImagePath = Path.Combine(_environment.WebRootPath, "img", existingProduct.HinhAnh);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Tạo thư mục img nếu chưa tồn tại
                        var imgFolder = Path.Combine(_environment.WebRootPath, "img");
                        if (!Directory.Exists(imgFolder))
                        {
                            Directory.CreateDirectory(imgFolder);
                        }

                        // Tạo tên file duy nhất
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhAnhFile.FileName);
                        var filePath = Path.Combine(imgFolder, fileName);

                        // Lưu file mới
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await HinhAnhFile.CopyToAsync(stream);
                        }

                        // Cập nhật tên file mới
                        sanPham.HinhAnh = fileName;
                    }

                    sanPham.NgayCapNhat = DateTime.Now;
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
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
            
            ViewBag.MaDanhMuc = new SelectList(
                _context.DanhMucs.Where(dm => dm.TrangThai), 
                "MaDanhMuc", 
                "TenDanhMuc", 
                sanPham.MaDanhMuc
            );
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
                .Include(sp => sp.DanhMuc)
                .FirstOrDefaultAsync(m => m.MaSanPham == id);
            
            if (sanPham == null)
            {
                return NotFound();
            }

            // KIỂM TRA: KHÔNG cho xóa khi trạng thái là ĐANG BÁN (TrangThai = true)
            if (sanPham.TrangThai) // TrangThai = true = Đang bán
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm ĐANG BÁN. Vui lòng chuyển trạng thái sang 'Ngừng bán' trước khi xóa.";
                return RedirectToAction(nameof(Index));
            }

            return View(sanPham);
        }

        // POST: Product/Delete/5 - XÓA VĨNH VIỄN
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                // KIỂM TRA: KHÔNG cho xóa khi trạng thái là ĐANG BÁN (TrangThai = true)
                if (sanPham.TrangThai) // TrangThai = true = Đang bán
                {
                    TempData["ErrorMessage"] = "Không thể xóa sản phẩm ĐANG BÁN!";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa ảnh nếu không phải ảnh mặc định
                if (!string.IsNullOrEmpty(sanPham.HinhAnh) && sanPham.HinhAnh != "demo.png")
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, "img", sanPham.HinhAnh);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // XÓA VĨNH VIỄN
                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Đã xóa vĩnh viễn sản phẩm '{sanPham.TenSanPham}'!";
            }
            
            return RedirectToAction(nameof(Index));
        }

        // POST: Product/Deactivate/5 - Chuyển sang NGỪNG BÁN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                sanPham.TrangThai = false; // Chuyển sang Ngừng bán
                sanPham.NgayCapNhat = DateTime.Now;
                _context.Update(sanPham);
                
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã chuyển trạng thái sản phẩm '{sanPham.TenSanPham}' sang 'Ngừng bán'!";
            }
            
            return RedirectToAction(nameof(Index));
        }

        // POST: Product/Activate/5 - Chuyển sang ĐANG BÁN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(Guid id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                sanPham.TrangThai = true; // Chuyển sang Đang bán
                sanPham.NgayCapNhat = DateTime.Now;
                _context.Update(sanPham);
                
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã kích hoạt lại sản phẩm '{sanPham.TenSanPham}'!";
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(Guid id)
        {
            return _context.SanPhams.Any(e => e.MaSanPham == id);
        }
    }
}