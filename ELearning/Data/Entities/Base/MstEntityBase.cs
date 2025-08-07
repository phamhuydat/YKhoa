namespace Data.Entities.Base
{
    public abstract class MstEntityBase
    {
        public int Id { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}