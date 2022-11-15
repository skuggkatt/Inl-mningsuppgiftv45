using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämningsuppgiftv45
{
    public class Student
    {
        public int StudentId { get; set; }

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string City { get; set; } = "";

        public virtual List<Course>? Courses { get; set; } = new();

        public Student()
        {

        }
        public Student(string firstname, string lastname, string city)
        {
            FirstName = firstname;
            LastName = lastname;
            City = city;
        }

        public override string? ToString()
        {
            return $"{StudentId}. {FirstName} {LastName} from {City}.";
        }
    }
}
