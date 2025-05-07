// See https://aka.ms/new-console-template for more information

using Octokit;
//using ProductHeaderValue = System.Net.Http.Headers.ProductHeaderValue;

Console.WriteLine("Hello, OctoKit!");

var pat = args[0];

var gitHubClient = new GitHubClient(new ProductHeaderValue("MyCoolApp"));
gitHubClient.Credentials = new Credentials(pat);

var fileName = args[1];

Console.WriteLine($"Will upload file {fileName} to github");

string base64String = GetImageBase64String(fileName);

var result = await gitHubClient.Repository.Content.CreateFile("b3b00"
    , "dendronNotes", "notes/assets/images/test.png",
    new CreateFileRequest($"First commit for {fileName}", base64String, "main", false));

var commit = result.Commit;
Console.WriteLine($"file uploaded with commit {commit.Message} - {commit.Sha} by {commit.Author.Name}");

static string GetImageBase64String(string imgPath)
{
    byte[] imageBytes = System.IO.File.ReadAllBytes(imgPath);
    return Convert.ToBase64String(imageBytes);
}

