using System.ComponentModel.DataAnnotations;
using Hive.Backend.DataModels;


namespace Hive.Backend.ViewModels
{
	public partial class BestContributorVM 
	{
		public BestContributorVM() 
		{
		}

		public int Posts  { get; set; }
		public double Answers  { get; set; }
		public string PictureUrl  { get; set; }
		public string Firstname  { get; set; }
		public string Lastname  { get; set; }
		public string Department  { get; set; }
		public string City  { get; set; }
		public string Country  { get; set; }
        public int Likes { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public BestContributorVM ConvertFromModel(UserProfile userProfile) =>
			new BestContributorVM
			{
				Department = userProfile.Department,
				City = userProfile.City,
				Country = userProfile.Country,
				Firstname = userProfile.User.FirstName,
				Lastname = userProfile.User.LastName,
				PictureUrl = userProfile.User.PictureUrl,
			};

		public UserProfile GetUserProfileFromViewModel() =>
			new UserProfile
			{
				Department = Department,
				City = City,
				Country = Country
			};
			
	}
}