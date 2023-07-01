using CMS_WebDesignCore.Entities;
using CMS_WebDesignCore.Entities.ConfirmationCodes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions) { } 
        public DbSet<PhatTu> PhatTu { get; set; }
        public DbSet<DaoTrang> DaoTrang { get; set; }
        public DbSet<DonDangKy> DonDangKy { get; set; }
        public DbSet<KieuThanhVien> KieuThanhVien { get; set; }
        public DbSet<PhatTuDaoTrang> PhatTuDaoTrang { get; set; }
        public DbSet<Chua> Chua { get; set; }
        public DbSet<ConfirmationCode> Codes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=ADMIN-PC;Integrated Security=true;Initial Catalog=QuanLyPhatTu_new_v3;MultipleActiveResultSets=True;");

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DonDangKy>(entity =>
            {
                entity.HasOne<PhatTu>(x => x.PhatTu).WithMany(x => x.DonDangKys).HasForeignKey(ddk => ddk.PhatTuId).OnDelete(DeleteBehavior.NoAction);
            });
            //modelBuilder.Entity<DonDangKy>(entity =>
            //{
            //    entity.HasOne(x => x.NguoiXuLy).WithMany().HasForeignKey("PhatTuId").OnDelete(DeleteBehavior.NoAction);
            //});
            modelBuilder.Entity<PhatTuDaoTrang>(entity =>
            {
                entity.HasOne<PhatTu>(x => x.PhatTu).WithMany(x => x.listPhatTuDaoTrang).HasForeignKey(x => x.PhatTuId).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<ConfirmationCode>(entity =>
            {
                entity.HasOne<PhatTu>(x => x.PhatTu).WithOne(x =>x.code).HasForeignKey<ConfirmationCode>(code => code.PhatTuId).OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
