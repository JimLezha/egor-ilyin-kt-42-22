using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace egorIlyinKT_42_22.Models
{
    public class Discipline
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        
        public virtual ICollection<Load> Loads { get; set; } = new List<Load>();
    }
}
