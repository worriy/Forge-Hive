using System.Collections.Generic;

namespace Hive.Backend.ViewModels
{
	public partial class UpdateMembersVM 
	{
		public UpdateMembersVM() 
		{
		}

		public List<string> UserIds  { get; set; }
		public string GroupId  { get; set; }	
	}
}