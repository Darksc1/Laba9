using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using Castle.Core.Internal;
using System.Web;
using Laba9.Entities;
using Laba9.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Laba9.Controllers
{
    public class HomeController : Controller
    {
        public ChatContext context = new ChatContext();
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            LoginModel empty = new LoginModel();
            return View(empty);
        }
        [HttpPost]
        public async Task<IActionResult> Register(string login, string pswd)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                LoginModel model = new LoginModel()
                {
                    Password = pswd,
                    ErrorMessage = "Имя пользователя не указано!"
                };
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(pswd))
            {
                LoginModel model = new LoginModel()
                {
                    Login = login,
                    ErrorMessage = "Пароль не корректный!"
                };
                return View(model);
            }
            UserInfo usr = context.Users.FirstOrDefault(u => u.Username == login);
            if (usr != null)
            {
                LoginModel model = new LoginModel()
                {
                    Password = pswd,
                    ErrorMessage = "Данное имя пользователя уже используется"
                };
                return View(model);
            }
            else
            {
                UserInfo user = new UserInfo()
                {
                    Username = login,
                    PasswordHash = Crypto.HashPassword(pswd)
                };
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return Redirect("/Home/Index");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginModel empty = new LoginModel();
            return View(empty);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
                };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }


        [HttpPost]
        public async Task<IActionResult> Login(string login, string pswd)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                LoginModel model = new LoginModel()
                {
                    Password = pswd,
                    ErrorMessage = "Имя пользователя не указано!"
                };
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(pswd))
            {
                LoginModel model = new LoginModel()
                {
                    Login = login,
                    ErrorMessage = "Пароль не корректный!"
                };
                return View(model);
            }
            UserInfo usr = context.Users.FirstOrDefault(u => u.Username == login);
            if (usr == null)
            {
                LoginModel model = new LoginModel()
                {
                    ErrorMessage = "Пользователя не существует!"
                };
                return View(model);
            }
            else if (!Crypto.VerifyHashedPassword(usr.PasswordHash,pswd))
            {
                LoginModel model = new LoginModel()
                {
                    ErrorMessage = "Неверный пароль!"
                };
                return View(model);
            }
            else
            {
                await Authenticate(login);
                return Redirect("/Home/UserPage");
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Home/Index");
        }
        [Authorize]
        public async Task<IActionResult> UserPage()
        {
            //IQueryable<Message> model = context.Messages;
            //foreach (Message msg in model)
            //    await context.Entry(msg).Reference(m => m.Sender).LoadAsync();
            return View(context.Links.ToList());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserPage(string link, string info, string sourceLink, string targetLink, string buttonType)
        {
            
            UserInfo user = context.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (buttonType == "Добавить")
            {
                IQueryable<Link> findLink = from contextLink in context.Links
                    where (user.Username == contextLink.Sender.Username) &&
                          (contextLink.Text == link)
                    select contextLink;
                if (!findLink.Any() && !link.IsNullOrEmpty())
                {
                    Link msg = new Link()
                    {
                        Text = link,
                        Sender = user,
                        Info = info
                        
                    };
                await context.Links.AddAsync(msg);
                await context.SaveChangesAsync();
                }
                else
                {
                    ViewBag.message = "<script>alert('Введите корректную и неповторяющуюся ссылку')</script>";
                }
                
            }
            if (buttonType == "Удалить")
            {
                IQueryable<Link> findLink = from contextLink in context.Links
                    where (user.Username == contextLink.Sender.Username) && 
                          (contextLink.Text == link)
                    select contextLink;

                if (findLink.Any())
                {
                    try
                    {
                        foreach (var line in findLink)
                        {
                            context.Remove(line);
                        }
                    }
                    catch (Exception e)
                    {
                        ViewBag.message = "<script>alert('Ошибка при удалении')</script>";
                        Console.WriteLine("Ошибка при удалении");
                        throw;
                    }
                }
                await context.SaveChangesAsync();
            }
            if (buttonType == "Изменить")
            {
                IQueryable<Link> findLink = from contextLink in context.Links
                    where (user.Username == contextLink.Sender.Username) && 
                          (contextLink.Text == sourceLink)
                    select contextLink;
                IQueryable<Link> repeatLink = from contextLink in context.Links
                    where (user.Username == contextLink.Sender.Username) && 
                          (contextLink.Text == targetLink)
                    select contextLink;
                if (findLink.Any() && !repeatLink.Any())
                {
                    try
                    {
                        foreach (var line in findLink)
                        {
                            line.Text = targetLink;
                        }
                    }
                    catch (Exception e)
                    {
                        ViewBag.message = "<script>alert('Ошибка при изменении')</script>";
                        Console.WriteLine("Ошибка при изменении");
                        throw;
                    }
                }
                else
                {
                    ViewBag.message = "<script>alert('Ошибка при изменении')</script>";
                    Console.WriteLine("Ошибка при изменении");
                }
                await context.SaveChangesAsync();
            }

            return View(context.Links.ToList());
            
        }
    }
}
