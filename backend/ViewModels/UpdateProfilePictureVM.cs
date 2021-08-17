using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.ViewModels
{
    public class UpdateProfilePictureVM
    {
        public UpdateProfilePictureVM()
        {
        }

        public string IdUser { get; set; }
        public string Picture { get; set; }
    }
}
