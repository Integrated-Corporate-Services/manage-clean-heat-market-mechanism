﻿// <auto-generated />
using System;
using Desnz.Chmm.Identity.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Desnz.Chmm.Identity.Api.Migrations
{
    [DbContext(typeof(IdentityContext))]
    [Migration("20231124161704_OrganisationFiles")]
    partial class OrganisationFiles
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ChmmRoleChmmUser", b =>
                {
                    b.Property<Guid>("ChmmRolesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChmmUsersId")
                        .HasColumnType("uuid");

                    b.HasKey("ChmmRolesId", "ChmmUsersId");

                    b.HasIndex("ChmmUsersId");

                    b.ToTable("ChmmRoleChmmUser");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.ChmmRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ChmmRoles");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.ChmmUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("JobTitle")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<Guid?>("OrganisationId")
                        .HasColumnType("uuid");

                    b.Property<string>("ResponsibleOfficerOrganisationName")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("TelephoneNumber")
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Name");

                    b.HasIndex("OrganisationId");

                    b.ToTable("ChmmUsers");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.Organisation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ApplicantId")
                        .HasColumnType("uuid");

                    b.Property<string>("CompaniesHouseNumber")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ContactEmail")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ContactName")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ContactTelephoneNumber")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<string[]>("HeatPumpBrands")
                        .HasColumnType("varchar(100)[]");

                    b.Property<bool>("IsFossilFuelBoilerSeller")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsGroupRegistration")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<Guid>("ResponsibleOfficerId")
                        .HasColumnType("uuid");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.OrganisationAddress", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("County")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<string>("LineOne")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("LineTwo")
                        .HasColumnType("varchar(100)");

                    b.Property<Guid>("OrganisationId")
                        .HasColumnType("uuid");

                    b.Property<string>("PostCode")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.ToTable("OrganisationAddresss");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.OrganisationApprovalFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<string>("FileKey")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<Guid>("OrganisationId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.ToTable("OrganisationApprovalFiles");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.OrganisationComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChmmUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<Guid>("OrganisationId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ChmmUserId");

                    b.HasIndex("OrganisationId");

                    b.ToTable("OrganisationComments");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.OrganisationStructureFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<string>("FileKey")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<Guid>("OrganisationId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.ToTable("OrganisationStructureFiles");
                });

            modelBuilder.Entity("ChmmRoleChmmUser", b =>
                {
                    b.HasOne("Desnz.Chmm.Identity.Api.Entities.ChmmRole", null)
                        .WithMany()
                        .HasForeignKey("ChmmRolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Desnz.Chmm.Identity.Api.Entities.ChmmUser", null)
                        .WithMany()
                        .HasForeignKey("ChmmUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.ChmmUser", b =>
                {
                    b.HasOne("Desnz.Chmm.Identity.Api.Entities.Organisation", "Organisation")
                        .WithMany("ChmmUsers")
                        .HasForeignKey("OrganisationId");

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.OrganisationAddress", b =>
                {
                    b.HasOne("Desnz.Chmm.Identity.Api.Entities.Organisation", "Organisation")
                        .WithMany("Addresses")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.OrganisationApprovalFile", b =>
                {
                    b.HasOne("Desnz.Chmm.Identity.Api.Entities.Organisation", "Organisation")
                        .WithMany("OrganisationApprovalFiles")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.OrganisationComment", b =>
                {
                    b.HasOne("Desnz.Chmm.Identity.Api.Entities.ChmmUser", "ChmmUser")
                        .WithMany("Comments")
                        .HasForeignKey("ChmmUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Desnz.Chmm.Identity.Api.Entities.Organisation", "Organisation")
                        .WithMany("Comments")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChmmUser");

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.OrganisationStructureFile", b =>
                {
                    b.HasOne("Desnz.Chmm.Identity.Api.Entities.Organisation", "Organisation")
                        .WithMany("OrganisationStructureFiles")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.ChmmUser", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("Desnz.Chmm.Identity.Api.Entities.Organisation", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("ChmmUsers");

                    b.Navigation("Comments");

                    b.Navigation("OrganisationApprovalFiles");

                    b.Navigation("OrganisationStructureFiles");
                });
#pragma warning restore 612, 618
        }
    }
}
