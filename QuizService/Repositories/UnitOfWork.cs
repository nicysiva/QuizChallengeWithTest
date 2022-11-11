using QuizService.Interfaces;

namespace QuizService.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IQuizRepository quizRepository)
        {
            Quizes = quizRepository;
        }
        public IQuizRepository Quizes { get; }
    }
}
