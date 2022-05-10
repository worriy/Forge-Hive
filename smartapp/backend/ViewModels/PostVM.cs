using System;

namespace Hive.Backend.ViewModels
{
	public partial class PostVM 
	{
		public PostVM() 
		{
		}

		public string Id  { get; set; }
		public string Content  { get; set; }
		public DateTime PublicationDate  { get; set; }
		public DateTime EndDate  { get; set; }
		public string Status  { get; set; }
		public string Type  { get; set; }
			
	}
}