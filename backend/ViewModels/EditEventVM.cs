using Hive.Backend.DataModels;
using Hive.Backend.Models.JoinTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Hive.Backend.ViewModels
{
    public class EditEventVM
    {
        public EditEventVM()
        {

        }

        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> TargetGroupsIds { get; set; }
        public string PictureId { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }

    }
}
