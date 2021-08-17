using System.ComponentModel.DataAnnotations;

namespace Hive.Backend.ViewModels
{
	public partial class SignInVM 
	{
		public SignInVM() 
		{
		}

		public string Email  { get; set; }
		public string Password  { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public SignInVM ConvertFromModel()
		{

			return this;
		}

	
	}
}