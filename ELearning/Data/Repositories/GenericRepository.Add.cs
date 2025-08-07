using Data.Entities;
using Data.Entities.Base;

namespace Data.Repositories
{
    public partial class GenericRepository
    {
        /// <summary>
        /// Thêm 1 record vào database
        /// </summary>
        /// <typeparam name="TEntity">Model của bảng trong DB</typeparam>
        /// <param name="entity">Record cần thêm</param>
        /// <param name="isDeleted">Record này có bị đánh dấu là  "đã xóa" hay không, True => đã xóa</param>

        public virtual async Task AddAsync<TEntity>(
            TEntity entity,
            bool isDeleted = false)
            where TEntity : AppEntityBase
        {
            this.BeforeAdd(entity, isDeleted);
            await _db.Set<TEntity>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Nếu dữ liệu cần thêm > 1000 record mỗi lần thì không nên dùng hàm này
        /// </summary>
        public virtual async Task AddAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : AppEntityBase
        {
            var len = entities.Count();
            for (int i = 0; i < len; i++)
            {
                this.BeforeAdd(entities.ElementAt(i));
            }
            await _db.AddRangeAsync(entities);
            await _db.SaveChangesAsync();
        }
        public virtual async Task AddQuestionsAsync(List<Question> questions)
        {
            var count = questions.Count;

            // Perform any preprocessing or validation before adding
            for (int i = 0; i < count; i++)
            {
                this.BeforeAdd(questions[i]); // Assuming BeforeAdd processes or validates a single entity
            }

            // Add questions to the database
            await _db.Question.AddRangeAsync(questions); // Adjust the DbSet name if needed
            await _db.SaveChangesAsync();
        }

        #region Bảng Master
        public virtual async Task AddMstAsync<TEntity>(
            TEntity entity,
            bool isDeleted = false)
            where TEntity : MstEntityBase
        {
            var now = DateTime.Now;
            entity.CreatedDate = now;
            if (isDeleted)
            {
                entity.DeletedDate = now;
            }
            else
            {
                entity.DeletedDate = null;
            }
            await _db.Set<TEntity>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }
        #endregion
    }
}
