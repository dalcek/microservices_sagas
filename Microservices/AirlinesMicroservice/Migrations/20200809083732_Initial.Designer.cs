﻿// <auto-generated />
using System;
using AirlinesMicroservice.Models.ContextData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AirlinesMicroservice.Migrations
{
    [DbContext(typeof(AirlineContext))]
    [Migration("20200809083732_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Airline", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Admin")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Img")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<double>("Lat")
                        .HasColumnType("double");

                    b.Property<double>("Lon")
                        .HasColumnType("double");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<DateTime?>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.HasKey("Id");

                    b.ToTable("AirlineCompanies");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Destination", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("AirlineId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("FlightId")
                        .HasColumnType("int");

                    b.Property<string>("Img")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("AirlineId");

                    b.HasIndex("FlightId");

                    b.ToTable("Destination");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.DestinationPopular", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("AirlineId")
                        .HasColumnType("int");

                    b.Property<int?>("destinationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AirlineId");

                    b.HasIndex("destinationId");

                    b.ToTable("DestinationPopular");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Flight", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("AirlineId")
                        .HasColumnType("int");

                    b.Property<string>("DepartureDate")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Duration")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Extra")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("FromId")
                        .HasColumnType("int");

                    b.Property<string>("IdCompany")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("LuggageId")
                        .HasColumnType("int");

                    b.Property<int>("NumOfPassengers")
                        .HasColumnType("int");

                    b.Property<int?>("PovratniletId")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("Rate")
                        .HasColumnType("int");

                    b.Property<DateTime?>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<int?>("ToId")
                        .HasColumnType("int");

                    b.Property<int>("Trip")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AirlineId");

                    b.HasIndex("FromId");

                    b.HasIndex("LuggageId");

                    b.HasIndex("PovratniletId");

                    b.HasIndex("ToId");

                    b.ToTable("Flight");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Luggage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Dimensions")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Weight")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Luggage");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Rater", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("AirlineId")
                        .HasColumnType("int");

                    b.Property<int?>("FlightId")
                        .HasColumnType("int");

                    b.Property<int>("Rate")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AirlineId");

                    b.HasIndex("FlightId");

                    b.ToTable("Rater");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Seat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("FlightId")
                        .HasColumnType("int");

                    b.Property<bool>("IsSelected")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Taken")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("TravellerId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FlightId");

                    b.HasIndex("TravellerId");

                    b.ToTable("Seat");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.SoldTicket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("FlightId")
                        .HasColumnType("int");

                    b.Property<int?>("ticketId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FlightId");

                    b.HasIndex("ticketId");

                    b.ToTable("SoldTicket");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("AirlineId")
                        .HasColumnType("int");

                    b.Property<int>("Discount")
                        .HasColumnType("int");

                    b.Property<int?>("FlightId")
                        .HasColumnType("int");

                    b.Property<int?>("SeatId")
                        .HasColumnType("int");

                    b.Property<bool>("gaveRate")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("gaveRateCompany")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("reqTick")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("userId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("AirlineId");

                    b.HasIndex("FlightId");

                    b.HasIndex("SeatId");

                    b.ToTable("Ticket");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Traveller", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Email")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("IdUser")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Passport")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Traveller");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Destination", b =>
                {
                    b.HasOne("AirlinesMicroservice.Models.Model.Airline", null)
                        .WithMany("Destinations")
                        .HasForeignKey("AirlineId");

                    b.HasOne("AirlinesMicroservice.Models.Model.Flight", null)
                        .WithMany("Stops")
                        .HasForeignKey("FlightId");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.DestinationPopular", b =>
                {
                    b.HasOne("AirlinesMicroservice.Models.Model.Airline", null)
                        .WithMany("PopDestinaations")
                        .HasForeignKey("AirlineId");

                    b.HasOne("AirlinesMicroservice.Models.Model.Destination", "destination")
                        .WithMany()
                        .HasForeignKey("destinationId");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Flight", b =>
                {
                    b.HasOne("AirlinesMicroservice.Models.Model.Airline", null)
                        .WithMany("Flights")
                        .HasForeignKey("AirlineId");

                    b.HasOne("AirlinesMicroservice.Models.Model.Destination", "From")
                        .WithMany()
                        .HasForeignKey("FromId");

                    b.HasOne("AirlinesMicroservice.Models.Model.Luggage", "Luggage")
                        .WithMany()
                        .HasForeignKey("LuggageId");

                    b.HasOne("AirlinesMicroservice.Models.Model.Flight", "Povratnilet")
                        .WithMany()
                        .HasForeignKey("PovratniletId");

                    b.HasOne("AirlinesMicroservice.Models.Model.Destination", "To")
                        .WithMany()
                        .HasForeignKey("ToId");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Rater", b =>
                {
                    b.HasOne("AirlinesMicroservice.Models.Model.Airline", null)
                        .WithMany("Raters")
                        .HasForeignKey("AirlineId");

                    b.HasOne("AirlinesMicroservice.Models.Model.Flight", null)
                        .WithMany("Raters")
                        .HasForeignKey("FlightId");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Seat", b =>
                {
                    b.HasOne("AirlinesMicroservice.Models.Model.Flight", null)
                        .WithMany("Seats")
                        .HasForeignKey("FlightId");

                    b.HasOne("AirlinesMicroservice.Models.Model.Traveller", "Traveller")
                        .WithMany()
                        .HasForeignKey("TravellerId");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.SoldTicket", b =>
                {
                    b.HasOne("AirlinesMicroservice.Models.Model.Flight", null)
                        .WithMany("SoldTickets")
                        .HasForeignKey("FlightId");

                    b.HasOne("AirlinesMicroservice.Models.Model.Ticket", "ticket")
                        .WithMany()
                        .HasForeignKey("ticketId");
                });

            modelBuilder.Entity("AirlinesMicroservice.Models.Model.Ticket", b =>
                {
                    b.HasOne("AirlinesMicroservice.Models.Model.Airline", null)
                        .WithMany("FastTickets")
                        .HasForeignKey("AirlineId");

                    b.HasOne("AirlinesMicroservice.Models.Model.Flight", "Flight")
                        .WithMany("AllTickets")
                        .HasForeignKey("FlightId");

                    b.HasOne("AirlinesMicroservice.Models.Model.Seat", "Seat")
                        .WithMany()
                        .HasForeignKey("SeatId");
                });
#pragma warning restore 612, 618
        }
    }
}
