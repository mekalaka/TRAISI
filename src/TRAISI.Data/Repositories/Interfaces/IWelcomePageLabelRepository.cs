using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRAISI.Data.Models;
using TRAISI.Data.Models.Surveys;

namespace TRAISI.Data.Repositories.Interfaces
{
	public interface IWelcomePageLabelRepository : IRepository<WelcomePageLabel>
	{
		Task<WelcomePageLabel> GetWelcomePageLabelAsync(int surveyId, string surveyViewName, string language = null);
    }
}