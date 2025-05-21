using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace egorIlyinKT_42_22.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime FoundedDate { get; set; }

        public int? HeadId { get; set; }

        [ForeignKey("HeadId")]
        public virtual Teacher? Head { get; set; }

        public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();


        public bool IsValidName()
        {
            return Regex.IsMatch(Name, @"Department", RegexOptions.IgnoreCase) ||
              Regex.IsMatch(Name, @"Кафедра", RegexOptions.IgnoreCase);
        }
    }
}
