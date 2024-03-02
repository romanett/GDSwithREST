using GDSwithREST.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GDSwithREST.Infrastructure
{
    public partial class GdsDbContext : DbContext
    {
        public GdsDbContext()
        {
        }

        public GdsDbContext(DbContextOptions<GdsDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationName> ApplicationNames { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<CertificateRequest> CertificateRequests { get; set; }
        public virtual DbSet<ServerEndpoint> ServerEndpoints { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationName>(entity =>
            {
                entity.HasIndex(e => e.ApplicationId)
                    .HasDatabaseName("IX_FK_ApplicationNames_ApplicationId");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Locale)
                    .HasMaxLength(10)
                    .IsRequired(false);

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ApplicationNames)
                    .HasForeignKey(d => d.ApplicationId)
                    .HasConstraintName("FK_ApplicationNames_ApplicationId");
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.Property(e => e.HttpsTrustListId).IsRequired(false);

                entity.Property(e => e.TrustListId).IsRequired(false);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ApplicationName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ApplicationUri)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ProductUri)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ServerCapabilities).HasMaxLength(500);

            });

            modelBuilder.Entity<CertificateRequest>(entity =>
            {
                entity.HasIndex(e => e.ApplicationId)
                    .HasDatabaseName("IX_FK_CertificateRequests_Applications");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AuthorityId)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(e => e.CertificateGroupId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CertificateTypeId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PrivateKeyFormat).HasMaxLength(3);

                entity.Property(e => e.PrivateKeyPassword).HasMaxLength(100);

                entity.Property(e => e.SubjectName).HasMaxLength(1000);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.CertificateRequests)
                    .HasForeignKey(d => d.ApplicationId)
                    .HasConstraintName("FK_CertificateRequests_Applications");
            });

            modelBuilder.Entity<ServerEndpoint>(entity =>
            {
                entity.HasIndex(e => e.ApplicationId)
                    .HasDatabaseName("IX_FK_ServerEndpoints_ApplicationId");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DiscoveryUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ServerEndpoints)
                    .HasForeignKey(d => d.ApplicationId)
                    .HasConstraintName("FK_ServerEndpoints_ApplicationId");
            });
        }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<GdsDbContext>
    {
        public GdsDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
            var connectionString = configuration.GetConnectionString("Default");
            var builder = new DbContextOptionsBuilder<GdsDbContext>();
            builder.UseSqlServer(connectionString);
            return new GdsDbContext(builder.Options);
        }
    }
}
