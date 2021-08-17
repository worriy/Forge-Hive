using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hive.Backend.ViewModels
{
    public class EditableIdeaVM
    {
		public EditableIdeaVM()
		{
		}

		public EditableIdeaVM(string _id, string _content, DateTime _publicationDate, DateTime _endDate, HashSet<string> _targetGroupsIds, string _picture)
		{
			this.Id = _id;
			this.Content = _content;
			this.PublicationDate = _publicationDate;
			this.EndDate = _endDate;
			this.TargetGroupsIds = _targetGroupsIds;
			this.Picture = _picture;

		}

		public string Id { get; set; }
		public string Content { get; set; }
		public DateTime PublicationDate { get; set; }
		public DateTime EndDate { get; set; }
		public HashSet<string> TargetGroupsIds { get; set; }
		public string Picture { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }

    }
}
