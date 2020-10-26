using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CarsMicroservice.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RentACarCompanies",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    MainLocationId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    AvrageRating = table.Column<int>(nullable: false),
                    Activated = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentACarCompanies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FullAddress = table.Column<string>(nullable: true),
                    Longitude = table.Column<double>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    RentACarID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_RentACarCompanies_RentACarID",
                        column: x => x.RentACarID,
                        principalTable: "RentACarCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Car",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Brand = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    PricePerDay = table.Column<double>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Passengers = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    AvrageRating = table.Column<int>(nullable: false),
                    Removed = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    RentACarID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Car", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Car_RentACarCompanies_RentACarID",
                        column: x => x.RentACarID,
                        principalTable: "RentACarCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarReservation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CarID = table.Column<int>(nullable: true),
                    RatedCar = table.Column<int>(nullable: false),
                    RatedCompany = table.Column<int>(nullable: false),
                    PickUpLocation = table.Column<string>(nullable: true),
                    ReturnLocation = table.Column<string>(nullable: true),
                    PickUpTime = table.Column<string>(nullable: true),
                    ReturnTime = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    TotalPrice = table.Column<double>(nullable: false),
                    ResUser = table.Column<string>(nullable: true),
                    RentACarID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarReservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarReservation_Car_CarID",
                        column: x => x.CarID,
                        principalTable: "Car",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarReservation_RentACarCompanies_RentACarID",
                        column: x => x.RentACarID,
                        principalTable: "RentACarCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuickReservation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscountedCarID = table.Column<int>(nullable: true),
                    RentACarID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuickReservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuickReservation_Car_DiscountedCarID",
                        column: x => x.DiscountedCarID,
                        principalTable: "Car",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuickReservation_RentACarCompanies_RentACarID",
                        column: x => x.RentACarID,
                        principalTable: "RentACarCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Rate = table.Column<int>(nullable: false),
                    CarID = table.Column<int>(nullable: true),
                    RentACarID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rating_Car_CarID",
                        column: x => x.CarID,
                        principalTable: "Car",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rating_RentACarCompanies_RentACarID",
                        column: x => x.RentACarID,
                        principalTable: "RentACarCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExtraAmenity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    OneTimePayment = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<DateTime>(rowVersion: true, nullable: true)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    CarReservationId = table.Column<int>(nullable: true),
                    RentACarID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraAmenity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraAmenity_CarReservation_CarReservationId",
                        column: x => x.CarReservationId,
                        principalTable: "CarReservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExtraAmenity_RentACarCompanies_RentACarID",
                        column: x => x.RentACarID,
                        principalTable: "RentACarCompanies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Date",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DateStr = table.Column<string>(nullable: true),
                    CarID = table.Column<int>(nullable: true),
                    CarReservationId = table.Column<int>(nullable: true),
                    QuickReservationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Date", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Date_Car_CarID",
                        column: x => x.CarID,
                        principalTable: "Car",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Date_CarReservation_CarReservationId",
                        column: x => x.CarReservationId,
                        principalTable: "CarReservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Date_QuickReservation_QuickReservationId",
                        column: x => x.QuickReservationId,
                        principalTable: "QuickReservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_RentACarID",
                table: "Address",
                column: "RentACarID");

            migrationBuilder.CreateIndex(
                name: "IX_Car_RentACarID",
                table: "Car",
                column: "RentACarID");

            migrationBuilder.CreateIndex(
                name: "IX_CarReservation_CarID",
                table: "CarReservation",
                column: "CarID");

            migrationBuilder.CreateIndex(
                name: "IX_CarReservation_RentACarID",
                table: "CarReservation",
                column: "RentACarID");

            migrationBuilder.CreateIndex(
                name: "IX_Date_CarID",
                table: "Date",
                column: "CarID");

            migrationBuilder.CreateIndex(
                name: "IX_Date_CarReservationId",
                table: "Date",
                column: "CarReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Date_QuickReservationId",
                table: "Date",
                column: "QuickReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraAmenity_CarReservationId",
                table: "ExtraAmenity",
                column: "CarReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraAmenity_RentACarID",
                table: "ExtraAmenity",
                column: "RentACarID");

            migrationBuilder.CreateIndex(
                name: "IX_QuickReservation_DiscountedCarID",
                table: "QuickReservation",
                column: "DiscountedCarID");

            migrationBuilder.CreateIndex(
                name: "IX_QuickReservation_RentACarID",
                table: "QuickReservation",
                column: "RentACarID");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_CarID",
                table: "Rating",
                column: "CarID");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_RentACarID",
                table: "Rating",
                column: "RentACarID");

            migrationBuilder.CreateIndex(
                name: "IX_RentACarCompanies_MainLocationId",
                table: "RentACarCompanies",
                column: "MainLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RentACarCompanies_Address_MainLocationId",
                table: "RentACarCompanies",
                column: "MainLocationId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_RentACarCompanies_RentACarID",
                table: "Address");

            migrationBuilder.DropTable(
                name: "Date");

            migrationBuilder.DropTable(
                name: "ExtraAmenity");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "QuickReservation");

            migrationBuilder.DropTable(
                name: "CarReservation");

            migrationBuilder.DropTable(
                name: "Car");

            migrationBuilder.DropTable(
                name: "RentACarCompanies");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
