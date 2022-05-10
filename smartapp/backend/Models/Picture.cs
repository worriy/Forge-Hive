
namespace Hive.Backend.DataModels
{
	public class Picture : Identifier
	{
		public Picture() : base()
		{
		}

		public string PicBase64 { get; set; }
	}
}