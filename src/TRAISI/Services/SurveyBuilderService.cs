using DAL;
using DAL.Models.Questions;
using DAL.Models.Surveys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRAISI.Helpers;
using TRAISI.SDK;
using TRAISI.SDK.Interfaces;
using TRAISI.Services.Interfaces;

namespace TRAISI.Services
{

    /// <summary>
    /// Service for building surveys.
    /// </summary>
    public class SurveyBuilderService : ISurveyBuilderService
    {
        private IUnitOfWork _unitOfWork;

        private IQuestionTypeManager _questions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="questions"></param>
        public SurveyBuilderService(IUnitOfWork unitOfWork, IQuestionTypeManager questions)
        {
            this._unitOfWork = unitOfWork;
            this._questions = questions;
        }


        /// <summary>
        /// Adds a new view to the specified survey
        /// </summary>
        /// <param name="survey"></param>
        /// <param name="viewName"></param>
        public SurveyView AddSurveyView(Survey survey, string viewName)
        {
            var surveyView = new SurveyView()
            {
                ViewName = viewName
            };
            survey.SurveyViews.Add(surveyView);

            return surveyView;
        }

        /// <summary>
        /// Removes the specified view from the targetted survey
        /// </summary>
        /// <param name="survey"></param>
        /// <param name="id"></param>
        public void RemoveSurveyView(Survey survey, int id)
        {
            survey.SurveyViews.Remove(survey.SurveyViews.Single(s => s.Id == id));
        }

