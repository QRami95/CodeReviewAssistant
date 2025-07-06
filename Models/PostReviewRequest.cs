namespace CodeReviewAssistant.Models
{
    public class PostReviewRequest
    {
        public required string Owner { get; set; }
        public required string Repository { get; set; }
        public required int PullRequestNumber { get; set; }
        public required string CommitSha { get; set; }
        public required string FilePath { get; set; }
        public required int Position { get; set; }
        public required string Comment { get; set; }
    }
}
