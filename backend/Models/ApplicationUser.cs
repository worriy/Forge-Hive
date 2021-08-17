using Microsoft.AspNetCore.Identity;

namespace Hive.Backend.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<string>
    {
		// Extended Properties
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public long? FacebookId { get; set; }
		public string PictureUrl { get; set; }
    }
}
