using System.Collections.Generic;
using TRAISI.ViewModels.Questions;
using FluentValidation;

namespace TRAISI.ViewModels.SurveyBuilder
{
    public class SBQuestionPartViewModel
    {
        public int Id { get; set; }

        public string QuestionType { get; set; }

        public ICollection<SBQuestionPartViewModel> QuestionPartChildren { get; set; }


        //public ICollection<QuestionConfigurationValueViewModel> QuestionConfigurations { get; set; }

        // public ICollection<QuestionOption> QuestionOptions { get; set; }

        //Whether this question part is responded to by the respondent group
        public bool IsGroupQuestion { get; set; } = false;
    }

		public class SBQuestionPartViewModelValidator : AbstractValidator<SBQuestionPartViewModel>
	{
		public SBQuestionPartViewModelValidator()
		{
			
		}
	}

}