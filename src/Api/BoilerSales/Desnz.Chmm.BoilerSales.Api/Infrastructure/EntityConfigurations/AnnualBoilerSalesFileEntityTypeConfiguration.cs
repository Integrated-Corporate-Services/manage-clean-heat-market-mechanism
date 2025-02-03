﻿using Desnz.Chmm.BoilerSales.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desnz.Chmm.BoilerSales.Api.Infrastructure.EntityConfigurations;

public class AnnualBoilerSalesFileEntityTypeConfiguration : IEntityTypeConfiguration<AnnualBoilerSalesFile>
{
    void IEntityTypeConfiguration<AnnualBoilerSalesFile>.Configure(EntityTypeBuilder<AnnualBoilerSalesFile> entity)
    {
        entity.HasKey(u => u.Id);

        entity.Property(u => u.Id).HasColumnType("uuid");
        entity.Property(u => u.CreationDate).HasColumnType("timestamptz").IsRequired();
        entity.Property(u => u.CreatedBy).HasColumnType("varchar(100)").IsRequired();

        entity.Property(u => u.FileKey).HasColumnType("varchar(255)").IsRequired();
        entity.Property(u => u.FileName).HasColumnType("varchar(100)").IsRequired();
        entity.Property(u => u.Type).HasColumnType("varchar(100)").IsRequired();

        entity.HasOne(u => u.AnnualBoilerSales).WithMany(u => u.Files).HasForeignKey(u => u.AnnualBoilerSalesId).IsRequired();
    }
}
