using System;
using System.ComponentModel.DataAnnotations;
using Hive.Backend.DataModels;


namespace Hive.Backend.ViewModels
{
	public partial class CreateGroupVM 
	{
		public CreateGroupVM() 
		{
		}

		public string Name  { get; set; }
		public string City  { get; set; }
		public string Country  { get; set; }
		public string CreatedbyId  { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public CreateGroupVM ConvertFromModel(Group group) =>
			new CreateGroupVM
			{
				Name = group.Name,
				City = group.City,
				Country = group.Country,
				CreatedbyId = group.Id.ToString()
			};

		public Group GetGroupFromViewModel() =>
			new Group
			{
				Name = Name,
				City = City,
				Country = Country,
				CreatedById = Guid.Parse(CreatedbyId)
			};

		public UserProfile GetUserProfileFromViewModel() =>
			new UserProfile
			{
				Id = Guid.Parse(CreatedbyId)
			};
			
	}
}