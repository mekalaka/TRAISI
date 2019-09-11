using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Models.Surveys;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.IO;


namespace DAL.Repositories
{
    public class TermsAndConditionsPageLabelRepository : Repository<TermsAndConditionsPageLabel>, ITermsAndConditionsPageLabelRepository
    {
        public TermsAndConditionsPageLabelRepository(ApplicationDbContext context) : base(context) { }

        public TermsAndConditionsPageLabelRepository(DbContext context) : base(context) { }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;

				public async Task<TermsAndConditionsPageLabel> GetTermsAndConditionsPageLabelAsync(int surveyId, string surveyViewName, string language = null)
        {
					if (language != null) {
            return await _appContext.TermsAndConditionsPageLabels
									.Where(w => w.SurveyView.Survey.Id == surveyId && w.SurveyView.ViewName == surveyViewName && w.Language == language)
									.SingleOrDefaultAsync();
					}
					else {
						return await _appContext.TermsAndConditionsPageLabels
									.Where(w => w.SurveyView.Survey.Id == surveyId && w.SurveyView.ViewName == surveyViewName && w.Language == w.SurveyView.Survey.DefaultLanguage)
									.SingleOrDefaultAsync();
					}
        }

    }
}