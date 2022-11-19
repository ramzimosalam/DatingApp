using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    public partial class Create_Update_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             var createProcSql = @"CREATE OR ALTER PROC usp_Create_Update_Users(@Data varchar(MAX),@MethodName varchar(100))AS
            BEGIN
	            -- SET NOCOUNT ON added to prevent extra result sets from
                -- interfering with SELECT statements.
                SET NOCOUNT ON;

                -- Insert statements for procedure here
                select GETDATE()
            END";
            migrationBuilder.Sql(createProcSql);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROC usp_Create_Update_Users";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}
