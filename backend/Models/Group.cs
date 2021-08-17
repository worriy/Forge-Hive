using System.Collections.Generic;
using System;
using Hive.Backend.Models.JoinTables;

namespace Hive.Backend.DataModels
{
    public class Group : Identifier
    {
        public Group() : base()
        {
        }

        public string Name { get; set; }
        public string Department { get; set; }
        public string City { get; set; }
        public DateTime CreationDate { get; set; }
        public string Country { get; set; }
        public Guid? CreatedById { get; set; }
        public virtual UserProfile CreatedBy { get; set; }
        //link to the join table for Many to Many navigation between Card and Group
        public List<CardGroup> CardGroup { get; set; }
        //link to the join table for many to many relation between group and user
        public List<GroupUser> GroupUser { get; set; }
    }
}