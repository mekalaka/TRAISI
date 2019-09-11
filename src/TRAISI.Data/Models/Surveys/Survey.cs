﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Models.Extensions;
using DAL.Models.Interfaces;
using DAL.Models.Questions;
using Newtonsoft.Json;

namespace DAL.Models.Surveys
{
    public class Survey : AuditableEntity, ISurvey, IEntity
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Group { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsOpen { get; set; }
        public string SuccessLink { get; set; }
        public string RejectionLink { get; set; }
        public string DefaultLanguage { get; set; }
        public string StyleTemplate { get; set; }

        public SurveyViewCollection<SurveyView> SurveyViews { get; set; }
        public ICollection<SurveyPermission> SurveyPermissions { get; set; }
        public ICollection<Groupcode> GroupCodes { get; set; }
        public ICollection<Shortcode> Shortcodes { get; set; }

        public ICollection<ExtensionConfiguration> ExtensionConfigurations { get; set; }

        public LabelCollection<TitlePageLabel> TitleLabels { get; set; }

        [NotMapped]
        public bool HasGroupCodes { get; set; }

        public Survey()
        {
            SurveyViews = new SurveyViewCollection<SurveyView>();
            ExtensionConfigurations = new List<ExtensionConfiguration>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PopulateDefaults()
        {
            DefaultLanguage = "en";
            TitleLabels = new LabelCollection<TitlePageLabel>
            {
                [DefaultLanguage] = new TitlePageLabel
                {
                    Value = "Default Survey Title",
                    Survey = this
                }
            };




            SurveyPermissions = new HashSet<SurveyPermission>();
            SurveyViews = new SurveyViewCollection<SurveyView>()
            {
                new SurveyView
                {
                    ViewName = "Standard",
                    Survey = this,
                    WelcomePageLabels = new LabelCollection<WelcomePageLabel>
                    {
                        [DefaultLanguage] =
                            new WelcomePageLabel
                            {
                                Value = null
                            }
                    },
                    ThankYouPageLabels = new LabelCollection<ThankYouPageLabel>
                    {
                        [DefaultLanguage] =
                            new ThankYouPageLabel
                            {
                                Value = null
                            }
                    },
                    TermsAndConditionsLabels = new LabelCollection<TermsAndConditionsPageLabel>
                    {
                        [DefaultLanguage] =
                            new TermsAndConditionsPageLabel
                            {
                                // language is set on the object with the helper
                                Value = null
                            }
                    },
                    ScreeningQuestionLabels = new LabelCollection<ScreeningQuestionsPageLabel>
                    {
                        [DefaultLanguage] =
                            new ScreeningQuestionsPageLabel
                            {
                                Value = null
                            }
                    }
                }
            };
        }
    }
}