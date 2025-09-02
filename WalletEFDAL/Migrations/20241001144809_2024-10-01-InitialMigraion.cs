using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WalletEFDAL.Migrations
{
    /// <inheritdoc />
    public partial class _20241001InitialMigraion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Address = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    TokenId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TokenIdHash = table.Column<int>(type: "INTEGER", nullable: false),
                    TokenData = table.Column<string>(type: "TEXT", nullable: false),
                    TokenDataSignature = table.Column<int>(type: "INTEGER", nullable: false),
                    WalletAddress = table.Column<Guid>(type: "TEXT", nullable: false),
                    WalletAddressHash = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_Tokens_Wallets_WalletAddress",
                        column: x => x.WalletAddress,
                        principalTable: "Wallets",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_WalletAddress",
                table: "Tokens",
                column: "WalletAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "Wallets");
        }
    }
}
