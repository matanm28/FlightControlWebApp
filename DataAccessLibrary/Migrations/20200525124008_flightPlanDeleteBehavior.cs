using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLibrary.Migrations
{
    public partial class flightPlanDeleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightPlans_Locations_InitialLocationId",
                table: "FlightPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_Segmentses_FlightPlans_FlightPlanId",
                table: "Segmentses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Segmentses",
                table: "Segmentses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locations",
                table: "Locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FlightPlans",
                table: "FlightPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiServer",
                table: "ApiServer");

            migrationBuilder.DropColumn(
                name: "FlightId",
                table: "Segmentses");

            migrationBuilder.DropColumn(
                name: "FlightId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "FlightId",
                table: "FlightPlans");

            migrationBuilder.DropColumn(
                name: "FlightId",
                table: "ApiServer");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Segmentses",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Locations",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "FlightPlans",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ApiServer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Segmentses",
                table: "Segmentses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locations",
                table: "Locations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlightPlans",
                table: "FlightPlans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiServer",
                table: "ApiServer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightPlans_Locations_InitialLocationId",
                table: "FlightPlans",
                column: "InitialLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Segmentses_FlightPlans_FlightPlanId",
                table: "Segmentses",
                column: "FlightPlanId",
                principalTable: "FlightPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightPlans_Locations_InitialLocationId",
                table: "FlightPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_Segmentses_FlightPlans_FlightPlanId",
                table: "Segmentses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Segmentses",
                table: "Segmentses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locations",
                table: "Locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FlightPlans",
                table: "FlightPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiServer",
                table: "ApiServer");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Segmentses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FlightPlans");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ApiServer");

            migrationBuilder.AddColumn<int>(
                name: "FlightId",
                table: "Segmentses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "FlightId",
                table: "Locations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "FlightId",
                table: "FlightPlans",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FlightId",
                table: "ApiServer",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Segmentses",
                table: "Segmentses",
                column: "FlightId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locations",
                table: "Locations",
                column: "FlightId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlightPlans",
                table: "FlightPlans",
                column: "FlightId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiServer",
                table: "ApiServer",
                column: "FlightId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightPlans_Locations_InitialLocationId",
                table: "FlightPlans",
                column: "InitialLocationId",
                principalTable: "Locations",
                principalColumn: "FlightId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Segmentses_FlightPlans_FlightPlanId",
                table: "Segmentses",
                column: "FlightPlanId",
                principalTable: "FlightPlans",
                principalColumn: "FlightId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
