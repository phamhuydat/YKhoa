using AutoMapper;
using Data;
using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text;
using Web.Services;

namespace Web.Areas.Admin.Controllers
{
    public class FileController : AdminBaseController
    {
        private readonly DataContext _db;
        private readonly IPDFService _pDFService;

        public FileController(DataContext db, GenericRepository repo,
                        IMapper mapper, IPDFService pDFService) : base(repo, mapper)
        {
            _db = db;
            _pDFService = pDFService;
        }

        // xuất file pdf bài thi của học sinh
        public async Task<string> ExportPDF(int userId, int examId)
        {

            var exam = await _repo.GetAll<Exam>(x => x.Id == examId)
                       .Include(x => x.Subject)
                       .FirstOrDefaultAsync();


            var info = await _repo.GetOneAsync<Users>(x => x.Id == userId);

            var result = await _repo.GetOneAsync<Result>(x => x.UserId == userId && x.ExamId == examId);

            var resultDetail = _db.ResultDetails
                .Where(x => x.ResultId == result.Id)
                .Join(_db.Question,
                    rd => rd.QuestionId,
                    q => q.Id,
                    (rd, q) => new { rd, q })
                .Select(x => new
                {
                    QuestionId = x.q.Id,
                    Content = x.q.Content,
                    UserIsCorrect = x.rd.AnswerId,
                    Check = _db.Answers.Where(a => a.Id == x.rd.AnswerId).Single().Status,
                    Answers = _db.Answers
                        .Where(a => a.QuestionId == x.q.Id)
                        .Select(a => new
                        {
                            AnswerId = a.Id,
                            AnswerContent = a.AnswerContent,
                            IsCorrect = ((a.Status && x.rd.AnswerId == a.Id) || a.Status) ? 1
                                    : (!a.Status && x.rd.AnswerId == a.Id) ? 0 : (int?)null
                        })
                    .ToList()
                });
            if (exam == null) throw new Exception("Exam not found.");
            if (info == null) throw new Exception("User info not found.");
            if (result == null) throw new Exception("Result not found.");
            if (!resultDetail.Any()) throw new Exception("No result details found.");



            var html = new StringBuilder();

            html.Append(@"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>
    <style>
        * {padding: 0;margin: 0;box-sizing: border-box;}
        body{font-family: 'Times New Roman', serif; padding: 50px 50px}
    </style>
</head>
<body>
    <table style='width:100%'>
        <tr>
            <td style='text-align: center;font-weight:bold'>
                TRƯỜNG ĐẠI HỌC<br>
                KHOA CÔNG NGHỆ THÔNG TIN
                <br><br><br>
            </td>
            <td style='text-align: center;'>
                <p style='font-weight:bold'>" + (exam.Title?.ToUpper() ?? "") + @"</p>
                <p style='font-weight:bold'>Học phần: " + (exam?.Subject?.SubjectCode ?? "0") + @"</p>
                <p style='font-weight:bold'>Mã học phần: " + (exam?.Subject?.SubjectName ?? "N/A") + @"</p>
                <p style='font-style:italic'>Thời gian làm bài: " + (exam?.WorkTime ?? 0) + @" phút</p>
            </td>
        </tr>
    </table>

    <table style='width:100%;margin-bottom:10px'>
        <tr style='width:100%'>
            <td>Mã sinh viên: " + (info?.MSSV ?? "") + @"</td>
            <td>Tên thí sinh: " + (info?.FullName ?? "") + @"</td>
        </tr>
        <tr style='width:100%'>
            <td>Số câu đúng: " + (result?.NumCorrect ?? 0) + "/" +
                        ((exam?.MQCount ?? 0) + (exam?.HQCount ?? 0) + (exam?.EQCount ?? 0)) + @"</td>
            <td>Điểm: " + (result?.TestScores ?? 0) + @"</td>
        </tr>
    </table>       

    <hr>
    <div style='margin-top:20px'>");

            int index = 1; // Bắt đầu đánh số câu hỏi
            foreach (var item in resultDetail)
            {
                html.Append(" <li style='list-style:none'>" +
                            "<strong>Câu " + index + "</strong>: " + (item.Content ?? "") + "<ol type='A' style='margin-left:30px'>");
                foreach (var answer in item.Answers)
                {
                    var dapAn = (answer.IsCorrect == 1) ? " (Đáp án chính xác)" : "";
                    var dapAnChon = (answer.AnswerId == item.UserIsCorrect) ? " (Đáp án chọn)" : "";
                    html.Append("<li>" + (answer.AnswerContent ?? "") + dapAnChon + dapAn + "</li>");
                }

                html.Append("</ol></li><br/>");
                index++; // Tăng số thứ tự câu hỏi
            }

            html.Append(@"
</div>
</body>
</html>");


            var resultPDF = _pDFService.GeneratePDF(html.ToString());

            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "result.pdf");

            //await System.IO.File.WriteAllBytesAsync(filePath, resultPDF);

            return Convert.ToBase64String(resultPDF);

        }


        //Export the list of scores for a class with all students and their completed exams
        public async Task<IActionResult> ExportExcelTranscript(int groupId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Add this line

            var group = await _repo.GetOneAsync<Group>(x => x.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }

            // Get the list of students and their exam results
            var results = await _db.Result
                .Where(x => x.exam.handOutExams.Any(g => g.GroupId == groupId))
                .Join(_db.Users,
                    r => r.UserId,
                    u => u.Id,
                    (r, u) => new { r, u })
                .Select(x => new
                {
                    x.r.exam.Title,
                    x.u.MSSV,
                    x.u.FullName,
                    x.r.TestScores,
                    ClassName = group.GroupName // Assuming the group name is the class name
                })
                .ToListAsync();


            if (results.Count == 0)
            {
                return Ok(new
                {
                    success = false,
                    message = "không có bài thi nào"
                });
            }

            var examTitles = results.Select(x => x.Title).Distinct().ToList();

            try
            {
                // Create a new Excel package
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Transcript");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "MSSV";
                    worksheet.Cells[1, 2].Value = "Họ Tên";
                    worksheet.Cells[1, 3].Value = "Tên Lớp";
                    for (int i = 0; i < examTitles.Count; i++)
                    {
                        worksheet.Cells[1, i + 4].Value = examTitles[i];
                    }

                    // Add data
                    var students = results.GroupBy(r => new { r.MSSV, r.FullName, r.ClassName }).ToList();
                    for (int i = 0; i < students.Count; i++)
                    {
                        var student = students[i].Key;
                        worksheet.Cells[i + 2, 1].Value = student.MSSV;
                        worksheet.Cells[i + 2, 2].Value = student.FullName;
                        worksheet.Cells[i + 2, 3].Value = student.ClassName;

                        foreach (var result in students[i])
                        {
                            var columnIndex = examTitles.IndexOf(result.Title) + 4;
                            worksheet.Cells[i + 2, columnIndex].Value = result.TestScores;
                        }

                    }

                    // Format the header
                    using (var range = worksheet.Cells[1, 1, 1, examTitles.Count + 3])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    // Add borders and color to all cells
                    using (var range = worksheet.Cells[1, 1, students.Count + 1, examTitles.Count + 3])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                    }

                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();

                    // Convert the package to a byte array
                    var fileContents = package.GetAsByteArray();

                    // Return the Excel file
                    return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Transcript.xlsx");
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }

    }
}
