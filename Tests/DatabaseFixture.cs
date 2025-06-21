using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
namespace Tests
{
    public class DatabaseFixture : IDisposable
    {
        public ToysShopContext Context { get; private set; }

        public DatabaseFixture()
        {
            
            var options = new DbContextOptionsBuilder<ToysShopContext>()
                .UseSqlServer("Data Source=DESKTOP-E0FAPSB;Initial Catalog=ToysShopTests;Integrated Security=True;Trust Server Certificate=True")
                .Options;
            Context = new ToysShopContext(options);
            Context.Database.EnsureCreated();// create the data base
        }

        public void Dispose()
        {
            // Clean up the test database after all tests are completed
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
