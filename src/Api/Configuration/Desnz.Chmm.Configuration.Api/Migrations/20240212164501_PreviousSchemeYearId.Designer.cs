﻿// <auto-generated />
using System;
using Desnz.Chmm.Configuration.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Desnz.Chmm.Configuration.Api.Migrations
{
    [DbContext(typeof(ConfigurationContext))]
    [Migration("20240212164501_PreviousSchemeYearId")]
    partial class PreviousSchemeYearId
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

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.AlternativeSystemFuelTypeWeighting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<Guid>("CreditWeightingId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal");

                    b.HasKey("Id");

                    b.HasIndex("CreditWeightingId");

                    b.ToTable("AlternativeSystemFuelTypeWeightings");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.CreditWeighting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<Guid>("SchemeYearId")
                        .HasColumnType("uuid");

                    b.Property<int>("TotalCapacity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SchemeYearId");

                    b.ToTable("CreditWeightings");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.HeatPumpTechnologyTypeWeighting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<Guid>("CreditWeightingId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal");

                    b.HasKey("Id");

                    b.HasIndex("CreditWeightingId");

                    b.ToTable("HeatPumpTechnologyTypeWeightings");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.SchemeYear", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<DateOnly>("CreditGenerationWindowEndDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("CreditGenerationWindowStartDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<int>("GasBoilerSalesThreshold")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<int>("OilBoilerSalesThreshold")
                        .HasColumnType("int");

                    b.Property<Guid?>("PreviousSchemeYearId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<decimal>("TargetRate")
                        .HasColumnType("decimal");

                    b.Property<DateOnly>("TradingWindowEndDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("TradingWindowStartDate")
                        .HasColumnType("date");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SchemeYears");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.SchemeYearQuarter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<Guid>("SchemeYearId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("SchemeYearId");

                    b.ToTable("SchemeYearQuarters");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.AlternativeSystemFuelTypeWeighting", b =>
                {
                    b.HasOne("Desnz.Chmm.Configuration.Api.Entities.CreditWeighting", "CreditWeighting")
                        .WithMany("AlternativeSystemFuelTypeWeightings")
                        .HasForeignKey("CreditWeightingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreditWeighting");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.CreditWeighting", b =>
                {
                    b.HasOne("Desnz.Chmm.Configuration.Api.Entities.SchemeYear", "SchemeYear")
                        .WithMany("CreditWeightings")
                        .HasForeignKey("SchemeYearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SchemeYear");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.HeatPumpTechnologyTypeWeighting", b =>
                {
                    b.HasOne("Desnz.Chmm.Configuration.Api.Entities.CreditWeighting", "CreditWeighting")
                        .WithMany("HeatPumpTechnologyTypeWeightings")
                        .HasForeignKey("CreditWeightingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreditWeighting");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.SchemeYearQuarter", b =>
                {
                    b.HasOne("Desnz.Chmm.Configuration.Api.Entities.SchemeYear", "SchemeYear")
                        .WithMany("Quarters")
                        .HasForeignKey("SchemeYearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SchemeYear");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.CreditWeighting", b =>
                {
                    b.Navigation("AlternativeSystemFuelTypeWeightings");

                    b.Navigation("HeatPumpTechnologyTypeWeightings");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.SchemeYear", b =>
                {
                    b.Navigation("CreditWeightings");

                    b.Navigation("Quarters");
                });
#pragma warning restore 612, 618
        }
    }
}
