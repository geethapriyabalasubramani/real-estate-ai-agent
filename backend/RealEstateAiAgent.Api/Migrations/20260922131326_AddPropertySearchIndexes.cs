using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateAiAgent.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertySearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Properties_Bedrooms",
                table: "Properties",
                column: "Bedrooms");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_City",
                table: "Properties",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_City_Bedrooms_Price",
                table: "Properties",
                columns: new[] { "City", "Bedrooms", "Price" });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_HasGarage",
                table: "Properties",
                column: "HasGarage");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Price",
                table: "Properties",
                column: "Price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Properties_Bedrooms",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_City",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_City_Bedrooms_Price",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_HasGarage",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_Price",
                table: "Properties");
        }
    }
}
