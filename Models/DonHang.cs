using System.ComponentModel.DataAnnotations;

namespace ASM_WebBanNuocUong.Models;

public class DonHang {
    [Key]
    public Guid MaDonHang { get; set; }
    public DateTime NgayDat { get; set; }
    public string TrangThai { get; set; } // "Chưa giao", "Đang giao", "Đã giao"
    
    public Guid MaNguoiDung { get; set; }
    public virtual NguoiDung? NguoiDung { get; set; }
    public virtual ICollection<ChiTietDonHang>? DanhSachChiTiet { get; set; }
}