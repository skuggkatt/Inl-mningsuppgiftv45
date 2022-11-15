using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämningsuppgiftv45
{
    public class Course
    {
        public int CourseId { get; set; }

        public string CourseName { get; set; } = "";

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Course()
        {

        }

        public Course(string courseName, DateTime startDate, DateTime endDate)
        {
            CourseName = courseName;
            StartDate = startDate;
            EndDate = endDate;
        }

        public virtual List<Student>? Students { get; set; } = new();
    }
}
