using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TareasMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    IF NOT EXISTS(Select Id from AspNetRoles where Id = 'f0d23871-da54-4cd0-80b9-88968bf99704' )
                    BEGIN 
                    INSERT AspNetRoles (Id, [Name], [NormalizedName])
                    VALUES ('f0d23871-da54-4cd0-80b9-88968bf99704','admin','ADMIN');
                    END
                    ");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE AspNetRoles Where Id ='f0d23871-da54-4cd0-80b9-88968bf99704'");
        }
    }
}
