using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laba9.Entities;

namespace Laba9.Models
{
    public class LinkModel
    {
        public string Sender { get; set; }
        public string Info { get; set; }
        public string Text { get; set; }
        
        public static LinkModel FromEntity(Link msg)
        {
            return new LinkModel()
            {
                Sender = msg.Sender.Username,
                Text = msg.Text,
                Info = msg.Info
            };
        }
    }
}
