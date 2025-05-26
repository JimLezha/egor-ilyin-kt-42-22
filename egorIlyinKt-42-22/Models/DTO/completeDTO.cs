namespace egorIlyinKT_42_22.Models.DTO
{
    public class Discipline2Dto
    {
        public string Name { get; set; }
        public int Hours { get; set; }
    }

    public class Teacher2Dto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Degree { get; set; } 
        public string Position { get; set; }
        public List<Discipline2Dto> Disciplines { get; set; }
    }

    public class Department2Dto
    {
        public string Name { get; set; }
        public string HeadName { get; set; }
        public List<Teacher2Dto> Teachers { get; set; }
    }
}
