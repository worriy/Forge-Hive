using Hive.Backend.DataModels;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hive.Backend.Models.JoinTables
{
    public class GroupUser
    {

        [ForeignKey("group")]
        public Guid GroupId { get; set; }
        public Group Group { get; set; }

        [ForeignKey("user")]
        public Guid UserId { get; set; }
        public UserProfile User { get; set; }

    }
}
