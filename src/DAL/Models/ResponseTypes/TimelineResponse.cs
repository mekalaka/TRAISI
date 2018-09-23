using System;
using DAL.Models.Questions;
using DAL.Models.Surveys;

namespace DAL.Models.ResponseTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class TimelineResponse : LocationResponse
    {

        public string Purpose {get;set;}

        public DateTime Time {get;set;}
    }
}