using System.ComponentModel.DataAnnotations;

namespace ASM_WebBanNuocUong.Models;

public class DanhMuc {
    [Key]
    public Guid MaDanhMuc { get; set; }
    [Required]
    public string TenDanhMuc { get; set; }
    public virtual ICollection<SanPham>? DanhSachSanPham { get; set; }
}