using Dapper;
using Microsoft.AspNetCore.Mvc;
using QuizService.Interfaces;
using QuizService.Model;
using QuizService.Model.Domain;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QuizService.Repositories
{
    public class QuizRepositories : IQuizRepository
    {
        private readonly IDbConnection _connection;

        public QuizRepositories(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IReadOnlyList<Quiz>> GetAllAsync()
        {
            const string sql = "SELECT * FROM Quiz;";
            var quizzes = await _connection.QueryAsync<Quiz>(sql);
            return quizzes.ToList();
            
        }

        public async Task<QuizResponseModel> GetByIdAsync(int id)
        {
            const string quizSql = "SELECT * FROM Quiz WHERE Id = @Id;";
            var quiz = await _connection.QuerySingleOrDefaultAsync<Quiz>(quizSql, new { Id = id });

            if(quiz == null)
            {
                return new QuizResponseModel
                {
                    Id = id,
                    Title = null
                };
            }

            const string questionsSql = "SELECT * FROM Question WHERE QuizId = @QuizId;";
            var questions = await _connection.QueryAsync<Question>(questionsSql, new { QuizId = id });

            const string answersSql = "SELECT a.Id, a.Text, a.QuestionId FROM Answer a INNER JOIN Question q ON a.QuestionId = q.Id WHERE q.QuizId = @QuizId;";
            var answers = _connection.Query<Answer>(answersSql, new { QuizId = id })
                .Aggregate(new Dictionary<int, IList<Answer>>(), (dict, answer) => {
                    if (!dict.ContainsKey(answer.QuestionId))
                        dict.Add(answer.QuestionId, new List<Answer>());
                    dict[answer.QuestionId].Add(answer);
                    return dict;
                });

            return new QuizResponseModel
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Questions = questions.Select(question => new QuizResponseModel.QuestionItem
                {
                    Id = question.Id,
                    Text = question.Text,
                    Answers = answers.ContainsKey(question.Id)
                        ? answers[question.Id].Select(answer => new QuizResponseModel.AnswerItem
                        {
                            Id = answer.Id,
                            Text = answer.Text
                        })
                        : new QuizResponseModel.AnswerItem[0],
                    CorrectAnswerId = question.CorrectAnswerId
                }),
                Links = new Dictionary<string, string>
            {
                {"self", $"/api/quizzes/{id}"},
                {"questions", $"/api/quizzes/{id}/questions"}
            }
            };
        }


        public async Task<object> Post(QuizCreateModel value)
        {
            var sql = $"INSERT INTO Quiz (Title) VALUES('{value.Title}'); SELECT LAST_INSERT_ROWID();";
            var id = await _connection.ExecuteScalarAsync(sql);
            return id;
        }

        public async Task<object> Put(int id, QuizUpdateModel value)
        {
            const string sql = "UPDATE Quiz SET Title = @Title WHERE Id = @Id";
            int rowsUpdated = await _connection.ExecuteAsync(sql, new { Id = id, Title = value.Title });
            
            return rowsUpdated;
        }

        public async Task<object> Delete(int id)
        {
            const string sql = "DELETE FROM Quiz WHERE Id = @Id";
            int rowsDeleted = await _connection.ExecuteAsync(sql, new { Id = id });
            
            return rowsDeleted;
        }

        public async Task<object> PostQuestion(int id, QuestionCreateModel value)
        {
            const string sql = "INSERT INTO Question (Text, QuizId) VALUES(@Text, @QuizId); SELECT LAST_INSERT_ROWID();";
            var questionId = await _connection.ExecuteScalarAsync(sql, new { Text = value.Text, QuizId = id });
           
            return questionId;
        }

        public async Task<object> PutQuestion(int id, int qid, QuestionUpdateModel value)
        {
            const string sql = "UPDATE Question SET Text = @Text, CorrectAnswerId = @CorrectAnswerId WHERE Id = @QuestionId";
            int rowsUpdated = await _connection.ExecuteAsync(sql, new { QuestionId = qid, Text = value.Text, CorrectAnswerId = value.CorrectAnswerId });
            
            return rowsUpdated;
        }

        public async Task<object> DeleteQuestion(int id, int qid)
        {
            const string sql = "DELETE FROM Question WHERE Id = @QuestionId";
            var rowdeleted = await _connection.ExecuteScalarAsync(sql, new { QuestionId = qid });

            return rowdeleted;
        }

        public async Task<object> PostAnswer(int id, int qid, AnswerCreateModel value)
        {
            const string sql = "INSERT INTO Answer (Text, QuestionId) VALUES(@Text, @QuestionId); SELECT LAST_INSERT_ROWID();";
            var answerId = await _connection.ExecuteScalarAsync(sql, new { Text = value.Text, QuestionId = qid });
           
            return answerId;
        }

        public async Task<object> PutAnswer(int id, int qid, int aid, AnswerUpdateModel value)
        {
            const string sql = "UPDATE Answer SET Text = @Text WHERE Id = @AnswerId";
            int rowsUpdated = await _connection.ExecuteAsync(sql, new { AnswerId = qid, Text = value.Text });
           
            return rowsUpdated;
        }

        public async Task<object> DeleteAnswer(int id, int qid, int aid)
        {
            const string sql = "DELETE FROM Answer WHERE Id = @AnswerId";
            var ansdeleted = await _connection.ExecuteScalarAsync(sql, new { AnswerId = aid });
            return ansdeleted;
        }

    }
}
