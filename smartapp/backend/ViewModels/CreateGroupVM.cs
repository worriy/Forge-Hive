using System;
using System.Collections.Generic;
using Hive.Backend.DataModels;

namespace Hive.Backend.ViewModels
{
	public partial class CreateGroupVM 
	{
		public string Name  { get; set; }
		public string City  { get; set; }
		public string Country  { get; set; }
		public string CreatedbyId  { get; set; }
		public List<string> UserIds { get; set; }

	}
}