using System.ComponentModel.DataAnnotations;

namespace ASM_WebBanNuocUong.Models;

public class SanPham {
    [Key]
    [Display(Name = "Mã Sản Phẩm")]
    public Guid MaSanPham { get; set; }
    
    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Tên sản phẩm phải từ 3 đến 30 ký tự")]
    [Display(Name = "Tên Sản Phẩm")]
    public string TenSanPham { get; set; }
    
    [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
    [Range(0.01, 10000000, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
    [Display(Name = "Giá")]
    [DataType(DataType.Currency)]
    public decimal Gia { get; set; }
    
    [Display(Name = "Hình Ảnh")]
    [Url(ErrorMessage = "URL hình ảnh không hợp lệ")]
    public string? HinhAnh { get; set; }
    
    [StringLength(200, ErrorMessage = "Mô tả không được quá 200 ký tự")]
    [Display(Name = "Mô Tả")]
    public string? MoTa { get; set; }
    
    [StringLength(20, ErrorMessage = "Chủ đề không được quá 20 ký tự")]
    [Display(Name = "Chủ Đề")]
    public string? ChuDe { get; set; }
    
    [Display(Name = "Trạng Thái")]
    public bool TrangThai { get; set; } = true; // true: đang bán, false: ngừng bán
    
    [Required(ErrorMessage = "Số lượng tồn là bắt buộc")]
    [Range(0, 10000, ErrorMessage = "Số lượng tồn phải từ 0 đến 10,000")]
    [Display(Name = "Số Lượng Tồn")]
    public int SoLuongTon { get; set; } = 0;
    
    [Display(Name = "Ngày Tạo")]
    [DataType(DataType.DateTime)]
    public DateTime NgayTao { get; set; } = DateTime.Now;
    
    [Display(Name = "Ngày Cập Nhật")]
    [DataType(DataType.DateTime)]
    public DateTime? NgayCapNhat { get; set; }
    
    [Required(ErrorMessage = "Mã danh mục là bắt buộc")]
    [Display(Name = "Mã Danh Mục")]
    public Guid MaDanhMuc { get; set; }
    
    public virtual DanhMuc? DanhMuc { get; set; }
    
    public virtual ICollection<ChiTietCombo>? DanhSachChiTietCombo { get; set; }
    public virtual ICollection<ChiTietDonHang>? DanhSachChiTietDonHang { get; set; }
}