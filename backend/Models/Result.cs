using System;

namespace Hive.Backend.DataModels
{
	public class Result : Identifier
	{
		public Result() : base()
		{
		}

		public double Value { get; set; }
		public Guid ChoiceId { get; set; }
		public Guid ReportingId { get; set; }
		public virtual Choice Choice { get; set; }
		public virtual Reporting Reporting { get; set; }
	}
}