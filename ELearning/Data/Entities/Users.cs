using Data.Entities.Base;

namespace Data.Entities
{
    public class Users : AppEntityBase
    {
        public Users()
        {
            Results = new HashSet<Result>();
            Assignments = new HashSet<Assignment>();
            GroupDetails = new HashSet<GroupDetails>();
        }
        public string MSSV { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string? GoogleId { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public int Status { get; set; }
        public string? Avatar { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Token { get; set; }
        public DateTime? BlockedTo { get; set; }
        public int? BlockedBy { get; set; }
        public string? OTP { get; set; }
        public int? AppRoleId { get; set; }
        //fk
        public Role Role { get; set; }
        public ICollection<Result> Results { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<GroupDetails> GroupDetails { get; set; }
    }
}