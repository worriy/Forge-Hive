using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Hive.Backend.Models;
using Hive.Backend.Models.JoinTables;

namespace Hive.Backend.DataModels
{
	public class UserProfile : Identifier
	{
		public UserProfile() : base()
		{
		}

		public string Job { get; set; }
		public string Department { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public Guid PictureId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        //link to the join table for many to many relation between user and group 
        public List<GroupUser> GroupUser { get; set; }
        //link to the join table for Many to Many navigation between Card and User (Views)
        public List<UserCardViews> UserCardViews { get; set; }
        //link to the join table for Many to Many navigation between Card and User (Likes)
        public List<UserCardLikes> UserCardLikes { get; set; }
    }
}