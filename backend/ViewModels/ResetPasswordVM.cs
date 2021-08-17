using System.ComponentModel.DataAnnotations;

namespace Hive.Backend.ViewModels
{
	public partial class ResetPasswordVM 
	{
		public ResetPasswordVM() 
		{
		}

		public string Id  { get; set; }
		public string Password  { get; set; }
		public string Token  { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public ResetPasswordVM ConvertFromModel()
		{

			return this;
		}

	
	}
}