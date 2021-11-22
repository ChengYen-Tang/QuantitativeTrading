﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MultilateralArbitrage.Models;

#nullable disable

namespace MultilateralArbitrage.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20211121170428_20211122")]
    partial class _20211122
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("MultilateralArbitrage.Models.AssetsRecord", b =>
                {
                    b.Property<float>("Assets")
                        .HasColumnType("real");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("MarketMix")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("AssetsRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
