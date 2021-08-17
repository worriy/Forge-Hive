using System;
using System.ComponentModel.DataAnnotations;
using Hive.Backend.DataModels;


namespace Hive.Backend.ViewModels
{
	public partial class GroupVM 
	{
		public GroupVM() 
		{
		}

		public string IdGroup  { get; set; }
		public string Name  { get; set; }
		public int NumberMembers  { get; set; }
		public string City  { get; set; }
		public string Country  { get; set; }
		public string CreatedbyId  { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public GroupVM ConvertFromModel(Group group) =>
			new GroupVM
			{
				IdGroup = group.Id.ToString(),
				Name = group.Name,
				City = group.City,
				Country = group.Country,
				CreatedbyId = group.CreatedById.ToString()
			};

		public Group GetGroupFromViewModel() =>
			new Group
			{
				Id = Guid.Parse(IdGroup),
				Name = Name,
				City = City,
				Country = Country
			};

		public UserProfile GetUserProfileFromViewModel() =>
			new UserProfile
			{
				Id = Guid.Parse(CreatedbyId)
			};
			
	}
}