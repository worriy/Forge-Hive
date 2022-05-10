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
		public bool IsAuthor  { get; set; }
			
	}
}