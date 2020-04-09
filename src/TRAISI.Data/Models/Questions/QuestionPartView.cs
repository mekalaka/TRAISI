using System.Collections.Generic;
using TRAISI.Data.Models.Surveys;
using TRAISI.Data.Models.Interfaces;
using TRAISI.Data.Models.Extensions;
using Newtonsoft.Json;

namespace TRAISI.Data.Models.Questions
{
    /// <summary>
    /// QuestionPartView model - container of QuestionParts 
    /// </summary>
    public class QuestionPartView : IQuestionPartView, IEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// Collection of Label objects for this view.
        /// </summary>
        /// <value></value>
        public LabelCollection<QuestionPartViewLabel> Labels { get; set; }

        /// <summary>
        /// The associated QuestionPart for this QuestionPartView
        /// </summary>
        /// <value></value>
        public QuestionPart QuestionPart { get; set; }

        public QuestionPartView ParentView { get; set; }

        /// List of conditionals existing on this
        public List<QuestionConditionalOperator> Conditionals { get; set; }

        [JsonIgnore]
        public int? ParentViewId { get; set; }

        /// <summary>
        /// The Survey View this question belongs to.
        /// </summary>
        /// <value></value>
        public SurveyView SurveyView { get; set; }

        [JsonIgnore]
        public int? SurveyViewId { get; set; }

        public ICollection<QuestionPartView> QuestionPartViewChildren { get; set; }

        /// <summary>
        /// The order (position) of this question part view inside of its containing survey view.
        /// </summary>
        /// <value></value>
        public int Order { get; set; }

        public bool IsOptional { get; set; }
        public bool IsHousehold { get; set; }

        public bool IsMultiView { get; set; }

        public bool IsDefaultHidden { get; set; }

        public QuestionPart RepeatSource { get; set; }

        public string Icon { get; set; }

        public QuestionPartView CATIDependent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public QuestionPartView()
        {
            this.Labels = new LabelCollection<QuestionPartViewLabel>();
            this.QuestionPartViewChildren = new List<QuestionPartView>();
            this.Conditionals = new List<QuestionConditionalOperator>();
        }
    }
}