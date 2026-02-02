using Microsoft.AspNetCore.Mvc;
using ASM_WebBanNuocUong.Data; 
using Microsoft.EntityFrameworkCore;
using ASM_WebBanNuocUong.Models;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Lấy danh sách sản phẩm từ DB truyền ra View
        var products = await _context.SanPhams.ToListAsync();
        return View(products);
    }
}