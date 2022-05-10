using Hive.Backend.Models;
using System;
using System.Collections.Generic;

namespace Hive.Backend.ViewModels
{
    public class CreatePostVM
    {
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> TargetGroupsIds { get; set; }
        public string AuthorId { get; set; }
        public string PictureId { get; set; }
        public List<string> Choices { get; set; }
        public CardTypes Type { get; set; }
    }
}
