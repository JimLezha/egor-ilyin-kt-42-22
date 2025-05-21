using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace egorIlyinKT_42_22.Models
{
    public class Load
    {
        [Key]
        public int Id { get; set; }

        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }

        public int DisciplineId { get; set; }

        [ForeignKey("DisciplineId")]
        public virtual Discipline Discipline { get; set; }

        public int Hours { get; set; } 
    }
}
