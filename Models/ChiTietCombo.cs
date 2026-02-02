using System.ComponentModel.DataAnnotations;

namespace ASM_WebBanNuocUong.Models;

public class ChiTietCombo {
    [Key]
    public Guid MaChiTietCombo { get; set; }
    public Guid MaCombo { get; set; }
    public virtual Combo? Combo { get; set; }
    public Guid MaSanPham { get; set; }
    public virtual SanPham? SanPham { get; set; }
}