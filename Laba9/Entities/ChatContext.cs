using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laba9.Entities
{
    public class ChatContext:DbContext
    {
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<Link> Links { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(@"Server=localhost,1433;Database=Lab9;User=sa;Password=@Passw0rd;");
        }

        public ChatContext()
        {
            Database.EnsureCreated();
        }
    }
}
