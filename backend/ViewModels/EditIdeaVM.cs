
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Hive.Backend.DataModels;
using Hive.Backend.Models.JoinTables;

namespace Hive.Backend.ViewModels
{
	public partial class EditIdeaVM 
	{
		public EditIdeaVM() 
		{
		}

		public string Id  { get; set; }
		public string Content  { get; set; }
		public DateTime PublicationDate  { get; set; }
		public DateTime EndDate  { get; set; }
        public List<string>  TargetGroupsIds  { get; set; }
		public string PictureId  { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public EditIdeaVM ConvertFromModel(Idea idea) =>
			new EditIdeaVM
			{
				Id = idea.Id.ToString(),
				Content = idea.Content,
				PublicationDate = idea.PublicationDate,
				EndDate = idea.EndDate,
				PictureId = idea.PictureId.ToString(),
				TargetGroupsIds = idea.CardGroup.Select(c => c.GroupId.ToString()).ToList()
			};

		public Idea GetIdeaFromViewModel()
		{
			var model = new Idea()
			{
				Id = Guid.Parse(Id),
				Content = Content,
				PublicationDate = PublicationDate,
				EndDate = EndDate,
				CardGroup = new List<CardGroup>(),
				PictureId = Guid.Parse(PictureId)
			};

            if (TargetGroupsIds != null)
            {
                foreach (var item in TargetGroupsIds)
                {
                    CardGroup cardGroup = new CardGroup();
                    cardGroup.GroupId = Guid.Parse(item);
                    cardGroup.CardId = Guid.Parse(Id);

                    model.CardGroup.Add(cardGroup);
                }
            }
			return model;
		}

	}
}