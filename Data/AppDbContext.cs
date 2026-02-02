using Microsoft.EntityFrameworkCore;
using ASM_WebBanNuocUong.Models;

namespace ASM_WebBanNuocUong.Data;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<NguoiDung> NguoiDungs { get; set; }
    public DbSet<SanPham> SanPhams { get; set; }
    public DbSet<DanhMuc> DanhMucs { get; set; }
    public DbSet<DonHang> DonHangs { get; set; }
    public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
    public DbSet<Combo> Combos { get; set; }
    public DbSet<ChiTietCombo> ChiTietCombos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        
        // ========== CẤU HÌNH PRECISION CHO DECIMAL FIELDS ==========
        modelBuilder.Entity<SanPham>().Property(p => p.Gia).HasPrecision(18, 2);
        modelBuilder.Entity<Combo>().Property(c => c.Gia).HasPrecision(18, 2);
        modelBuilder.Entity<ChiTietDonHang>().Property(od => od.Gia).HasPrecision(18, 2);
        modelBuilder.Entity<DonHang>().Property(dh => dh.TongTien).HasPrecision(18, 2);
        
        // ========== CẤU HÌNH QUAN HỆ GIỮA CÁC BẢNG ==========
        
        // DanhMuc ↔ SanPham (1-n)
        modelBuilder.Entity<SanPham>()
            .HasOne(sp => sp.DanhMuc)
            .WithMany(dm => dm.DanhSachSanPham)
            .HasForeignKey(sp => sp.MaDanhMuc)
            .OnDelete(DeleteBehavior.Restrict);
        
        // SanPham ↔ ChiTietCombo (1-n)
        modelBuilder.Entity<ChiTietCombo>()
            .HasOne(ctc => ctc.SanPham)
            .WithMany(sp => sp.DanhSachChiTietCombo)
            .HasForeignKey(ctc => ctc.MaSanPham)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Combo ↔ ChiTietCombo (1-n)
        modelBuilder.Entity<ChiTietCombo>()
            .HasOne(ctc => ctc.Combo)
            .WithMany(c => c.DanhSachChiTietCombo)
            .HasForeignKey(ctc => ctc.MaCombo)
            .OnDelete(DeleteBehavior.Cascade);
        
        // NguoiDung ↔ DonHang (1-n)
        modelBuilder.Entity<DonHang>()
            .HasOne(dh => dh.NguoiDung)
            .WithMany(nd => nd.DanhSachDonHang)
            .HasForeignKey(dh => dh.MaNguoiDung)
            .OnDelete(DeleteBehavior.Restrict);
        
        // DonHang ↔ ChiTietDonHang (1-n)
        modelBuilder.Entity<ChiTietDonHang>()
            .HasOne(ctdh => ctdh.DonHang)
            .WithMany(dh => dh.DanhSachChiTiet)
            .HasForeignKey(ctdh => ctdh.MaDonHang)
            .OnDelete(DeleteBehavior.Cascade);
        
        // SanPham ↔ ChiTietDonHang (1-n)
        modelBuilder.Entity<ChiTietDonHang>()
            .HasOne(ctdh => ctdh.SanPham)
            .WithMany(sp => sp.DanhSachChiTietDonHang)
            .HasForeignKey(ctdh => ctdh.MaSanPham)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Combo ↔ ChiTietDonHang (1-n)
        modelBuilder.Entity<ChiTietDonHang>()
            .HasOne(ctdh => ctdh.Combo)
            .WithMany(c => c.DanhSachChiTietDonHang)
            .HasForeignKey(ctdh => ctdh.MaCombo)
            .OnDelete(DeleteBehavior.Restrict);
        
        // ========== RÀNG BUỘC LOGIC CHO ChiTietDonHang ==========
        // Chỉ được có MaSanPham HOẶC MaCombo, không được có cả hai
        modelBuilder.Entity<ChiTietDonHang>()
            .HasCheckConstraint("CK_ChiTietDonHang_SanPhamOrCombo", 
                "([MaSanPham] IS NOT NULL AND [MaCombo] IS NULL) OR ([MaSanPham] IS NULL AND [MaCombo] IS NOT NULL)");
        
        // LoaiSanPham phải khớp với loại thực tế
        modelBuilder.Entity<ChiTietDonHang>()
            .HasCheckConstraint("CK_ChiTietDonHang_LoaiSanPham", 
                "(LoaiSanPham = 'SanPham' AND MaSanPham IS NOT NULL) OR " +
                "(LoaiSanPham = 'Combo' AND MaCombo IS NOT NULL)");
        
        // ========== CẤU HÌNH GIÁ TRỊ MẶC ĐỊNH ==========
        
        // Giá trị mặc định cho trạng thái
        modelBuilder.Entity<SanPham>()
            .Property(sp => sp.TrangThai)
            .HasDefaultValue(true);
        
        modelBuilder.Entity<DanhMuc>()
            .Property(dm => dm.TrangThai)
            .HasDefaultValue(true);
        
        modelBuilder.Entity<Combo>()
            .Property(c => c.TrangThai)
            .HasDefaultValue(true);
        
        modelBuilder.Entity<NguoiDung>()
            .Property(nd => nd.TrangThai)
            .HasDefaultValue(true);
        
        modelBuilder.Entity<NguoiDung>()
            .Property(nd => nd.VaiTro)
            .HasDefaultValue("Customer");
        
        modelBuilder.Entity<DonHang>()
            .Property(dh => dh.TrangThai)
            .HasDefaultValue("Chờ xác nhận");
        
        // Giá trị mặc định cho ngày tạo (sử dụng SQL Server function)
        modelBuilder.Entity<SanPham>()
            .Property(sp => sp.NgayTao)
            .HasDefaultValueSql("GETDATE()");
        
        modelBuilder.Entity<Combo>()
            .Property(c => c.NgayTao)
            .HasDefaultValueSql("GETDATE()");
        
        modelBuilder.Entity<DanhMuc>()
            .Property(dm => dm.NgayTao)
            .HasDefaultValueSql("GETDATE()");
        
        modelBuilder.Entity<NguoiDung>()
            .Property(nd => nd.NgayTao)
            .HasDefaultValueSql("GETDATE()");
        
        modelBuilder.Entity<DonHang>()
            .Property(dh => dh.NgayDat)
            .HasDefaultValueSql("GETDATE()");
        
        // ========== CẤU HÌNH ĐỘ DÀI CHO STRING FIELDS ==========
        
        modelBuilder.Entity<SanPham>()
            .Property(s => s.TenSanPham)
            .HasMaxLength(200);
        
        modelBuilder.Entity<SanPham>()
            .Property(s => s.MoTa)
            .HasMaxLength(1000);
        
        modelBuilder.Entity<SanPham>()
            .Property(s => s.ChuDe)
            .HasMaxLength(100);
        
        modelBuilder.Entity<Combo>()
            .Property(c => c.TenCombo)
            .HasMaxLength(200);
        
        modelBuilder.Entity<Combo>()
            .Property(c => c.MoTa)
            .HasMaxLength(1000);
        
        modelBuilder.Entity<DonHang>()
            .Property(d => d.TrangThai)
            .HasMaxLength(50);
        
        modelBuilder.Entity<DonHang>()
            .Property(d => d.GhiChu)
            .HasMaxLength(500);
        
        modelBuilder.Entity<NguoiDung>()
            .Property(n => n.HoTen)
            .HasMaxLength(100);
        
        modelBuilder.Entity<NguoiDung>()
            .Property(n => n.Email)
            .HasMaxLength(100);
        
        modelBuilder.Entity<NguoiDung>()
            .Property(n => n.VaiTro)
            .HasMaxLength(20);
        
        modelBuilder.Entity<DanhMuc>()
            .Property(d => d.TenDanhMuc)
            .HasMaxLength(100);
        
        modelBuilder.Entity<ChiTietDonHang>()
            .Property(c => c.LoaiSanPham)
            .HasMaxLength(10);
        
        // ========== CẤU HÌNH UNIQUE CONSTRAINTS ==========
        
        modelBuilder.Entity<NguoiDung>()
            .HasIndex(nd => nd.Email)
            .IsUnique();
        
        modelBuilder.Entity<SanPham>()
            .HasIndex(sp => sp.TenSanPham)
            .IsUnique();
        
        modelBuilder.Entity<DanhMuc>()
            .HasIndex(dm => dm.TenDanhMuc)
            .IsUnique();
        
        modelBuilder.Entity<Combo>()
            .HasIndex(c => c.TenCombo)
            .IsUnique();
        
        // ========== CẤU HÌNH TÊN BẢNG ==========
        modelBuilder.Entity<NguoiDung>().ToTable("Users");
        modelBuilder.Entity<SanPham>().ToTable("Products");
        modelBuilder.Entity<DanhMuc>().ToTable("Categories");
        modelBuilder.Entity<DonHang>().ToTable("Orders");
        modelBuilder.Entity<ChiTietDonHang>().ToTable("OrderDetails");
        modelBuilder.Entity<Combo>().ToTable("Combos");
        modelBuilder.Entity<ChiTietCombo>().ToTable("ComboDetails");
        
        // ========== SEED DATA ==========
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Tạo các GUID cố định để seed data
        var danhMuc1Id = new Guid("11111111-1111-1111-1111-111111111111");
        var danhMuc2Id = new Guid("22222222-2222-2222-2222-222222222222");
        var danhMuc3Id = new Guid("33333333-3333-3333-3333-333333333333");
        
        var sanPham1Id = new Guid("44444444-4444-4444-4444-444444444444");
        var sanPham2Id = new Guid("55555555-5555-5555-5555-555555555555");
        var sanPham3Id = new Guid("66666666-6666-6666-6666-666666666666");
        var sanPham4Id = new Guid("77777777-7777-7777-7777-777777777777");
        
        var combo1Id = new Guid("88888888-8888-8888-8888-888888888888");
        var combo2Id = new Guid("99999999-9999-9999-9999-999999999999");
        
        var chiTietCombo1Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var chiTietCombo2Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var chiTietCombo3Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc");
        
        var adminId = new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var customerId = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        
        // Seed DanhMuc
        modelBuilder.Entity<DanhMuc>().HasData(
            new DanhMuc 
            { 
                MaDanhMuc = danhMuc1Id, 
                TenDanhMuc = "Cà phê", 
                TrangThai = true,
                NgayTao = new DateTime(2024, 1, 1),
                NgayCapNhat = null
            },
            new DanhMuc 
            { 
                MaDanhMuc = danhMuc2Id, 
                TenDanhMuc = "Trà sữa", 
                TrangThai = true,
                NgayTao = new DateTime(2024, 1, 1),
                NgayCapNhat = null
            },
            new DanhMuc 
            { 
                MaDanhMuc = danhMuc3Id, 
                TenDanhMuc = "Nước ép", 
                TrangThai = true,
                NgayTao = new DateTime(2024, 1, 1),
                NgayCapNhat = null
            }
        );
        
        // Seed SanPham
        modelBuilder.Entity<SanPham>().HasData(
            new SanPham
            {
                MaSanPham = sanPham1Id,
                TenSanPham = "Cà phê đen",
                Gia = 25000,
                HinhAnh = "caphe_den.jpg",
                MoTa = "Cà phê đen nguyên chất",
                ChuDe = "Cà phê",
                TrangThai = true,
                SoLuongTon = 100,
                MaDanhMuc = danhMuc1Id,
                NgayTao = new DateTime(2024, 1, 1),
                NgayCapNhat = null
            },
            new SanPham
            {
                MaSanPham = sanPham2Id,
                TenSanPham = "Cà phê sữa",
                Gia = 30000,
                HinhAnh = "caphe_sua.jpg",
                MoTa = "Cà phê sữa thơm ngon",
                ChuDe = "Cà phê",
                TrangThai = true,
                SoLuongTon = 80,
                MaDanhMuc = danhMuc1Id,
                NgayTao = new DateTime(2024, 1, 1),
                NgayCapNhat = null
            },
            new SanPham
            {
                MaSanPham = sanPham3Id,
                TenSanPham = "Trà sữa trân châu",
                Gia = 35000,
                HinhAnh = "tra_sua_tran_chau.jpg",
                MoTa = "Trà sữa thượng hạng",
                ChuDe = "Trà sữa",
                TrangThai = true,
                SoLuongTon = 120,
                MaDanhMuc = danhMuc2Id,
                NgayTao = new DateTime(2024, 1, 1),
                NgayCapNhat = null
            },
            new SanPham
            {
                MaSanPham = sanPham4Id,
                TenSanPham = "Nước ép cam",
                Gia = 40000,
                HinhAnh = "nuoc_ep_cam.jpg",
                MoTa = "Nước ép cam tươi nguyên chất",
                ChuDe = "Nước ép",
                TrangThai = true,
                SoLuongTon = 60,
                MaDanhMuc = danhMuc3Id,
                NgayTao = new DateTime(2024, 1, 1),
                NgayCapNhat = null
            }
        );
        
        // Seed Combo
        modelBuilder.Entity<Combo>().HasData(
            new Combo
            {
                MaCombo = combo1Id,
                TenCombo = "Combo Sáng Đầy Năng Lượng",
                Gia = 60000,
                MoTa = "Combo hoàn hảo cho buổi sáng",
                HinhAnh = "combo_sang.jpg",
                TrangThai = true,
                NgayTao = new DateTime(2024, 1, 1),
                NgayCapNhat = null
            },
            new Combo
            {
                MaCombo = combo2Id,
                TenCombo = "Combo Chiều Thư Giãn",
                Gia = 80000,
                MoTa = "Combo thư giãn buổi chiều",
                HinhAnh = "combo_chieu.jpg",
                TrangThai = true,
                NgayTao = new DateTime(2024, 1, 1),
                NgayCapNhat = null
            }
        );
        
        // Seed ChiTietCombo
        modelBuilder.Entity<ChiTietCombo>().HasData(
            new ChiTietCombo
            {
                MaChiTietCombo = chiTietCombo1Id,
                MaCombo = combo1Id,
                MaSanPham = sanPham1Id,
                SoLuong = 1
            },
            new ChiTietCombo
            {
                MaChiTietCombo = chiTietCombo2Id,
                MaCombo = combo1Id,
                MaSanPham = sanPham3Id,
                SoLuong = 1
            },
            new ChiTietCombo
            {
                MaChiTietCombo = chiTietCombo3Id,
                MaCombo = combo2Id,
                MaSanPham = sanPham2Id,
                SoLuong = 2
            }
        );
        
        // Seed NguoiDung
        modelBuilder.Entity<NguoiDung>().HasData(
            new NguoiDung
            {
                MaNguoiDung = adminId,
                HoTen = "Admin Quản Trị",
                Email = "admin@nuocuong.com",
                MatKhau = "admin123", // Lưu ý: Trong thực tế nên mã hóa
                SoDienThoai = "0123456789",
                DiaChi = "123 Đường ABC, Quận 1, TP.HCM",
                NgaySinh = new DateTime(1990, 1, 1),
                VaiTro = "Admin",
                TrangThai = true,
                NgayTao = new DateTime(2024, 1, 1)
            },
            new NguoiDung
            {
                MaNguoiDung = customerId,
                HoTen = "Khách Hàng Mẫu",
                Email = "customer@example.com",
                MatKhau = "customer123", // Lưu ý: Trong thực tế nên mã hóa
                SoDienThoai = "0987654321",
                DiaChi = "456 Đường XYZ, Quận 2, TP.HCM",
                NgaySinh = new DateTime(1995, 5, 15),
                VaiTro = "Customer",
                TrangThai = true,
                NgayTao = new DateTime(2024, 1, 1)
            }
        );
    }
}