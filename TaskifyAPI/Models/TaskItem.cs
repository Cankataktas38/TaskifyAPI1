namespace TaskifyAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        //Relation to User
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
