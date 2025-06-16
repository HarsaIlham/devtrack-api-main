using Microsoft.EntityFrameworkCore;
using devtrack.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace devtrack.AppDBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MandorProject> MandorProjects { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Riwayat> Riwayats { get; set; }
        public DbSet<MandorProjectProject> MandorProjectProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔧 Mapping nama tabel & kolom
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");
                entity.HasKey(r => r.RoleId);
                entity.Property(r => r.RoleId).HasColumnName("role_id");
                entity.Property(r => r.RoleName).HasColumnName("role");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.UserId).HasColumnName("user_id");
                entity.Property(u => u.Nama).HasColumnName("nama");
                entity.Property(u => u.Email).HasColumnName("email");
                entity.Property(u => u.Password).HasColumnName("password");
                entity.Property(u => u.RoleId).HasColumnName("role");
                entity.Property(u => u.Alamat).HasColumnName("alamat");
                entity.Property(u => u.No_hp).HasColumnName("no_hp");
                entity.Property(u => u.foto).HasColumnName("foto");
                entity.Property(u => u.Is_active).HasColumnName("is_active");


                entity.HasOne(u => u.Role)
                      .WithMany()
                      .HasForeignKey(u => u.RoleId);
            });

            modelBuilder.Entity<MandorProject>(entity =>
            {
                entity.ToTable("mandor_project");
                entity.HasKey(mp => mp.MandorProyekId);
                entity.Property(mp => mp.MandorProyekId).HasColumnName("mandor_proyek_id");
                entity.Property(mp => mp.UserId).HasColumnName("user_id");
                entity.Property(mp => mp.IsWorking).HasColumnName("is_working");

                entity.HasOne(mp => mp.User)
                      .WithMany()
                      .HasForeignKey(mp => mp.UserId);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("project");
                entity.HasKey(p => p.ProjectId);
                entity.Property(p => p.ProjectId).HasColumnName("project_id");
                entity.Property(p => p.NamaProject).HasColumnName("nama_project");
                entity.Property(p => p.Lokasi).HasColumnName("lokasi");
                entity.Property(p => p.Deadline).HasColumnName("deadline");
                entity.Property(p => p.Status).HasColumnName("status");
                entity.Property(p => p.Foto).HasColumnName("foto");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("laporan_progres");
                entity.HasKey(r => r.LaporanId);
                entity.Property(r => r.LaporanId).HasColumnName("laporan_id");
                entity.Property(r => r.Tanggal).HasColumnName("tanggal");
                entity.Property(r => r.Deskripsi).HasColumnName("deskripsi");
                entity.Property(r => r.Material).HasColumnName("material");
                entity.Property(r => r.JumlahPekerja).HasColumnName("jumlah_pekerja");
                entity.Property(r => r.Kendala).HasColumnName("kendala");
                entity.Property(r => r.Foto).HasColumnName("foto");
                entity.Property(r => r.ProjectId).HasColumnName("project_id");

                entity.HasOne(r => r.Project)
                      .WithMany()
                      .HasForeignKey(r => r.ProjectId);
            });


            modelBuilder.Entity<Riwayat>(entity =>
            {
                entity.ToTable("riwayat");
                entity.HasKey(r => r.RiwayatId);
                entity.Property(r => r.RiwayatId).HasColumnName("riwayat_id");
                entity.Property(r => r.ProjectId).HasColumnName("project_id");

                entity.HasOne(r => r.Project)
                      .WithMany()
                      .HasForeignKey(r => r.ProjectId);
            });

            modelBuilder.Entity<MandorProjectProject>(entity =>
            {
                entity.ToTable("mandor_project_project");
                entity.HasKey(mpp => mpp.Id);
                entity.Property(mpp => mpp.Id).HasColumnName("id");
                entity.Property(mpp => mpp.MandorProyekId).HasColumnName("mandor_proyek_id");
                entity.Property(mpp => mpp.ProjectId).HasColumnName("project_id");

                entity.HasOne(mpp => mpp.MandorProject)
                      .WithMany()
                      .HasForeignKey(mpp => mpp.MandorProyekId);

                entity.HasOne(mpp => mpp.Project)
                      .WithMany()
                      .HasForeignKey(mpp => mpp.ProjectId);
            });
        }
    }
}
