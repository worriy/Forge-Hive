using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hive.Backend.DataModels;


namespace Hive.Backend.ViewModels
{
	public partial class PostDetailsVM 
	{
		public PostDetailsVM() 
		{
			Results = new HashSet<Result>();
		}

		public string Id  { get; set; }
		public string Content  { get; set; }
		public string Type  { get; set; }
		public DateTime PublicationDate  { get; set; }
		public DateTime EndDate  { get; set; }
		public int Views  { get; set; }
        public int Likes { get; set; }
		public int Answers  { get; set; }
		public string Picture  { get; set; }
		public string TargetGroups  { get; set; }
		public string Status  { get; set; }
		private HashSet<Result> _results; 
		public HashSet<Result> Results { get { return _results ??(_results = new HashSet<Result>()); } set {  _results = value; }}
        private HashSet<Question> _questions;
        public HashSet<Question> Questions { get { return _questions ?? (_questions = new HashSet<Question>()); } set { _questions = value; } }

        [Timestamp]
		public byte[] RawVersion { get; set; }

		public PostDetailsVM ConvertFromModel(Card card, Reporting reporting, Picture picture, HashSet<Result> result) =>
			new PostDetailsVM
			{
				Id = card.Id.ToString(),
				Content = card.Content,
				Type = card.Type,
				PublicationDate = card.PublicationDate,
				EndDate = card.EndDate,
				Views = reporting.Views,

				Likes = reporting.Likes,
				Answers = reporting.Answers,
				Picture = picture.PicBase64,
				_results = result
			};

		public Card GetCardFromViewModel() =>
			new Card
			{
				Id = Guid.Parse(Id),
				Content = Content,
				Type = Type,
				PublicationDate = PublicationDate,
				EndDate = EndDate
			};
		public Reporting GetReportingFromViewModel() =>
			new Reporting
			{
				Views = Views,
				Likes = Likes,
				Answers = Answers
			};
		public Picture GetPictureFromViewModel() =>
			new Picture
			{
				PicBase64 = Picture
			};
	}
}