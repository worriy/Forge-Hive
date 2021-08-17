using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Models.JoinTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hive.Backend.ViewModels
{
    public class CreateQuestionVM
    {
        public CreateQuestionVM()
        {
        }

        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public HashSet<string> TargetGroupsIds { get; set; }
        public string AuthorId { get; set; }
        public string PictureId { get; set; }
        public HashSet<string> Choices { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }

    }
}
