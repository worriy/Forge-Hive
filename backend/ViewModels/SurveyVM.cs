using Hive.Backend.DataModels;
using Hive.Backend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.ViewModels
{
    public class SurveyVM
    {
        public SurveyVM()
        {
            Results = new HashSet<Result>();
        }

        public string Id { get; set; }
        public string LinkedCardId { get; set; }
        public string Content { get; set; }
        public string TargetGroups { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Author { get; set; }
        public string Type { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Answers { get; set; }
        public string Picture { get; set; }
        private HashSet<Result> _results;
        public HashSet<Result> Results { get { return _results ?? (_results = new HashSet<Result>()); } set { _results = value; } }
        public string ContentSurvey { get; set; }
        //public string PictureSurvey { get; set; }
        public int MaxQuestions { get; set; }
        public int QuestionNumber { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }


        public SurveyVM ConvertFromModel(Card card) =>
            new SurveyVM
            {
                Id = card.Id.ToString(),
                LinkedCardId = card.LinkedCardId.ToString(),
                Content = card.Content,
                PublicationDate = card.PublicationDate,
                EndDate = card.EndDate,
                Type = card.Type,
                Answers = Type == CardTypes.Reporting.ToString() ? ((Reporting)card).Answers : 0,
                Views = Type == CardTypes.Reporting.ToString() ? ((Reporting)card).Views : 0,
                Likes = Type == CardTypes.Reporting.ToString() ? ((Reporting)card).Likes : 0,
                Author = card.CreatedBy != null && card.CreatedBy.User != null ? card.CreatedBy.User.FirstName + ' ' + card.CreatedBy.User.LastName : null,
                Picture = card.Picture != null ? card.Picture.PicBase64 : null
            };

        public Card GetCardFromViewModel() =>
            new Card
            {
                Id = Guid.Parse(Id),
                LinkedCardId = Guid.Parse(LinkedCardId),
                Content = Content,
                PublicationDate = PublicationDate,
                EndDate = EndDate,
                Type = Type
            };
    }
}
