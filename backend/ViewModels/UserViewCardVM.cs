
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hive.Backend.DataModels;


namespace Hive.Backend.ViewModels
{
	public partial class UserViewCardVM 
	{
		public UserViewCardVM() 
		{
		}

		public string CardId  { get; set; }
		public string UserId  { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }
			
	}
}