using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public interface ISurveyService
    {
        IQueryable<Survey> GetAll();
        Task<Survey> GetById(Guid id);
        Task<Survey> InsertSurvey(Survey survey, List<Question> questions);
        Task Save(Survey entity, List<Question> questions);
        Task<EditableSurveyVM> GetEditableSurvey(Guid surveyId);
        Task<IQueryable<Card>> GetSurveyquestions(Guid surveyId);
        Task<IQueryable<Card>> GetSurveyReportsquestions(Guid surveyId);
        Task<string> GetContentsurvey(Guid surveyId);
        Task<int> GetMaxQuestions(Guid surveyId);
        Task<string> GetPictureSurvey(Guid surveyReportId);
    }
}
