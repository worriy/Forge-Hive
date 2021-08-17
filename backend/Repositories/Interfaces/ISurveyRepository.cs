using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public interface ISurveyRepository : IRepository<Survey>
    {
        IQueryable<Survey> GetAllWithReferences();
        Task<Survey> GetByIdWithReferences(Guid id);
        Task<List<Question>> GetSurveyWithQuestions(Guid idSurvey);
        Task<Survey> InsertSurvey(Survey survey, List<Question> questions);
        Task UpdateSurvey(Survey oldEntity, Survey entity, List<Question> questions);
        Task<EditableSurveyVM> GetEditableSurvey(Guid surveyId);
        Task<IQueryable<Card>> GetSurveyquestions(Guid surveyId);
        Task<IQueryable<Card>> GetSurveyReportsquestions(Guid surveyId);
        //Task<EditableSurveyVM> GetEditableSurvey(int idSurvey);
        Task<string> GetContentsurvey(Guid surveyId);
        Task<int> GetMaxQuestions(Guid surveyId);
        Task<string> GetPictureSurvey(Guid surveyReportId);
    }
}
