﻿using System.ComponentModel.DataAnnotations;

namespace Demo.ASP.NET.Core.WebAPI.Server.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
