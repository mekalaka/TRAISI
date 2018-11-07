using System;
using System.Collections.Generic;
using TRAISI.SDK.Attributes;
using TRAISI.SDK.Enums;
using TRAISI.SDK.Interfaces;
using TRAISI.SDK.Library.ResponseTypes;

namespace TRAISI.SDK.Questions
{

    public class TextQuestionValidator : ResponseValidator
    {
        public TextQuestionValidator()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public override bool ValidateResponse(IResponseType data, Dictionary<string, IQuestionConfiguration> configuration)
        {
            return true;
        }
    }
}