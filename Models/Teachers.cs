using System.ComponentModel.DataAnnotations;

namespace CrudProject.Models
{
    public class Teachers
    {
        [Key]
        public int TeacherId { get; set; }

        public string Name { get; set; }

        public DateTime JoiningDate { get; set; }

        public string? Phone {  get; set; }

        public string Email { get; set; }

        public decimal Salary { get; set; }

        public bool IsActive {  get; set; }

        public int CourseId { get; set; }


    }
}
