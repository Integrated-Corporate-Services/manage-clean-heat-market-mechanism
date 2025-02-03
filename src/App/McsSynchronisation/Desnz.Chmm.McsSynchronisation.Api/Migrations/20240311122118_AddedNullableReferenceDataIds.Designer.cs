﻿// <auto-generated />
using System;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Desnz.Chmm.McsSynchronisation.Api.Migrations
{
    [DbContext(typeof(McsSynchronisationContext))]
    [Migration("20240311122118_AddedNullableReferenceDataIds")]
    partial class AddedNullableReferenceDataIds
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.AirTypeTechnology", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("AirTypeTechnologies");
                });

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.AlternativeSystemFuelType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("AlternativeSystemFuelTypes");
                });

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.AlternativeSystemType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("AlternativeSystemTypes");
                });

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.HeatPumpInstallation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int?>("AirTypeTechnologyId")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "AirTypeTechnologyID");

                    b.Property<int?>("AlternativeHeatingAgeId")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "AlternativeHeatingAgeID");

                    b.Property<int?>("AlternativeHeatingFuelId")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "AlternativeHeatingFuelID");

                    b.Property<int?>("AlternativeHeatingSystemId")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "AlternativeHeatingSystemID");

                    b.Property<int?>("CertificatesCount")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "HowManyCertificates");

                    b.Property<DateTime?>("CommissioningDate")
                        .HasColumnType("timestamptz")
                        .HasAnnotation("Relational:JsonPropertyName", "CommissioningDate");

                    b.Property<Guid>("InstallationRequestId")
                        .HasColumnType("uuid");

                    b.Property<bool?>("IsAlternativeHeatingSystemPresent")
                        .HasColumnType("boolean")
                        .HasAnnotation("Relational:JsonPropertyName", "IsAlternativeHeatingSystemPresent");

                    b.Property<bool?>("IsHybrid")
                        .HasColumnType("boolean")
                        .HasAnnotation("Relational:JsonPropertyName", "IsHybrid");

                    b.Property<int?>("IsNewBuildId")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "IsNewBuildID");

                    b.Property<int?>("MidId")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "ID");

                    b.Property<string>("Mpan")
                        .HasColumnType("varchar(200)")
                        .HasAnnotation("Relational:JsonPropertyName", "MPAN");

                    b.Property<int?>("RenewableSystemDesignId")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "RenewableSystemDesignID");

                    b.Property<int?>("TechnologyTypeId")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "TechnologyTypeID");

                    b.Property<decimal?>("TotalCapacity")
                        .HasColumnType("decimal")
                        .HasAnnotation("Relational:JsonPropertyName", "TotalCapacity");

                    b.HasKey("Id");

                    b.HasIndex("InstallationRequestId");

                    b.ToTable("HeatPumpInstallations");
                });

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.HeatPumpProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "ID");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasAnnotation("Relational:JsonPropertyName", "Code");

                    b.Property<int>("ManufacturerId")
                        .HasColumnType("integer")
                        .HasAnnotation("Relational:JsonPropertyName", "ManufacturerID");

                    b.Property<string>("ManufacturerName")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasAnnotation("Relational:JsonPropertyName", "Manufacturer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasAnnotation("Relational:JsonPropertyName", "Name");

                    b.HasKey("Id");

                    b.ToTable("HeatPumpProducts");
                });

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.InstallationAge", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("InstallationAges");
                });

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.InstallationRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int[]>("IsNewBuildIds")
                        .HasColumnType("integer[]");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int[]>("TechnologyTypeIds")
                        .HasColumnType("integer[]");

                    b.HasKey("Id");

                    b.ToTable("InstallationRequests");
                });

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.Manufacturer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Manufacturers");
                });

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.NewBuildOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("NewBuildOptions");
                });

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.RenewableSystemDesign", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("RenewableSystemDesigns");
                });

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.TechnologyType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("TechnologyTypes");
                });

            modelBuilder.Entity("HeatPumpInstallationProducts", b =>
                {
                    b.Property<Guid>("InstallationId")
                        .HasColumnType("uuid");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.HasKey("InstallationId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("HeatPumpInstallationProducts");
                });

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.HeatPumpInstallation", b =>
                {
                    b.HasOne("Desnz.Chmm.McsSynchronisation.Api.Entities.InstallationRequest", "InstallationRequest")
                        .WithMany("HeatPumpInstallations")
                        .HasForeignKey("InstallationRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InstallationRequest");
                });

            modelBuilder.Entity("HeatPumpInstallationProducts", b =>
                {
                    b.HasOne("Desnz.Chmm.McsSynchronisation.Api.Entities.HeatPumpInstallation", null)
                        .WithMany()
                        .HasForeignKey("InstallationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Desnz.Chmm.McsSynchronisation.Api.Entities.HeatPumpProduct", null)
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Desnz.Chmm.McsSynchronisation.Api.Entities.InstallationRequest", b =>
                {
                    b.Navigation("HeatPumpInstallations");
                });
#pragma warning restore 612, 618
        }
    }
}
