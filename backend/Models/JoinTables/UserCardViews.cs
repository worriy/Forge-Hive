using Hive.Backend.DataModels;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hive.Backend.Models.JoinTables
{
    public class UserCardViews
    {

        [ForeignKey("card")]
        public Guid CardId { get; set; }
        public Card Card { get; set; }

        [ForeignKey("user")]
        public Guid UserId { get; set; }
        public UserProfile User { get; set; }

    }
}
