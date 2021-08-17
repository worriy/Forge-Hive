
using System;
using System.ComponentModel.DataAnnotations;
using Hive.Backend.DataModels;


namespace Hive.Backend.ViewModels
{
	public partial class EditGroupVM 
	{
		public EditGroupVM() 
		{
		}

		public string Name  { get; set; }
		public string City  { get; set; }
		public string Country  { get; set; }
		public string Id  { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public EditGroupVM ConvertFromModel(Group group) =>
			new EditGroupVM
			{
				Name = group.Name,
				City = group.City,
				Country = group.Country,
				Id = group.Id.ToString()
			};

		public Group GetGroupFromViewModel() =>
			new Group()
			{
				Name = Name,
				City = City,
				Country = Country,
				Id = Guid.Parse(Id)
			};
			
	}
}