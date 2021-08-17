using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hive.Backend.DataModels;


namespace Hive.Backend.ViewModels
{
	public partial class UpdateMembersVM 
	{
		public UpdateMembersVM() 
		{
		}

		public List<string> UserIds  { get; set; }
		public string GroupId  { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public UpdateMembersVM ConvertFromModel(List<UserProfile> userProfiles, Group group) =>
			new UpdateMembersVM
			{
				UserIds = userProfiles.Select(u => u.Id.ToString()).ToList(),
				GroupId = group.Id.ToString()
			};	
	}
}