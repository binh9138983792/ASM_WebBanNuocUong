using System.ComponentModel.DataAnnotations;

namespace ASM_WebBanNuocUong.Models;

public class Combo {
    [Key]
    public Guid MaCombo { get; set; }
    
    [Required(ErrorMessage = "Tên combo là bắt buộc")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Tên combo phải từ 3 đến 30 ký tự")]
    [Display(Name = "Tên Combo")]
    public string TenCombo { get; set; }
    
    [Required(ErrorMessage = "Giá combo là bắt buộc")]
    [Range(0.01, 10000000, ErrorMessage = "Giá combo phải lớn hơn 0")]
    [Display(Name = "Giá Combo")]
    [DataType(DataType.Currency)]
    public decimal Gia { get; set; }
    
    [StringLength(200, ErrorMessage = "Mô tả không được quá 200 ký tự")]
    [Display(Name = "Mô Tả")]
    public string? MoTa { get; set; }
    
    [Display(Name = "Hình Ảnh")]
    public string? HinhAnh { get; set; }
    
    [Display(Name = "Trạng Thái")]
    public bool TrangThai { get; set; } = true; // true: đang bán, false: ngừng bán
    
    [Display(Name = "Ngày Tạo")]
    [DataType(DataType.DateTime)]
    public DateTime NgayTao { get; set; } = DateTime.Now;
    
    [Display(Name = "Ngày Cập Nhật")]
    [DataType(DataType.DateTime)]
    public DateTime? NgayCapNhat { get; set; }
    
    public virtual ICollection<ChiTietCombo>? DanhSachChiTietCombo { get; set; }
    public virtual ICollection<ChiTietDonHang>? DanhSachChiTietDonHang { get; set; }
}