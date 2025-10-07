using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;
using Microsoft.Identity.Web;
using System.Text;

namespace ClearStore.Controllers
{
    [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
    public class TestController : Controller
    {
        private readonly GraphServiceClient _client;

        public TestController(GraphServiceClient client)
        {
            _client = client;
        }


        public async Task<IActionResult> Index()
        {
            StringBuilder body = new StringBuilder();
            body.AppendLine("<p>Hello from ASP.NET Core MVC</p>");

            var messageBody = new SendMailPostRequestBody
            {
                Message = new Message
                {
                    Subject = "Test",
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = body.ToString()
                    },
                    ToRecipients = new List<Recipient>()
                    {
                        new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = "markhughes@clearplanconsulting.com"
                            }
                        }
                    }
                },
                SaveToSentItems = true
            };

            await _client.Me.SendMail.PostAsync(messageBody);

            return View();
        }
    }
}
