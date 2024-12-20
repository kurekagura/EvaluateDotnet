﻿// <auto-generated />
using System;
using ConsoleEF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ConsoleEF.Migrations
{
    [DbContext(typeof(SampleDbContext))]
    [Migration("20241115162306_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ConsoleEF.Models.Mylog", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<byte[]>("BinData")
                        .HasColumnType("varbinary(max)")
                        .HasColumnName("bin_data");

                    b.Property<DateTime>("Crt")
                        .HasColumnType("datetime")
                        .HasColumnName("crt");

                    b.Property<string>("ProcessId")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("process_id");

                    b.Property<string>("TxtData")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("txt_data");

                    b.Property<DateTime>("Upd")
                        .HasColumnType("datetime")
                        .HasColumnName("upd");

                    b.HasKey("Id")
                        .HasName("PK__mylog__3213E83FBE931EBC");

                    b.ToTable("mylog", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
