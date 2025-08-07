using Data.Configurations;
using Data.DataSeeders;
using Data.Entities;
using Microsoft.EntityFrameworkCore;


namespace Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opt) : base(opt)
        {

        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<MstPermission> MstPermissions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Assignment> Assignment { get; set; }
        public DbSet<AutomaticExam> AutomaticExam { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Exam> Exam { get; set; }
        public DbSet<ExamDetails> ExamDetails { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupDetails> GroupDetails { get; set; }
        public DbSet<HandOutExam> HandOutExam { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<NotificationDetails> NotificationDetails { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Result> Result { get; set; }
        public DbSet<ResultDetails> ResultDetails { get; set; }
        public DbSet<Subject> Subject { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AnswerConfig());
            modelBuilder.ApplyConfiguration(new AssignmentConfig());
            modelBuilder.ApplyConfiguration(new ChapterConfig());
            modelBuilder.ApplyConfiguration(new ExamConfig());
            modelBuilder.ApplyConfiguration(new AutomaticExamConfig());
            modelBuilder.ApplyConfiguration(new ExamDetailsConfig());
            modelBuilder.ApplyConfiguration(new GroupConfig());
            modelBuilder.ApplyConfiguration(new GroupDetailsConfig());
            modelBuilder.ApplyConfiguration(new HandOutExamConfig());
            modelBuilder.ApplyConfiguration(new MstPermissionConfig());
            modelBuilder.ApplyConfiguration(new NotificationConfig());
            modelBuilder.ApplyConfiguration(new QuestionConfig());
            modelBuilder.ApplyConfiguration(new ResultConfig());
            modelBuilder.ApplyConfiguration(new ResultDetailsConfig());
            modelBuilder.ApplyConfiguration(new ResultConfig());
            modelBuilder.ApplyConfiguration(new RolePermissionConfig());
            modelBuilder.ApplyConfiguration(new SubjectConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());



            // seender data
            modelBuilder.Entity<MstPermission>().SeedData();
            modelBuilder.Entity<Role>().SeedData();
            modelBuilder.Entity<Users>().SeedData();
            modelBuilder.Entity<RolePermission>().SeedData();


        }

    }
}
