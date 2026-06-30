using ClearStore.Data;
using ClearStore.Extensions;
using ClearStore.Models;
using ClearStore.Models.Dto;
using ClearStore.Security;
using ClearStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;
using Microsoft.Identity.Web;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Claims;
using System.Text;

namespace ClearStore.Controllers
{
    [Route("product-orders")]
    [Authorize]
    [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
    public class ProductOrdersController : Controller
    {
        private readonly StoreContext _context;
        private readonly GraphServiceClient _client;
        private readonly string adminEmail = "jodygoldenberg@clearplanconsulting.com";
        private readonly string chelseyEmail = "chelseymoore@clearplanconsulting.com";

        public ProductOrdersController(StoreContext context, GraphServiceClient client)
        {
            _context = context;
            _client = client;
        }

        [Authorize(Policy = "storeadmins")]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.ProductOrders
                .Include(d => d.StatusCategory)
                .OrderByDescending(d => d.ProductOrderId)
                .ToListAsync();

            return View(orders);
        }


        [Route("create/{cartId}")]
        public async Task<IActionResult> Create(int cartId)
        {
            var userCart = await _context.ProductCarts
                .Where(d => d.ProductCartId == cartId && d.Status == 1)
                .Include(d => d.ProductItems)
                .OrderByDescending(d => d.ProductCartId)
                .FirstOrDefaultAsync();

            if (userCart == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var me = await _client.Me.GetAsync();
            var user = (ClaimsPrincipal)User;
            var emailAddress = user.FindFirst("preferred_username")?.Value;

            var orderItems = await _context.ProductItems
                    .Where(pi => pi.ProductCartId == userCart.ProductCartId)
                    .Select(pi => new ProductDetailDto
                    {
                        ProductCartId = pi.ProductCartId,
                        ProductItemId = pi.ProductItemId,
                        ProductId = pi.ProductId,
                        ProductName = pi.Product != null ? pi.Product.Name : null,
                        ProductSizeId = pi.ProductSizeId,
                        SizeName = pi.ProductSize != null ? pi.ProductSize.Name : null,
                        ProductColorId = pi.ProductColorId,
                        ColorName = pi.ProductColor != null ? pi.ProductColor.Name : null,
                        ProductGenderId = pi.ProductGenderId,
                        GenderName = pi.Product != null && pi.Product.ProductGender != null ? pi.Product.ProductGender.Name : null,
                        Quantity = pi.Quantity,
                        ProductInventoryId = pi.ProductInventoryId,
                        Image = _context.ProductImages
                            .Where(d => d.ProductId == pi.ProductId)
                            .Select(i => i.ImageData)
                            .FirstOrDefault(),
                        ImageDetail = _context.ProductImages
                            .Where(d => d.ProductId == pi.ProductId)
                            .Select(i => new ImageDetail
                            {
                                ImageId = i.Id,
                                ImageData = i.ImageData,
                                ImageName = i.ImageName ?? string.Empty
                            })
                            .FirstOrDefault()
                    })
                    .ToListAsync();

            if (!orderItems.Any())
            {
                TempData["CartCountError"] = true;
                return RedirectToAction("Index", "Home");
            }

            var model = new ProductOrderModel
            {
                ProductCart = userCart,
                OrderItems = orderItems,
            };

            ViewBag.EmailAddress = emailAddress;

            return View(model);
        }


        [Route("create/{cartId}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int cartId, ProductOrderModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (ModelState.IsValid)
                {
                    var cart = await _context.ProductCarts
                        .Where(d => d.ProductCartId == model.ProductOrder.ProductCartId)
                        .FirstOrDefaultAsync();

                    if (cart != null)
                    {
                        cart.Status = 2;
                    }

                    await _context.ProductOrders.AddAsync(model.ProductOrder);
                    await _context.SaveChangesAsync();

                    await SendConfirmationEmailAsync(model);

                    await transaction.CommitAsync();

                    TempData["OrderComplete"] = true;
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred placing this order: {ex.Message}");
                return View(model);
            }

            var userCart = await _context.ProductCarts
                .Where(d => d.ProductCartId == cartId && d.Status == 1)
                .Include(d => d.ProductItems)
                .OrderByDescending(d => d.ProductCartId)
                .FirstOrDefaultAsync();

            model.ProductCart = userCart;

            var user = (ClaimsPrincipal)User;
            var emailAddress = user.FindFirst("preferred_username")?.Value;

            ViewBag.EmailAddress = emailAddress;

            return View(model);
        }


        [Authorize(Policy = "storeadmins")]
        [Route("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var productOrder = await _context.ProductOrders
                .FindAsync(id);

            if (productOrder == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var orderItems = await _context.ProductItems
                        .Where(pi => pi.ProductCartId == productOrder.ProductCartId)
                        .Select(pi => new ProductOrderItemDto
                        {
                            IsApparel = pi.Product != null ? (bool?)pi.Product.IsApparel : null,
                            ProductCartId = pi.ProductCartId,
                            ProductItemId = pi.ProductItemId,
                            ProductId = pi.ProductId,
                            ProductName = pi.Product != null ? pi.Product.Name : null,
                            ProductSizeId = pi.ProductSizeId,
                            SizeName = pi.ProductSize != null ? pi.ProductSize.Name : null,
                            ProductColorId = pi.ProductColorId,
                            ColorName = pi.ProductColor != null ? pi.ProductColor.Name : null,
                            ProductGenderId = pi.ProductGenderId,
                            GenderName = pi.Product != null && pi.Product.ProductGender != null ? pi.Product.ProductGender.Name : null,
                            Quantity = pi.Quantity,
                            ProductInventoryId = pi.ProductInventoryId,
                            ImageDetail = _context.ProductImages
                                .Where(d => d.ProductId == pi.ProductId)
                                .Select(i => new ImageDetail
                                {
                                    ImageId = i.Id,
                                    ImageData = i.ImageData,
                                    ImageName = i.ImageName ?? string.Empty
                                })
                                .FirstOrDefault()
                        })
                        .ToListAsync();

            var model = new ProductOrderDetailModel
            {
                ProductOrder = productOrder,
                ProductOrderItems = orderItems
            };

            return View(model);
        }


        [Route("details/{id}")]
        [HttpPost]
        public async Task<IActionResult> Details(int id, ProductOrderDetailModel model)
        {
            if (ModelState.IsValid)
            {
                _context.ProductOrders.Update(model.ProductOrder);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var orderItems = await _context.ProductItems
                        .Where(pi => pi.ProductCartId == model.ProductOrder.ProductCartId)
                        .Select(pi => new ProductOrderItemDto
                        {
                            ProductCartId = pi.ProductCartId,
                            ProductItemId = pi.ProductItemId,
                            ProductId = pi.ProductId,
                            ProductName = pi.Product != null ? pi.Product.Name : null,
                            ProductSizeId = pi.ProductSizeId,
                            SizeName = pi.ProductSize != null ? pi.ProductSize.Name : null,
                            ProductColorId = pi.ProductColorId,
                            ColorName = pi.ProductColor != null ? pi.ProductColor.Name : null,
                            Quantity = pi.Quantity,
                            ProductInventoryId = pi.ProductInventoryId,
                            ImageDetail = _context.ProductImages
                                .Where(d => d.ProductId == pi.ProductId)
                                .Select(i => new ImageDetail
                                {
                                    ImageId = i.Id,
                                    ImageData = i.ImageData,
                                    ImageName = i.ImageName ?? string.Empty
                                })
                                .FirstOrDefault()
                        })
                        .ToListAsync();

            var user = (ClaimsPrincipal)User;
            var emailAddress = user.FindFirst("preferred_username")?.Value;

            model.ProductOrder.Email = emailAddress;
            model.ProductOrderItems = orderItems;

            return View(model);
        }


        private async Task SendConfirmationEmailAsync(ProductOrderModel model)
        {
            string hyperlink = $"https://clearstore.azurewebsites.net/product-orders/details/{model.ProductOrder.ProductOrderId}";
            string subject = $"New ClearStore order - {model.ProductOrder.Recipient}";
            StringBuilder body = new StringBuilder();
            var attachments = new List<Attachment>();

            body.AppendLine("<div style=\"max-width:1366px;margin:0 auto;\">");
            body.AppendLine("<div style=\"font-family:system-ui;font-size:14px;\">");
            body.AppendLine("<h3>Shipping details</h3>");
            body.AppendLine("<br>");

            // user information
            body.AppendLine("<table style=\"padding: 4px;\">");
            body.AppendLine("<tr>");
            body.AppendLine($"<td>Name: </td><td>{model.ProductOrder.Recipient}</td>");
            body.AppendLine("</tr>");
            body.AppendLine("<tr>");
            body.AppendLine($"<td>Address: </td>");
            body.AppendLine($"<td>");
            body.AppendLine($"<div>{model.ProductOrder.Address}</div>");
            body.AppendLine($"<div>{model.ProductOrder.City}, {model.ProductOrder.State} {model.ProductOrder.ZipCode}</div>");
            body.AppendLine($"</td>");
            body.AppendLine("</tr>");

            if (model.ProductOrder.PhoneNumber.HasValue)
            {
                body.AppendLine("<tr>");
                body.AppendLine($"<td>Phone number: </td><td>{String.Format("{0:###-###-####}", model.ProductOrder.PhoneNumber.Value)}</td>");
                body.AppendLine("</tr>");
            }

            body.AppendLine("</table>");

            if (!string.IsNullOrEmpty(model.ProductOrder.Notes))
            {
                body.AppendLine("<h3>Additional notes:</h3>");
                body.AppendLine($"<p>{model.ProductOrder.Notes}</p>");
            }

            body.AppendLine("<h3>Order summary:</h3>");

            body.AppendLine("<table style=\"border-collapse:collapse;width:100%;\">");
            body.AppendLine("<tbody>");

            if (model.OrderItems.Any())
            {
                foreach (var item in model.OrderItems)
                {
                    body.AppendLine("<tr>");

                    if (item.ImageDetail != null)
                    {
                        string cid = $"img-{item.ProductItemId}";
                        string imageType = GeneralExtensions.GetImageContentType(item.ImageDetail.ImageName);

                        var attachment = new FileAttachment
                        {
                            Name = cid,
                            ContentType = imageType,
                            IsInline = true,
                            OdataType = "#microsoft.graph.fileAttachment",
                            ContentBytes = item.ImageDetail?.ImageData,
                            ContentId = cid
                        };

                        attachments.Add(attachment);

                        body.AppendLine($"<td style=\"width:180px;padding:4px;\">");
                        body.AppendLine($@"<img src=""cid:{cid}"" style=""width:120px;height:120px;object-fit:contain;object-position:center;"" />");
                        body.AppendLine($"</td>");
                    }

                    body.AppendLine($"<td style=\"padding:4px;\">");
                    body.AppendLine($"<table>");
                    body.AppendLine($"<tr><td>Name:</td><td>{item.ProductName}</td></tr>");

                    if (item.ColorName != null)
                    {
                        body.AppendLine($"<tr><td>Color: </td><td>{item.ColorName}</td></tr>");
                    }
                    if (item.SizeName != null)
                    {
                        body.AppendLine($"<tr><td>Size: </td><td>{item.SizeName}</td></tr>");
                    }
                    if(item.GenderName != null)
                    {
                        body.AppendLine($"<tr><td>Gender: </td><td>{item.GenderName}</td></tr>");
                    }

                    // quantity
                    body.AppendLine($"<tr><td>Quantity: </td><td>{item.Quantity}</td></tr>");

                    body.AppendLine($"</table>");
                    body.AppendLine("</td>");
                    body.AppendLine("</tr>");
                }
            }

            body.AppendLine("</tbody>");
            body.AppendLine("</table>");

            body.AppendLine("<h4></h4>");
            body.AppendLine($"<a href=\"{hyperlink}\" target=\"_blank\">{hyperlink}</a>");
            body.AppendLine("</div>");
            body.AppendLine("</div>");

            if (model.ProductOrder.Email != null)
            {
                string orderName = model.ProductOrder.Recipient.Replace(" ", "-").ToLower();
                string orderFileName = $"clearstore-order-{orderName}-{model.ProductOrder.ProductOrderId}.pdf";
                var pdfBytes = await CreatePdfAsync(model);

                attachments.Add(new FileAttachment
                {
                    OdataType = "#microsoft.graph.fileAttachment",
                    Name = orderFileName,
                    ContentType = "application/pdf",
                    ContentBytes = pdfBytes
                });

                var recipients = await GetGroupEmailRecipientsAsync(PermissionGroup.AppWebStoreOrderRecipients);

                var messageBody = new SendMailPostRequestBody
                {
                    Message = new Message
                    {
                        Subject = subject,
                        Body = new ItemBody
                        {
                            ContentType = BodyType.Html,
                            Content = body.ToString()
                        },
                        Importance = Importance.Normal,
                        ToRecipients = recipients,
                        CcRecipients = new List<Recipient>()
                        {
                            new Recipient
                            {
                                EmailAddress = new EmailAddress
                                {
                                    Address = model.ProductOrder.Email.Trim()
                                }
                            }
                        },
                        Attachments = attachments
                    },
                    SaveToSentItems = true
                };

                await _client.Me.SendMail.PostAsync(messageBody);
            }
        }


        private async Task<byte[]> CreatePdfAsync(ProductOrderModel model)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            byte[]? byteArray = null;

            //var orderItems = await _context.ProductItems
            //            .Where(pi => pi.ProductCartId == order.ProductCartId)
            //            .Select(pi => new ProductOrderItemDto
            //            {
            //                IsApparel = pi.Product != null ? (bool?)pi.Product.IsApparel : null,
            //                ProductCartId = pi.ProductCartId,
            //                ProductItemId = pi.ProductItemId,
            //                ProductId = pi.ProductId,
            //                ProductName = pi.Product != null ? pi.Product.Name : null,
            //                ProductSizeId = pi.ProductSizeId,
            //                SizeName = pi.ProductSize != null ? pi.ProductSize.Name : null,
            //                ProductColorId = pi.ProductColorId,
            //                ColorName = pi.ProductColor != null ? pi.ProductColor.Name : null,
            //                ProductGenderId = pi.ProductGenderId,
            //                GenderName = pi.Product != null && pi.Product.ProductGender != null ? pi.Product.ProductGender.Name : null,
            //                Quantity = pi.Quantity,
            //                ProductInventoryId = pi.ProductInventoryId,
            //                ImageDetail = _context.ProductImages
            //                    .Where(d => d.ProductId == pi.ProductId)
            //                    .Select(i => new ImageDetail
            //                    {
            //                        ImageId = i.Id,
            //                        ImageData = i.ImageData,
            //                        ImageName = i.ImageName ?? string.Empty
            //                    })
            //                    .FirstOrDefault()
            //            })
            //            .ToListAsync();

            var order = model.ProductOrder;
            var orderItems = model.OrderItems;

            string orderUrl = $"https://clearstore.azurewebsites.net/product-orders/details/{order.ProductOrderId}";
            string orderName = order.Recipient.Replace(" ", "-").ToLower();
            string orderFileName = $"clearstore-order-{orderName}-{order.ProductOrderId}.pdf";

            using var interRegular = System.IO.File.OpenRead("wwwroot/fonts/Inter-Regular.ttf");
            FontManager.RegisterFontWithCustomName("Inter-Regular", interRegular);

            using var interBold = System.IO.File.OpenRead("wwwroot/fonts/Inter-Bold.ttf");
            FontManager.RegisterFontWithCustomName("Inter-Bold", interBold);

            var logo = SvgImage.FromFile("wwwroot/img/cp-logo-grouped.svg");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // defaults
                    page.Size(PageSizes.A4.Portrait());
                    page.Margin((float)0.5, Unit.Inch);
                    page.DefaultTextStyle(t => t.FontSize(9).FontFamily("Inter-Regular"));

                    // header
                    page.Header()
                        .ShowOnce()
                        .Row(row =>
                        {
                            // clearplan logo and address
                            row.ConstantItem(150).Column(col =>
                            {
                                col.Spacing(2);
                                col.Item().Svg(logo).FitArea();
                                col.Item().PaddingTop(10);
                                col.Item().Text("2000 West Park Dr., Ste. 200");
                                col.Item().Text("Westborough, MA 01581");
                            });

                            // spacer
                            row.RelativeItem();

                            // flex space
                            row.RelativeItem().Column(col =>
                            {
                                col.Spacing(2);
                                col.Item().Element(c => QuestExtensions.HeadingStyle(c, "Inter-Bold")).Text($"Order # {order.ProductOrderId}").AlignRight();
                                col.Item().Text($"Order date {order.CreatedDate?.ToShortDateString()}").AlignRight();
                            });
                        });

                    // content
                    page.Content()
                        .PaddingVertical(20)
                        .Column(col =>
                        {
                            col.Spacing(1);

                            col.Item()
                                .PaddingBottom(10)
                                .Text($"Shipping details: ")
                                .FontFamily("Inter-Bold")
                                .FontSize(12)
                                .SemiBold();

                            col.Item().Text(order.Recipient);
                            col.Item().Text(order.Address);
                            col.Item().Text($"{order.City.Trim()}, {order.State.Trim()} {order.ZipCode}");

                            if (order.PhoneNumber != null)
                            {
                                string phone = GeneralExtensions.ToPhoneNumber(order.PhoneNumber.Value.ToString());
                                col.Item().Text($"{phone}");
                            }

                            if (!string.IsNullOrEmpty(order.Notes))
                            {
                                col.Item()
                                    .PaddingTop(10)
                                    .Text(text =>
                                    {
                                        text.Span("Notes: ").FontFamily("Inter-Bold");
                                        text.Span($"{order.Notes}");
                                    });
                            }

                            col.Item().PaddingVertical(20).Table(table =>
                            {
                                table.ColumnsDefinition(def =>
                                {
                                    def.ConstantColumn(150);
                                    def.RelativeColumn();
                                });

                                if (orderItems.Any())
                                {
                                    foreach (var item in orderItems)
                                    {
                                        string quantity = item.Quantity != null ? item.Quantity.Value.ToString() : "N/A";

                                        table.Cell().Element(cell =>
                                        {
                                            if (item.ImageDetail != null && item.ImageDetail.ImageData != null)
                                            {
                                                cell.AlignCenter()
                                                    .Height(150)
                                                    .Padding(10)
                                                    .Image(item.ImageDetail.ImageData)
                                                    .FitArea();
                                            }
                                            else
                                            {
                                                Color[] colors = { Colors.Grey.Lighten2, Colors.White };
                                                cell.BackgroundLinearGradient(45, colors);
                                            }
                                        });


                                        table.Cell()
                                            .ShowEntire()
                                            .PaddingVertical(10)
                                            .PaddingHorizontal(20)
                                            .Text(t =>
                                            {
                                                t.ParagraphSpacing(4);
                                                t.Line($"{item.ProductName}")
                                                    .FontFamily("Inter-Bold")
                                                    .FontSize(12)
                                                    .SemiBold();

                                                t.Line($"Color: {item.ColorName ?? "N/A"}");
                                                t.Line($"Size: {item.SizeName ?? "N/A"}");
                                                t.Line($"Gender: {item.GenderName ?? "N/A"}");
                                                t.Line($"Quantity: {quantity}");
                                            });
                                    }
                                }
                            });
                        });

                    // footer
                    page.Footer()
                        .Padding(10)
                        .Column(col =>
                        {
                            col.Item().Text(t =>
                            {
                                t.Span("To view the web version, please visit ");
                                t.Hyperlink(orderUrl, orderUrl)
                                    .FontColor(Colors.Blue.Medium)
                                    .Underline();
                            });
                        });
                });
            });

            byteArray = document.GeneratePdf();

            return byteArray;

            //StringBuilder body = new StringBuilder();
            //body.AppendLine($"<p>A new ClearStore order has been placed for <b>{order.Recipient}</b>. Please see the attached invoice with the order details.</p>");
            //body.AppendLine($"<p><a href=\"{orderUrl}\">{orderUrl}</a></p>");

            //var email = new Microsoft.Graph.Me.SendMail.SendMailPostRequestBody
            //{
            //    Message = new Message
            //    {
            //        Subject = $"New ClearStore order for {order.Recipient}",
            //        Body = new ItemBody
            //        {
            //            ContentType = BodyType.Html,
            //            Content = body.ToString()
            //        },
            //        ToRecipients = new List<Recipient>()
            //        {
            //            new Recipient 
            //            { 
            //                EmailAddress = new EmailAddress 
            //                { 
            //                    Address = "markhughes@clearplanconsulting.com" 
            //                } 
            //            }
            //        },
            //        Attachments = new List<Attachment>
            //        {
            //            new FileAttachment
            //            {
            //                OdataType = "#microsoft.graph.fileAttachment",
            //                Name = orderFileName,
            //                ContentType = "application/pdf",
            //                ContentBytes = byteArray
            //            }
            //        },
            //        Importance = Importance.Normal
            //    },
            //    SaveToSentItems = true
            //};

            //await _client.Me.SendMail.PostAsync(email);
        }


        private async Task<List<Recipient>> GetGroupEmailRecipientsAsync(string groupId)
        {
            var recipients = new List<Recipient>();

            var members = await _client.Groups[groupId].Members.GetAsync(config =>
            {
                config.QueryParameters.Select = ["id", "displayName", "mail", "userPrincipalName"];
            });

            while (members?.Value != null)
            {
                foreach (var member in members.Value)
                {
                    if (member is User user)
                    {
                        var address = !string.IsNullOrWhiteSpace(user.Mail)
                            ? user.Mail
                            : user.UserPrincipalName;

                        if (!string.IsNullOrWhiteSpace(address))
                        {
                            recipients.Add(new Recipient
                            {
                                EmailAddress = new EmailAddress
                                {
                                    Address = address
                                }
                            });
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(members.OdataNextLink))
                {
                    break;
                }

                members = await _client.Groups[groupId].Members
                    .WithUrl(members.OdataNextLink)
                    .GetAsync();
            }

            return recipients;
        }
    }
}
