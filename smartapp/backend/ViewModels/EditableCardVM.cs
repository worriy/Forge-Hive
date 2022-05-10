using Hive.Backend.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.ViewModels
{
    public class EditableCardVM
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> TargetGroupsIds { get; set; }
        public string Picture { get; set; }
        public List<Choice> Choices { get; set; }
    }
}
