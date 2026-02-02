using System.ComponentModel.DataAnnotations;

namespace ASM_WebBanNuocUong.Models;

public class SanPham {
    [Key]
    public Guid MaSanPham { get; set; }
    [Required] public string TenSanPham { get; set; }
    public decimal Gia { get; set; }
    public string? HinhAnh { get; set; }
    public string? MoTa { get; set; }
    public string? ChuDe { get; set; } // Phục vụ tìm kiếm theo chủ đề
    
    public Guid MaDanhMuc { get; set; }
    public virtual DanhMuc? DanhMuc { get; set; }
}