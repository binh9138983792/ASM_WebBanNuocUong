using System.ComponentModel.DataAnnotations;

namespace ASM_WebBanNuocUong.Models;

public class DanhMuc {
    [Key]
    [Display(Name = "Mã Danh Mục")]
    public Guid MaDanhMuc { get; set; }
    
    [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Tên danh mục phải từ 3 đến 20 ký tự")]
    [Display(Name = "Tên Danh Mục")]
    public string TenDanhMuc { get; set; }
    
    [Display(Name = "Trạng Thái")]
    public bool TrangThai { get; set; } = true;
    
    public virtual ICollection<SanPham>? DanhSachSanPham { get; set; }
    
    [Display(Name = "Ngày Tạo")]
    [DataType(DataType.DateTime)]
    public DateTime NgayTao { get; set; } = DateTime.Now;
    
    [Display(Name = "Ngày Cập Nhật")]
    [DataType(DataType.DateTime)]
    public DateTime? NgayCapNhat { get; set; }
}