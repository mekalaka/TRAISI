using System.Collections.Generic;
using System.Threading.Tasks;
using DAL;
using DAL.Models.Questions;
using DAL.Models.Surveys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TRAISI.ViewModels;

namespace TRAISI.Controllers.SurveyViewer {
	
	[Authorize]
	[Route("api/[controller]")]
	public class SurveyViewContoller {
		
		private IUnitOfWork _unitOfWork;

		public SurveyViewContoller (IUnitOfWork unitOfWork) {
			this._unitOfWork = unitOfWork;

		}
		
		/// <summary>
		/// Return all questions for a given survey view.
		/// </summary>
		[HttpGet]
		[Produces (typeof (List<QuestionPart>))]
		public async Task<IActionResult> GetSurveyViews (int surveyId)
		{
			var surveys = await this._unitOfWork.SurveyViews.GetSurveyViews(surveyId);

			return new ObjectResult (surveys);
		}
		
		/// <summary>
		/// Return all questions for a given survey view.
		/// </summary>
		[HttpGet]
		[Produces (typeof (List<SurveyView>))]
		public async Task<IActionResult> GetSurveyViewQuestions (int viewId) {
			var surveys = await this._unitOfWork.SurveyViews.GetAsync (viewId);

			return new ObjectResult (surveys);
		}

	}
}