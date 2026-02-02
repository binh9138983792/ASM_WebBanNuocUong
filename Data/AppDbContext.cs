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
        // Cấu hình Fluent API để tránh lỗi làm tròn tiền
        modelBuilder.Entity<SanPham>().Property(p => p.Gia).HasPrecision(18, 2);
        modelBuilder.Entity<Combo>().Property(c => c.Gia).HasPrecision(18, 2);
        modelBuilder.Entity<ChiTietDonHang>().Property(od => od.Gia).HasPrecision(18, 2);
        
        // Cấu hình tên bảng (nếu muốn giữ tên tiếng Anh cho bảng trong database)
        modelBuilder.Entity<NguoiDung>().ToTable("Users");
        modelBuilder.Entity<SanPham>().ToTable("Products");
        modelBuilder.Entity<DanhMuc>().ToTable("Categories");
        modelBuilder.Entity<DonHang>().ToTable("Orders");
        modelBuilder.Entity<ChiTietDonHang>().ToTable("OrderDetails");
        modelBuilder.Entity<Combo>().ToTable("Combos");
        modelBuilder.Entity<ChiTietCombo>().ToTable("ComboDetails");
    }
}