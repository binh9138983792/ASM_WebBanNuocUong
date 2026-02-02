using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASM_WebBanNuocUong.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    MaDanhMuc = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.MaDanhMuc);
                });

            migrationBuilder.CreateTable(
                name: "Combos",
                columns: table => new
                {
                    MaCombo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenCombo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combos", x => x.MaCombo);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    MaNguoiDung = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Customer"),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.MaNguoiDung);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    MaSanPham = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenSanPham = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChuDe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaDanhMuc = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.MaSanPham);
                    table.ForeignKey(
                        name: "FK_Products_Categories_MaDanhMuc",
                        column: x => x.MaDanhMuc,
                        principalTable: "Categories",
                        principalColumn: "MaDanhMuc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    MaDonHang = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    NgayGiao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Chờ xác nhận"),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaNguoiDung = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.MaDonHang);
                    table.ForeignKey(
                        name: "FK_Orders_Users_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "Users",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComboDetails",
                columns: table => new
                {
                    MaChiTietCombo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaCombo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaSanPham = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboDetails", x => x.MaChiTietCombo);
                    table.ForeignKey(
                        name: "FK_ComboDetails_Combos_MaCombo",
                        column: x => x.MaCombo,
                        principalTable: "Combos",
                        principalColumn: "MaCombo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComboDetails_Products_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "Products",
                        principalColumn: "MaSanPham",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    MaChiTietDonHang = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaDonHang = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaSanPham = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaCombo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LoaiSanPham = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.MaChiTietDonHang);
                    table.CheckConstraint("CK_ChiTietDonHang_LoaiSanPham", "(LoaiSanPham = 'SanPham' AND MaSanPham IS NOT NULL) OR (LoaiSanPham = 'Combo' AND MaCombo IS NOT NULL)");
                    table.CheckConstraint("CK_ChiTietDonHang_SanPhamOrCombo", "([MaSanPham] IS NOT NULL AND [MaCombo] IS NULL) OR ([MaSanPham] IS NULL AND [MaCombo] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Combos_MaCombo",
                        column: x => x.MaCombo,
                        principalTable: "Combos",
                        principalColumn: "MaCombo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_MaDonHang",
                        column: x => x.MaDonHang,
                        principalTable: "Orders",
                        principalColumn: "MaDonHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "Products",
                        principalColumn: "MaSanPham",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "MaDanhMuc", "NgayCapNhat", "NgayTao", "TenDanhMuc", "TrangThai" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cà phê", true },
                    { new Guid("22222222-2222-2222-2222-222222222222"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trà sữa", true },
                    { new Guid("33333333-3333-3333-3333-333333333333"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nước ép", true }
                });

            migrationBuilder.InsertData(
                table: "Combos",
                columns: new[] { "MaCombo", "Gia", "HinhAnh", "MoTa", "NgayCapNhat", "NgayTao", "TenCombo", "TrangThai" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888888"), 60000m, "combo_sang.jpg", "Combo hoàn hảo cho buổi sáng", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Combo Sáng Đầy Năng Lượng", true },
                    { new Guid("99999999-9999-9999-9999-999999999999"), 80000m, "combo_chieu.jpg", "Combo thư giãn buổi chiều", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Combo Chiều Thư Giãn", true }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "MaNguoiDung", "DiaChi", "Email", "HoTen", "MatKhau", "NgaySinh", "NgayTao", "SoDienThoai", "TrangThai", "VaiTro" },
                values: new object[,]
                {
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "123 Đường ABC, Quận 1, TP.HCM", "admin@nuocuong.com", "Admin Quản Trị", "admin123", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "0123456789", true, "Admin" },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "456 Đường XYZ, Quận 2, TP.HCM", "customer@example.com", "Khách Hàng Mẫu", "customer123", new DateTime(1995, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "0987654321", true, "Customer" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "MaSanPham", "ChuDe", "Gia", "HinhAnh", "MaDanhMuc", "MoTa", "NgayCapNhat", "NgayTao", "SoLuongTon", "TenSanPham", "TrangThai" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Cà phê", 25000m, "caphe_den.jpg", new Guid("11111111-1111-1111-1111-111111111111"), "Cà phê đen nguyên chất", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 100, "Cà phê đen", true },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Cà phê", 30000m, "caphe_sua.jpg", new Guid("11111111-1111-1111-1111-111111111111"), "Cà phê sữa thơm ngon", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 80, "Cà phê sữa", true },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Trà sữa", 35000m, "tra_sua_tran_chau.jpg", new Guid("22222222-2222-2222-2222-222222222222"), "Trà sữa thượng hạng", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 120, "Trà sữa trân châu", true },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "Nước ép", 40000m, "nuoc_ep_cam.jpg", new Guid("33333333-3333-3333-3333-333333333333"), "Nước ép cam tươi nguyên chất", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 60, "Nước ép cam", true }
                });

            migrationBuilder.InsertData(
                table: "ComboDetails",
                columns: new[] { "MaChiTietCombo", "MaCombo", "MaSanPham", "SoLuong" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("88888888-8888-8888-8888-888888888888"), new Guid("44444444-4444-4444-4444-444444444444"), 1 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("88888888-8888-8888-8888-888888888888"), new Guid("66666666-6666-6666-6666-666666666666"), 1 },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("99999999-9999-9999-9999-999999999999"), new Guid("55555555-5555-5555-5555-555555555555"), 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenDanhMuc",
                table: "Categories",
                column: "TenDanhMuc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComboDetails_MaCombo",
                table: "ComboDetails",
                column: "MaCombo");

            migrationBuilder.CreateIndex(
                name: "IX_ComboDetails_MaSanPham",
                table: "ComboDetails",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_Combos_TenCombo",
                table: "Combos",
                column: "TenCombo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_MaCombo",
                table: "OrderDetails",
                column: "MaCombo");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_MaDonHang",
                table: "OrderDetails",
                column: "MaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_MaSanPham",
                table: "OrderDetails",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MaNguoiDung",
                table: "Orders",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_Products_MaDanhMuc",
                table: "Products",
                column: "MaDanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenSanPham",
                table: "Products",
                column: "TenSanPham",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComboDetails");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Combos");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
