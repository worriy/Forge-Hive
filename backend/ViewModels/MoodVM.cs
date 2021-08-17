using Hive.Backend.DataModels;
using Hive.Backend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hive.Backend.ViewModels
{
    public class MoodVM
    {
        public MoodVM()
        {
        }

        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public HashSet<string> TargetGroupsIds { get; set; }
        public string AuthorId { get; set; }
        public string PictureId { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }

        public MoodVM ConvertFromModel(Mood mood) =>
            new MoodVM
            {
                PublicationDate = mood.PublicationDate,
                EndDate = mood.EndDate
            };

        public Mood GetMoodFromViewModel() =>
            new Mood
            {
                PublicationDate = PublicationDate,
                EndDate = EndDate,
                Type = CardTypes.Mood.ToString()
            };
    }
}
