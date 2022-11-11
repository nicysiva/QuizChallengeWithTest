namespace QuizService.Interfaces
{
    public interface IUnitOfWork
    {
        IQuizRepository Quizes { get; }
    }
}
