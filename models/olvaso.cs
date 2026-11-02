using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KonyvtarBackend.Models
{
    public class Olvaso
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nev { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [EmailAddress] 
        public string Email { get; set; } = string.Empty;

        
        public List<Konyv> Konyvek { get; set; } = new List<Konyv>();
    }
}
