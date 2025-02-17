﻿// <auto-generated />
using System;
using Desnz.Chmm.BoilerSales.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Desnz.Chmm.BoilerSales.Api.Migrations
{
    [DbContext(typeof(BoilerSalesContext))]
    [Migration("20240311113354_AddIndexes")]
    partial class AddIndexes
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

            modelBuilder.Entity("Desnz.Chmm.BoilerSales.Api.Entities.AnnualBoilerSales", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<int>("Gas")
                        .HasColumnType("integer");

                    b.Property<int>("Oil")
                        .HasColumnType("integer");

                    b.Property<Guid>("OrganisationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SchemeYearId")
                        .HasColumnType("uuid");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("SchemeYearId");

                    b.HasIndex("OrganisationId", "SchemeYearId");

                    b.ToTable("AnnualBoilerSales");
                });

            modelBuilder.Entity("Desnz.Chmm.BoilerSales.Api.Entities.AnnualBoilerSalesChange", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AnnualBoilerSalesId")
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<int>("NewGas")
                        .HasColumnType("integer");

                    b.Property<int>("NewOil")
                        .HasColumnType("integer");

                    b.Property<string>("NewStatus")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("OldGas")
                        .HasColumnType("integer");

                    b.Property<int?>("OldOil")
                        .HasColumnType("integer");

                    b.Property<string>("OldStatus")
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("AnnualBoilerSalesId");

                    b.ToTable("AnnualBoilerSalesChanges");
                });

            modelBuilder.Entity("Desnz.Chmm.BoilerSales.Api.Entities.AnnualBoilerSalesFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AnnualBoilerSalesId")
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<string>("FileKey")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("AnnualBoilerSalesId");

                    b.ToTable("AnnualBoilerSalesFiles");
                });

            modelBuilder.Entity("Desnz.Chmm.BoilerSales.Api.Entities.QuarterlyBoilerSales", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<int>("Gas")
                        .HasColumnType("integer");

                    b.Property<int>("Oil")
                        .HasColumnType("integer");

                    b.Property<Guid>("OrganisationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SchemeYearId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SchemeYearQuarterId")
                        .HasColumnType("uuid");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("SchemeYearId", "OrganisationId");

                    b.HasIndex("SchemeYearQuarterId", "OrganisationId");

                    b.ToTable("QuarterlyBoilerSales");
                });

            modelBuilder.Entity("Desnz.Chmm.BoilerSales.Api.Entities.QuarterlyBoilerSalesChange", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<int>("NewGas")
                        .HasColumnType("integer");

                    b.Property<int>("NewOil")
                        .HasColumnType("integer");

                    b.Property<string>("NewStatus")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("OldGas")
                        .HasColumnType("integer");

                    b.Property<int?>("OldOil")
                        .HasColumnType("integer");

                    b.Property<string>("OldStatus")
                        .HasColumnType("varchar(50)");

                    b.Property<Guid>("QuarterlyBoilerSalesId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("QuarterlyBoilerSalesId");

                    b.ToTable("QuarterlyBoilerSalesChanges");
                });

            modelBuilder.Entity("Desnz.Chmm.BoilerSales.Api.Entities.QuarterlyBoilerSalesFile", b =>
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
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<Guid>("QuarterlyBoilerSalesId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("QuarterlyBoilerSalesId");

                    b.ToTable("QuarterlyBoilerSalesFiles");
                });

            modelBuilder.Entity("Desnz.Chmm.BoilerSales.Api.Entities.AnnualBoilerSalesChange", b =>
                {
                    b.HasOne("Desnz.Chmm.BoilerSales.Api.Entities.AnnualBoilerSales", "AnnualBoilerSales")
                        .WithMany("Changes")
                        .HasForeignKey("AnnualBoilerSalesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AnnualBoilerSales");
                });

            modelBuilder.Entity("Desnz.Chmm.BoilerSales.Api.Entities.AnnualBoilerSalesFile", b =>
                {
                    b.HasOne("Desnz.Chmm.BoilerSales.Api.Entities.AnnualBoilerSales", "AnnualBoilerSales")
                        .WithMany("Files")
                        .HasForeignKey("AnnualBoilerSalesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AnnualBoilerSales");
                });

            modelBuilder.Entity("Desnz.Chmm.BoilerSales.Api.Entities.QuarterlyBoilerSalesChange", b =>
                {
                    b.HasOne("Desnz.Chmm.BoilerSales.Api.Entities.QuarterlyBoilerSales", "QuarterlyBoilerSales")
                        .WithMany("Changes")
                        .HasForeignKey("QuarterlyBoilerSalesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuarterlyBoilerSales");
                });

            modelBuilder.Entity("Desnz.Chmm.BoilerSales.Api.Entities.QuarterlyBoilerSalesFile", b =>
                {
                    b.HasOne("Desnz.Chmm.BoilerSales.Api.Entities.QuarterlyBoilerSales", "QuarterlyBoilerSales")
                        .WithMany("Files")
                        .HasForeignKey("QuarterlyBoilerSalesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuarterlyBoilerSales");
                });

            modelBuilder.Entity("Desnz.Chmm.BoilerSales.Api.Entities.AnnualBoilerSales", b =>
                {
                    b.Navigation("Changes");

                    b.Navigation("Files");
                });

            modelBuilder.Entity("Desnz.Chmm.BoilerSales.Api.Entities.QuarterlyBoilerSales", b =>
                {
                    b.Navigation("Changes");

                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
