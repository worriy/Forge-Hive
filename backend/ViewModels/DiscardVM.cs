using System;
using System.ComponentModel.DataAnnotations;
using Hive.Backend.DataModels;


namespace Hive.Backend.ViewModels
{
    public partial class DiscardVM
    {
        public DiscardVM()
        {
        }

        public string UserProfileId { get; set; }
        public string CardId { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }

    }
}