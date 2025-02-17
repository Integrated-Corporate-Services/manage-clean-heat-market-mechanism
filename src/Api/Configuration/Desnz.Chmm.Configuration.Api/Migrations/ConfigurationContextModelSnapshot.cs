﻿// <auto-generated />
using System;
using Desnz.Chmm.Configuration.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Desnz.Chmm.Configuration.Api.Migrations
{
    [DbContext(typeof(ConfigurationContext))]
    partial class ConfigurationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<Guid>("AlternativeSystemFuelTypeWeightingValueId")
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

                    b.HasKey("Id");

                    b.HasIndex("AlternativeSystemFuelTypeWeightingValueId");

                    b.HasIndex("CreditWeightingId");

                    b.ToTable("AlternativeSystemFuelTypeWeightings");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.AlternativeSystemFuelTypeWeightingValue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal");

                    b.HasKey("Id");

                    b.ToTable("AlternativeSystemFuelTypeWeightingValues");
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

                    b.HasIndex("SchemeYearId")
                        .IsUnique();

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

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.ObligationCalculations", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<decimal>("CreditCarryOverPercentage")
                        .HasColumnType("numeric");

                    b.Property<int>("GasBoilerSalesThreshold")
                        .HasColumnType("int");

                    b.Property<int>("GasCreditsCap")
                        .HasColumnType("int");

                    b.Property<int>("OilBoilerSalesThreshold")
                        .HasColumnType("int");

                    b.Property<int>("OilCreditsCap")
                        .HasColumnType("int");

                    b.Property<decimal>("PercentageCap")
                        .HasColumnType("decimal");

                    b.Property<Guid>("SchemeYearId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("TargetMultiplier")
                        .HasColumnType("decimal");

                    b.Property<decimal>("TargetRate")
                        .HasColumnType("decimal");

                    b.HasKey("Id");

                    b.HasIndex("SchemeYearId")
                        .IsUnique();

                    b.ToTable("ObligationCalculations");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.SchemeYear", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("BoilerSalesSubmissionEndDate")
                        .HasColumnType("date");

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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<Guid?>("PreviousSchemeYearId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("SurrenderDayDate")
                        .HasColumnType("date");

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
                    b.HasOne("Desnz.Chmm.Configuration.Api.Entities.AlternativeSystemFuelTypeWeightingValue", "AlternativeSystemFuelTypeWeightingValue")
                        .WithMany("AlternativeSystemFuelTypeWeightings")
                        .HasForeignKey("AlternativeSystemFuelTypeWeightingValueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Desnz.Chmm.Configuration.Api.Entities.CreditWeighting", "CreditWeighting")
                        .WithMany("AlternativeSystemFuelTypeWeightings")
                        .HasForeignKey("CreditWeightingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AlternativeSystemFuelTypeWeightingValue");

                    b.Navigation("CreditWeighting");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.CreditWeighting", b =>
                {
                    b.HasOne("Desnz.Chmm.Configuration.Api.Entities.SchemeYear", "SchemeYear")
                        .WithOne("CreditWeightings")
                        .HasForeignKey("Desnz.Chmm.Configuration.Api.Entities.CreditWeighting", "SchemeYearId")
                        .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.ObligationCalculations", b =>
                {
                    b.HasOne("Desnz.Chmm.Configuration.Api.Entities.SchemeYear", "SchemeYear")
                        .WithOne("ObligationCalculations")
                        .HasForeignKey("Desnz.Chmm.Configuration.Api.Entities.ObligationCalculations", "SchemeYearId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("SchemeYear");
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

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.AlternativeSystemFuelTypeWeightingValue", b =>
                {
                    b.Navigation("AlternativeSystemFuelTypeWeightings");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.CreditWeighting", b =>
                {
                    b.Navigation("AlternativeSystemFuelTypeWeightings");

                    b.Navigation("HeatPumpTechnologyTypeWeightings");
                });

            modelBuilder.Entity("Desnz.Chmm.Configuration.Api.Entities.SchemeYear", b =>
                {
                    b.Navigation("CreditWeightings");

                    b.Navigation("ObligationCalculations");

                    b.Navigation("Quarters");
                });
#pragma warning restore 612, 618
        }
    }
}
