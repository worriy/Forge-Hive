using Hive.Backend.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.ViewModels
{
    public class EditableQuoteVM
    {
        public EditableQuoteVM()
        {

        }

        public EditableQuoteVM(string _id, string _content, DateTime _publicationDate, DateTime _endDate)
        {
            this.Id = _id;
            this.Content = _content;
            this.PublicationDate = _publicationDate;
            this.EndDate = _endDate;

        }

        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }



        public EditableQuoteVM ConvertFromModel(Quote quote) =>
            new EditableQuoteVM
            {
                Id = quote.Id.ToString(),
                Content = quote.Content,
                PublicationDate = quote.PublicationDate,
                EndDate = quote.EndDate
            };

        public Quote GetQuoteFromViewModel() =>
            new Quote
            {
                Id = Guid.Parse(Id),
                Content = Content,
                PublicationDate = PublicationDate,
                EndDate = EndDate
            };
    }
}
