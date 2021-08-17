using Hive.Backend.DataModels;
using Hive.Backend.Models.JoinTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.ViewModels
{
    public class EditQuestionVM
    {
        public EditQuestionVM()
        {

        }

        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> TargetGroupsIds { get; set; }
        public string PictureId { get; set; }
        public HashSet<Choice> Choices { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }
    }
}
