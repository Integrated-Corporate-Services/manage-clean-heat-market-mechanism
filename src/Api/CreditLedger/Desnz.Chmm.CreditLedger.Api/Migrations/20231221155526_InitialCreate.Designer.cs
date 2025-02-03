﻿// <auto-generated />
using System;
using Desnz.Chmm.CreditLedger.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Desnz.Chmm.CreditLedger.Api.Migrations
{
    [DbContext(typeof(CreditLedgerContext))]
    [Migration("20231221155526_InitialCreate")]
    partial class InitialCreate
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

            modelBuilder.Entity("Desnz.Chmm.CreditLedger.Api.Entities.CreditTransfer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<DateTime>("DateOfTransaction")
                        .HasColumnType("timestamptz");

                    b.Property<Guid>("DestinationOrganisationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SchemeYearId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SourceOrganisationId")
                        .HasColumnType("uuid");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<Guid?>("TransactionId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId")
                        .IsUnique();

                    b.ToTable("CreditTransfers");
                });

            modelBuilder.Entity("Desnz.Chmm.CreditLedger.Api.Entities.InstallationCredit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<DateTime>("DateCreditGenerated")
                        .HasColumnType("timestamptz");

                    b.Property<int>("HeatPumpInstallationId")
                        .HasColumnType("int");

                    b.Property<Guid>("LicenseHolderId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SchemeYearId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal");

                    b.HasKey("Id");

                    b.ToTable("InstallationCredits");
                });

            modelBuilder.Entity("Desnz.Chmm.CreditLedger.Api.Entities.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<DateTime>("DateOfTransaction")
                        .HasColumnType("timestamptz");

                    b.Property<Guid>("SchemeYearId")
                        .HasColumnType("uuid");

                    b.Property<string>("TransactionType")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Desnz.Chmm.CreditLedger.Api.Entities.TransactionEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamptz");

                    b.Property<Guid>("OrganisationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId");

                    b.ToTable("TransactionEntries");
                });

            modelBuilder.Entity("Desnz.Chmm.CreditLedger.Api.Entities.CreditTransfer", b =>
                {
                    b.HasOne("Desnz.Chmm.CreditLedger.Api.Entities.Transaction", "Transaction")
                        .WithOne("CreditTransfer")
                        .HasForeignKey("Desnz.Chmm.CreditLedger.Api.Entities.CreditTransfer", "TransactionId");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("Desnz.Chmm.CreditLedger.Api.Entities.TransactionEntry", b =>
                {
                    b.HasOne("Desnz.Chmm.CreditLedger.Api.Entities.Transaction", "Transaction")
                        .WithMany("Entries")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("Desnz.Chmm.CreditLedger.Api.Entities.Transaction", b =>
                {
                    b.Navigation("CreditTransfer");

                    b.Navigation("Entries");
                });
#pragma warning restore 612, 618
        }
    }
}
