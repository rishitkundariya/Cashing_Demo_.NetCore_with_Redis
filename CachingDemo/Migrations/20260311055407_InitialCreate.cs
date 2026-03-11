using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CachingDemo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Salary = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    HireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 11, 5, 54, 3, 473, DateTimeKind.Utc).AddTicks(77), "Information Technology", "IT", new DateTime(2026, 3, 11, 5, 54, 3, 473, DateTimeKind.Utc).AddTicks(84) },
                    { 2, new DateTime(2026, 3, 11, 5, 54, 3, 473, DateTimeKind.Utc).AddTicks(1866), "Human Resources", "HR", new DateTime(2026, 3, 11, 5, 54, 3, 473, DateTimeKind.Utc).AddTicks(1867) },
                    { 3, new DateTime(2026, 3, 11, 5, 54, 3, 473, DateTimeKind.Utc).AddTicks(1871), "Sales Department", "Sales", new DateTime(2026, 3, 11, 5, 54, 3, 473, DateTimeKind.Utc).AddTicks(1872) }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "DepartmentId", "Email", "FirstName", "HireDate", "LastName", "PhoneNumber", "Salary", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 11, 5, 54, 3, 474, DateTimeKind.Utc).AddTicks(917), 1, "john.doe@company.com", "John", new DateTime(2026, 3, 11, 5, 54, 3, 474, DateTimeKind.Utc).AddTicks(916), "Doe", "123-456-7890", 50000m, new DateTime(2026, 3, 11, 5, 54, 3, 474, DateTimeKind.Utc).AddTicks(918) },
                    { 2, new DateTime(2026, 3, 11, 5, 54, 3, 474, DateTimeKind.Utc).AddTicks(3411), 1, "jane.smith@company.com", "Jane", new DateTime(2026, 3, 11, 5, 54, 3, 474, DateTimeKind.Utc).AddTicks(3411), "Smith", "123-456-7891", 60000m, new DateTime(2026, 3, 11, 5, 54, 3, 474, DateTimeKind.Utc).AddTicks(3412) },
                    { 3, new DateTime(2026, 3, 11, 5, 54, 3, 474, DateTimeKind.Utc).AddTicks(3416), 2, "mike.johnson@company.com", "Mike", new DateTime(2026, 3, 11, 5, 54, 3, 474, DateTimeKind.Utc).AddTicks(3416), "Johnson", "123-456-7892", 55000m, new DateTime(2026, 3, 11, 5, 54, 3, 474, DateTimeKind.Utc).AddTicks(3417) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
