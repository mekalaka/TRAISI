using System;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Models.Questions;
using DAL.Models.Surveys;
using TRAISI.ViewModels;
using TRAISI.ViewModels.SurveyViewer;
using TRAISI.ViewModels.SurveyViewer.Enums;

namespace TRAISI.Services.Interfaces
{
    public interface ISurveyViewerService
    {
        /// <summary>
        /// Retrieves a QuestionPartView at index number in the SurveyView
        /// </summary>
        /// <param name="view"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        QuestionPartView GetQuestion(SurveyView view, int number);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="shortcode"></param>
        /// <returns></returns>
        Task<(bool loginSuccess, ApplicationUser user)> SurveyLogin(int surveyId, string shortcode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        Task<QuestionOptionsViewModel> GetQuestionOptions(int questionId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        Task<SurveyView> GetDefaultSurveyView(int surveyId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="survey"></param>
        /// <returns></returns>
        SurveyView GetDefaultSurveyView(Survey survey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="survey"></param>
        /// <param name="shortcode"></param>
        /// <returns></returns>
        bool AuthorizeSurveyUser(Survey survey, string shortcode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<SurveyStartViewModel> GetSurveyWelcomeView(string name);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="language"></param>
        /// <param name="viewType"></param>
        /// <returns></returns>
        Task<SurveyViewTermsAndConditionsViewModel> GetSurveyTermsAndConditionsText(int surveyId,
        string language = null,
        SurveyViewType viewType = SurveyViewType.CatiView);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        Task<QuestionPartView> GetSurveyViewPageQuestions(int viewId, int pageNumber);

    }
}