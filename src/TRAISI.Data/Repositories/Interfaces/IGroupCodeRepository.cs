using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRAISI.Data.Models;
using TRAISI.Data.Models.Surveys;

namespace TRAISI.Data.Repositories.Interfaces
{
    public interface IGroupcodeRepository : IRepository<Groupcode>
    {
        Task<IEnumerable<Groupcode>> GetGroupCodesForSurveyAsync(int surveyId, bool isTest, int pageIndex, int pageSize);
        IEnumerable<Groupcode> GetGroupCodesForSurvey(int surveyId, bool isTest);
        Task<int> GetCountOfGroupCodesForSurveyAsync(int surveyId, bool isTest);
        bool IsUniqueGroupCodeForSurvey(int surveyId, string code);
        IEnumerable<string> GetUniqueCodes(int surveyId, IEnumerable<string> codesToCheck);

        Task<Groupcode> GetGroupcodeForSurvey(Survey survey, string groupcode);

    }
}