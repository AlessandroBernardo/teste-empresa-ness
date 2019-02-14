using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TesteNess.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Column(TypeName = "varchar(50)")]
        [Required(ErrorMessage = "Por favor, insira um nome com até 50 caracteres!")]
        public string Name { get; set; }
        public bool Principal { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }

    }
}
