using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        
        public string Longitude { get; set; }
       
        public string Latitude { get; set; }

    }
}
