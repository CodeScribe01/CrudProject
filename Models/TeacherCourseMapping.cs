using System.ComponentModel.DataAnnotations;

namespace CrudProject.Models
{
    public class TeacherCourseMapping
    {

        [Key]
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int CourseId { get; set; }
        public bool IsActive { get; set; }
    }
}
