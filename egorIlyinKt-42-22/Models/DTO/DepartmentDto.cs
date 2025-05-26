using System.ComponentModel.DataAnnotations;

namespace egorIlyinKT_42_22.Models.DTO
{
    public class CreateDepartmentDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime FoundedDate { get; set; }

        public int? HeadId { get; set; }
    }
}
