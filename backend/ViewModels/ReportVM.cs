
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hive.Backend.DataModels;


namespace Hive.Backend.ViewModels
{
	public partial class ReportVM 
	{
		public ReportVM() 
		{
			Results = new HashSet<Result>();
		}

		public string Id  { get; set; }
		public string Content  { get; set; }
		public string Author  { get; set; }
		public int Views  { get; set; }
        public int Likes { get; set; }
		public int Answers  { get; set; }
		private HashSet<Result> _results; 
		public HashSet<Result> Results { get { return _results ??(_results = new HashSet<Result>()); } set {  _results = value; }}

		[Timestamp]
		public byte[] RawVersion { get; set; }

		public ReportVM ConvertFromModel(Reporting reporting) =>
			new ReportVM
			{
				Id = reporting.Id.ToString(),
				Content = reporting.Content,
				Author = reporting.CreatedBy != null && reporting.CreatedBy.User != null ? reporting.CreatedBy.User.FirstName + ' ' + reporting.CreatedBy.User.LastName : null,
				Views = reporting.Views,
				Likes = reporting.Likes,
				Answers = reporting.Answers,
				_results = reporting.Results
			};

		public Reporting GetReportingFromViewModel() =>
			new Reporting
			{
				Id = Guid.Parse(Id),
				Content = Content,
				Views = Views,
				Likes = Likes,
				Answers = Answers
			};
	}
}