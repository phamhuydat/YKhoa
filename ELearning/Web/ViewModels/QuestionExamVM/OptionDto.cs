namespace Web.ViewModels.QuestionExamVM
{
	public class OptionDto
	{
		public int Id { get; set; } // Mã định danh của tùy chọn
		public string Text { get; set; } // Văn bản của tùy chọn
		public bool IsCorrect { get; set; } // Chỉ định xem tùy chọn này có phải là đúng hay không
		public string Explanation { get; set; }

	}
}
