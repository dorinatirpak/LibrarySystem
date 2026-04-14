using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 

namespace LibrarySystem.Migrations
{
    public partial class InitialSQLite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Author = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Publisher = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Edition = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ISBN = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IsLoanable = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CopyCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Librarians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Librarians", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Contact = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MemberType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    MemberId = table.Column<int>(type: "INTEGER", nullable: false),
                    LoanDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loans_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "CopyCount", "Edition", "ISBN", "IsDeleted", "IsLoanable", "Publisher", "Title", "Year" },
                values: new object[,]
                {
                    { 1, "Knuth, Donald E.", 3, "3rd", "978-0201038040", false, true, "Addison-Wesley", "The Art of Computer Programming", 1968 },
                    { 2, "Martin, Robert C.", 2, "1st", "978-0132350884", false, true, "Prentice Hall", "Clean Code", 2008 },
                    { 3, "Gamma et al.", 1, "1st", "978-0201633610", false, true, "Addison-Wesley", "Design Patterns", 1994 }
                });

            migrationBuilder.InsertData(
                table: "Librarians",
                columns: new[] { "Id", "FullName", "PasswordHash", "Username" },
                values: new object[] { 1, "Rendszergazda", "240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9", "admin" });

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "Id", "Address", "Contact", "IsDeleted", "MemberType", "Name" },
                values: new object[,]
                {
                    { 1, "Budapest, Fo u. 1.", "kovacs.anna@edu.hu", false, 0, "Kovacs Anna" },
                    { 2, "Debrecen, Kossuth u. 5.", "nagy.peter@univ.hu", false, 1, "Dr. Nagy Peter" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Librarians_Username",
                table: "Librarians",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BookId",
                table: "Loans",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_MemberId",
                table: "Loans",
                column: "MemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Librarians");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
