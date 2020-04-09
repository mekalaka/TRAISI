using System.Collections.Generic;
using System.Threading.Tasks;
using TRAISI.Data.Models.Questions;

namespace TRAISI.Data.Repositories.Interfaces
{
    public interface IQuestionOptionConditionalRepository  : IRepository<QuestionOptionConditional> {
        void DeleteSourceConditionals(int questionPartId, List<int> retainedConditionals);
        void DeleteTargetConditionals(int questionPartId, List<int> retainedConditionals);
        Task<IEnumerable<QuestionOptionConditional>> GetQuestionOptionConditionalsAsync(int questionPartId);

        void ValidateSourceConditionals(int questionPartId, List<int> viableQuestionTargets);
        void ValidateTargetConditionals(int questionPartId, List<int> viableQuestionSources);
    }
}