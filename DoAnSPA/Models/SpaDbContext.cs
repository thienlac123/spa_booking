// DoAnSPA/Data/SpaDbContext.cs
using Microsoft.EntityFrameworkCore;
using DoAnSPA.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DoAnSPA.Data
{
    public class SpaDbContext : IdentityDbContext<SpaUser>
    {
        public SpaDbContext(DbContextOptions<SpaDbContext> options) : base(options) { }

        // Bảng hiện có
         // sẽ ngừng dùng như “tài khoản”
        public DbSet<DichVu> DichVus { get; set; }
        public DbSet<LichHen> LichHens { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<PhanHoi> PhanHois { get; set; }
        public DbSet<LienHe> LienHes { get; set; }
        public DbSet<ComboDichVu> ComboDichVus { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<GioHangItem> GioHangItems { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<DonHangChiTiet> DonHangChiTiets { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        // MỚI:
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1) Gọi base TRƯỚC
            base.OnModelCreating(modelBuilder);

            // 2) Many-to-many ComboDichVu–DichVu
            modelBuilder.Entity<ComboDichVu>()
                .HasMany(c => c.DichVus)
                .WithMany(d => d.Combos)
                .UsingEntity<Dictionary<string, object>>(
                    "ComboDichVuDichVu",
                    j => j.HasOne<DichVu>().WithMany().HasForeignKey("DichVuId"),
                    j => j.HasOne<ComboDichVu>().WithMany().HasForeignKey("ComboDichVuId")
                );

            // 3) CustomerProfile 1–1 SpaUser
            modelBuilder.Entity<CustomerProfile>(e =>
            {
                e.ToTable("CustomerProfile");
                e.HasKey(x => x.UserId);

                e.HasOne(x => x.User)
                 .WithOne(u => u.CustomerProfile)
                 .HasForeignKey<CustomerProfile>(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.FullName).HasMaxLength(200).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(20).IsRequired();
                e.Property(x => x.Address).HasMaxLength(255);
                e.HasIndex(x => x.Phone).HasDatabaseName("IX_CustomerProfile_Phone");
            });

            // 4) LichHen: tất cả cấu hình gom 1 chỗ
            modelBuilder.Entity<LichHen>(e =>
            {
                // FK tới SpaUser
                e.HasOne(x => x.Customer)
                 .WithMany()
                 .HasForeignKey(x => x.customerId)
                 .OnDelete(DeleteBehavior.Restrict);

                // FK tùy chọn tới DichVu
                e.HasOne(x => x.DichVu)
                 .WithMany()
                 .HasForeignKey(x => x.dichVuId)
                 .OnDelete(DeleteBehavior.Restrict);

                // FK tùy chọn tới ComboDichVu
                e.HasOne(x => x.ComboDichVu)
                 .WithMany()
                 .HasForeignKey(x => x.comboId)
                 .OnDelete(DeleteBehavior.Restrict);

                // XOR: đúng 1 trong 2
                e.ToTable(tb => tb.HasCheckConstraint(
                    "CK_LichHen_DichVu_Combo_XOR",
                    @"(CASE WHEN [dichVuId] IS NOT NULL THEN 1 ELSE 0 END
              + CASE WHEN [comboId]  IS NOT NULL THEN 1 ELSE 0 END) = 1"));

                // Index phục vụ tra cứu theo khách + thời gian
                e.HasIndex(x => new { x.customerId, x.ngayHen, x.gioHen })
                 .HasDatabaseName("IX_LichHen_Customer_Time");

               
            });
            // Tra cứu nhanh theo thời gian (không unique)
            modelBuilder.Entity<LichHen>()
                .HasIndex(x => new { x.ngayHen, x.gioHen })
                .HasDatabaseName("IX_LichHen_Time");

            // ❗Unique theo KHÁCH HÀNG: 1 khách không được có 2 lịch cùng khung giờ (trừ Đã huỷ)
            modelBuilder.Entity<LichHen>()
                .HasIndex(x => new { x.customerId, x.ngayHen, x.gioHen })
                .IsUnique()
                .HasFilter("[trangThai] <> 'DaHuy'");

            // ❗Unique theo NHÂN VIÊN (nếu có chọn): 1 NV không được bị trùng ca
            modelBuilder.Entity<LichHen>()
                .HasIndex(x => new { x.nhanVienId, x.ngayHen, x.gioHen })
                .IsUnique()
                .HasFilter("[nhanVienId] IS NOT NULL AND [trangThai] <> 'DaHuy'");

        }



    }
}
