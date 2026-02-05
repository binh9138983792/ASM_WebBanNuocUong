using System.ComponentModel.DataAnnotations;

namespace ASM_WebBanNuocUong.Models;

public class DonHang {
    [Key]
    [Display(Name = "Mã Đơn Hàng")]
    public Guid MaDonHang { get; set; }
    
    [Required(ErrorMessage = "Ngày đặt là bắt buộc")]
    [Display(Name = "Ngày Đặt")]
    [DataType(DataType.DateTime)]
    public DateTime NgayDat { get; set; }
    
    [Display(Name = "Ngày Giao")]
    [DataType(DataType.DateTime)]
    public DateTime? NgayGiao { get; set; }
    
    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    [StringLength(20, ErrorMessage = "Trạng thái không được quá 20 ký tự")]
    [Display(Name = "Trạng Thái")]
    [RegularExpression("^(Chờ xác nhận|Đang giao|Đã giao|Đã hủy)$", ErrorMessage = "Trạng thái không hợp lệ")]
    public string TrangThai { get; set; } = "Chờ xác nhận"; // "Chờ xác nhận", "Đang giao", "Đã giao", "Đã hủy"
    
    [StringLength(500, ErrorMessage = "Ghi chú không được quá 500 ký tự")]
    [Display(Name = "Ghi Chú")]
    public string? GhiChu { get; set; }
    
    [Required(ErrorMessage = "Tổng tiền là bắt buộc")]
    [Range(0, 100000000, ErrorMessage = "Tổng tiền phải từ 0 đến 100,000,000")]
    [Display(Name = "Tổng Tiền")]
    [DataType(DataType.Currency)]
    public decimal TongTien { get; set; }
    
    [Required(ErrorMessage = "Mã người dùng là bắt buộc")]
    [Display(Name = "Mã Người Dùng")]
    public Guid MaNguoiDung { get; set; }
    
    public virtual NguoiDung? NguoiDung { get; set; }
    public virtual ICollection<ChiTietDonHang>? DanhSachChiTiet { get; set; }
}