using Azure.Identity;
using ClearStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Graph;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;
using System.Security.Claims;
using SendMailRequestBody = Microsoft.Graph.Users.Item.SendMail;

namespace ClearScore.Extensions
{
    public static class GraphExtensions
    {
        public static async System.Threading.Tasks.Task<bool> SendEmailAsync(
            this GraphServiceClient client,
            string subject,
            string body,
            string to,
            List<string>? ccRecipients = null)
        {
            try
            {
                if (string.IsNullOrEmpty(to))
                {
                    to = "markhughes@clearplanconsulting.com";
                }

                var messageBody = new SendMailPostRequestBody
                {
                    Message = new Message
                    {
                        ToRecipients = new List<Recipient>
                        {
                            new Recipient
                            {
                                EmailAddress = new EmailAddress { Address = to }
                            }
                        },
                        Subject = subject,
                        Body = new ItemBody
                        {
                            ContentType = BodyType.Html,
                            Content = body
                        },
                        Importance = Importance.Normal
                    }
                };

                if (ccRecipients != null && ccRecipients.Any() && ccRecipients.Count > 0)
                {
                    var recipients = new List<Recipient>();
                    foreach (var cc in ccRecipients)
                    {
                        var recipient = new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = cc
                            }
                        };
                        recipients.Add(recipient);
                    }

                    messageBody.Message.CcRecipients = recipients;
                }

                await client.Me.SendMail.PostAsync(messageBody);
                return true;
            }
            catch (Exception error)
            {
                throw new Exception($"{error.Message}");
            }
        }


        public static async System.Threading.Tasks.Task<string> GetEmailAsync(this GraphServiceClient client, string userId)
        {
            var user = await client.Users[userId].GetAsync();
            return user!.Mail!.ToString();
        }


        public static async System.Threading.Tasks.Task<string> CurrentUserRole(this IAuthorizationService svc, ClaimsPrincipal claimsUser)
        {
            string userRole = string.Empty;
            if ((await svc.AuthorizeAsync(user: claimsUser, "administrators")).Succeeded)
            {
                userRole = "administrators";
            }
            else if ((await svc.AuthorizeAsync(user: claimsUser, "assessors")).Succeeded)
            {
                userRole = "assessors";
            }
            else if ((await svc.AuthorizeAsync(user: claimsUser, "executives")).Succeeded)
            {
                userRole = "executives";
            }
            else if ((await svc.AuthorizeAsync(user: claimsUser, "managers")).Succeeded)
            {
                userRole = "managers";
            }
            else if ((await svc.AuthorizeAsync(user: claimsUser, "hr")).Succeeded)
            {
                userRole = "hr";
            }
            else if ((await svc.AuthorizeAsync(user: claimsUser, "recruiting")).Succeeded)
            {
                userRole = "recruiting";
            }
            else if ((await svc.AuthorizeAsync(user: claimsUser, "accounting")).Succeeded)
            {
                userRole = "accounting";
            }
            else if ((await svc.AuthorizeAsync(user: claimsUser, "operations")).Succeeded)
            {
                userRole = "operations";
            }
            return userRole;
        }

        public static CPUser ToCPUser(this User user)
        {
            return new CPUser
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                GivenName = user.GivenName,
                Surname = user.Surname,
                Mail = user.Mail,
                PhoneNumber = user.MobilePhone
            };
        }
    }
}
