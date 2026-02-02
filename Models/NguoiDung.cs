using System.ComponentModel.DataAnnotations;

namespace ASM_WebBanNuocUong.Models;

public class NguoiDung {
    [Key]
    public Guid MaNguoiDung { get; set; }
    [Required] public string HoTen { get; set; } // 1
    [Required] public string Email { get; set; }    // 2
    [Required] public string MatKhau { get; set; } // 3
    public string? SoDienThoai { get; set; }               // 4
    public string? DiaChi { get; set; }             // 5
    public DateTime NgaySinh { get; set; }          // 6
    public string VaiTro { get; set; } // "Admin" hoặc "Customer"
}