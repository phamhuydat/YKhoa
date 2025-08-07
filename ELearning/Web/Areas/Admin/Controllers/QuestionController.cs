using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Data.Entities;
using Data.Repositories;
using DocumentFormat.OpenXml.Packaging;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Share.Consts;
using System.Text.RegularExpressions;
using Web.Areas.Admin.ViewModels.AnswerVM;
using Web.Areas.Admin.ViewModels.ChapterVM;
using Web.Areas.Admin.ViewModels.QuestionVM;
using Web.Areas.Admin.ViewModels.SubjectVM;
using Web.Common;
using Web.WebConfig;
using X.PagedList;

namespace Web.Areas.Admin.Controllers
{
    public class QuestionController : AdminBaseController
    {
        public readonly DataContext _db;
        public QuestionController(DataContext db, GenericRepository repo, IMapper mapper) : base(repo, mapper)
        {
            _db = db;
        }

        [AppAuthorize(AuthConst.AppQuestion.VIEW_DETAIL)]
        public IActionResult Index() => View();


        [HttpGet]
        [Route("/Admin/Question/ListItem")]
        public IActionResult GetQuestion()
        {
            var data = _repo.GetAll<Question>()
                    .ProjectTo<ListQuestionVM>(AutoMapperProfile.QuestionIndexConf)
                    .ToList();
            return Ok(data);
        }

        [HttpGet]
        public IActionResult GetSubject()
        {
            var model = new List<Subject>();

            var data = _repo.GetAll<Subject>()
                .ProjectTo<ListSubjectVM>(AutoMapperProfile.SubjectIndexConf).ToList();
            return Ok(data);
        }

        [HttpGet]
        public IActionResult GetChapter(int subjectId)
        {
            var data = _repo.GetAll<Chapter>()
                .Where(c => c.SubjectId == subjectId)
                .ProjectTo<ListChapterVM>(AutoMapperProfile.ChapterIndexConf)
                .ToList();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var question = await _repo.GetOneAsync<Question>(q => q.Id == id);
            if (question == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không tìm thấy câu hỏi"
                });
            }

            var model = _mapper.Map<QuestionAddOrEditVM>(question);

            model.Options = _repo.GetAll<Answer>()
                .Where(a => a.QuestionId == question.Id)
                .ProjectTo<AnswerAddOrEdit>(AutoMapperProfile.AnswerIndexConf)
                .ToList();

