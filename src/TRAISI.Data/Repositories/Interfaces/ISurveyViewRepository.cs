using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRAISI.Data.Models;
using TRAISI.Data.Models.Surveys;

namespace TRAISI.Data.Repositories.Interfaces
{
	public interface ISurveyViewRepository : IRepository<SurveyView>
	{
		Task<List<SurveyView>> GetSurveyViews(int surveyId);
		Task<SurveyView> GetSurveyViewWithPagesStructureAsync(int surveyId, string viewName);
        SurveyView GetSurveyViewWithPagesStructure(int surveyId, string viewName);
        SurveyView GetSurveyViewQuestionAndOptionStructure(int surveyId, string viewName);
        SurveyView GetSurveyViewQuestionStructure(int surveyId, string viewName);
    }
}