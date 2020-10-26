using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AirlinesMicroservice.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirlineCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Lon = table.Column<double>(nullable: false),
                    Lat = table.Column<double>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Img = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    Admin = table.Column<int>(nullable: false),
                    RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Luggage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Weight = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Dimensions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Luggage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Traveller",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    IdUser = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Passport = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traveller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Destination",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Img = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AirlineId = table.Column<int>(nullable: true),
                    FlightId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Destination_AirlineCompanies_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "AirlineCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DestinationPopular",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    destinationId = table.Column<int>(nullable: true),
                    AirlineId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinationPopular", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinationPopular_AirlineCompanies_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "AirlineCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DestinationPopular_Destination_destinationId",
                        column: x => x.destinationId,
                        principalTable: "Destination",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Flight",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdCompany = table.Column<string>(nullable: true),
                    Duration = table.Column<string>(nullable: true),
                    Extra = table.Column<string>(nullable: true),
                    PovratniletId = table.Column<int>(nullable: true),
                    Price = table.Column<int>(nullable: false),
                    NumOfPassengers = table.Column<int>(nullable: false),
                    DepartureDate = table.Column<string>(nullable: true),
                    LuggageId = table.Column<int>(nullable: true),
                    FromId = table.Column<int>(nullable: true),
                    ToId = table.Column<int>(nullable: true),
                    Rate = table.Column<int>(nullable: false),
                    Trip = table.Column<int>(nullable: false),
                    RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    AirlineId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flight", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flight_AirlineCompanies_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "AirlineCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flight_Destination_FromId",
                        column: x => x.FromId,
                        principalTable: "Destination",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flight_Luggage_LuggageId",
                        column: x => x.LuggageId,
                        principalTable: "Luggage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flight_Flight_PovratniletId",
                        column: x => x.PovratniletId,
                        principalTable: "Flight",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flight_Destination_ToId",
                        column: x => x.ToId,
                        principalTable: "Destination",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rater",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Rate = table.Column<int>(nullable: false),
                    AirlineId = table.Column<int>(nullable: true),
                    FlightId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rater", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rater_AirlineCompanies_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "AirlineCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rater_Flight_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flight",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Taken = table.Column<bool>(nullable: false),
                    IsSelected = table.Column<bool>(nullable: false),
                    TravellerId = table.Column<string>(maxLength: 128, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    FlightId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seat_Flight_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flight",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seat_Traveller_TravellerId",
                        column: x => x.TravellerId,
                        principalTable: "Traveller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FlightId = table.Column<int>(nullable: true),
                    SeatId = table.Column<int>(nullable: true),
                    Discount = table.Column<int>(nullable: false),
                    gaveRate = table.Column<bool>(nullable: false),
                    gaveRateCompany = table.Column<bool>(nullable: false),
                    userId = table.Column<string>(nullable: true),
                    reqTick = table.Column<string>(nullable: true),
                    AirlineId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ticket_AirlineCompanies_AirlineId",
                        column: x => x.AirlineId,
                        principalTable: "AirlineCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Flight_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flight",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Seat_SeatId",
                        column: x => x.SeatId,
                        principalTable: "Seat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SoldTicket",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ticketId = table.Column<int>(nullable: true),
                    FlightId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldTicket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoldTicket_Flight_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flight",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SoldTicket_Ticket_ticketId",
                        column: x => x.ticketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Destination_AirlineId",
                table: "Destination",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_Destination_FlightId",
                table: "Destination",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationPopular_AirlineId",
                table: "DestinationPopular",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationPopular_destinationId",
                table: "DestinationPopular",
                column: "destinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_AirlineId",
                table: "Flight",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_FromId",
                table: "Flight",
                column: "FromId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_LuggageId",
                table: "Flight",
                column: "LuggageId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_PovratniletId",
                table: "Flight",
                column: "PovratniletId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_ToId",
                table: "Flight",
                column: "ToId");

            migrationBuilder.CreateIndex(
                name: "IX_Rater_AirlineId",
                table: "Rater",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_Rater_FlightId",
                table: "Rater",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_FlightId",
                table: "Seat",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_TravellerId",
                table: "Seat",
                column: "TravellerId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldTicket_FlightId",
                table: "SoldTicket",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldTicket_ticketId",
                table: "SoldTicket",
                column: "ticketId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_AirlineId",
                table: "Ticket",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_FlightId",
                table: "Ticket",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_SeatId",
                table: "Ticket",
                column: "SeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Destination_Flight_FlightId",
                table: "Destination",
                column: "FlightId",
                principalTable: "Flight",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Destination_AirlineCompanies_AirlineId",
                table: "Destination");

            migrationBuilder.DropForeignKey(
                name: "FK_Flight_AirlineCompanies_AirlineId",
                table: "Flight");

            migrationBuilder.DropForeignKey(
                name: "FK_Destination_Flight_FlightId",
                table: "Destination");

            migrationBuilder.DropTable(
                name: "DestinationPopular");

            migrationBuilder.DropTable(
                name: "Rater");

            migrationBuilder.DropTable(
                name: "SoldTicket");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Seat");

            migrationBuilder.DropTable(
                name: "Traveller");

            migrationBuilder.DropTable(
                name: "AirlineCompanies");

            migrationBuilder.DropTable(
                name: "Flight");

            migrationBuilder.DropTable(
                name: "Destination");

            migrationBuilder.DropTable(
                name: "Luggage");
        }
    }
}
