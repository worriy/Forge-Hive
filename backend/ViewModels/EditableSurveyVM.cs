using Hive.Backend.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.ViewModels
{
    public class EditableSurveyVM
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public HashSet<string> TargetGroupsIds { get; set; }
        public string Picture { get; set; }
        public HashSet<Question> Questions { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }
        public EditableSurveyVM()
        {

        }

        public EditableSurveyVM(string _id, string _content, DateTime _publicationDate, DateTime _endDate, HashSet<string> _targetGroupsIds, string _picture, HashSet<Question> _questions)
        {
            this.Id = _id;
            this.Content = _content;
            this.PublicationDate = _publicationDate;
            this.EndDate = _endDate;
            this.TargetGroupsIds = _targetGroupsIds;
            this.Picture = _picture;
            this.Questions = _questions;
        }



        public EditableSurveyVM ConvertFromModel(Survey survey, HashSet<Question> questions, Picture picture) =>
            new EditableSurveyVM
            {
                Id = survey.Id.ToString(),
                Content = survey.Content,
                PublicationDate = survey.PublicationDate,
                EndDate = survey.EndDate,
                Picture = picture.PicBase64,
                Questions = questions
            };

        public Survey GetSurveyFromViewModel() =>
            new Survey
            {
                Id = Guid.Parse(Id),
                Content = Content,
                PublicationDate = PublicationDate,
                EndDate = EndDate
            };
    }
}
