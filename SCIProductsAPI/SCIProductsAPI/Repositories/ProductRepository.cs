using Microsoft.Data.SqlClient;
using SCIProductsAPI.Data;
using SCIProductsAPI.Models;
using System.Data;


namespace SCIProductsAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ProductRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        #region public
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var products = new List<Product>();

            using (var connection = _connectionFactory.CreateConnection())
            using (var command = new SqlCommand("sp_GetAllProducts", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        products.Add(MapProduct(reader));
                    }
                }
            }

            return products;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            Product? product = null;

            using (var connection = _connectionFactory.CreateConnection())
            using (var command = new SqlCommand("sp_GetProductById", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductId", id);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        product = MapProduct(reader);
                    }
                }
            }

            return product;
        }

        public async Task<Product?> CreateAsync(Product product)
        {
            Product? createdProduct = null;

            using (var connection = _connectionFactory.CreateConnection())
            using (var command = new SqlCommand("sp_CreateProduct", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@Price", product.Price);

                var outputIdParam = new SqlParameter("@ProductId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputIdParam);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        createdProduct = MapProduct(reader);
                    }
                }
            }

            return createdProduct;
        }


        public async Task<Product?> UpdateAsync(int id, Product product)
        {
            Product? updatedProduct = null;

            using (var connection = _connectionFactory.CreateConnection())
            using (var command = new SqlCommand("sp_UpdateProduct", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductId", id);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@Price", product.Price);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        updatedProduct = MapProduct(reader);
                    }
                }
            }

            return updatedProduct;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            bool deleted = false;

            using (var connection = _connectionFactory.CreateConnection())
            using (var command = new SqlCommand("sp_DeleteProduct", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductId", id);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        // Se considera eliminado si IsActive = false
                        deleted = !(bool)reader["IsActive"];
                    }
                }
            }

            return deleted;
        }

        #endregion public

        #region private
        private Product MapProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader["Description"] == DBNull.Value ? null : reader.GetString(reader.GetOrdinal("Description")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
        #endregion private

    }
}
