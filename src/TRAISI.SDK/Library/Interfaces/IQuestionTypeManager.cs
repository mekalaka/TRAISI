using System.Collections.Generic;

namespace TRAISI.SDK.Interfaces
{
    /// <summary>
    /// Interface definition for the injectable QuestionTypeManager service.
    /// </summary>
    public interface IQuestionTypeManager
    {
        ICollection<QuestionTypeDefinition> QuestionTypeDefinitions { get; }

        void LoadQuestionExtensions();
    }
}