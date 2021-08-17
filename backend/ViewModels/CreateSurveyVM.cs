using Hive.Backend.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hive.Backend.ViewModels
{
    public class CreateSurveyVM
    {
        public CreateSurveyVM()
        {

        }

        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public HashSet<string> TargetGroupsIds { get; set; }
        public string AuthorId { get; set; }
        public string PictureId { get; set; }
        public HashSet<CreateQuestionVM> Questions { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }

    }
}
