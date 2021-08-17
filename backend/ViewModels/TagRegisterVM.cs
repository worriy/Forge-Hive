using System.ComponentModel.DataAnnotations;

namespace Hive.Backend.ViewModels
{
	public partial class TagRegisterVM 
	{
		public TagRegisterVM() 
		{
		}

		public string InstallationId  { get; set; }
		public string RegistrationId  { get; set; }
		public string[] Tags  { get; set; }
		public string Platform  { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public TagRegisterVM ConvertFromModel()
		{

			return this;
		}

	
	}
}