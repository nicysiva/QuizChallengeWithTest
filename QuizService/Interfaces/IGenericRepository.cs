using QuizService.Model;
using QuizService.Model.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizService.Interfaces
{
    public interface IGenericRepository <T> where T : class
    {
        //TODO: Using this type of business transaction, we will aggregate all Repository transactions into a single transaction.
        Task<IReadOnlyList<T>> GetAllAsync();

        Task<QuizResponseModel> GetByIdAsync(int id);

        Task<object> Post(QuizCreateModel value);

        Task<object> Put(int id, QuizUpdateModel value);

        Task<object> Delete(int id);

        Task<object> PostQuestion(int id, QuestionCreateModel value);

        Task<object> PutQuestion(int id, int qid, QuestionUpdateModel value);
        Task<object> DeleteQuestion(int id, int qid);

        Task<object> PostAnswer(int id, int qid, AnswerCreateModel value);

        Task<object> PutAnswer(int id, int qid, int aid, AnswerUpdateModel value);

        Task<object> DeleteAnswer(int id, int qid, int aid);
    }
}
