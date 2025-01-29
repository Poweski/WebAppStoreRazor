﻿using System.ComponentModel.DataAnnotations;

namespace Lab12.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
