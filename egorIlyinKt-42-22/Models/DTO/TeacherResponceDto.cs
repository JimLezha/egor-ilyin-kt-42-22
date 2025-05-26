namespace egorIlyinKT_42_22.Models.DTO
{
    public class TeacherResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DegreeId { get; set; }
        public string Degree { get; set; }
        public int PositionId { get; set; }
        public string Position { get; set; }
        public int? DepartmentId { get; set; }
        public string Department { get; set; }
        public List<Load> Loads { get; set; } 
    }
}
