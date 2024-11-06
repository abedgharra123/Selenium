using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium_Practice
{

    [TestFixture]
    [Ignore("Ignoring all tests in this class")]
    public class DatabaseTests
    {
        private ApplicationDbContext _dbContext;
        private string _connectionString;
        private DbContextOptions<ApplicationDbContext> _options;

        [SetUp]
        public void SetUp()
        {
            _connectionString = "Data Source=ABEDGH;Initial Catalog=test;Integrated Security=True;TrustServerCertificate=True;";
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;
            _dbContext = new ApplicationDbContext(_options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        [Test]
        public async Task TestCreateTableAndInsertData()
        {
            var newProduct = new Product { Name = "Test Product", Price = 19.99M };
            await _dbContext.Products.AddAsync(newProduct);
            await _dbContext.SaveChangesAsync();

            var product = await _dbContext.Products
                                          .FirstOrDefaultAsync(p => p.Name == "Test Product");

            Assert.NotNull(product, "Product should exist in the database.");
            Assert.IsTrue("Test Product" == product.Name, "Product name does not match.");
            Assert.IsTrue(19.99M == product.Price, "Product price does not match.");
        }

        [Test]
        public async Task TestCheckProductCount()
        {
            var newProduct = new Product { Name = "Test Product", Price = 19.99M };
            await _dbContext.Products.AddAsync(newProduct);
            await _dbContext.SaveChangesAsync();

            var productCount = await _dbContext.Products.CountAsync();
            Assert.GreaterOrEqual(productCount, 1, "There should be at least one product in the database.");
        }

        [Test]
        public async Task TestDeleteData()
        {
            var newProduct = new Product { Name = "Test Product", Price = 19.99M };
            await _dbContext.Products.AddAsync(newProduct);
            await _dbContext.SaveChangesAsync();

            var product = await _dbContext.Products
                                          .FirstOrDefaultAsync(p => p.Name == "Test Product");
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            var deletedProduct = await _dbContext.Products
                                                 .FirstOrDefaultAsync(p => p.Name == "Test Product");
            Assert.IsNull(deletedProduct, "Product should not exist in the database after deletion.");
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}

