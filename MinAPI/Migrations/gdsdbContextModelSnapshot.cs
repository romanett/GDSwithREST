﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MinAPI.Data;

#nullable disable

namespace MinAPI.Migrations
{
    [DbContext(typeof(gdsdbContext))]
    partial class gdsdbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MinAPI.Models.ApplicationNames", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ApplicationId")
                        .HasColumnType("int");

                    b.Property<string>("Locale")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId")
                        .HasDatabaseName("IX_FK_ApplicationNames_ApplicationId");

                    b.ToTable("ApplicationNames");
                });

            modelBuilder.Entity("MinAPI.Models.Applications", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApplicationName")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int>("ApplicationType")
                        .HasColumnType("int");

                    b.Property<string>("ApplicationUri")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<byte[]>("Certificate")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("HttpsCertificate")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int?>("HttpsTrustListId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("ProductUri")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("ServerCapabilities")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int?>("TrustListId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("HttpsTrustListId")
                        .HasDatabaseName("IX_FK_Applications_HttpsTrustListId");

                    b.HasIndex("TrustListId")
                        .HasDatabaseName("IX_FK_Applications_TrustListId");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("MinAPI.Models.CertificateRequests", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ApplicationId")
                        .HasColumnType("int");

                    b.Property<string>("AuthorityId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CertificateGroupId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<byte[]>("CertificateSigningRequest")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("CertificateTypeId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("DomainNames")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PrivateKeyFormat")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("PrivateKeyPassword")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("RequestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("SubjectName")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId")
                        .HasDatabaseName("IX_FK_CertificateRequests_Applications");

                    b.ToTable("CertificateRequests");
                });

            modelBuilder.Entity("MinAPI.Models.CertificateStores", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthorityId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.ToTable("CertificateStores");
                });

            modelBuilder.Entity("MinAPI.Models.ServerEndpoints", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ApplicationId")
                        .HasColumnType("int");

                    b.Property<string>("DiscoveryUrl")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId")
                        .HasDatabaseName("IX_FK_ServerEndpoints_ApplicationId");

                    b.ToTable("ServerEndpoints");
                });

            modelBuilder.Entity("MinAPI.Models.ApplicationNames", b =>
                {
                    b.HasOne("MinAPI.Models.Applications", "Application")
                        .WithMany("ApplicationNames")
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ApplicationNames_ApplicationId");

                    b.Navigation("Application");
                });

            modelBuilder.Entity("MinAPI.Models.Applications", b =>
                {
                    b.HasOne("MinAPI.Models.CertificateStores", "HttpsTrustList")
                        .WithMany("ApplicationsHttpsTrustList")
                        .HasForeignKey("HttpsTrustListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Applications_HttpsTrustListId");

                    b.HasOne("MinAPI.Models.CertificateStores", "TrustList")
                        .WithMany("ApplicationsTrustList")
                        .HasForeignKey("TrustListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Applications_TrustListId");

                    b.Navigation("HttpsTrustList");

                    b.Navigation("TrustList");
                });

            modelBuilder.Entity("MinAPI.Models.CertificateRequests", b =>
                {
                    b.HasOne("MinAPI.Models.Applications", "Application")
                        .WithMany("CertificateRequests")
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_CertificateRequests_Applications");

                    b.Navigation("Application");
                });

            modelBuilder.Entity("MinAPI.Models.ServerEndpoints", b =>
                {
                    b.HasOne("MinAPI.Models.Applications", "Application")
                        .WithMany("ServerEndpoints")
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ServerEndpoints_ApplicationId");

                    b.Navigation("Application");
                });

            modelBuilder.Entity("MinAPI.Models.Applications", b =>
                {
                    b.Navigation("ApplicationNames");

                    b.Navigation("CertificateRequests");

                    b.Navigation("ServerEndpoints");
                });

            modelBuilder.Entity("MinAPI.Models.CertificateStores", b =>
                {
                    b.Navigation("ApplicationsHttpsTrustList");

                    b.Navigation("ApplicationsTrustList");
                });
#pragma warning restore 612, 618
        }
    }
}
