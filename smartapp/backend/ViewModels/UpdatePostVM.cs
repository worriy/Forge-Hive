using Hive.Backend.DataModels;
using System;
using System.Collections.Generic;

namespace Hive.Backend.ViewModels
{
    public class UpdatePostVM
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> TargetGroupsIds { get; set; }
        public string PictureId { get; set; }
        public List<Choice> Choices { get; set; }
    }
}
