using System.ComponentModel.DataAnnotations;


namespace Hive.Backend.ViewModels
{
	public partial class FullUserVM 
	{
		public FullUserVM() 
		{
		}

		public string Firstname  { get; set; }
		public string Lastname  { get; set; }
		public string PhoneNumber  { get; set; }
		public string Email  { get; set; }
		public string Country  { get; set; }
		public string City  { get; set; }
        public string Department { get; set; }
		public string Job  { get; set; }
		public string UserProfileId  { get; set; }

			
	}
}