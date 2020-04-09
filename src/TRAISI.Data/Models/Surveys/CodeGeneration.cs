using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRAISI.Data.Core;
using TRAISI.Data.Models.Surveys;

namespace TRAISI.Data.Models.Surveys {
    public class CodeGeneration
		{
			public int Id { get; set; }
			public int SurveyId { get; set; }
			public string GroupName { get; set; }
			public string Pattern { get; set; }
			public int CodeLength { get; set; }
			public int NumberOfCodes { get; set; }
			public Boolean IsGroupCode { get; set; }
			public Boolean UsePattern { get; set; }
			public Boolean IsTest { get; set; }
		}

	
}
