using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
	/// <inheritdoc />
	public partial class RenameStateToCountry : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Tüm tablolar için State kolonunu Country olarak değiştirin
			migrationBuilder.RenameColumn(
				name: "State",
				table: "Companies", // Tablo adınızı buraya yazın
				newName: "Country"
			);

			// Bu işlemi birden fazla tablo için yapmanız gerekiyorsa aynı işlemi diğer tablolar için tekrarlayın
			migrationBuilder.RenameColumn(
				name: "State",
				table: "OrderHeaders",
				newName: "Country"
			);
		}

	}
}
