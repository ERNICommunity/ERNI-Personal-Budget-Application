﻿// <auto-generated />
using System;
using ERNI.PBA.Server.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ERNI.PBA.Server.DataAccess.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20200401073610_addCreateDateForRequest")]
    partial class addCreateDateForRequest
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ERNI.PBA.Server.DataAccess.Model.Budget", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount");

                    b.Property<int>("BudgetType");

                    b.Property<string>("Title");

                    b.Property<int>("UserId");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Budgets");
                });

            modelBuilder.Entity("ERNI.PBA.Server.DataAccess.Model.InvoiceImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Data");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("RequestId");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.ToTable("InvoiceImage");
                });

            modelBuilder.Entity("ERNI.PBA.Server.DataAccess.Model.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount");

                    b.Property<int>("BudgetId");

                    b.Property<int?>("CategoryId");

                    b.Property<DateTime>("CreateDate");

                    b.Property<DateTime>("Date");

                    b.Property<int>("State");

                    b.Property<string>("Title");

                    b.Property<string>("Url");

                    b.Property<int>("UserId");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("BudgetId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("ERNI.PBA.Server.DataAccess.Model.RequestCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsUrlNeeded");

                    b.Property<int?>("SpendLimit");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("RequestCategories");
                });

            modelBuilder.Entity("ERNI.PBA.Server.DataAccess.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsAdmin");

                    b.Property<bool>("IsSuperior");

                    b.Property<bool>("IsViewer");

                    b.Property<string>("LastName");

                    b.Property<int>("State");

                    b.Property<int?>("SuperiorId");

                    b.Property<string>("UniqueIdentifier");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.HasKey("Id");

                    b.HasIndex("SuperiorId");

                    b.HasIndex("UniqueIdentifier")
                        .IsUnique()
                        .HasFilter("[UniqueIdentifier] IS NOT NULL");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ERNI.PBA.Server.DataAccess.Model.Budget", b =>
                {
                    b.HasOne("ERNI.PBA.Server.DataAccess.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ERNI.PBA.Server.DataAccess.Model.InvoiceImage", b =>
                {
                    b.HasOne("ERNI.PBA.Server.DataAccess.Model.Request", "Request")
                        .WithMany()
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ERNI.PBA.Server.DataAccess.Model.Request", b =>
                {
                    b.HasOne("ERNI.PBA.Server.DataAccess.Model.Budget", "Budget")
                        .WithMany("Requests")
                        .HasForeignKey("BudgetId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("ERNI.PBA.Server.DataAccess.Model.RequestCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("ERNI.PBA.Server.DataAccess.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ERNI.PBA.Server.DataAccess.Model.User", b =>
                {
                    b.HasOne("ERNI.PBA.Server.DataAccess.Model.User", "Superior")
                        .WithMany()
                        .HasForeignKey("SuperiorId");
                });
#pragma warning restore 612, 618
        }
    }
}
