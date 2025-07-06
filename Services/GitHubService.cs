using CodeReviewAssistant.Models;
using Octokit;

namespace CodeReviewAssistant.Services
{
    public interface IGitHubService
    {
        Task<ReviewRequest> GetReviewRequestAsync(GetGitHubPullRequest input);
        Task PostReviewCommentAsync(PostReviewRequest request);
    }

    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;

        public GitHubService(IConfiguration config)
        {
            var token = config["GitHub:Token"];
            _client = new GitHubClient(new ProductHeaderValue(config["GitHub:ProfileName"]))
            {
                Credentials = new Credentials(token)
            };
        }

        public async Task<ReviewRequest> GetReviewRequestAsync(GetGitHubPullRequest input)
        {
            var pr = await _client.PullRequest.Get(input.Owner, input.Repository, input.PullRequestNumber);
            var prFiles = await _client.PullRequest.Files(input.Owner, input.Repository, input.PullRequestNumber);

            if (!prFiles.Any())
                throw new Exception("No files found in pull request.");

            var reviewFiles = prFiles.Select(f => new ReviewFile
            {
                FilePath = f.FileName,
                Diff = f.Patch,
                CommitSha = pr.Head.Sha // Use PR latest commit SHA here
            }).ToList();

            return new ReviewRequest
            {
                Owner = input.Owner,
                Repo = input.Repository,
                PullRequestNumber = input.PullRequestNumber,
                Files = reviewFiles
            };
        }

        public static Dictionary<int, int> GetChangedLinePositions(string patch)
        {
            var positions = new Dictionary<int, int>();
            int diffPosition = 0;  // GitHub's diff position counter
            int currentLineInDiff = 0;

            var lines = patch.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith("+") && !line.StartsWith("+++"))
                {
                    diffPosition++;
                    positions[currentLineInDiff] = diffPosition;
                }
                else if (!line.StartsWith("-") && !line.StartsWith("---"))
                {
                    diffPosition++;
                }
                currentLineInDiff++;
            }
            return positions;
        }

        public async Task PostReviewCommentAsync(PostReviewRequest request)
        {
            var prComment = new PullRequestReviewCommentCreate(request.Comment, request.CommitSha, request.FilePath, request.Position);
            await _client.PullRequest.ReviewComment.Create(request.Owner, request.Repository, request.PullRequestNumber, prComment);
        }
    }

}
