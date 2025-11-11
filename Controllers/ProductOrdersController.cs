using ClearScore.Extensions;
using ClearStore.Data;
using ClearStore.Models;
using ClearStore.Models.Dto;
using ClearStore.ViewComponents;
using ClearStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;
using Microsoft.Identity.Web;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace ClearStore.Controllers
{
    [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
    [Route("product-orders")]
    public class ProductOrdersController : Controller
    {
        private readonly StoreContext _context;
        private readonly GraphServiceClient _client;

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
                        Quantity = pi.Quantity,
                        ProductInventoryId = pi.ProductInventoryId,
                        Image = _context.ProductImages
                            .Where(d => d.ProductId == pi.ProductId)
                            .Select(i => i.ImageData)
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
            var currentUser = await _client.Me.GetAsync();

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

                    // testing email
                    string to = "heatherlaudani@clearplanconsulting.com";
                    string subject = "New store order";
                    StringBuilder body = new StringBuilder();

                    body.AppendLine("<div style=\"max-width:1366px;margin:0 auto;\">");
                    body.AppendLine("<div style=\"font-family:system-ui;font-size:14px;\">");
                    body.AppendLine("<h3>Shipping details</h3>");
                    body.AppendLine("<br>");

                    // user information
                    body.AppendLine("<table>");
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
                            if (item.Image != null)
                            {
                                body.AppendLine($"<td style=\"width:180px;padding:4px;\">");
                                body.AppendLine($"<img src=\"data:image/jpeg;base64,{Convert.ToBase64String(item.Image)}\" style=\"width:120px;height:120px;object-fit:contain;object-position:center;\" />");
                                body.AppendLine($"</td>");
                            }

                            body.AppendLine($"<td style=\"padding:4px;\">");
                            body.AppendLine($"<dl>");
                            body.AppendLine($"<dt>Name:</dt><dd>{item.ProductName}</dd>");

                            if (item.ColorName != null)
                            {
                                body.AppendLine($"<dt>Color: </dt><dd>{item.ColorName}</dd>");
                            }
                            if (item.SizeName != null)
                            {
                                body.AppendLine($"<dt>Size: </dt><dd>{item.SizeName}</dd>");
                            }
                            // OUTPUT THE GENDER HERE

                            body.AppendLine($"</dl>");
                            body.AppendLine("</td>");
                            body.AppendLine("</tr>");
                        }
                    }

                    body.AppendLine("</tbody>");
                    body.AppendLine("</table>");

                    body.AppendLine("<h4></h4>");
                    body.AppendLine($"<a href=\"https://clearstore.azurewebsites.net/product-orders/details/{model.ProductOrder.ProductOrderId}\" target=\"_blank\">https://clearstore.azurewebsites.net/orders/details/{model.ProductOrder.ProductOrderId}</a>");
                    body.AppendLine("</div>");
                    body.AppendLine("</div>");

                    if (model.ProductOrder.Email != null)
                    {
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
                                ToRecipients = new List<Recipient>()
                                {
                                    new Recipient { EmailAddress = new EmailAddress { Address = to?.Trim() } }
                                },
                                CcRecipients = new List<Recipient>()
                                {
                                    new Recipient{ EmailAddress = new EmailAddress { Address = model.ProductOrder.Email.Trim() } }
                                }
                            },
                            SaveToSentItems = true
                        };

                        await _client.Me.SendMail.PostAsync(messageBody);
                    }

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
                            Image = _context.ProductImages
                                .Where(d => d.ProductId == pi.ProductId)
                                .Select(i => i.ImageData)
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
                            Image = _context.ProductImages
                                .Where(d => d.ProductId == pi.ProductId)
                                .Select(i => i.ImageData)
                                .FirstOrDefault()
                        })
                        .ToListAsync();

            var user = (ClaimsPrincipal)User;
            var emailAddress = user.FindFirst("preferred_username")?.Value;

            model.ProductOrder.Email = emailAddress;
            model.ProductOrderItems = orderItems;

            return View(model);
        }



    }
}
