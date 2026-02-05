using System.ComponentModel.DataAnnotations;

namespace ASM_WebBanNuocUong.Models;

public class NguoiDung {
    [Key]
    public Guid MaNguoiDung { get; set; }
    [Required] public string HoTen { get; set; }
    [Required] public string Email { get; set; }
    [Required] public string MatKhau { get; set; }
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public DateTime NgaySinh { get; set; }
    public string VaiTro { get; set; } = "Customer";
    public bool TrangThai { get; set; } = true;
    public DateTime NgayTao { get; set; } = DateTime.Now;
    
    public virtual ICollection<DonHang>? DanhSachDonHang { get; set; }
}