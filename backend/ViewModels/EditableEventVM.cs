using Hive.Backend.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.ViewModels
{
    public class EditableEventVM
    {
        public EditableEventVM()
        {

        }

        public EditableEventVM(string _id, string _content, DateTime _publicationDate, DateTime _endDate, HashSet<string> _targetGroupsIds, string _picture, HashSet<Choice> _choices)
        {
            this.Id = _id;
            this.Content = _content;
            this.PublicationDate = _publicationDate;
            this.EndDate = _endDate;
            this.TargetGroupsIds = _targetGroupsIds;
            this.Picture = _picture;
            this.Choices = _choices;

        }

        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public HashSet<string> TargetGroupsIds { get; set; }
        public string Picture { get; set; }
        public HashSet<Choice> Choices { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }

        public EditableEventVM ConvertFromModel(Event ev, Group group, Picture picture) =>
            new EditableEventVM
            {
                Id = ev.Id.ToString(),
                Content = ev.Content,
                PublicationDate = ev.PublicationDate,
                EndDate = ev.EndDate,
                Picture = picture.PicBase64,
                Choices = ev.Choices
            };

        public Event GetQuestionFromViewModel() =>
            new Event
            {
                Id = Guid.Parse(Id),
                Content = Content,
                PublicationDate = PublicationDate,
                EndDate = EndDate,
                Choices = Choices
            };

        public Picture GetPictureFromViewModel() =>
            new Picture
            {
                PicBase64 = Picture
            };
    }
}
