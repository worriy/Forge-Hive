using System;
using System.ComponentModel.DataAnnotations;
using Hive.Backend.DataModels;


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

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public TargetGroupVM ConvertFromModel(Group group) =>
			new TargetGroupVM
			{
				Id = group.Id.ToString(),
				Name = group.Name
			};

		public Group GetGroupFromViewModel() =>
			new Group
			{
				Id = Guid.Parse(Id),
				Name = Name
			};
			
	}
}