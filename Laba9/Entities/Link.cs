using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laba9.Entities
{
    public class Link
    {
        public int Id { get; set; }
        public virtual UserInfo Sender { get; set; }
        public string Text { get; set; }
        public string Info { get; set; }
        
    }
}
