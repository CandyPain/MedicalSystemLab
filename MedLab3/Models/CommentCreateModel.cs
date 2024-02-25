namespace MedLab3.Models
{
    public class CommentCreateModel
    {
        public string Content { get; set; }
        public Guid ParentId { get; set; }
    }
}
