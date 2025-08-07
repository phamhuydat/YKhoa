using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Data.Entities;
using Data.Repositories;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.ViewModels.ClientExamVM;
using Web.ViewModels.ExamVM;
using Web.ViewModels.QuestionExamVM;
using Web.WebConfig;

namespace Web.Controllers
{
	public class YKhoa : BaseController
	{
		public readonly DataContext _db;
		public YKhoa(DataContext db, GenericRepository repo, IMapper mapper) : base(repo, mapper)
		{
			_db = db;
		}


		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> StartQuiz(int idQuiz)
		{
			try
			{
				var Quiz = await _repo.GetOneAsync<Exam>(x => x.Id == idQuiz);
				if ((Quiz.EQCount + Quiz.HQCount + Quiz.MQCount) == 0)
					return BadRequest(new { error = "No questions available" });

				var examdetail = _db.ExamDetails.Where(x => x.ExamId == idQuiz && x.DisplayOrder == 1).FirstOrDefault();

				var userTestQuiz = await _repo.GetOneAsync<Result>(x => x.ExamId == idQuiz && x.UserId == this.CurrentUserId);

				if (userTestQuiz == null)
				{
					var quizResult = new Result
					{
						ExamId = idQuiz,
						UserId = this.CurrentUserId,
						CurrentQuestion = examdetail?.QuestionId ?? 0,
						StartTime = DateTime.UtcNow,
						NumCorrect = 0,
						NumTSC = 1,
						TotalWorkTime = 0,
					};
					// Lưu kết quả vào cơ sở dữ liệu
					await _repo.AddAsync(quizResult);

					var orderQuestion = _db.ExamDetails
						.Where(x => x.ExamId == idQuiz && x.QuestionId == quizResult.CurrentQuestion)
						.Select(x => x.DisplayOrder)
						.FirstOrDefault();

					return Ok(new
					{
						quizResult.ExamId,
						quizResult.Id,
						totalQuestions = Quiz.EQCount + Quiz.HQCount + Quiz.MQCount,
						currentQuestion = quizResult.CurrentQuestion,
						orderQuestion = orderQuestion
					});
				}
				else
				{
					userTestQuiz.StartTime = DateTime.UtcNow;
					userTestQuiz.NumCorrect = 0;
					userTestQuiz.NumTSC = 1;
					userTestQuiz.TotalWorkTime = 0;

					// Lưu kết quả vào cơ sở dữ liệu
					await _repo.UpdateAsync(userTestQuiz);

					var orderQuestion = _db.ExamDetails
						.Where(x => x.ExamId == idQuiz && x.QuestionId == userTestQuiz.CurrentQuestion)
						.Select(x => x.DisplayOrder)
						.FirstOrDefault();

					return Ok(new
					{
						userTestQuiz.Id,
						userTestQuiz.ExamId,
						totalQuestions = Quiz.EQCount + Quiz.HQCount + Quiz.MQCount,
						currentQuestion = userTestQuiz.CurrentQuestion,
						orderQuestion = orderQuestion
					});
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "Failed to start quiz", details = ex.Message });
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetQuestion(int orderQuestion, [FromHeader(Name = "X-Result-ID")] int resultId)
		{
			try
			{
				var quizResult = await _repo.GetOneAsync<Result>(x => x.Id == resultId);
				if (quizResult == null)
					return Unauthorized(new { error = "Invalid result ID" });

				var questionNumber = _db.ExamDetails
					.Where(x => x.ExamId == quizResult.ExamId && x.DisplayOrder == orderQuestion)
					.Select(x => x.QuestionId)
					.FirstOrDefault();

				var question = await _repo.GetAll<Question>()
					.Include(q => q.answers)
					.FirstOrDefaultAsync(q => q.Id == questionNumber);

				if (question == null)
					return NotFound(new { error = "Question not found" });

				// Update CurrentQuestion
				quizResult.CurrentQuestion = questionNumber;
				await _repo.UpdateAsync(quizResult);

				return Ok(new QuestionDto
				{
					QuestionNumber = question.Id,
					Title = question.Content,
					QuestionText = question.Content,
					Options = question.answers.Select(a => new OptionDto
					{
						Id = a.Id,
						Text = a.AnswerContent,
						IsCorrect = a.Status,
						Explanation = a.Feedback
					}).ToList()
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "Failed to retrieve question", details = ex.Message });
			}
		}
		[HttpGet("answers")]
		public async Task<IActionResult> GetAnswers([FromHeader(Name = "X-Result-ID")] int resultId)
		{
			try
			{
				var quizResult = await _repo.GetOneAsync<Result>(x => x.Id == resultId);
				if (quizResult == null)
					return Unauthorized(new { error = "Invalid result ID" });

				var answers = await _repo.GetAll<ResultDetails>()
					.Where(d => d.ResultId == resultId)
					.Join(
						_repo.GetAll<Question>().Include(q => q.answers),
						detail => detail.QuestionId,
						question => question.Id,
						(detail, question) => new
						{
							questionNumber = detail.QuestionId,
							dislayOrder = detail.DisplayOrder,
							selectedOption = detail.AnswerId,
							isCorrect = question.answers.Where(a => a.Id == detail.AnswerId).Select(a => a.Status).FirstOrDefault(),
							explanation = question.answers.Where(a => a.Id == detail.AnswerId).Select(a => a.Feedback).FirstOrDefault(),
							correctOption = question.answers.Where(a => a.Status).Select(a => a.Id).FirstOrDefault()
						})
					.ToListAsync();

				return Ok(answers);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "Failed to retrieve answers", details = ex.Message });
			}
		}

