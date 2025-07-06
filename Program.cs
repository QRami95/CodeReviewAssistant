using CodeReviewAssistant.Models;
using CodeReviewAssistant.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICodeReviewService, CodeReviewService>();
builder.Services.AddScoped<IGitHubService, GitHubService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/pullRequest", async (HttpRequest req, IGitHubService gitHubService, ICodeReviewService codeReviewService) =>
{
    using var reader = new StreamReader(req.Body);
    var body = await reader.ReadToEndAsync();
    var input = JsonSerializer.Deserialize<GetGitHubPullRequest>(body);

    var reviewRequest = await gitHubService.GetReviewRequestAsync(input);

    var reviews = new List<string>();

    foreach (var file in reviewRequest.Files)
    {
        var singleFileRequest = new ReviewRequest
        {
            Owner = reviewRequest.Owner,
            Repo = reviewRequest.Repo,
            PullRequestNumber = reviewRequest.PullRequestNumber,
            Files = new List<ReviewFile> { file }
        };

        var review = await codeReviewService.PerformReviewAsync(singleFileRequest);

        reviews.Add($"**Review for {file.FilePath}:**\n{review}");

        var positions = GitHubService.GetChangedLinePositions(file.Diff);

        // Post comment for each changed line — you can customize this logic to post one comment per file or per line
        foreach (var (lineNumber, diffPosition) in positions)
        {
            // For demo, post the whole review as a comment on the first changed line only to avoid spamming
            // You can customize this to post comments per line snippet, etc.
            await gitHubService.PostReviewCommentAsync(
                new PostReviewRequest
                {
                    Owner = reviewRequest.Owner,
                    Repository = reviewRequest.Repo,
                    PullRequestNumber = reviewRequest.PullRequestNumber,
                    CommitSha = file.CommitSha,
                    FilePath = file.FilePath,
                    Position = diffPosition,
                    Comment = review
                });
            break; // Comment only once per file here
        }

    }

    return Results.Ok(new { Reviews = reviews });

})
.WithName("ReviewPullRequest");

app.Run();

