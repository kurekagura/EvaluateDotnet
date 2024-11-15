using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleEF.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mylog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    process_id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    txt_data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bin_data = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    crt = table.Column<DateTime>(type: "datetime", nullable: false),
                    upd = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__mylog__3213E83FBE931EBC", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mylog");
        }
    }
}
