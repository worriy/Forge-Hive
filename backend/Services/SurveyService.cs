using Hive.Backend.DataModels;
using Hive.Backend.Repositories;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _repository;

        public SurveyService(ISurveyRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<Survey> GetAll()
        {
            return _repository.GetAllWithReferences();
        }

        public async Task<Survey> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }

        

        public async Task<Survey> InsertSurvey(Survey survey, List<Question> questions)
        {
            if (survey == null)
            {
                throw new ArgumentNullException(nameof(survey));
            }

            return await _repository.InsertSurvey(survey, questions);
        }

        public async Task Save(Survey entity, List<Question> questions)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var oldEntity = await GetById(entity.Id);
            await _repository.UpdateSurvey(oldEntity, entity, questions);
        }

        public async Task<EditableSurveyVM> GetEditableSurvey(Guid surveyId)
        {
            return await _repository.GetEditableSurvey(surveyId);
        }

        public async Task<IQueryable<Card>> GetSurveyquestions(Guid surveyId)
        {
            return await _repository.GetSurveyquestions(surveyId);
        }

        public async Task<IQueryable<Card>> GetSurveyReportsquestions(Guid surveyReportId)
        {
            return await _repository.GetSurveyReportsquestions(surveyReportId);
        }

        #region  HELPERS

        public async Task<string> GetContentsurvey(Guid surveyId)
        {
            return await _repository.GetContentsurvey(surveyId);
        }

        public async Task<int> GetMaxQuestions(Guid surveyId)
        {
            return await _repository.GetMaxQuestions(surveyId);
        }

        public async Task<string> GetPictureSurvey(Guid surveyReportId)
        {
            return await _repository.GetPictureSurvey(surveyReportId);
        }
        #endregion
    }
}
