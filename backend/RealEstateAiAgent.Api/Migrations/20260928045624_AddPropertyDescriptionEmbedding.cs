using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace RealEstateAiAgent.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyDescriptionEmbedding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.AddColumn<Vector>(
                name: "DescriptionEmbedding",
                table: "Properties",
                type: "vector(1024)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionEmbedding",
                table: "Properties");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");
        }
    }
}
