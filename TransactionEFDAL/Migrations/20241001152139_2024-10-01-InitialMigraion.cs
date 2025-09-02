using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionEFDAL.Migrations
{
    /// <inheritdoc />
    public partial class _20241001InitialMigraion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TransactionType = table.Column<int>(type: "INTEGER", nullable: false),
                    TokenFromOld = table.Column<string>(type: "TEXT", nullable: true),
                    TokenToOld = table.Column<string>(type: "TEXT", nullable: true),
                    TokenType = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    FeeFrom = table.Column<decimal>(type: "TEXT", nullable: false),
                    FeeTo = table.Column<decimal>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TransactionStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionSignature = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionIniciator = table.Column<string>(type: "TEXT", nullable: false),
                    ErrorMsg = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    EtheriumProof = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
