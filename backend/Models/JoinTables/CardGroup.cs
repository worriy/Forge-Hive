using Hive.Backend.DataModels;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hive.Backend.Models.JoinTables
{
    public class CardGroup
    {

        public CardGroup()
        {

        }

        public CardGroup(Guid _cardId, Guid _groupId)
        {
            this.CardId = _cardId;
            this.GroupId = _groupId;
        }

        [ForeignKey("card")]
        public Guid CardId { get; set; }
        public Card Card { get; set; }

        [ForeignKey("group")]
        public Guid GroupId { get; set; }
        public Group Group { get; set; }

    }
}
