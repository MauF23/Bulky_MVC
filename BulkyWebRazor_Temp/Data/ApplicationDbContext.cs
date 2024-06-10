using BulkyWebRazor_Temp.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazor_Temp.Data
{
	public class ApplicationDbContext : DbContext
	{
		//Obtain the connection string
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		//Create a Table, to call this function use this command on the PackageManagerConsole: add-migration AddCategoryTableToDb
		//finally use this command: update-database to push the migrations to the SSManagment Studio
		public DbSet<Category> Categories { get; set; }

		//This helper functions allows to create Database models, like adding values to a table, gets called on the package console with the
		//add-migration SeedCategoryTable command followed by the update-databse one tu pussh the migrations.
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//in the model builder param we defined the values of the table of type Category
			modelBuilder.Entity<Category>().HasData
				(
					new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
					new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
					new Category { Id = 3, Name = "History", DisplayOrder = 3 }
				);
		}
	}
}
