using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient; // ✅ Nuevo paquete
using Microsoft.Extensions.Configuration;
using SCIProducts.Domain.Entities;
using SCIProducts.Infrastructure.Repositories.Interfaces;

namespace SCIProducts.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            var products = new List<Product>();

            using (var connection = new SqlConnection(_connectionString))
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


        public async Task<Product?> GetById(int id)
        {
            Product? product = null;

            using (var connection = new SqlConnection(_connectionString))
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

        public async Task<Product?> Add(Product product)
        {
            Product? createdProduct = null;

            using (var connection = new SqlConnection(_connectionString))
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

        public async Task<Product?> Update(int id, Product product)
        {
            Product? updatedProduct = null;

            using (var connection = new SqlConnection(_connectionString))
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

        public async Task<bool> Delete(int id)
        {
            bool deleted = false;

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_DeleteProduct", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductId", id);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        deleted = !(bool)reader["IsActive"];
                    }
                }
            }

            return deleted;
        }

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
