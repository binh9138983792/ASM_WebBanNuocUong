using Microsoft.AspNetCore.Mvc;
using ASM_WebBanNuocUong.Models;
using ASM_WebBanNuocUong.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ASM_WebBanNuocUong.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    // --- 1. ĐĂNG KÝ ---
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(NguoiDung user, string NhapLaiMatKhau)
    {
        // Kiểm tra Email trùng
        if (_context.NguoiDungs.Any(u => u.Email == user.Email))
        {
            ViewBag.Loi = "Email này đã được đăng ký!";
            return View();
        }

        // Kiểm tra mật khẩu khớp
        if (user.MatKhau != NhapLaiMatKhau)
        {
            ViewBag.Loi = "Mật khẩu nhập lại không khớp!";
            return View();
        }

        // Mã hóa mật khẩu (đơn giản bằng SHA256)
        user.MatKhau = HashPassword(user.MatKhau);
        
        // Set giá trị mặc định
        user.VaiTro = "Customer";
        user.TrangThai = true;
        user.NgayTao = DateTime.Now;
        user.MaNguoiDung = Guid.NewGuid();

        // Lưu vào Database
        _context.NguoiDungs.Add(user);
        _context.SaveChanges();

        TempData["ThongBao"] = "Đăng ký thành công! Mời đăng nhập.";
        return RedirectToAction("Login");
    }

    // --- 2. ĐĂNG NHẬP THƯỜNG ---
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var hashedPassword = HashPassword(password);
        var user = _context.NguoiDungs
            .FirstOrDefault(u => u.Email == email && u.MatKhau == hashedPassword);

        if (user != null && user.TrangThai)
        {
            // Lưu thông tin vào Session (Guid chuyển thành string)
            HttpContext.Session.SetString("UserID", user.MaNguoiDung.ToString());
            HttpContext.Session.SetString("UserName", user.HoTen);
            HttpContext.Session.SetString("VaiTro", user.VaiTro);

            // Chuyển hướng vào trang chủ
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Loi = "Sai email hoặc mật khẩu!";
        return View();
    }

    // --- 3. ĐĂNG NHẬP GOOGLE ---
    public IActionResult LoginGoogle()
    {
        var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded) return RedirectToAction("Login");

        // Lấy thông tin Google trả về
        var email = result.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
        var name = result.Principal.FindFirstValue(ClaimTypes.Name) ?? "Khách";

        // Kiểm tra xem người này đã có trong DB chưa
        var user = _context.NguoiDungs.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            // Chưa có -> Tạo mới tự động
            user = new NguoiDung
            {
                MaNguoiDung = Guid.NewGuid(),
                Email = email,
                HoTen = name,
                MatKhau = HashPassword(Guid.NewGuid().ToString()), // Random password
                VaiTro = "Customer",
                TrangThai = true,
                NgaySinh = DateTime.Now.AddYears(-20), // Mặc định 20 tuổi
                NgayTao = DateTime.Now,
                SoDienThoai = "",
                DiaChi = ""
            };
            _context.NguoiDungs.Add(user);
            _context.SaveChanges();
        }

        // Lưu Session
        HttpContext.Session.SetString("UserID", user.MaNguoiDung.ToString());
        HttpContext.Session.SetString("UserName", user.HoTen);
        HttpContext.Session.SetString("VaiTro", user.VaiTro);

        return RedirectToAction("Index", "Home");
    }

    // --- 4. ĐĂNG XUẤT ---
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }

    // --- HÀM BỔ TRỢ: Mã hóa mật khẩu ---
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}