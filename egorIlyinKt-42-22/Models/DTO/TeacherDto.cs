using System.ComponentModel.DataAnnotations;

namespace egorIlyinKT_42_22.Models.DTO
{
    public class TeacherDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PositionId { get; set; }
        public int DegreeId { get; set; }
        public int? DepartmentId { get; set; }
    }
}
