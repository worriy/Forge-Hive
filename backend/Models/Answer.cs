using System;

namespace Hive.Backend.DataModels
{
	public class Answer : Identifier
	{
		public Answer() : base()
		{
		}

		public DateTime AnswerDate { get; set; }
		public Guid AnsweredById { get; set; }
		public Guid CardId { get; set; }
		public Guid ChoiceId { get; set; }
		public virtual UserProfile AnsweredBy { get; set; }
		public virtual Card Card { get; set; }
		public virtual Choice Choice { get; set; }
	}
}