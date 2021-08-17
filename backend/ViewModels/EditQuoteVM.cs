using Hive.Backend.DataModels;
using Hive.Backend.Models.JoinTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.ViewModels
{
    public class EditQuoteVM
    {
        public EditQuoteVM()
        {

        }

        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime EndDate { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }
    }
}
