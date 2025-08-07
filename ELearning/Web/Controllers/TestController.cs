using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.ViewModels.ClientExamVM;
using Web.ViewModels.QuestionExamVM;
using Web.ViewModels.ResultVM;
using Web.WebConfig;

namespace Web.Controllers
{
    public class TestController : BaseController
    {
        public readonly DataContext _db;
        public TestController(DataContext db, GenericRepository repo, IMapper mapper) : base(repo, mapper)
        {
            _db = db;
        }


        public IActionResult Index() { return View(); }

        [HttpGet]
        public async Task<IActionResult> StartTest(int id)
        {
            var exam = await _repo.GetOneAsync<Exam>(x => x.Id == id);
            var subject = await _repo.GetOneAsync<Subject>(x => x.Id == exam.SubjectId);

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
                SubjectName = subject.SubjectCode + " - " + subject.SubjectName,
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

        // lấy toàn bộ các bài thi của người dùng ở trong các nhóm học phần đang hoạt động
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


        public async Task<IActionResult> ExamDetail(int id)
        {
            var exam = await _repo.GetOneAsync<Exam>(x => x.Id == id);
            var subject = await _repo.GetOneAsync<Subject>(x => x.Id == exam.SubjectId);

            if (exam == null)
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
                SubjectName = subject.SubjectCode + " - " + subject.SubjectName,
            };

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> TakeExam(int id)
        {
            var data = await _repo.GetOneAsync<Exam>(x => x.Id == id);
            var check = await _repo.GetOneAsync<Result>(x => x.ExamId == id && x.UserId == this.CurrentUserId);

            // check bài thi có tồn tại không
            if ((data == null)
                || (data.TimeStart > DateTime.Now)
                || (data.TimeEnd < DateTime.Now)
                || check?.EndTime != null)
            {
                return NotFound();
            }
            else if (check != null)
            {
                TimeSpan timeDifference = DateTime.Now - check.StartTime;
                check.TotalWorkTime = (int)timeDifference.TotalSeconds;
                if (check.TotalWorkTime >= data.WorkTime * 60)
                {
                    check.TotalWorkTime = data.WorkTime * 60;
                    check.EndTime = check.StartTime.AddMinutes(data.WorkTime);
                    await _repo.UpdateAsync(check);
                    return NotFound();
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        //view bài thi load data
        [HttpGet]
        public async Task<IActionResult> TakeExamServer(int id)
        {
            var exam = await _repo.GetOneAsync<Exam>(x => x.Id == id);

            if (exam == null)
            {
                return NotFound();
            }


            List<ResQuestionVM> questions = new List<ResQuestionVM>();
            var check = await _repo.GetOneAsync<Result>(x => x.ExamId == id && x.UserId == this.CurrentUserId);

            // nếu người dùng chưa làm bài => tạo câu hỏi 
            if (check == null)
            {    // kiểm tra dạng bài thi
                if (exam.IsAutomatic == false)
                {
                    questions = _db.ExamDetails
                        .Where(x => x.ExamId == id)
                        .Select(x => new ResQuestionVM
                        {
                            Id = x.QuestionId,
                            AnswerId = 0,
                            Content = x.Question.Content,
                            answers = x.Question.answers.Select(a => new Answer
                            {
                                Id = a.Id,
                                AnswerContent = a.AnswerContent,

                            }).ToList()
                        }).ToList();
                }
                else
                {
                    // Fetch the chapters associated with the exam
                    var chapters = _db.AutomaticExam
                        .Where(ae => ae.ExamId == id)
                        .Select(ae => ae.ChapterId)
                        .ToList();

                    // Fetch the number of questions required for each level
                    var questionLevels = new Dictionary<int, int>
                    {
                        { 1, exam.EQCount },
                        { 2, exam.MQCount },
                        { 3, exam.HQCount }
                    };

                    foreach (var level in questionLevels.Keys)
                    {
                        var levelQuestions = _db.Question
                            .Where(q => chapters.Contains(q.ChapterId) && q.Level == level)
                            .OrderBy(q => Guid.NewGuid())
                            .Take(questionLevels[level])
                            .Select(q => new ResQuestionVM
                            {
                                Id = q.Id,
                                AnswerId = 0,
                                Content = q.Content,
                                answers = q.answers
                                    .OrderByDescending(a => a.Status) // Ensure correct answers are prioritized
                                    .ThenBy(a => Guid.NewGuid()) // Randomize the order while keeping at least one correct answer
                                    .Take(4)
                                    .ToList()
                            }).ToList();

                        questions.AddRange(levelQuestions);
                    }
                }

                // Save result details, lưu đề thi
                var result = new Result
                {
                    ExamId = exam.Id,
                    UserId = this.CurrentUserId,
                    StartTime = DateTime.Now
                };
                try
                {
                    await _repo.AddAsync(result);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error adding result: {ex.Message}");
                    return StatusCode(500, "An error occurred while saving the result.");
                }

                // lưu câu hỏi
                foreach (var question in questions)
                {
                    var resultDetail = new ResultDetails
                    {
                        ResultId = result.Id,
                        QuestionId = question.Id
                    };
                    try
                    {
                        await _repo.AddAsync(resultDetail);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Error adding result detail: {ex.Message}");
                        return StatusCode(500, "An error occurred while saving the result details.");
                    }
                }
            }
            // nếu người dùng đã làm bài thi thì cập nhật số lần thoát ra 
            else
            {
                check.NumTSC += 1;
                questions = _db.ResultDetails
                   .Where(x => x.Result.ExamId == id && x.Result.UserId == this.CurrentUserId)
                   .Select(x => new ResQuestionVM
                   {
                       Id = x.QuestionId,
                       AnswerId = x.AnswerId ?? 0,
                       Content = x.Question.Content,
                       answers = x.Question.answers.Select(a => new Answer
                       {
                           Id = a.Id,
                           AnswerContent = a.AnswerContent,
                       }).ToList()
                   }).ToList();

                TimeSpan timeDifference = DateTime.Now - check.StartTime;
                check.TotalWorkTime = (int)timeDifference.TotalSeconds;
                await _repo.UpdateAsync(check);
            }

            // get data ra cho view
            try
            {
                var tem = await _repo.GetOneAsync<Result>(x => x.ExamId == id && x.UserId == this.CurrentUserId);
                var examVM = new ExamDetailsVM
                {
                    Id = exam.Id,
                    UserName = this.CurrentUserName,
                    ResultId = tem.Id,
                    WorkTime = exam.WorkTime * 60 - (check?.TotalWorkTime ?? 0), // đổi thời gian thành giây
                    StartTime = check == null ? DateTime.Now : check.StartTime,
                    EndTime = check == null ? DateTime.Now.AddMinutes(exam.WorkTime) : check.StartTime.AddMinutes(exam.WorkTime),
                };


                if (exam.MixQuestion)
                {
                    questions = questions.OrderBy(x => Guid.NewGuid()).ToList();
                }
                if (exam.MixAnswer)
                {
                    foreach (var question in questions)
                    {
                        question.answers = question.answers.OrderBy(x => Guid.NewGuid()).ToList();
                    }
                }

                return Ok(new { examVM, questions });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error updating result: {ex.Message}");
                return StatusCode(500, "Lỗi server kìa đồ ngu a hi hi!!.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAnswers([FromBody] List<UserAnswerVM> answers)
        {
            if (answers == null || !answers.Any())
            {
                return Ok(new { message = "dữ liệu trống", success = true });
                //return BadRequest("lỗi");
            }

            foreach (var answer in answers)
            {
                try
                {
                    var resultDetail = await _repo.GetOneAsync<ResultDetails>(
                        rd => rd.QuestionId == answer.QuestionId && rd.ResultId == answer.ResultId);

                    if (resultDetail != null)
                    {
                        resultDetail.AnswerId = answer.AnswerId;
                        await _repo.UpdateAsync(resultDetail);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error updating result detail: {ex.Message}");
                    return StatusCode(500, ex.Message);
                }
            }

            try
            {
                // Calculate score and update result
                var result = await _repo.GetOneAsync<Result>(x => x.Id == answers[0].ResultId);

                var exam = await _repo.GetOneAsync<Exam>(x => x.Id == result.ExamId);


                var resultDetails = await _repo.GetAll<ResultDetails>(x => x.ResultId == result.Id).ToListAsync();
                int totalQuestion = exam.EQCount + exam.MQCount + exam.HQCount;
                int numCorrect = 0;
                // Tải tất cả các câu trả lời hợp lệ trước
                var validAnswers = await _repo.GetAll<Answer>(x => x.Status == true).ToListAsync();
                foreach (var resultDetail in resultDetails)
                {
                    var answer = validAnswers.FirstOrDefault(x => x.QuestionId == resultDetail.QuestionId);

                    if (answer != null && answer.Id == resultDetail.AnswerId)
                    {
                        numCorrect++;
                    }
                }

                result.TestScores = Math.Round((double)numCorrect / totalQuestion * 10, 2);
                TimeSpan temp = DateTime.Now - result.StartTime;
                result.TotalWorkTime = (int)temp.TotalSeconds;
                result.NumCorrect = numCorrect;
                result.EndTime = DateTime.Now;
                await _repo.UpdateAsync(result);

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error updating result detail: {ex.Message}");
                return StatusCode(500, ex.Message);
            }

            return Ok(new { message = "bạn đã nộp bài thi" });

        }

    }
}
