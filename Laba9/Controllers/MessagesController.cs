using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Laba9.Entities;
using Laba9.Models;

namespace Laba9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private ChatContext context = new ChatContext();
        [HttpGet]
        public IEnumerable<LinkModel> Get(string username, string password)
        {
            UserInfo sender = context.Users.SingleOrDefault(usr => usr.Username == username);
            if (sender != null && Crypto.VerifyHashedPassword(sender.PasswordHash, password))
            {
                IQueryable<Link> allMesages = from msg in context.Links
                    select msg;
                return allMesages.ToList().ConvertAll(msg => LinkModel.FromEntity(msg));
            }
            else
            {
                return null;
            }
        }
    }
}
