using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Models.Surveys;

namespace DAL.Repositories.Interfaces
{
	public interface ISiteSurveyTemplateRepository : IRepository<SiteSurveyTemplate>
	{
		Task<IEnumerable<SiteSurveyTemplate>> GetSiteSurveyTemplatesAsync();
	}
}