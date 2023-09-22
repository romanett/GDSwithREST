﻿using System;
using GDSwithREST.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MinAPI.Data
{
    public partial class GdsdbContext : DbContext
    {
        public GdsdbContext()
        {
        }

        public GdsdbContext(DbContextOptions<GdsdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationNames> ApplicationNames { get; set; }
        public virtual DbSet<Applications> Applications { get; set; }
        public virtual DbSet<CertificateRequests> CertificateRequests { get; set; }
        public virtual DbSet<CertificateStores> CertificateStores { get; set; }
        public virtual DbSet<ServerEndpoints> ServerEndpoints { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationNames>(entity =>
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

            modelBuilder.Entity<Applications>(entity =>
            {
                entity.HasIndex(e => e.HttpsTrustListId)
                    .HasDatabaseName("IX_FK_Applications_HttpsTrustListId");

                entity.HasIndex(e => e.TrustListId)
                    .HasDatabaseName("IX_FK_Applications_TrustListId");

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

                entity.HasOne(d => d.HttpsTrustList)
                    .WithMany(p => p.ApplicationsHttpsTrustList)
                    .HasForeignKey(d => d.HttpsTrustListId)
                    .HasConstraintName("FK_Applications_HttpsTrustListId")
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TrustList)
                    .WithMany(p => p.ApplicationsTrustList)
                    .HasForeignKey(d => d.TrustListId)
                    .HasConstraintName("FK_Applications_TrustListId")
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CertificateRequests>(entity =>
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

            modelBuilder.Entity<CertificateStores>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AuthorityId)
                    .HasMaxLength(50)
                    .IsRequired(false);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<ServerEndpoints>(entity =>
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
}
