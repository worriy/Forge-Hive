using System.Collections.Generic;
using System;
using Hive.Backend.Models.JoinTables;

namespace Hive.Backend.DataModels
{
    public class Card : Identifier
    {
        public Card() : base()
        {
        }

        public DateTime PublicationDate { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public DateTime EndDate { get; set; }
        public Guid LinkedCardId { get; set; }
        public Guid CreatedById { get; set; }
        public virtual UserProfile CreatedBy { get; set; }
        public Guid PictureId { get; set; }
        public virtual Picture Picture { get; set; }
        public List<Choice> Choices { get; set; }
        //link to the join table for Many to Many navigation between Card and Group
        public List<CardGroup> CardGroup { get; set; }
        //link to the join table for Many to Many navigation between Card and User For Views
        public List<UserCardViews> UserCardViews { get; set; }
        //link to the join table for Many to Many navigation between Card and User For Likes
        public List<UserCardLikes> UserCardLikes { get; set; }
    }
}