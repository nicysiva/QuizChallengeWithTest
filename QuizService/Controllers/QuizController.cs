using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using QuizService.Model;
using QuizService.Model.Domain;
using System.Linq;
using QuizService.Interfaces;
using System.Threading.Tasks;
using System;

namespace QuizService.Controllers;

[Route("api/quizzes")]
public class QuizController : Controller
{
    private readonly IUnitOfWork unitOfWork;

    public QuizController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    // GET api/quizzes
    [HttpGet]
    public async Task<IEnumerable<QuizResponseModel>> Get()
    {
        var quizzes = await unitOfWork.Quizes.GetAllAsync();

        return quizzes.Select(quiz =>
            new QuizResponseModel
            {
                Id = quiz.Id,
                Title = quiz.Title
            });
    }

    // GET api/quizzes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<QuizResponseModel>> GetByIDAsync(int id)
    {
        try
        {
            var data = await unitOfWork.Quizes.GetByIdAsync(id);
            if (data.Title == null)
                return NotFound();
            return data;
        }
        catch(Exception)
        {
            return BadRequest();
        }
    }

    // POST api/quizzes
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] QuizCreateModel value)
    {
        var id = await unitOfWork.Quizes.Post(value);
        return Created($"/api/quizzes/{id}", null);
    }

    // PUT api/quizzes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] QuizUpdateModel value)
    {
        object rowsUpdated = await unitOfWork.Quizes.Put(id, value);
        if ( rowsUpdated.ToString() == "0")
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        object rowsDeleted = await unitOfWork.Quizes.Delete(id);
        if (rowsDeleted.ToString() == "0")
            return NotFound();
        return NoContent();
    }

    // POST api/quizzes/5/questions
    [HttpPost]
    [Route("{id}/questions")]
    public async Task<IActionResult> PostQuestion(int id, [FromBody] QuestionCreateModel value)
    {
        var questionId = await unitOfWork.Quizes.PostQuestion(id, value);
        return Created($"/api/quizzes/{id}/questions/{questionId}", null);
    }

    // PUT api/quizzes/5/questions/6
    [HttpPut("{id}/questions/{qid}")]
    public async Task<IActionResult> PutQuestion(int id, int qid, [FromBody] QuestionUpdateModel value)
    {
        object rowsUpdated = await unitOfWork.Quizes.PutQuestion(id, qid, value);
        if (rowsUpdated.ToString() == "0")
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6
    [HttpDelete]
    [Route("{id}/questions/{qid}")]
    public async Task<IActionResult> DeleteQuestion(int id, int qid)
    {
        await unitOfWork.Quizes.DeleteQuestion(id, qid);
        return NoContent();
    }

    // POST api/quizzes/5/questions/6/answers
    [HttpPost]
    [Route("{id}/questions/{qid}/answers")]
    public async Task<IActionResult> PostAnswer(int id, int qid, [FromBody] AnswerCreateModel value)
    {
        var answerId = await unitOfWork.Quizes.PostAnswer(id, qid, value);
        return Created($"/api/quizzes/{id}/questions/{qid}/answers/{answerId}", null);
    }

    // PUT api/quizzes/5/questions/6/answers/7
    [HttpPut("{id}/questions/{qid}/answers/{aid}")]
    public async Task<IActionResult> PutAnswer(int id, int qid, int aid, [FromBody] AnswerUpdateModel value)
    {
        object rowsUpdated = await unitOfWork.Quizes.PutAnswer(id, qid, aid, value);
        if (rowsUpdated.ToString() == "0")
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6/answers/7
    [HttpDelete]
    [Route("{id}/questions/{qid}/answers/{aid}")]
    public async Task<IActionResult> DeleteAnswer(int id, int qid, int aid)
    {
        await unitOfWork.Quizes.DeleteAnswer(id, qid, aid);
        return NoContent();
    }
}