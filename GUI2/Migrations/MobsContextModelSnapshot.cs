﻿// <auto-generated />
using System;
using EFCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EFCore.Migrations
{
    [DbContext(typeof(MobsContext))]
    partial class MobsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("EFCore.Data.Mob", b =>
                {
                    b.Property<int>("MobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateOfCapture")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("SpeciesId")
                        .HasColumnType("INTEGER");

                    b.HasKey("MobId");

                    b.HasIndex("SpeciesId");

                    b.ToTable("Mobs");
                });

            modelBuilder.Entity("EFCore.Data.Species", b =>
                {
                    b.Property<int>("SpeciesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Hostility")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("SpeciesId");

                    b.ToTable("Species");
                });

            modelBuilder.Entity("EFCore.Data.Mob", b =>
                {
                    b.HasOne("EFCore.Data.Species", "Species")
                        .WithMany("Mobs")
                        .HasForeignKey("SpeciesId");

                    b.Navigation("Species");
                });

            modelBuilder.Entity("EFCore.Data.Species", b =>
                {
                    b.Navigation("Mobs");
                });
#pragma warning restore 612, 618
        }
    }
}
