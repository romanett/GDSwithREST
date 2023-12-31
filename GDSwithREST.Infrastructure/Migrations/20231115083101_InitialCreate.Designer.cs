﻿// <auto-generated />
using System;
using GDSwithREST.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GDSwithREST.Infrastructure.Migrations
{
    [DbContext(typeof(GdsDbContext))]
    [Migration("20231115083101_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GDSwithREST.Domain.Entities.Application", b =>
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
                        .HasColumnType("varbinary(max)");

                    b.Property<int?>("HttpsTrustListId")
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
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("HttpsTrustListId")
                        .HasDatabaseName("IX_FK_Applications_HttpsTrustListId");

                    b.HasIndex("TrustListId")
                        .HasDatabaseName("IX_FK_Applications_TrustListId");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("GDSwithREST.Domain.Entities.ApplicationName", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ApplicationId")
                        .HasColumnType("int");

                    b.Property<string>("Locale")
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

            modelBuilder.Entity("GDSwithREST.Domain.Entities.CertificateRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ApplicationId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("AuthorityId")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CertificateGroupId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<byte[]>("CertificateSigningRequest")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("CertificateTypeId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("DomainNames")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PrivateKeyFormat")
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("PrivateKeyPassword")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("RequestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("SubjectName")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId")
                        .HasDatabaseName("IX_FK_CertificateRequests_Applications");

                    b.ToTable("CertificateRequests");
                });

            modelBuilder.Entity("GDSwithREST.Domain.Entities.CertificateStore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthorityId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.ToTable("CertificateStores");
                });

            modelBuilder.Entity("GDSwithREST.Domain.Entities.ServerEndpoint", b =>
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

            modelBuilder.Entity("GDSwithREST.Domain.Entities.Application", b =>
                {
                    b.HasOne("GDSwithREST.Domain.Entities.CertificateStore", "HttpsTrustList")
                        .WithMany("ApplicationsHttpsTrustList")
                        .HasForeignKey("HttpsTrustListId")
                        .IsRequired()
                        .HasConstraintName("FK_Applications_HttpsTrustListId");

                    b.HasOne("GDSwithREST.Domain.Entities.CertificateStore", "TrustList")
                        .WithMany("ApplicationsTrustList")
                        .HasForeignKey("TrustListId")
                        .IsRequired()
                        .HasConstraintName("FK_Applications_TrustListId");

                    b.Navigation("HttpsTrustList");

                    b.Navigation("TrustList");
                });

            modelBuilder.Entity("GDSwithREST.Domain.Entities.ApplicationName", b =>
                {
                    b.HasOne("GDSwithREST.Domain.Entities.Application", "Application")
                        .WithMany("ApplicationNames")
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ApplicationNames_ApplicationId");

                    b.Navigation("Application");
                });

            modelBuilder.Entity("GDSwithREST.Domain.Entities.CertificateRequest", b =>
                {
                    b.HasOne("GDSwithREST.Domain.Entities.Application", "Application")
                        .WithMany("CertificateRequests")
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_CertificateRequests_Applications");

                    b.Navigation("Application");
                });

            modelBuilder.Entity("GDSwithREST.Domain.Entities.ServerEndpoint", b =>
                {
                    b.HasOne("GDSwithREST.Domain.Entities.Application", "Application")
                        .WithMany("ServerEndpoints")
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ServerEndpoints_ApplicationId");

                    b.Navigation("Application");
                });

            modelBuilder.Entity("GDSwithREST.Domain.Entities.Application", b =>
                {
                    b.Navigation("ApplicationNames");

                    b.Navigation("CertificateRequests");

                    b.Navigation("ServerEndpoints");
                });

            modelBuilder.Entity("GDSwithREST.Domain.Entities.CertificateStore", b =>
                {
                    b.Navigation("ApplicationsHttpsTrustList");

                    b.Navigation("ApplicationsTrustList");
                });
#pragma warning restore 612, 618
        }
    }
}
