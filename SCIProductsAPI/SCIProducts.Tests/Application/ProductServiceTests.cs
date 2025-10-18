using FluentAssertions;
using Moq;
using SCIProducts.Application.Services;
using SCIProducts.Domain.Entities;
using SCIProducts.Infrastructure.Repositories.Interfaces;
using Xunit;

namespace SCIProducts.Tests.Application
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _service = new ProductService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { Id = 1, Name = "Laptop", Description = "HP", Price = 1000 },
                new() { Id = 2, Name = "Mouse", Description = "Logitech", Price = 50 }
            };
            _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(products);

            // Act
            var result = await _service.GetAll();

            // Assert
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Laptop");
        }

        [Fact]
        public async Task GetById_ShouldReturnProduct_WhenExists()
        {
            var product = new Product { Id = 1, Name = "Keyboard", Price = 80 };
            _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(product);

            var result = await _service.GetById(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Keyboard");
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetById(99)).ReturnsAsync((Product?)null);

            var result = await _service.GetById(99);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Create_ShouldCallRepositoryAndReturnProduct()
        {
            var product = new Product { Name = "Monitor", Price = 200 };
            _mockRepo.Setup(r => r.Add(It.IsAny<Product>())).ReturnsAsync(product);

            var result = await _service.Add(product);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Monitor");
            _mockRepo.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnUpdatedProduct_WhenSuccessful()
        {
            var updated = new Product { Id = 1, Name = "Monitor 4K", Price = 300 };
            _mockRepo.Setup(r => r.Update(1, updated)).ReturnsAsync(updated);

            var result = await _service.Update(1, updated);

            result.Should().NotBeNull();
            result!.Price.Should().Be(300);
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenSuccessful()
        {
            _mockRepo.Setup(r => r.Delete(1)).ReturnsAsync(true);

            var result = await _service.Delete(1);

            result.Should().BeTrue();
        }
    }
}
