namespace CodeReviewAssistant.Models
{
    public class ReviewRequest
    {
        public string Repo { get; set; }
        public string Owner { get; set; }
        public int PullRequestNumber { get; set; }
        public List<ReviewFile> Files { get; set; }
    }

    public class ReviewFile
    {
        public string FilePath { get; set; }
        public string Diff { get; set; }
        public string CommitSha { get; set; }
    }
}
