﻿// <auto-generated />
using System;
using EquipmentManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EquipmentManagementSystem.Migrations
{
    [DbContext(typeof(ManagementContext))]
    [Migration("20190516093854_OwnerAdded")]
    partial class OwnerAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("EquipmentManagementSystem.Models.Equipment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EquipType");

                    b.Property<bool>("IDCheck");

                    b.Property<string>("IP");

                    b.Property<DateTime>("LastEdited");

                    b.Property<string>("Location");

                    b.Property<string>("MobileNumber");

                    b.Property<string>("Model");

                    b.Property<string>("Notes");

                    b.Property<int?>("OwnerID");

                    b.Property<int>("Ports");

                    b.Property<string>("Resolution");

                    b.Property<string>("Serial");

                    b.HasKey("ID");

                    b.HasIndex("OwnerID");

                    b.ToTable("Equipment");
                });

            modelBuilder.Entity("EquipmentManagementSystem.Models.Owner", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Added");

                    b.Property<string>("Address");

                    b.Property<string>("FirstName");

                    b.Property<DateTime>("LastEdited");

                    b.Property<string>("LastName");

                    b.Property<string>("Mail");

                    b.Property<string>("SSN");

                    b.Property<string>("TelNr");

                    b.HasKey("ID");

                    b.ToTable("Owners");
                });

            modelBuilder.Entity("EquipmentManagementSystem.Models.Equipment", b =>
                {
                    b.HasOne("EquipmentManagementSystem.Models.Owner", "Owner")
                        .WithMany("Equipment")
                        .HasForeignKey("OwnerID");
                });
#pragma warning restore 612, 618
        }
    }
}
