public class GetGitHubPullRequest
{
    public required string Owner { get; set; }
    public required string Repository { get; set; }
    public int PullRequestNumber { get; set; }
}