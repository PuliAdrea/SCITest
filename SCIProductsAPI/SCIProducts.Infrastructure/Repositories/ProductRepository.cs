using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SCIProducts.Domain.Entities;
using SCIProducts.Infrastructure.Repositories.Interfaces;
using System.Data;

namespace SCIProducts.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(IConfiguration configuration, ILogger<ProductRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            var products = new List<Product>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_GetAllProducts", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    products.Add(new Product
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"])
                    });
                }

                return products;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while retrieving products from the database.");
                throw new ApplicationException("Error while retrieving products.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetAll.");
                throw;
            }
        }

        public async Task<Product?> GetById(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_GetProductById", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ProductId", id);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Product
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"])
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while retrieving product with ID {ProductId}.", id);
                throw new ApplicationException($"Error while retrieving product {id}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetById for ID {ProductId}.", id);
                throw;
            }
        }

        public async Task<Product?> Add(Product product)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_AddProduct", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Product added successfully: {@Product}", product);
                return product;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while adding product {@Product}.", product);
                throw new ApplicationException("Error while adding product.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Add for {@Product}.", product);
                throw;
            }
        }

        public async Task<Product?> Update(int id, Product product)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_UpdateProduct", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ProductId", id);
                command.Parameters.AddWithValue("@Name", product.Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);

                var priceParam = command.Parameters.Add("@Price", SqlDbType.Decimal);
                priceParam.Precision = 18;
                priceParam.Scale = 2;
                priceParam.Value = Math.Round(product.Price, 2);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var updatedProduct = new Product
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"])
                    };

                    _logger.LogInformation("Product updated successfully: {@Product}", updatedProduct);
                    return updatedProduct;
                }

                _logger.LogWarning("Product with ID {ProductId} not found for update.", id);
                return null;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while updating product {@Product}.", product);
                throw new ApplicationException("Error while updating product.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Update for {@Product}.", product);
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_DeleteProduct", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ProductId", id);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    bool isActive = Convert.ToBoolean(reader["IsActive"]);
                    if (!isActive)
                    {
                        _logger.LogInformation("Product deleted successfully with ID {ProductId}.", id);
                        return true;
                    }
                }

                _logger.LogWarning("Product with ID {ProductId} not found for deletion.", id);
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while deleting product with ID {ProductId}.", id);
                throw new ApplicationException($"Error while deleting product {id}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Delete for ID {ProductId}.", id);
                throw;
            }
        }
    }
}
