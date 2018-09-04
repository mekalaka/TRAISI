﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Models.ResponseTypes;
using DAL.Models.Surveys;


namespace DAL.Models.Questions
{

    /// <summary>
    /// 
    /// </summary>
    public class QuestionPart : IQuestionPart
    {

        public QuestionPart()
        {

            QuestionPartChildren = new HashSet<QuestionPart>();
            QuestionOptions = new HashSet<QuestionOption>();
            QuestionConfigurations = new HashSet<QuestionConfiguration>();
        }

        public int Id { get; set; }

        public string QuestionType { get; set; }
        
        public string Name { get; set; }

        public ICollection<QuestionPart> QuestionPartChildren { get; set; }


        public ICollection<QuestionConfiguration> QuestionConfigurations { get; set; }

        public ICollection<QuestionOption> QuestionOptions { get; set; }

        //conditionals where this question is the source and a question is the target
        public ICollection<QuestionConditional> QuestionConditionalsSource { get; set; }

        //conditionals where this question is the target
        public ICollection<QuestionConditional> QuestionConditionalsTarget { get; set; }

        //conditionals where this question is the source and a question option is the target
        public ICollection<QuestionOptionConditional> QuestionOptionConditionalsSource { get; set; }

        //Whether this question part is responded to by the respondent group
        public bool IsGroupQuestion { get; set; } = false;

    }
}