        /// <summary>
        /// Sets a configuration value for the specified question part
        /// </summary>
        /// <param name="questionPart"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public QuestionConfiguration SetQuestionConfiguration(QuestionPart questionPart, string name, object value)
        {
            var configuration = questionPart.QuestionConfigurations.SingleOrDefault(c => c.Name == name);

            if (configuration == null) {
                QuestionConfiguration qc = new QuestionConfiguration()
                {
                    Name = name,
                    Value = value.ToString()
                };
                questionPart.QuestionConfigurations.Add(qc);
                return qc;
            }

            configuration.Value = value.ToString();
            return configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qpv"></param>
        /// <param name="text"></param>
        /// <param name="language"></param>
        public void SetQuestionPartViewLabel(QuestionPartView qpv, string text, string language = null)
        {
            qpv.Labels[language] = new QuestionPartViewLabel()
            {
                Language = language ?? "en",
                Value = text,
                QuestionPartView = qpv
            };


        }


        /// <summary>
        /// Sets a question option value on the specified question part. If langauge is null then the default label is used, otherwise 
        /// the value data is set on the matched langauge label.
        /// </summary>
        /// <param name="questionPart"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="language"></param>
        public void SetQuestionOption(QuestionPart questionPart, string name, string value, string language = null)
        {
            var option = questionPart.QuestionOptions.SingleOrDefault(o => o.Name == name);
            if (option != null) {
                if (language == null) {
                    option.QuestionOptionLabels.First().Value = value;
                }
                else {
                    var optionLabel = option.QuestionOptionLabels.FirstOrDefault(v => v.Language == language);
                    if (optionLabel == null) {
                        option.QuestionOptionLabels.Add(new QuestionOptionLabel()
                        {
                            Language = language,
                            Value = value,
                            QuestionOption = option
                        });
                    }
                    else {
                        optionLabel.Value = value;
                    }
                }
            }
            else {
                option.QuestionOptionLabels.Add(new QuestionOptionLabel()
                {
                    Language = language,
                    Value = value,
                    QuestionOption = option
                });

                questionPart.QuestionOptions.Add(option);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="part"></param>
        /// <param name="name"></param>
        /// <param name="language"></param>
        public void AddQuestionOption(QuestionPart part, string name, string value, string label, string language = null)
        {
            //check if the option has a value / allows multiple

            var definition =
                this._questions.QuestionTypeDefinitions.FirstOrDefault(d => d.TypeName == part.QuestionType);
            if (definition != null) {
                var hasOption = definition.QuestionOptions.Count(c => c.Key == name);

                if (definition.QuestionOptions.Keys.Contains(name)) {
                    if (definition.QuestionOptions[name].IsMultipleAllowed) {
                        part.QuestionOptions.Add(new QuestionOption()
                        {
                            Name = name

                        });
                    }
                    else if (part.QuestionOptions.Count(o => o.Name == name) == 0) {
                        part.QuestionOptions.Add(new QuestionOption()
                        {
                            Name = name
                        });
                    }
                    else {
                        throw new InvalidOperationException("Cannot asssign new question option, remove first.");
                    }
                }
                else {
                    throw new ArgumentException("Question Option does not exist for this question type.");
                }
            }
        }

        /// <summary>
        /// Removes a set question option from the specified question part
        /// </summary>
        /// <param name="part"></param>
        /// <param name="name"></param>
        /// <param name="language">Null to remove all instances, otherwise remove a specific label</param>
        public void RemoveQuestionOption(QuestionPart part, string name, string language = null)
        {
            var option = part.QuestionOptions.SingleOrDefault(c => c.Name == name);
            if (language == null) {
                part.QuestionOptions.Remove(option);
            }
            else {
                option?.QuestionOptionLabels.Remove(
                    option.QuestionOptionLabels.SingleOrDefault(v => v.Language == language));
            }
        }

        /// <summary>
        /// Removes a configuration value (resets to default) from the specified QuestionPart.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="name"></param>
        public void RemoveQuestionConfiguration(QuestionPart part, string name)
        {
            part.QuestionConfigurations.Remove(part.QuestionConfigurations.SingleOrDefault(c => c.Name == name));
        }

        /// <summary>
        /// Retrieves the list of question configurations associated with this question part. Only values that differ
        /// from default values will be returned (default values are not stored).
        /// </summary>
        /// <param name="questionPart"></param>
        /// <returns></returns>
        public IEnumerable<QuestionConfiguration> GetQuestionConfigurations(QuestionPart questionPart)
        {
            return questionPart.QuestionConfigurations;
        }

        /// <summary>
        /// Returns the set of question options associated with this question part.
        /// </summary>
        /// <param name="questionPart"></param>
        /// <param name="language">Specify a main label language</param>
        /// <returns></returns>
        public IEnumerable<QuestionOption> GetQuestionOptions(QuestionPart questionPart, string language = null)
        {
            return questionPart.QuestionOptions;
        }

        /// <summary>
        /// Adds a page to a survey
        /// </summary>
        /// <param name="view"></param>
        /// <param name="newPage"></param>
        public void AddSurveyPage(SurveyView view, QuestionPartView newPage)
        {
            view.QuestionPartViews.Add(newPage);
            newPage.Order = view.QuestionPartViews.Count - 1;
        }

        /// <summary>
        /// Removes a page from a survey
        /// </summary>
        /// <param name="view"></param>
        /// <param name="pageId"></param>
        public void RemoveSurveyPage(SurveyView view, int pageId)
        {
            List<QuestionPartView> pages = view.QuestionPartViews as List<QuestionPartView>;
            QuestionPartView toDelete = null;
            int pageIndex = Int32.MaxValue;
            for (int i = 0; i < pages.Count; i++) {
                if (pages[i].Order > pageIndex) {
                    pages[i].Order--;
                }
                else if (pages[i].Id == pageId) {
                    toDelete = pages[i];
                    pageIndex = toDelete.Order;
                }
            }
            view.QuestionPartViews.Remove(toDelete);
        }

        public void ReOrderPages(SurveyView view, List<QuestionPartView> newOrder)
        {
            Dictionary<int, int> newOrderDict = newOrder.ToDictionary(r => r.Id, r => r.Order);
            foreach (var qpartView in view.QuestionPartViews) {
                qpartView.Order = newOrderDict[qpartView.Id];
            }
        }

        /// <summary>
        /// Adds a question to a page/question part
        /// </summary>
        /// <param name="view"></param>
        /// <param name="newPage"></param>
        public void AddQuestionPartView(QuestionPartView ParentQuestionPartView, QuestionPartView ChildQuestionPartView)
        {
            //update orders for existing questions
            foreach (var question in ParentQuestionPartView.QuestionPartViewChildren) {
                if (question.Order >= ChildQuestionPartView.Order) {
                    question.Order++;
                }
            }
            ParentQuestionPartView.QuestionPartViewChildren.Add(ChildQuestionPartView);
        }

        /// <summary>
        /// Removes a question/question part from a survey
        /// </summary>
        /// <param name="questionPartView"></param>
        /// <param name="childQuestionPartViewId"></param>
        public void RemoveQuestionPartView(QuestionPartView questionPartView, int childQuestionPartViewId)
        {
            if (questionPartView != null) {
                var childQuestions = questionPartView.QuestionPartViewChildren.OrderBy(q => q.Order);
                QuestionPartView toDelete = null;
                int questionIndex = Int32.MaxValue;
                foreach (var childQuestion in childQuestions) {
                    if (childQuestion.Order > questionIndex) {
                        childQuestion.Order--;
                    }
                    else if (childQuestion.Id == childQuestionPartViewId) {
                        toDelete = childQuestion;
                        questionIndex = toDelete.Order;
                    }
                }
                questionPartView.QuestionPartViewChildren.Remove(toDelete);
            }
        }

        /// <summary>
        /// Reorder question within question part
        /// </summary>
        /// <param name="questionPartView"></param>
        /// <param name="newOrder"></param>
        public void ReOrderQuestions(QuestionPartView questionPartView, List<QuestionPartView> newOrder)
        {
            Dictionary<int, int> newOrderDict = newOrder.ToDictionary(r => r.Id, r => r.Order);
            foreach (var qpartView in questionPartView.QuestionPartViewChildren) {
                qpartView.Order = newOrderDict[qpartView.Id];
            }
        }


        /// <summary>
        /// Adds a question part to the specified SurveyView - this creates a new QuestionPartView from the part
        /// </summary>
        /// <param name="view"></param>
        /// <param name="part"></param>
        /// <param name="position">0-index order, position of the qustion</param>
        /// <returns>A reference to the created QuestionPartView</returns>
        public QuestionPartView AddQuestionPart(SurveyView view, QuestionPart part, QuestionTypeDefinition definition,
            int position = -1)
        {
            QuestionPartView questionPartView = new QuestionPartView
            {
                QuestionPart = part
            };
            if (position >= 0) {
                questionPartView.Order = position;
                (view.QuestionPartViews as List<QuestionPartView>)?.Insert(position, questionPartView);
            }
            else {
                (view.QuestionPartViews as List<QuestionPartView>)?.Add(questionPartView);
                questionPartView.Order = view.QuestionPartViews.Count - 1;
            }

            return questionPartView;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="part"></param>
        /// <param name="definition"></param>
        public void AddQuestionPartChild(QuestionPart part, QuestionTypeDefinition definition)
        {
            part.QuestionType = definition.TypeName;
            part.QuestionPartChildren.Add(part);
        }

        /// <summary>
        /// Sets a survey title for the specified language.
        /// </summary>
        /// <param name="survey"></param>
        /// <param name="title"></param>
        /// <param name="language"></param>
        public void SetSurveyTitle(Survey survey, string title, string language)
        {
            survey.TitleLabels[language] = new TitlePageLabel
            {
                Language = language,
                Value = title
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="part"></param>
        /// <param name="definition"></param>
        /// <param name="position"></param>
        public QuestionPartView AddQuestion(SurveyView view, QuestionTypeDefinition definition, int position = -1)
        {
            QuestionPartView qpv = new QuestionPartView();
            qpv.QuestionPart = new QuestionPart()
            {
                QuestionType = definition.TypeName
            };
            if (position < 0) {
                view.QuestionPartViews.Add(qpv);
            }
            else {
                ((List<QuestionPartView>)view.QuestionPartViews).Insert(position, qpv);
            }

            return qpv;
        }

    }
}