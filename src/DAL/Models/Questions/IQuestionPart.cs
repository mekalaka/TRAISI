using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Models.Surveys;

namespace DAL.Models.Questions
{
    public interface IQuestionPart
    {

        int Id { get; set; }

        [NotMapped]
        string Text { get; set; }

        ICollection<Label> TextLabels { get; set; }

        ICollection<QuestionPart> QuestionPartChildren { get; set; }

        IQuestionConfiguration QuestionConfiguration { get; set; }

		IQuestionConfiguration QuestionSettings{get;set;}

        SurveyView SurveyView { get; set; }

    }
}