using System.ComponentModel.DataAnnotations;

namespace egorIlyinKT_42_22.Filters.TeacherFilters
{
    public class TeacherFilter
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Degree { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
    }
}
