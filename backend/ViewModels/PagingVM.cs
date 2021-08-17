using System.ComponentModel.DataAnnotations;

namespace Hive.Backend.ViewModels
{
	public partial class PagingVM 
	{
		public PagingVM() 
		{
		}

		public int Step  { get; set; }
		public int Page  { get; set; }

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public PagingVM ConvertFromModel()
		{

			return this;
		}
	}
}