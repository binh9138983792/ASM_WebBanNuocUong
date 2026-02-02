using System.ComponentModel.DataAnnotations;

namespace ASM_WebBanNuocUong.Models;

public class ChiTietDonHang {
    [Key]
    public Guid MaChiTietDonHang { get; set; }
    public Guid MaDonHang { get; set; }
    public virtual DonHang? DonHang { get; set; }
    public Guid MaSanPham { get; set; }
    public virtual SanPham? SanPham { get; set; }
    public int SoLuong { get; set; }
    public decimal Gia { get; set; }
}