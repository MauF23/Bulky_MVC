using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWebRazor_Temp.Models
{
	public class Category
	{
		[Key] //with this data annotation we specify which variable is the primary key.
		public int Id { get; set; } //primary key of the table, naming a variable Id without a [Key] annotation makes it the default primary key for the MVC framework

		[Required] //this annotation makes it that the variable cannot be a null value
		[DisplayName("Category Name")] //this annotation makes it so the variable when referenced in views appears with the specified name
		[MaxLength(30, ErrorMessage = "Name must be 30 characters or less")]//use as a validation for a max character lenght
		public string Name { get; set; } //Category name

		[DisplayName("Display Order")]
		[Range(1, 100, ErrorMessage = "Display Order must be between 1-100")]//use as a validation to encapsulate a range
		public int DisplayOrder { get; set; } //order in which the categories are listed
	}
}
