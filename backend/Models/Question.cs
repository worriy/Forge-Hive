
using System;

namespace Hive.Backend.DataModels
{
    public class Question : Card
    {
        public Question() : base()
        {

        }
        public Guid SurveyId { get; set; }
    }
}