            return Ok(model);
        }


        // add question
        [HttpPost]
        [Route("Admin/Question/CreateQuestion")]
        //[AppAuthorize(AuthConst.AppQuestion.CREATE)]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionAddOrEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ"
                });
            }

            // Extract inner text from <p> tags
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(model.Content);
            var contentNode = htmlDoc.DocumentNode.SelectSingleNode("//p");
            var innerText = contentNode?.InnerText ?? model.Content;


            var question = _mapper.Map<Question>(model);


            question.answers = model.Options.Select((option, index) => new Answer
            {
                AnswerContent = option.AnswerContent,
                Status = option.Status,
                QuestionId = question.Id,
                CreatedBy = this.CurrentUserId,
                CreatedDate = DateTime.Now,
            }).ToList();

            question.CreatedBy = this.CurrentUserId;
            question.CreatedDate = DateTime.Now;

            await _repo.AddAsync(question);
            return Ok(new
            {
                success = true,
                message = "Thêm mới câu hỏi và câu trả lời thành công"
            });
        }

        //edit question
        [HttpPost]
        [AppAuthorize(AuthConst.AppQuestion.UPDATE)]
        public async Task<IActionResult> EditQuestion(int id, [FromBody] QuestionAddOrEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ"
                });
            }

            var questionOld = await _repo.GetOneAsync<Question>(q => q.Id == id);
            if (questionOld == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không tìm thấy câu hỏi"
                });
            }

            questionOld = _mapper.Map(model, questionOld);

            // Update existing answers
            // Lấy tất cả câu trả lời hiện có cho câu hỏi này
            var existingAnswers = _repo.GetAll<Answer>().Where(a => a.QuestionId == questionOld.Id).ToList();

            // Duyệt qua tất cả các option trong model để xử lý thêm mới hoặc cập nhật
            foreach (var option in model.Options)
            {
                var answer = existingAnswers.FirstOrDefault(a => a.Id == option.Id);
                if (answer != null)
                {
                    // Nếu tồn tại, cập nhật nội dung và trạng thái
                    answer.AnswerContent = option.AnswerContent;
                    answer.Status = option.Status;
                    answer.UpdatedBy = this.CurrentUserId;
                    answer.UpdatedDate = DateTime.Now;
                }
            }

            // Xóa các câu trả lời không có trong model.Options
            var optionIds = model.Options.Select(o => o.Id).ToList();
            var answersToRemove = existingAnswers.Where(a => !optionIds.Contains(a.Id)).ToList();
            foreach (var answerToRemove in answersToRemove)
            {
                questionOld.answers.Remove(answerToRemove);
                // Thêm dòng này nếu muốn xóa luôn khỏi database
                await _repo.DeleteAsync(answerToRemove);
            }

            questionOld.UpdatedBy = this.CurrentUserId;
            questionOld.UpdatedDate = DateTime.Now;

            await _repo.UpdateAsync(questionOld);

            return Ok(new
            {
                success = true,
                message = "Cập nhật câu hỏi và câu trả lời thành công"
            });
        }

        [HttpPost]
        [AppAuthorize(AuthConst.AppQuestion.DELETE)]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _repo.GetOneAsync<Question>(q => q.Id == id);
            if (question == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không tìm thấy câu hỏi"
                });
            }

            await _repo.DeleteAsync(question);

            return Ok(new
            {
                success = true,
                message = "Xóa câu hỏi thành công"
            });
        }

        [HttpPost]
        [AppAuthorize(AuthConst.AppQuestion.CREATE)]
        public async Task<IActionResult> ImportFileWord(IFormFile fileWord, int subjectId, int chapterId)
        {
            if (fileWord == null || fileWord.Length == 0)
            {
                return Ok(new { message = "No file uploaded!", success = false });
            }
            try
            {
                // Process the Word file and return the data
                var questions = await ProcessFileAsync(fileWord, subjectId, chapterId);

                if (questions == null || !questions.Any())
                {
                    return Ok(new { message = "No questions found in the file!", success = false });
                }

                var listQuestion = new List<Question>();
                foreach (var item in questions)
                {
                    var question = _mapper.Map<Question>(item);
                    if (item.answers == null || !item.answers.Any())
                    {
                        continue; // Skip questions without answers
                    }

                    question.answers = item.answers.Select((option, index) => new Answer
                    {
                        AnswerContent = option.AnswerContent,
                        Status = option.Status,
                        QuestionId = question.Id,
                        CreatedBy = this.CurrentUserId,
                        CreatedDate = DateTime.Now,
                        Feedback = option.Feedback ?? string.Empty,
                    }).ToList();

                    question.CreatedBy = this.CurrentUserId;
                    question.CreatedDate = DateTime.Now;
                    listQuestion.Add(question);
                }

                if (!listQuestion.Any())
                {
                    return Ok(new { message = "No valid questions to add!", success = false });
                }

                await _repo.AddAsync(listQuestion);
                return Ok(new { message = "Thêm dữ liệu thành công!", success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<List<Question>> ProcessFileAsync(IFormFile file, int subjectId, int chapterId)
        {
            var questions = new List<Question>();

            // Save the file temporarily
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(file.FileName));

            // Ensure the file is completely written before accessing it
            using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(stream);
            }

            // Now process the file
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(tempPath, false))
            {
                var body = wordDoc.MainDocumentPart?.Document?.Body;
                if (body == null)
                {
                    throw new InvalidOperationException("The document body is null.");
                }

                // Extract paragraphs from the Word document
                var paragraphs = body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>()
                                     .Select(p => p.InnerText.Trim())
                                     .Where(text => !string.IsNullOrWhiteSpace(text))
                                     .ToList();

                // Combine paragraphs into a single string
                var questionText = string.Join("\n", paragraphs);

                // Split the text into question blocks based on the pattern
                var questionBlocks = questionText.Split(new[] { "\n[" }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(q => q.Trim())
                                                 .Select(q => q.StartsWith("[") ? q : "[" + q) // Ensure proper format
                                                 .ToList();


                // Process each question block
                foreach (var block in questionBlocks)
                {
                    var lines = block.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length < 2) continue;

                    var ANSWER = Regex.Replace(lines[lines.Length - 1], @"^\[\d+\]\s*", "").Trim();
                    var isCol = ANSWER.Last();

                    // Create a new question
                    var question = new Question
                    {
                        Content = Regex.Replace(lines[0], @"^\[\d+\]\s*", "").Trim(),
                        Level = int.Parse(Regex.Match(lines[0], @"\d+").Value),
                        answers = new List<Answer>(),
                        CreatedDate = DateTime.Now,
                        CreatedBy = this.CurrentUserId,
                    };

                    // Add options
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var optionText = lines[i].Trim();
                        if (optionText.StartsWith("A. ") || optionText.StartsWith("B. ") || optionText.StartsWith("C. ") || optionText.StartsWith("D. "))
                        {
                            question.answers.Add(new Answer
                            {
                                Status = optionText.First() == isCol,
                                AnswerContent = optionText.Length > 3 ? optionText.Substring(3).Trim() : optionText.Trim(),
                                CreatedDate = DateTime.Now,
                                CreatedBy = this.CurrentUserId,
                            });
                        }
                    }

                    // Add subjectId and chapterId to the question
                    question.SubjectId = subjectId;
                    question.ChapterId = chapterId;
                    questions.Add(question);
                }
            }

            // Ensure temporary file is deleted even if an exception occurs
            try
            {
                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Delete(tempPath);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to delete temp file: {ex.Message}");
            }

            return questions;
        }


    }
}
