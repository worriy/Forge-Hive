
namespace Hive.Backend.ViewModels
{
	public partial class TargetGroupVM 
	{
		public TargetGroupVM() 
		{
		}

        public TargetGroupVM(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

		public string Id  { get; set; }
		public string Name  { get; set; }
			
	}
}