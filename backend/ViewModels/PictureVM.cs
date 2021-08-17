using System.ComponentModel.DataAnnotations;
using Hive.Backend.DataModels;

namespace Hive.Backend.ViewModels
{
	public partial class PictureVM 
	{
		public PictureVM() 
		{
		}

		public string Picture  { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public PictureVM ConvertFromModel(Picture picture) =>
			new PictureVM
			{
				Picture = picture.PicBase64
			};

		public Picture GetPictureFromViewModel() =>
			new Picture
			{
				PicBase64 = Picture
			};
	}
}