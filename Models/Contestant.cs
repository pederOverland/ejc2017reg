using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ecreg.Models
{
    public class Contestant
    {
        public int ContestantId { get; set; }
        [NotMapped]
        public IFormFile Picture { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string PassportNumber { get; set; }
        public string Role { get; set; }
        public string Nation { get; set; }
        public DateTime Modified { get; set; }
    }
}