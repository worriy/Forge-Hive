using Hive.Backend.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.ViewModels
{
    public class EditSuggestionVM
    {
        public EditSuggestionVM()
        {

        }

        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PictureId { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }

    }
}
