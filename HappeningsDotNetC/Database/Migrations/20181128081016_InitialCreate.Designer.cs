﻿// <auto-generated />
using System;
using HappeningsDotNetC.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HappeningsDotNetC.Database.Migrations
{
    [DbContext(typeof(HappeningsContext))]
    [Migration("20181128081016_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HappeningsDotNetC.Entities.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<DateTime>("FriendlyName");

                    b.Property<Guid>("PrimaryUserId");

                    b.HasKey("Id");

                    b.HasIndex("PrimaryUserId");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("HappeningsDotNetC.Entities.JoinEntities.EventUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("EventId");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("UserId");

                    b.ToTable("EventUser");
                });

            modelBuilder.Entity("HappeningsDotNetC.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("CalendarVisibleToOthers");

                    b.Property<string>("FriendlyName");

                    b.Property<string>("Name");

                    b.Property<int>("Role");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HappeningsDotNetC.Entities.Event", b =>
                {
                    b.HasOne("HappeningsDotNetC.Entities.User", "PrimaryUser")
                        .WithMany()
                        .HasForeignKey("PrimaryUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HappeningsDotNetC.Entities.JoinEntities.EventUser", b =>
                {
                    b.HasOne("HappeningsDotNetC.Entities.Event", "Event")
                        .WithMany("AllUsers")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("HappeningsDotNetC.Entities.User", "User")
                        .WithMany("Events")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
