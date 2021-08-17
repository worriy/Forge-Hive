using Hive.Backend.DataModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hive.Backend.ViewModels
{
    public class EditableSuggestionVM
    {
        public EditableSuggestionVM()
        {

        }

        public EditableSuggestionVM(string _id, string _content, DateTime _publicationDate, DateTime _endDate, string _picture)
        {
            this.Id = _id;
            this.Content = _content;
            this.PublicationDate = _publicationDate;
            this.EndDate = _endDate;
            this.Picture = _picture;

        }

        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Picture { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }



        public EditableSuggestionVM ConvertFromModel(Suggestion suggestion, Picture picture) =>
            new EditableSuggestionVM
            {
                Id = suggestion.Id.ToString(),
                Content = suggestion.Content,
                PublicationDate = suggestion.PublicationDate,
                EndDate = suggestion.EndDate,
                Picture = picture.PicBase64
            };

        public Suggestion GetSuggestionFromViewModel() =>
            new Suggestion
            {
                Id = Guid.Parse(Id),
                Content = Content,
                PublicationDate = PublicationDate,
                EndDate = EndDate
            };

        public Picture GetPictureFromViewModel() =>
            new Picture
            {
                PicBase64 = Picture
            };
    }
}
