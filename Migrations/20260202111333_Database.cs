using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_WebBanNuocUong.Migrations
{
    /// <inheritdoc />
    public partial class Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    MaDanhMuc = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    TenCombo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    TenSanPham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChuDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaDanhMuc = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DanhMucMaDanhMuc = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.MaSanPham);
                    table.ForeignKey(
                        name: "FK_Products_Categories_DanhMucMaDanhMuc",
                        column: x => x.DanhMucMaDanhMuc,
                        principalTable: "Categories",
                        principalColumn: "MaDanhMuc");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    MaDonHang = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaNguoiDung = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiDungMaNguoiDung = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.MaDonHang);
                    table.ForeignKey(
                        name: "FK_Orders_Users_NguoiDungMaNguoiDung",
                        column: x => x.NguoiDungMaNguoiDung,
                        principalTable: "Users",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "ComboDetails",
                columns: table => new
                {
                    MaChiTietCombo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaCombo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComboMaCombo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaSanPham = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SanPhamMaSanPham = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboDetails", x => x.MaChiTietCombo);
                    table.ForeignKey(
                        name: "FK_ComboDetails_Combos_ComboMaCombo",
                        column: x => x.ComboMaCombo,
                        principalTable: "Combos",
                        principalColumn: "MaCombo");
                    table.ForeignKey(
                        name: "FK_ComboDetails_Products_SanPhamMaSanPham",
                        column: x => x.SanPhamMaSanPham,
                        principalTable: "Products",
                        principalColumn: "MaSanPham");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    MaChiTietDonHang = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaDonHang = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonHangMaDonHang = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaSanPham = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SanPhamMaSanPham = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.MaChiTietDonHang);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_DonHangMaDonHang",
                        column: x => x.DonHangMaDonHang,
                        principalTable: "Orders",
                        principalColumn: "MaDonHang");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_SanPhamMaSanPham",
                        column: x => x.SanPhamMaSanPham,
                        principalTable: "Products",
                        principalColumn: "MaSanPham");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComboDetails_ComboMaCombo",
                table: "ComboDetails",
                column: "ComboMaCombo");

            migrationBuilder.CreateIndex(
                name: "IX_ComboDetails_SanPhamMaSanPham",
                table: "ComboDetails",
                column: "SanPhamMaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_DonHangMaDonHang",
                table: "OrderDetails",
                column: "DonHangMaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_SanPhamMaSanPham",
                table: "OrderDetails",
                column: "SanPhamMaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_NguoiDungMaNguoiDung",
                table: "Orders",
                column: "NguoiDungMaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DanhMucMaDanhMuc",
                table: "Products",
                column: "DanhMucMaDanhMuc");
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
