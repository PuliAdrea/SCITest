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
                _logger.LogError(ex, "Error SQL al obtener los productos desde la base de datos.");
                throw new ApplicationException("Error al obtener los productos.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en GetAll.");
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
                _logger.LogError(ex, "Error SQL al obtener el producto con ID {ProductId}.", id);
                throw new ApplicationException($"Error al obtener el producto {id}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en GetById para ID {ProductId}.", id);
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

                _logger.LogInformation("Producto agregado correctamente: {@Product}", product);
                return product;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error SQL al agregar el producto {@Product}.", product);
                throw new ApplicationException("Error al agregar el producto.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en Add para {@Product}.", product);
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

                    _logger.LogInformation("Producto actualizado correctamente: {@Product}", updatedProduct);
                    return updatedProduct;
                }

                _logger.LogWarning("No se encontró el producto con ID {ProductId} para actualizar.", id);
                return null;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error SQL al actualizar el producto {@Product}.", product);
                throw new ApplicationException("Error al actualizar el producto.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en Update para {@Product}.", product);
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
                        _logger.LogInformation("Producto eliminado correctamente con ID {ProductId}.", id);
                        return true;
                    }
                }

                _logger.LogWarning("No se encontró el producto con ID {ProductId} para eliminar.", id);
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error SQL al eliminar el producto con ID {ProductId}.", id);
                throw new ApplicationException($"Error al eliminar el producto {id}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en Delete para ID {ProductId}.", id);
                throw;
            }
        }
    }
}
