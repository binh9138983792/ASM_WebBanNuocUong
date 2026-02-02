using System.ComponentModel.DataAnnotations;

namespace ASM_WebBanNuocUong.Models;

public class Combo {
    [Key]
    public Guid MaCombo { get; set; }
    [Required] public string TenCombo { get; set; }
    public decimal Gia { get; set; }
    public string? MoTa { get; set; }
    public virtual ICollection<ChiTietCombo>? DanhSachChiTietCombo { get; set; }
}