		// Submit answer
		[HttpPost]
		public async Task<IActionResult> SubmitAnswer(int orderQuestion, [FromBody] AnswerRequest request, [FromHeader(Name = "X-Result-ID")] int resultId)
		{
			try
			{
				var quizResult = await _repo.GetOneAsync<Result>(x => x.Id == resultId);
				if (quizResult == null)
					return Unauthorized(new { error = "Invalid result ID" });

				var questionNumber = _db.ExamDetails
					.Where(x => x.ExamId == quizResult.ExamId && x.DisplayOrder == orderQuestion)
					.Select(x => x.QuestionId)
					.FirstOrDefault();

				var question = _repo.GetAll<Question>()
					.Include(x => x.answers)
					.Where(q => q.Id == questionNumber);

				if (question == null)
					return NotFound(new { error = "Question not found" });

				if (request.SelectedOption <= 0)
					return BadRequest(new { error = "Invalid option selected" });


				//var isCorrect = question.answers
				//	.Where(a => a.Status)
				//	.Select(a => a.Id)
				//	.Contains(request.SelectedOption);

				var isCorrect = question.Any(q => q.answers.Any(a => a.Id == request.SelectedOption && a.Status));


				// Check if answer already exists
				var existingAnswer = await _repo.GetOneAsync<ResultDetails>(d => d.ResultId == resultId && d.QuestionId == questionNumber);

				if (existingAnswer != null)
				{
					// Update existing answer
					existingAnswer.AnswerId = request.SelectedOption;
					//existingAnswer.IsCorrect = isCorrect;
				}
				else
				{
					var newResult = new ResultDetails
					{
						ResultId = resultId,
						QuestionId = questionNumber,
						AnswerId = request.SelectedOption,
						//IsCorrect = isCorrect
					};
					// Add new answer							      
					//await _repo.AddAsync(newResult);	  
				}

				// Update quiz result
				quizResult.CurrentQuestion = questionNumber;
				var answers = _repo.GetAll<ResultDetails>()
					.Where(d => d.ResultId == resultId)
					.ToList();

				//quizResult.CorrectAnswers = answers.Count(a => a.IsCorrect);
				//quizResult.TestScores = answers.Any() ? (int)Math.Round((double)quizResult.CorrectAnswers / answers.Count * 100) : 0;

				quizResult.TestScores = 0;

				//await _repo.UpdateAsync(quizResult);

				//var feedback = question.answers
				//	.Where(a => a.Id == request.SelectedOption)
				//	.Select(a => a.Feedback)
				//	.FirstOrDefault();

				var feedback = question.SelectMany(q => q.answers)
					.Where(a => a.Id == request.SelectedOption)
					.Select(a => a.Feedback)
					.FirstOrDefault();

				feedback = feedback == "" ? "Không có giải thích cho đáp án sai này" : feedback;

				return Ok(new AnswerResponse
				{
					IsCorrect = isCorrect,
					Explanation = feedback,
					CorrectOption = isCorrect ? null : question.SelectMany(q => q.answers).FirstOrDefault(a => a.Status)?.Id
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "Failed to submit answer", details = ex.Message });
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetResults([FromHeader(Name = "X-Result-ID")] int resultId)
		{
			try
			{
				var quizResult = await _repo.GetOneAsync<ResultDetails>(x => x.ResultId == resultId);
				if (quizResult == null)
					return Unauthorized(new { error = "Invalid result ID" });

				var answers = await _repo.GetAll<ResultDetails>()
					.Where(d => d.ResultId == resultId)
					.ToListAsync();

				var totalAnswered = answers.Count;
				//var correctAnswers = answers.Count(a => a.IsCorrect);
				//var correctAnswers = answers.Count(a => a.AnswerId == a.Question.answers.FirstOrDefault(x => x.Status)?.Id);
				var correctAnswers = 2;
				var score = totalAnswered > 0 ? (int)Math.Round((double)correctAnswers / totalAnswered * 100) : 0;

				return Ok(new ResultDto
				{
					CorrectAnswers = correctAnswers,
					TotalAnswered = totalAnswered,
					Score = score,
					CurrentQuestion = 10
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "Failed to retrieve results", details = ex.Message });
			}
		}


		public async Task<IActionResult> LoadListExam()
		{
			// lây các bài thi của người dùng ở trong các nhóm học phần đang hoạt động và được phát bài thi ở handoutexam
			var listExam = _repo.GetAll<Exam>(
				x => x.DeletedDate == null
				&& x.handOutExams.Any(he =>
				he.group.GroupDetails.Any(y => y.UserId == this.CurrentUserId)))
				.ProjectTo<ListExamUserVM>(AutoMapperProfile.ExamIndexClientConf)
				.ToList();

			var result = await _repo.GetAll<Result>(x => x.UserId == this.CurrentUserId).ToListAsync();

			foreach (var item in listExam)
			{
				var temp = result.FirstOrDefault(x => x.ExamId == item.Id);
				if (temp != null)
				{
					item.TotalScore = temp.TestScores;
					item.UserStartTime = temp.StartTime;
					item.TotalWorkTime = temp.TotalWorkTime;
					item.UserEndTime = temp.EndTime;
				}
				else
				{
					item.TotalScore = 0;
					item.UserStartTime = null;
					item.UserEndTime = null;
					item.TotalWorkTime = 0;
				}
			}

			return Ok(listExam);
		}

		[HttpGet]
		public async Task<IActionResult> StartTest(int id)
		{
			var exam = await _repo.GetOneAsync<Exam>(x => x.Id == id);
			//var subject = await _repo.GetOneAsync<Subject>(x => x.Id == exam.SubjectId);

			var result = await _repo.GetOneAsync<Result>(x => x.ExamId == id && x.UserId == this.CurrentUserId);

			if (exam == null && result == null)

			{
				return NotFound();
			}

			var data = new ListExamUserVM
			{
				Id = exam.Id,
				StartTime = exam.TimeStart,
				EndTime = exam.TimeEnd,
				ExamName = exam.Title,
				WorkTime = exam.WorkTime,
				//SubjectName = subject.SubjectCode + " - " + subject.SubjectName,
				TotalScore = result?.TestScores ?? 0,
				SeeAnswer = exam.SeeAnswer,
				TotalQuestion = exam.EQCount + exam.MQCount + exam.HQCount,
			};

			if (result != null)
			{
				data.UserStartTime = result.StartTime;
				data.UserEndTime = result.EndTime;
				data.TotalWorkTime = result.TotalWorkTime;
				data.TotalCorrectAnswer = result.NumCorrect;
			}
			else
			{
				data.UserStartTime = null;
				data.UserEndTime = null;
				data.TotalWorkTime = 0;
				data.TotalCorrectAnswer = 0;
			}

			return View(data);
		}

		public async Task<List<ResQuestionVM>> GetListQuestion(int QuestionSetId)
		{
			var questionSet = _repo.GetAll<Chapter>()
				.Include(qs => qs.questions)
				.ThenInclude(q => q.answers)
				.FirstOrDefault(qs => qs.Id == QuestionSetId);

			return await _repo.GetAll<Question>()
					.Where(x => x.ChapterId == QuestionSetId)
					.Include(x => x.answers)
					.OrderBy(x => x.Id)
					.Select(q => new ResQuestionVM
					{
						Id = q.Id,
						Content = q.Content,
						answers = q.answers.Select(a => new Answer
						{
							Id = a.Id,
							AnswerContent = a.AnswerContent,
						}).ToList()

					}).ToListAsync();
		}

		public async Task<IActionResult> QuizStart(int questionSetId)
		{
			var questions = await GetListQuestion(questionSetId);

			ViewBag.QuestionList = questions;
			ViewBag.QuizId = questionSetId;
			return View(questions);
		}
		/// <summary>
		/// Kiểm tra câu trả lời của người dùng cho một câu hỏi cụ thể.
		/// </summary>
		/// <param name="QuestionId"></param>
		/// <param name="selectAnswer"></param>
		/// <returns></returns>


		[HttpPost]
		public async Task<IActionResult> SubmitAnswers([FromBody] SubmitAnswerRequest request)
		{
			var selectIds = request.SelectAnswer;


			var question = await _repo.GetOneAsync<Question>(q => q.Id == request.QuestionId);
			if (question == null)
			{
				return NotFound("Câu hỏi không tồn tại.");
			}
			var allAnswers = _repo.GetAll<Answer>()
			.Where(a => a.QuestionId == request.QuestionId)
			.ToList();

			var correctIds = _repo.GetAll<Answer>()
			.Where(a => a.QuestionId == request.QuestionId && a.Status)
			.Select(a => a.Id)
			.ToList();

			//câu người dùng chọn sai
			var wrongSelectedIds = selectIds.Except(correctIds).ToList();

			// Người dùng bỏ sót đáp án đúng
			var missedCorrectIds = correctIds.Except(selectIds).ToList();

			bool isCorrect = !wrongSelectedIds.Any() &&
					 !missedCorrectIds.Any();

			// đáp án sai
			var wrongAnswerExplanations = allAnswers
			.Where(a => wrongSelectedIds.Contains(a.Id))
			.Select(a => new
			{
				Content = a.AnswerContent,
				Explanation = a.Feedback
			})
			.ToList();
			// đáp án đúng bỏ xót
			var missedAnswers = allAnswers
			.Where(a => missedCorrectIds.Contains(a.Id))
			.Select(a => a.AnswerContent)
			.ToList();

			// đáp án đúng được chọn
			var correctAnswers = allAnswers
			.Where(a => a.Status && selectIds.Contains(a.Id))
			.Select(a => a.AnswerContent)
			.ToList();


			// Lưu câu trả lời của người dùng
			// var userAnswer = new UserAnswer
			// {
			// 	QuestionId = questionId,
			// 	AnswerId = selectedAnswerIds.FirstOrDefault(),
			// 	UserId = User.Identity.Name, // Hoặc ID người dùng
			// 	AnsweredAt = DateTime.Now
			// };
			// _context.UserAnswers.Add(userAnswer);
			// _context.SaveChanges();	

			return Json(new
			{
				isCorrect,
				wrongAnswers = wrongAnswerExplanations,
				missedAnswers,
				correctAnswers
			});
		}
	}
}
