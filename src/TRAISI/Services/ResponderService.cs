using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL;
using DAL.Models;
using DAL.Models.Questions;
using DAL.Models.ResponseTypes;
using DAL.Models.Surveys;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TRAISI.SDK.Enums;
using TRAISI.SDK.Interfaces;
using TRAISI.Services.Interfaces;

namespace TRAISI.Services
{
    /// <summary>
    /// Service for handling business logic related to respondents and the updating and querying
    /// of survey responses.
    /// </summary>
    public class ResponderService : IResponderService
    {
        private IUnitOfWork _unitOfWork;
        private IQuestionTypeManager _questionTypeManager;

        private ILoggerFactory _loggerFactory;

        private ILogger<ResponderService> _logger;

        public static readonly string LOCATION_RESPONSE = "location";
        public static readonly string TIMELINE_RESPONSE = "location";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_unitOfWork"></param>
        /// <param name="manager"></param>
        /// <param name="loggerFactory"></param>
        public ResponderService(IUnitOfWork _unitOfWork,
        IQuestionTypeManager manager,
        ILoggerFactory loggerFactory)
        {
            this._unitOfWork = _unitOfWork;
            this._questionTypeManager = manager;

            _loggerFactory = loggerFactory;

            _logger = loggerFactory.CreateLogger<ResponderService>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="subRespondentId"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public async Task<bool> SaveSubResponse(int questionId, int subRespondentId, JObject responseData)
        {
            var respondent = await this._unitOfWork.SurveyRespondents.GetSubRespondentAsync(subRespondentId);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="shortcode"></param>
        /// <param name="questionId"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public async Task<bool> SaveResponse(int surveyId, int questionId, ApplicationUser user, int respondentId, JObject responseData, int repeat)
        {


            var question = await this._unitOfWork.QuestionParts.GetAsync(questionId);
            var survey = await this._unitOfWork.Surveys.GetAsync(surveyId);
            var type = this._questionTypeManager.QuestionTypeDefinitions[question.QuestionType];

            var respondent = await this._unitOfWork.SurveyRespondents.GetAsync(respondentId);

            //var respondent = await this._unitOfWork.SurveyRespondents.GetPrimaryRespondentForUserAsync(user);

            if (respondent == null) {
                await this._unitOfWork.SurveyRespondents.CreatePrimaryResponentForUserAsnyc(user);
            }

            if (type.ResponseValidator != null) {
                type.ResponseValidator.ValidateResponse(null);
            }

            var surveyResponse = await this._unitOfWork.SurveyResponses.GetMostRecentResponseForQuestionByRespondentAsync(questionId,
                           (SurveyRespondent)respondent, repeat);
            bool isUpdate = false;

            if (surveyResponse == null) {
                surveyResponse = new SurveyResponse()
                {
                    QuestionPart = question,
                    Respondent = respondent,
                };
            }
            else {
                isUpdate = true;
            }


            surveyResponse.Repeat = repeat;

            switch (type.ResponseType) {
                case QuestionResponseType.String:
                    SaveStringResponse(survey, question, user, responseData, surveyResponse);
                    break;

                case QuestionResponseType.Decimal:
                    SaveDecimalResponse(survey, question, user, responseData, surveyResponse);
                    break;

                case QuestionResponseType.Integer:
                    SaveIntegerResponse(survey, question, user, responseData, surveyResponse);
                    break;

                case QuestionResponseType.Location:
                    SaveLocationResponse(survey, question, user, responseData, surveyResponse);
                    break;
                case QuestionResponseType.Timeline:
                    SaveTimelineResponse(survey, question, user, responseData, surveyResponse);
                    break;
                case QuestionResponseType.OptionSelect:
                    //
                    SaveOptionSelectResponse(survey, question, user, responseData, surveyResponse);
                    break;
            }

            try {

                if (!isUpdate) {
                    this._unitOfWork.SurveyResponses.Add(surveyResponse);
                }


                await this._unitOfWork.SaveChangesAsync();
            }
            catch (Exception e) {
                this._logger.LogError(e, "Error saving response.");
                return false;
            }

            return true;




        }

        internal void SavePrimaryRespondentName()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="survey"></param>
        /// <param name="question"></param>
        /// <param name="respondent"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        internal void SaveStringResponse(Survey survey, QuestionPart question, ApplicationUser respondent, JObject responseData, SurveyResponse response)
        {
            if (response.ResponseValues.Count == 0) {
                //response.ResponseValues = new List<ResponseValue>();
                response.ResponseValues.Add(new StringResponse());
            }


            (response.ResponseValues[0] as StringResponse).Value = responseData.GetValue("value").ToObject<string>();

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="survey"></param>
        /// <param name="question"></param>
        /// <param name="respondent"></param>
        /// <param name="responseData"></param>
        /// <param name="response"></param>
        internal void SaveIntegerResponse(Survey survey, QuestionPart question, ApplicationUser respondent, JObject responseData, SurveyResponse response)
        {
            if (response.ResponseValues.Count == 0) {
                //response.ResponseValues = new List<ResponseValue>();
                response.ResponseValues.Add(new IntegerResponse());
            }


            (response.ResponseValues[0] as IntegerResponse).Value = responseData.GetValue("value").ToObject<int>();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="survey"></param>
        /// <param name="question"></param>
        /// <param name="respondent"></param>
        /// <param name="responseData"></param>
        /// <param name="response"></param>
        internal void SaveOptionSelectResponse(Survey survey, QuestionPart question, ApplicationUser respondent, JObject responseData, SurveyResponse response)
        {
            if (response.ResponseValues.Count == 0) {
                //response.ResponseValues = new List<ResponseValue>();
                response.ResponseValues.Add(new OptionSelectResponse());
            }


            (response.ResponseValues[0] as OptionSelectResponse).Value = responseData.GetValue("value").ToObject<string>();

            return;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="survey"></param>
        /// <param name="question"></param>
        /// <param name="respondent"></param>
        /// <param name="responseData"></param>
        /// <param name="response"></param>
        internal void SaveDecimalResponse(Survey survey, QuestionPart question, ApplicationUser respondent, JObject responseData, SurveyResponse response)
        {
            if (response.ResponseValues.Count == 0) {
                //response.ResponseValues = new List<ResponseValue>();
                response.ResponseValues.Add(new DecimalResponse());
            }


            (response.ResponseValues[0] as DecimalResponse).Value = responseData.GetValue("value").ToObject<double>();

        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="survey"></param>
        /// <param name="question"></param>
        /// <param name="respondent"></param>
        /// <param name="response"></param> 
        /// <returns></returns>
        internal void SaveTimelineResponse(Survey survey, QuestionPart question, ApplicationUser respondent, JObject responseData, SurveyResponse response)
        {
            List<TimelineResponse> values = responseData["values"].ToObject<List<TimelineResponse>>();


            response.ResponseValues.Clear();

            response.ResponseValues.AddRange(values);

            return;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="survey"></param>
        /// <param name="question"></param>
        /// <param name="respondent"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        internal void SaveLocationResponse(Survey survey, QuestionPart question, ApplicationUser respondent, JObject responseData, SurveyResponse response)
        {
            if (response.ResponseValues.Count == 0) {
                //response.ResponseValues = new List<ResponseValue>();
                response.ResponseValues.Add(new LocationResponse());
            }
            var value = responseData.ToObject<LocationResponse>();
            (response.ResponseValues[0] as LocationResponse).Latitude = value.Latitude;
            (response.ResponseValues[0] as LocationResponse).Longitude = value.Longitude;
            (response.ResponseValues[0] as LocationResponse).Address = value.Address;

            //LocationResponse locationResponseValue = responseData.ToObject<LocationResponse>();

            return;


        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="shortcode"></param>
        /// <param name="questionId"></param>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public async Task<List<SurveyResponse>> ListResponses(int surveyId, string questionId)
        {

            var responses = await this._unitOfWork.SurveyResponses.GetAllAsync();

            return new List<SurveyResponse>();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="type"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<SurveyResponse>> ListResponsesOfType(int surveyId, string type, ApplicationUser user)
        {
            var respondent = await this._unitOfWork.SurveyRespondents.GetPrimaryRespondentForUserAsync(user);

            if (respondent == null) {
                await this._unitOfWork.SurveyRespondents.CreatePrimaryResponentForUserAsnyc(user);
            }

            var result = await this._unitOfWork.SurveyResponses.ListSurveyResponsesForRespondentByTypeAsync(surveyId, respondent, type);
            return result;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="List"></typeparam>
        /// <returns></returns>
        public async Task<SurveyResponse> GetRespondentMostRecentResponseForQuestion(int surveyId, int questionId, int respondentId, int repeat,
            ApplicationUser user)
        {

            //var respondent = await this._unitOfWork.SurveyRespondents.GetPrimaryRespondentForUserAsync(user);
            var respondent = await this._unitOfWork.SurveyRespondents.GetAsync(respondentId);
            var response =
                await this._unitOfWork.SurveyResponses.GetMostRecentResponseForQuestionByRespondentAsync(questionId,
                    respondent, repeat);

            return response;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="questionIds"></param>
        /// <param name="respondentId"></param>
        /// <returns></returns>
        public async Task<List<SurveyResponse>> ListSurveyResponsesForQuestionsAsync(List<int> questionIds, int respondentId)
        {
            var respondent = await this._unitOfWork.SurveyRespondents.GetAsync(respondentId);
            var responses = await this._unitOfWork.SurveyResponses.ListSurveyResponsesForQuestionsAsync(questionIds, respondent);
            return responses;
        }
    }
}