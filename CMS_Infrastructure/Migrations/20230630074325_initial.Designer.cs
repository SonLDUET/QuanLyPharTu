﻿// <auto-generated />
using System;
using CMS_Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CMS_Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230630074325_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CMS_WebDesignCore.Entities.Chua", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("CapNhat")
                        .HasColumnType("datetime2");

                    b.Property<string>("DiaChi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("NgayThanhLap")
                        .HasColumnType("datetime2");

                    b.Property<string>("TenChua")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TruTriId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Chua");
                });

            modelBuilder.Entity("CMS_WebDesignCore.Entities.ConfirmationCodes.ConfirmationCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CodeExpireTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("PhatTuId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PhatTuId")
                        .IsUnique()
                        .HasFilter("[PhatTuId] IS NOT NULL");

                    b.ToTable("Codes");
                });

            modelBuilder.Entity("CMS_WebDesignCore.Entities.DaoTrang", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("DaKetThuc")
                        .HasColumnType("bit");

                    b.Property<int>("NguoiChuTriId")
                        .HasColumnType("int");

                    b.Property<string>("NoiDung")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NoiToChuc")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SoThanhVienThamGia")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ThoiGianToChuc")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("DaoTrang");
                });

            modelBuilder.Entity("CMS_WebDesignCore.Entities.DonDangKy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("DaoTrangId")
                        .HasColumnType("int");

                    b.Property<DateTime>("NgayGuiDon")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayXuLy")
                        .HasColumnType("datetime2");

                    b.Property<int?>("NguoiXuLyId")
                        .HasColumnType("int");

                    b.Property<int>("PhatTuId")
                        .HasColumnType("int");

                    b.Property<int>("TrangThaiDon")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DaoTrangId");

                    b.HasIndex("PhatTuId");

                    b.ToTable("DonDangKy");
                });

            modelBuilder.Entity("CMS_WebDesignCore.Entities.KieuThanhVien", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("TenKieu")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("KieuThanhVien");
                });

            modelBuilder.Entity("CMS_WebDesignCore.Entities.PhatTu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<byte[]>("AnhChup")
                        .HasColumnType("varbinary(max)");

                    b.Property<int?>("ChuaId")
                        .HasColumnType("int");

                    b.Property<int?>("CodeId")
                        .HasColumnType("int");

                    b.Property<bool>("DaHoanTuc")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfimed")
                        .HasColumnType("bit");

                    b.Property<int>("GioiTinh")
                        .HasColumnType("int");

                    b.Property<string>("Ho")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsTokenRevoked")
                        .HasColumnType("bit");

                    b.Property<int>("KieuThanhVienId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("NgayCapNhat")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayHoanTuc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("NgaySinh")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayXuatGia")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhapDanh")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SoDienThoai")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ten")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("refreshTokenExpiredTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ChuaId")
                        .IsUnique()
                        .HasFilter("[ChuaId] IS NOT NULL");

                    b.HasIndex("KieuThanhVienId");

                    b.ToTable("PhatTu");
                });

            modelBuilder.Entity("CMS_WebDesignCore.Entities.PhatTuDaoTrang", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("DaThamGia")
                        .HasColumnType("bit");

                    b.Property<int>("DaoTrangId")
                        .HasColumnType("int");

                    b.Property<string>("LyDoKhongThamGia")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PhatTuId")
                        .HasColumnType("int");

                    b.Property<int>("pt-dt")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DaoTrangId");

                    b.HasIndex("PhatTuId");

                    b.HasIndex("pt-dt");

                    b.ToTable("PhatTuDaoTrang");
                });

            modelBuilder.Entity("CMS_WebDesignCore.Entities.ConfirmationCodes.ConfirmationCode", b =>
                {
                    b.HasOne("CMS_WebDesignCore.Entities.PhatTu", "PhatTu")
                        .WithOne("code")
                        .HasForeignKey("CMS_WebDesignCore.Entities.ConfirmationCodes.ConfirmationCode", "PhatTuId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("PhatTu");
                });

            modelBuilder.Entity("CMS_WebDesignCore.Entities.DonDangKy", b =>
                {
                    b.HasOne("CMS_WebDesignCore.Entities.DaoTrang", "daoTrang")
                        .WithMany()
                        .HasForeignKey("DaoTrangId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CMS_WebDesignCore.Entities.PhatTu", "PhatTu")
                        .WithMany("DonDangKys")
                        .HasForeignKey("PhatTuId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("PhatTu");

                    b.Navigation("daoTrang");
                });

            modelBuilder.Entity("CMS_WebDesignCore.Entities.PhatTu", b =>
                {
                    b.HasOne("CMS_WebDesignCore.Entities.Chua", "Chua")
                        .WithOne("ThuTri")
                        .HasForeignKey("CMS_WebDesignCore.Entities.PhatTu", "ChuaId");

                    b.HasOne("CMS_WebDesignCore.Entities.KieuThanhVien", "KieuThanhVien")
                        .WithMany()
                        .HasForeignKey("KieuThanhVienId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chua");

                    b.Navigation("KieuThanhVien");
                });

            modelBuilder.Entity("CMS_WebDesignCore.Entities.PhatTuDaoTrang", b =>
                {
                    b.HasOne("CMS_WebDesignCore.Entities.DaoTrang", "DaoTrang")
                        .WithMany()
                        .HasForeignKey("DaoTrangId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CMS_WebDesignCore.Entities.PhatTu", null)
                        .WithMany("listPhatTuDaoTrang")
                        .HasForeignKey("PhatTuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CMS_WebDesignCore.Entities.PhatTu", "PhatTu")
                        .WithMany()
                        .HasForeignKey("pt-dt")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("DaoTrang");

                    b.Navigation("PhatTu");
                });

            modelBuilder.Entity("CMS_WebDesignCore.Entities.Chua", b =>
                {
                    b.Navigation("ThuTri");
                });

            modelBuilder.Entity("CMS_WebDesignCore.Entities.PhatTu", b =>
                {
                    b.Navigation("DonDangKys");

                    b.Navigation("code");

                    b.Navigation("listPhatTuDaoTrang");
                });
#pragma warning restore 612, 618
        }
    }
}
