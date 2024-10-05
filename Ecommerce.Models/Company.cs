using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
       public string? Address { get; set; }
        [ValidateNever]
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PhoneNo { get; set; }
        public string? PostalCode { get; set; }

    }
}
