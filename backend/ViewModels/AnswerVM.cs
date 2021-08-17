using System;
using System.ComponentModel.DataAnnotations;
using Hive.Backend.DataModels;


namespace Hive.Backend.ViewModels
{
    public partial class AnswerVM
    {
        public AnswerVM()
        {
        }

        public string IdUser { get; set; }
        public string IdCard { get; set; }
        public string IdChoice { get; set; }

        [Timestamp]
        public byte[] RawVersion { get; set; }

        public AnswerVM ConvertFromModel(UserProfile userProfile, Card card, Choice choice) =>
            new AnswerVM
            {
                IdUser = userProfile.Id.ToString(),
                IdCard = card.Id.ToString(),
                IdChoice = choice.Id.ToString()
            };

        public UserProfile GetUserProfileFromViewModel() =>
            new UserProfile
            {
                Id = Guid.Parse(IdUser)
            };

        public Card GetCardFromViewModel() =>
            new Card
            {
                Id = Guid.Parse(IdCard)
            };

        public Choice GetChoiceFromViewModel() =>
            new Choice
            {
                Id = Guid.Parse(IdChoice)
            };

    }
}