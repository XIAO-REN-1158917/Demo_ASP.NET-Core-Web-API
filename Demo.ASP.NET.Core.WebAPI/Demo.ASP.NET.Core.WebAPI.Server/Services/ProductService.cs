﻿using Demo.ASP.NET.Core.WebAPI.Server.DTOs;
using Demo.ASP.NET.Core.WebAPI.Server.Models;
using Demo.ASP.NET.Core.WebAPI.Server.Repositories;
using Demo.ASP.NET.Core.WebAPI.Server.Validation;

namespace Demo.ASP.NET.Core.WebAPI.Server.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;

        public ProductService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Get all products and convert to DTOs
        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category?.Name
            });
        }

        // Get a product by ID and convert to DTO
        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            // Map entity to response DTO
            return MapToResponseDto(product);
        }

        // Check if product name already exists
        public async Task<bool> IsProductNameTakenAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Product name cannot be null or empty.", nameof(name));
            }

            return await _productRepository.ExistsAsync(p => p.Name == name);
        }

        // Add a new product
        public async Task<ProductResponseDto> AddProductAsync(ProductCreateDto productDto)
        {
            // Validate input
            DtoValidator.ValidateProductCreateDto(productDto);

            // Validate if product name is unique
            if (await IsProductNameTakenAsync(productDto.ProductName))
            {
                throw new InvalidOperationException("Product name already exists.");
            }

            // Find the category
            var category = await GetCategoryOrThrowAsync(productDto.CategoryName);

            // Map DTO to Product entity
            var product = new Product
            {
                Name = productDto.ProductName,
                Category = category
            };

            // Save product to the database
            await _productRepository.AddAsync(product);

            // Map entity to response DTO
            return MapToResponseDto(product);
        }

        
        private async Task<Category> GetCategoryOrThrowAsync(string categoryName)
        {
            var category = await _productRepository.GetCategoryByNameAsync(categoryName);
            if (category == null)
            {
                throw new InvalidOperationException($"Category '{categoryName}' does not exist.");
            }
            return category;
        }

        private ProductResponseDto MapToResponseDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                ProductName = product.Name,
                CategoryName = product.Category.Name 
            };
        }

    }
}
