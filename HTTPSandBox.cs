using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.IO;
using Octokit;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace AlsideRFIDDotNetCsharp
{
    static class HttpResponseMessageExtensions
    {
        internal static string WriteRequestToConsole(this HttpResponseMessage response)
          {
            if (response is null)
            {
                return "";
            }

            var request = response.RequestMessage;

            string strValue = ($"{request?.Method} ");
            strValue += ($"{request?.RequestUri} ");
            strValue += ($"HTTP/{request?.Version}");

            return strValue;
        }
    }

    class HTTPSandBox
    {
        // HttpClient lifecycle management best practices:
        // https://learn.microsoft.com/dotnet/fundamentals/networking/http/httpclient-guidelines#recommended-use
        public static HttpClient sharedClient = new()
        {
            BaseAddress = new Uri("https://jsonplaceholder.typicode.com"),
        };

        static async Task PostAsJsonAsync(HttpClient httpClient, string displayType)
        {
            using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                "todos",
                new Todo(UserId: 68, Id: 99, Title: "YoungJ A Materials Show extensions", Completed: false));

            var requestToConsole = response.EnsureSuccessStatusCode()
                .WriteRequestToConsole();

            var todo = await response.Content.ReadFromJsonAsync<Todo>();
            Console.WriteLine($"{todo}\n");

            // Expected output:
            //   POST https://jsonplaceholder.typicode.com/todos HTTP/1.1
            //   Todo { UserId = 9, Id = 201, Title = Show extensions, Completed = False }

            var cRFID = new AlsideRFIDDotNetCsharp.Program();
            cRFID.Initialize(Globals2.CmdLineParm);

            AlwinDataDTO alwinDataDTO = new AlwinDataDTO();
            alwinDataDTO = cRFID.GetAlWinDataAlternate();

            alwinDataDTO.HTTPCall = requestToConsole;
            alwinDataDTO.JsonResponse = $"{todo}\n";

            cRFID.ImportDtoToTmpTable(alwinDataDTO, displayType);
        }

        public static async Task PostAsync(HttpClient httpClient, string displayType)
        {
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    userId = 68,
                    id = 1,
                    title = "YoungJ A Materials code sample",
                    completed = false
                }),
                Encoding.UTF8,
                "application/json");

            using HttpResponseMessage response = await httpClient.PostAsync(
                "todos",
                jsonContent);

            var requestToConsole = response.EnsureSuccessStatusCode()
                .WriteRequestToConsole();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{jsonResponse}\n");

            // Expected output:
            //   POST https://jsonplaceholder.typicode.com/todos HTTP/1.1
            //   {
            //     "userId": 77,
            //     "id": 201,
            //     "title": "write code sample",
            //     "completed": false
            //   }

            var cRFID = new AlsideRFIDDotNetCsharp.Program();
            cRFID.Initialize(Globals2.CmdLineParm);

            AlwinDataDTO alwinDataDTO = new AlwinDataDTO();
            alwinDataDTO = cRFID.GetAlWinDataAlternate();

            alwinDataDTO.HTTPCall = requestToConsole;
            alwinDataDTO.JsonResponse = jsonResponse;
                        
            cRFID.ImportDtoToTmpTable(alwinDataDTO, displayType);
        }

        static async Task GetAsync(HttpClient httpClient, string displayType)
        {
            using HttpResponseMessage response = await httpClient.GetAsync("todos/3");

            //var statusCode = response.StatusCode;
            //Globals2.Status++;

            var requestToConsole = response.EnsureSuccessStatusCode()
              .WriteRequestToConsole();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            //Console.WriteLine($"{jsonResponse}\n");

            // Expected output:
            //   GET https://jsonplaceholder.typicode.com/todos/3 HTTP/ 1.1
            //   {
            //     "userId": 1,
            //     "id": 3,
            //     "title": "fugiat veniam minus",
            //     "completed": false
            //   }


            var cRFID = new AlsideRFIDDotNetCsharp.Program();
            cRFID.Initialize(Globals2.CmdLineParm);

            AlwinDataDTO alwinDataDTO = new AlwinDataDTO();
            alwinDataDTO = cRFID.GetAlWinDataAlternate();

            alwinDataDTO.HTTPCall = requestToConsole;
            alwinDataDTO.JsonResponse = jsonResponse;

            cRFID.ImportDtoToTmpTable(alwinDataDTO, displayType);
            

        }

        static async Task DoTokenTutorial(string displayType)
        {
            try 
            { 
                //https://contentlab.io/using-c-code-to-access-the-github-api/

                //must use GitHub's nuget package, named Oktokit

                //client must identify itself
                //MKS127103
                var GitHubIdentity = "Youngj2";

                #region Unauthenticated Access
                //unauthenticated client; least access; most restrictive rate limits; not recomended except to create an OAuth token
                var productInformation = new ProductHeaderValue(GitHubIdentity);
                var client = new GitHubClient(productInformation);
                #endregion

                #region Basic Authentication
                //application shares a username and a password with GitHub; least likely used by desktop applications
                var productInformationBA = new ProductHeaderValue(GitHubIdentity);
                var credentialsBA = new Credentials(GitHubIdentity, Globals2.MyGitHubPwd, AuthenticationType.Basic);
                var clientBA = new GitHubClient(productInformation) { Credentials = credentialsBA };
                #endregion

                #region OAuth Token Authentication Using Token generated and retrieved at GitHub page
                var productInformationOTA = new ProductHeaderValue(GitHubIdentity);
                //creating a personal access token using OAuth; a user authenticates using a token that was issued by the GitHub web site:
                //https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token
                
                var credentialsOTA = new Credentials(Globals2.GbToken);
                var clientOTA = new GitHubClient(productInformation) { Credentials = credentialsOTA };                
                #endregion

                #region accessing GitHub repos
                //var owner = "octokit";
                //var repo = "octokit.net";
                var owner = GitHubIdentity;
                var repo = "Youngj2TestRepoTwo";                

                Octokit.Repository repository = await clientOTA.Repository.Get(owner, repo);

                //search for a label
                //SearchLabelsResult result = await client.Search.SearchLabels(
                //    new SearchLabelsRequest("category", repository.Id));

                //search code
                SearchCodeResult result = await client.Search.SearchCode(
                //new SearchCodeRequest("issue")
                new SearchCodeRequest("await")
                {
                    In = new CodeInQualifier[] { CodeInQualifier.Path },
                    Language = Language.CSharp,
                    //Repos = new RepositoryCollection { "octokit/octokit.net" }
                    Repos = new RepositoryCollection {repository.FullName}
                });

                Console.WriteLine("done");
                #endregion


                #region send repo data to tmp table in our database
                GitHubRepoDTO gitHubRepoDTO = new GitHubRepoDTO();
                gitHubRepoDTO.RepositoryID = "Repository ID: " + repository.Id.ToString();
                gitHubRepoDTO.CloneUrl = "Clone Url: " + repository.CloneUrl;
                gitHubRepoDTO.Description = "Description: " + repository.Description;
                gitHubRepoDTO.FullName = "Full Name: " + repository.FullName;
                gitHubRepoDTO.HtmlUrl = "Html Url: " + repository.HtmlUrl;
                gitHubRepoDTO.Name = "Name: " + repository.Name;
                gitHubRepoDTO.UserId = "User ID: " + repository.Owner.Id.ToString();
                gitHubRepoDTO.UserLogin = "User Login: " + repository.Owner.Login;
                gitHubRepoDTO.Url = "Url: " + repository.Url;

                var cRFID = new AlsideRFIDDotNetCsharp.Program();
                cRFID.Initialize(Globals2.CmdLineParm);
                cRFID.ImportToTmpTableGitHubRepoData(gitHubRepoDTO, displayType);
                #endregion


            }
            catch (Exception e)

                {
                    string strErrMsg = e.Message;
                    Console.WriteLine(strErrMsg);
                }
            }

        static async Task PutAsync(HttpClient httpClient, string displayType)
        {
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    userId = 1,
                    id = 1,
                    title = "foo bar",
                    completed = false
                }),
                Encoding.UTF8,
                "application/json");

            using HttpResponseMessage response = await httpClient.PutAsync(
                "todos/1",
                jsonContent);

            var requestToConsole = response.EnsureSuccessStatusCode()
                .WriteRequestToConsole();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{jsonResponse}\n");

            // Expected output:
            //   PUT https://jsonplaceholder.typicode.com/todos/1 HTTP/1.1
            //   {
            //     "userId": 1,
            //     "id": 1,
            //     "title": "foo bar",
            //     "completed": false
            //   }

            var cRFID = new AlsideRFIDDotNetCsharp.Program();
            cRFID.Initialize(Globals2.CmdLineParm);

            AlwinDataDTO alwinDataDTO = new AlwinDataDTO();
            alwinDataDTO = cRFID.GetAlWinDataAlternate();

            alwinDataDTO.HTTPCall = requestToConsole;
            alwinDataDTO.JsonResponse = jsonResponse;

            cRFID.ImportDtoToTmpTable(alwinDataDTO, displayType);
        }

        public void DoHTTPSandbox(string displayType)
        {
           
            switch (displayType)
            {
                case "JSON GET":
                    GetFromJsonAsync(sharedClient, displayType).ToString();
                    break;
                case "HTTP POST":
                    _ = PostAsync(sharedClient, displayType);
                    break;
                case "JSON POST":
                    _ = PostAsJsonAsync(sharedClient, displayType);
                    break;
                case "HTTP PUT":                    
                    _ = PutAsync(sharedClient, displayType);
                    break;
                case "TOKEN":
                    _ = DoTokenTutorial(displayType);
                    break;
                default:
                    GetAsync(sharedClient, displayType).ToString();
                    break;
            }
        }
               
        static async Task<List<Repository>> ProcessRepositoriesAsync(HttpClient client)
        {
            using Stream stream =
                await client.GetStreamAsync("https://api.github.com/orgs/dotnet/repos");
            var repositories =
                await JsonSerializer.DeserializeAsync<List<Repository>>(stream);
            return repositories ?? new();
        }

        public async Task DoWebAPIClientAsync(string displayType)
        {

            try
            {
                int i = 0;
                using HttpClient client = new();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                var repositories = await ProcessRepositoriesAsync(client);
                WebAPIClientExampleDTO webAPIClientDTO = new WebAPIClientExampleDTO();

                var cRFID = new AlsideRFIDDotNetCsharp.Program();
                cRFID.Initialize(Globals2.CmdLineParm);

                foreach (var repo in repositories)
                {

                    webAPIClientDTO.RepoName = ($"Name: {repo.Name}");
                    webAPIClientDTO.RepoHomePage = ($"Homepage: {repo.Homepage}");
                    webAPIClientDTO.RepoGitHubHomeUrl = ($"GitHub: {repo.GitHubHomeUrl}");
                    webAPIClientDTO.RepoDescription = ($"Description: {repo.Description}");
                    webAPIClientDTO.RepoWatchers = ($"Watchers: {repo.Watchers:#,0}");
                    webAPIClientDTO.RepoLastPush = ($"{repo.LastPush}");

                    cRFID.ImportDtoToTmpTableWebAPIClient(webAPIClientDTO, displayType);

                    i++;
                    if (i > 5)
                    {
                        break;
                    }
                }
            }

            catch (Exception e)

            {
                string strErrMsg = e.Message;
                Console.WriteLine(strErrMsg);
            }
        }

        static async Task GetFromJsonAsync(HttpClient httpClient, string displayType)
        {

            var todos = await httpClient.GetFromJsonAsync<List<Todo>>(
                "todos?userId=1&completed=false");

            Console.WriteLine("GET https://jsonplaceholder.typicode.com/todos?userId=1&completed=false HTTP/1.1");
            todos?.ForEach(Console.WriteLine);
            Console.WriteLine();

            var cRFID = new AlsideRFIDDotNetCsharp.Program();
            cRFID.Initialize(Globals2.CmdLineParm);

            foreach (Todo varSomething in todos)
            {
                cRFID.ImportDtoToTmpTableJson(varSomething.ToString(), displayType);
            }

            // Expected output:
            //   GET https://jsonplaceholder.typicode.com/todos?userId=1&completed=false HTTP/1.1
            //   Todo { UserId = 1, Id = 1, Title = delectus aut autem, Completed = False }
            //   Todo { UserId = 1, Id = 2, Title = quis ut nam facilis et officia qui, Completed = False }
            //   Todo { UserId = 1, Id = 3, Title = fugiat veniam minus, Completed = False }
            //   Todo { UserId = 1, Id = 5, Title = laboriosam mollitia et enim quasi adipisci quia provident illum, Completed = False }
            //   Todo { UserId = 1, Id = 6, Title = qui ullam ratione quibusdam voluptatem quia omnis, Completed = False }
            //   Todo { UserId = 1, Id = 7, Title = illo expedita consequatur quia in, Completed = False }
            //   Todo { UserId = 1, Id = 9, Title = molestiae perspiciatis ipsa, Completed = False }
            //   Todo { UserId = 1, Id = 13, Title = et doloremque nulla, Completed = False }
            //   Todo { UserId = 1, Id = 18, Title = dolorum est consequatur ea mollitia in culpa, Completed = False }
        }
    }
}
