namespace DAL.Models.Surveys
{
    public  abstract class SurveyRespondent : ISurveyRespondent
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Relationship {get;set;}
        public SurveyRespondentGroup SurveyRespondentGroup { get; set; }

    }
}