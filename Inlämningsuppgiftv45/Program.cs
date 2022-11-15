using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Inlämningsuppgiftv45
{
    internal class Program
    {
        class StudentInfo
        {
            private StudentDbContext dbCtx = new();


            public void GenerateStudents() // Metod bara för att inte ha en tom lista första gången jag kör programmet
            {
                if (dbCtx.Students != null)
                {
                    if (!dbCtx.Students.Any())
                    {
                        dbCtx.Add(new Student("Kenta", "Jonsson", "Umeå"));
                        dbCtx.Add(new Student("Örjan", "Bengtsforst", "Övik"));
                        dbCtx.Add(new Student("Ingemar", "Andersson", "Stockholm"));

                        dbCtx.SaveChanges();
                    }
                }

            }

            public void MainMenu()
            {
                while (true)
                {

                    string[] menuOptions = { "[1] Add Student", "[2] Edit Student", "[3] Print Student Info", "[4] Course Menu", "\n[9] Exit Application" };
                    int menuSelection;

                    Console.Clear();
                    Console.WriteLine("- Student Registration Application -");
                    Console.WriteLine("-- Press number of desired option --\n");

                    for (int i = 0; i < menuOptions.Length; i++)
                    {
                        Console.WriteLine(menuOptions[i]);
                    }

                    try
                    {
                        menuSelection = int.Parse(Console.ReadKey(true).KeyChar.ToString());
                    }

                    catch (FormatException)
                    {
                        continue;
                    }

                    switch (menuSelection)
                    {
                        case 1:
                            AddStudent();
                            break;
                        case 2:
                            EditStudent();
                            break;
                        case 3:
                            PrintStudent();
                            break;
                        case 4:
                            CourseMenu();
                            break;
                        case 9:
                            Environment.Exit(0);
                            break;
                    }

                }
            }

            public void AddStudent()
            {
                Console.Clear();
                Console.WriteLine("-Add new student-\n");
                Console.WriteLine("");

                Console.WriteLine("Enter the Firstname of new student:");
                string? firstname = Console.ReadLine();
                Console.WriteLine("Enter the Lastname of new student:");
                string? lastname = Console.ReadLine();
                Console.WriteLine("Enter the City of new student:");
                string? city = Console.ReadLine();
                if (firstname != null && lastname != null && city != null)
                {
                    dbCtx.Add(new Student(firstname, lastname, city));
                    dbCtx.SaveChanges();
                    Console.WriteLine($"{firstname} {lastname} from {city} succesfully added.");
                    Console.WriteLine("Press Enter to return to menu");
                    Console.ReadKey();
                    MainMenu();
                }
                else
                {
                    Console.WriteLine("Input was invalid, press ENTER to try again.");
                    Console.ReadKey();
                    AddStudent();
                }


            }

            public void EditStudent()
            {
                while (true)
                {
                    int selection;
                    Console.Clear();
                    Console.WriteLine("-- Edit students--\n");
                    Console.WriteLine("\n{0,-5} {1,-10} {2,-15} {3,-10}\n", "ID", "Firstname", "Lastname", "City");
                    if (dbCtx.Students is not null)
                    {
                        foreach (var item in dbCtx.Students)
                        {

                            Console.WriteLine("{0,-5} {1,-10} {2,-15} {3,-10}", item.StudentId, item.FirstName, item.LastName, item.City);
                        }
                        Console.WriteLine("\nInput the ID of the student you want to edit:");
                        bool success = int.TryParse(Console.ReadLine(), out int id);
                        if (success)
                        {
                            var std = dbCtx.Students.Include(c => c.Courses).FirstOrDefault(s => s.StudentId == id);

                            if (std is not null)
                            {
                                int courseId;
                                Console.WriteLine($"Editing StudentId:{id}, what do you want to edit:");
                                Console.WriteLine("[1] Firstname");
                                Console.WriteLine("[2] Lastname");
                                Console.WriteLine("[3] City");
                                Console.WriteLine("[4] Add Course");
                                Console.WriteLine("[5] Remove Course");
                                Console.WriteLine("[6] Delete Student");

                                try
                                {
                                    selection = int.Parse(Console.ReadKey(true).KeyChar.ToString());
                                }

                                catch (FormatException)
                                {
                                    continue;
                                }

                                switch (selection)
                                {
                                    case 1:
                                        Console.WriteLine("Input new Firstname:");
                                        string? firstname = Console.ReadLine();
                                        if (std is not null && firstname is not null)
                                        {
                                            std.FirstName = firstname;
                                            dbCtx.SaveChanges();
                                            Console.WriteLine($"Firstname succesfully changed to {firstname}");
                                            PrintStudent();
                                        }

                                        else
                                        {
                                            Console.WriteLine("Invalid inputs, press ENTER to try again");
                                            Console.ReadKey();
                                            EditStudent();
                                        }

                                        break;

                                    case 2:
                                        Console.WriteLine("Input new Lastname:");
                                        string? lastname = Console.ReadLine();
                                        if (std is not null && lastname is not null)
                                        {
                                            std.LastName = lastname;
                                            dbCtx.SaveChanges();
                                            Console.WriteLine($"Lastname succesfully changed to {lastname}");
                                            PrintStudent();
                                        }

                                        else
                                        {
                                            Console.WriteLine("Invalid inputs, press ENTER to try again");
                                            Console.ReadKey();
                                            EditStudent();
                                        }
                                        break;

                                    case 3:
                                        Console.WriteLine("Input new City:");
                                        string? city = Console.ReadLine();
                                        if (std is not null && city is not null)
                                        {
                                            std.City = city;
                                            dbCtx.SaveChanges();
                                            Console.WriteLine($"City succesfully changed to {city}");
                                            PrintStudent();
                                        }

                                        else
                                        {
                                            Console.WriteLine("Invalid inputs, press ENTER to try again");
                                            Console.ReadKey();
                                            EditStudent();
                                        }
                                        break;

                                    case 4:

                                        if (dbCtx.Courses is not null)
                                        {
                                            string? cId;
                                            Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-10}\n", "ID", "CourseName", "StartDate", "EndDate");
                                            foreach (var crs in dbCtx.Courses)
                                            {
                                                if (crs.Students is not null)
                                                    if (!crs.Students.Contains(std))
                                                    {

                                                        Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-10}", crs.CourseId, crs.CourseName,
                                                                      crs.StartDate.ToShortDateString(), crs.EndDate.ToShortDateString());
                                                    }
                                            }

                                            Console.WriteLine("\nInput the ID of the course you want to add student to:");


                                            try
                                            {

                                                courseId = int.TryParse(Console.ReadLine());
                                            }

                                            catch
                                            {
                                                Console.WriteLine("Invalid input, Press ENTER to try again");
                                                Console.ReadKey();
                                                break;
                                            }

                                            var course = dbCtx.Courses.Where(c => c.CourseId == courseId).FirstOrDefault();
                                            if (course is not null && std.Courses is not null)
                                            {
                                                if (!std.Courses.Contains(course))
                                                {
                                                    std.Courses.Add(course);
                                                    dbCtx.SaveChanges();
                                                    Console.WriteLine($"Student id:{id} succesfully added to Courseid:{courseId}");
                                                    Console.WriteLine("Press ENTER to continue");
                                                    Console.ReadKey();
                                                    MainMenu();
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Invalid input, Press ENTER to try again");
                                                    Console.ReadKey();
                                                }
                                            }
                                        }
                                        break;
                                    case 5:
                                        Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-10}\n", "ID", "CourseName", "StartDate", "EndDate");

                                        if (std.Courses is not null)
                                        {
                                            foreach (var crs in std.Courses)
                                            {

                                                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-10}", crs.CourseId, crs.CourseName,
                                                              crs.StartDate.ToShortDateString(), crs.EndDate.ToShortDateString());

                                            }
                                        }

                                        Console.WriteLine("Input ID of the Course you want to remove the student from:");

                                        try
                                        {
                                            courseId = int.Parse(Console.ReadLine());
                                        }

                                        catch
                                        {
                                            Console.WriteLine("Invalid input, Press ENTER to try again");
                                            Console.ReadKey();
                                            break;
                                        }

                                        if (dbCtx.Courses is not null)
                                        {
                                            var course2 = dbCtx.Courses.First(c => c.CourseId == courseId);

                                            if (std.Courses is not null && course2 is not null)
                                            {
                                                if (std.Courses.Contains(course2))
                                                {
                                                    std.Courses.Remove(course2);
                                                    dbCtx.SaveChanges();
                                                    Console.WriteLine($"Course ID{courseId} successfully removed from Student ID{id}");
                                                    Console.WriteLine("Press ENTER to continue");
                                                    Console.ReadKey();
                                                    MainMenu();

                                                }
                                                else
                                                {
                                                    Console.WriteLine("Invalid input, Press ENTER to try again");
                                                    Console.ReadKey();
                                                }
                                            }
                                        }

                                        break;
                                    case 6:
                                        Console.WriteLine("Are you sure you want to delete student?");
                                        Console.WriteLine("Press Y to confirm, Else press ENTER.");
                                        var keyPressed = Console.ReadKey();
                                        if (keyPressed.Key == ConsoleKey.Y)
                                        {
                                            dbCtx.Students.Remove(std);
                                            dbCtx.SaveChanges();
                                            Console.WriteLine($"\nStudent ID {id} successfully removed");
                                            Console.WriteLine("Press ENTER to continue");
                                            Console.ReadKey();
                                            PrintStudent();
                                        }
                                        break;
                                }
                            }

                            else
                            {
                                Console.WriteLine("Invalid ID Press enter to try again");
                                Console.ReadKey();
                                EditStudent();
                            }
                        }

                        else
                        {
                            Console.WriteLine("Invalid unput, press ENTER to try again");
                            Console.ReadKey();
                            EditStudent();
                        }
                    }
                }
            }

            public void PrintStudent()
            {
                while (true)
                {
                    int selection;
                    Console.Clear();
                    Console.WriteLine("-- List of students --\n");
                    Console.WriteLine("\n{0,-5} {1,-10} {2,-15} {3,-10}\n", "ID", "Firstname", "Lastname", "City");
                    if (dbCtx.Students is not null)
                    {
                        foreach (var item in dbCtx.Students)
                        {
                            Console.WriteLine("{0,-5} {1,-10} {2,-15} {3,-10}", item.StudentId, item.FirstName, item.LastName, item.City);
                        }
                    }
                    Console.WriteLine("\n [1] Add student");
                    Console.WriteLine("\n [2] Edit student");
                    Console.WriteLine("\n [3] Main Menu");

                    try
                    {
                        selection = int.Parse(Console.ReadKey(true).KeyChar.ToString());
                    }

                    catch (FormatException)
                    {
                        continue;
                    }

                    switch (selection)
                    {
                        case 1:
                            AddStudent();
                            break;

                        case 2:
                            EditStudent();
                            break;

                        case 3:
                            MainMenu();
                            break;

                    }
                }
            }
            public void CourseMenu()
            {
                while (true)
                {
                    int selection;
                    Console.Clear();
                    Console.WriteLine("-- List of Courses --\n");
                    Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-10}\n", "ID", "CourseName", "StartDate", "EndDate");

                    if (dbCtx.Courses is not null)
                    {
                        foreach (var course in dbCtx.Courses)
                        {

                            Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-10}", course.CourseId, course.CourseName,
                                course.StartDate.ToShortDateString(), course.EndDate.ToShortDateString());
                        }
                    }

                    Console.WriteLine("[1]Add Course");
                    Console.WriteLine("[2]Edit Course");
                    Console.WriteLine("[3]View Students");

                    Console.WriteLine("\n[4]Main Menu");

                    try
                    {
                        selection = int.Parse(Console.ReadKey(true).KeyChar.ToString());
                    }

                    catch (FormatException)
                    {
                        continue;
                    }

                    switch (selection)
                    {
                        case 1:
                            var cultureInfo = new CultureInfo("sv-SE");
                            Console.WriteLine("Enter the name of the Course:");
                            string? courseName = Console.ReadLine();
                            Console.WriteLine("Enter the Course starting date(YYYY/MM/DD):");
                            string? startDate = Console.ReadLine();
                            DateTime parsedStartDate = new DateTime();

                            if (startDate is not null)
                            {
                                try
                                {
                                    parsedStartDate = DateTime.Parse(startDate, cultureInfo);

                                }
                                catch (FormatException)
                                {
                                    Console.WriteLine("Invalid Input, input date in this form(YYYY/MM/DD) inlcuding slashes");
                                    Console.WriteLine("Press ENTER, to try again");
                                    Console.ReadKey();
                                    CourseMenu();
                                }
                            }


                            Console.WriteLine("Enter the Course ending date(YYYY/MM/DD):");
                            string? endDate = Console.ReadLine();
                            DateTime parsedEndDate = new DateTime();

                            if (endDate is not null)
                            {
                                try
                                {
                                    parsedEndDate = DateTime.Parse(endDate, cultureInfo);
                                }
                                catch (FormatException)
                                {
                                    Console.WriteLine("Invalid Input, input date in this form(YYYY/MM/DD) inlcuding slashes");
                                    Console.WriteLine("Press ENTER, to try again");
                                    Console.ReadKey();
                                    CourseMenu();
                                }
                            }

                            if (courseName is not null)
                            {
                                dbCtx.Add(new Course(courseName, parsedStartDate, parsedEndDate));
                                dbCtx.SaveChanges();
                                Console.WriteLine($"{courseName} [{parsedStartDate.ToShortDateString()}]-[{parsedEndDate.ToShortDateString()}] succesfully added.");
                                Console.WriteLine("Press Enter to return to menu");
                                Console.ReadKey();
                                CourseMenu();
                            }

                            break;
                        case 2:
                            while (true)
                            {
                                Console.WriteLine("Enter the ID of the course you want to edit");
                                bool success = int.TryParse(Console.ReadLine(), out int id);
                                if (success)
                                {
                                    int select;
                                    var cCultureInfo = new CultureInfo("sv-SE");
                                    if (dbCtx.Courses is not null)
                                    {
                                        var crs = dbCtx.Courses.Include(c => c.Students).Where(s => s.CourseId == id).FirstOrDefault();
                                        if (crs is not null)
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Editing course, what do you want to edit?\n");
                                            Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-10}", crs.CourseId, crs.CourseName,
                                                                               crs.StartDate.ToShortDateString(), crs.EndDate.ToShortDateString());

                                            Console.WriteLine("[1]Edit Name");
                                            Console.WriteLine("[2]Edit StartDate ");
                                            Console.WriteLine("[3]Edit EndDate");
                                            Console.WriteLine("[4]View students");
                                            Console.WriteLine("[5]Delete Course");


                                            try
                                            {
                                                select = int.Parse(Console.ReadKey(true).KeyChar.ToString());
                                            }

                                            catch (FormatException)
                                            {
                                                continue;
                                            }

                                            switch (select)
                                            {
                                                case 1:
                                                    Console.WriteLine("Enter new name for course:");
                                                    string? name = Console.ReadLine();
                                                    if (name is not null)
                                                    {
                                                        crs.CourseName = name;
                                                        dbCtx.SaveChanges();
                                                        Console.WriteLine($"Coursname succesfully changed to {name}");
                                                        Console.WriteLine("Press Enter, to return to menu");
                                                        Console.ReadKey();
                                                        CourseMenu();
                                                    }
                                                    break;
                                                case 2:
                                                    Console.WriteLine("Enter new StartDate for course:");
                                                    string? cStartDate = Console.ReadLine();
                                                    DateTime parsedCStartDate = new DateTime();
                                                    if (cStartDate is not null)
                                                    {
                                                        try
                                                        {
                                                            parsedCStartDate = DateTime.Parse(cStartDate, cCultureInfo);
                                                        }
                                                        catch (FormatException)
                                                        {
                                                            Console.WriteLine("Invalid Input, input date in this form(YYYY/MM/DD) inlcuding slashes");
                                                            Console.WriteLine("Press ENTER, to try again");
                                                            Console.ReadKey();
                                                            break;
                                                        }
                                                        crs.StartDate = parsedCStartDate;
                                                        dbCtx.SaveChanges();
                                                        Console.WriteLine($"Course StartDate succesfully changed to {parsedCStartDate.ToShortDateString()}");
                                                        Console.WriteLine("Press Enter, to return to menu");
                                                        Console.ReadKey();
                                                        CourseMenu();
                                                    }

                                                    break;
                                                case 3:
                                                    Console.WriteLine("Enter new EndDate for course:");
                                                    string? cEndDate = Console.ReadLine();
                                                    if (cEndDate is not null)
                                                    {
                                                        DateTime parsedCEndDate = new DateTime();
                                                        try
                                                        {
                                                            parsedCStartDate = DateTime.Parse(cEndDate, cCultureInfo);
                                                        }
                                                        catch (FormatException)
                                                        {
                                                            Console.WriteLine("Invalid Input, input date in this form(YYYY/MM/DD) inlcuding slashes");
                                                            Console.WriteLine("Press ENTER, to try again");
                                                            Console.ReadKey();
                                                            break;
                                                        }
                                                        crs.EndDate = parsedCEndDate;
                                                        dbCtx.SaveChanges();
                                                        Console.WriteLine($"Course EndDate succesfully changed to {parsedCEndDate.ToShortDateString()}");
                                                        Console.WriteLine("Press Enter, to return to menu");
                                                        Console.ReadKey();
                                                        CourseMenu();
                                                    }

                                                    break;
                                                case 4:
                                                    while (true)
                                                    {
                                                        Console.Clear();
                                                        Console.WriteLine("--Course Students--");

                                                        Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-10}", crs.CourseId, crs.CourseName,
                                                                                 crs.StartDate.ToShortDateString(), crs.EndDate.ToShortDateString());

                                                        Console.WriteLine("\n{0,-5} {1,-10} {2,-15} {3,-10}\n", "ID", "Firstname", "Lastname", "City");
                                                        if (crs.Students is not null)
                                                        {
                                                            foreach (var student in crs.Students)
                                                            {

                                                                Console.WriteLine("{0,-5} {1,-10} {2,-15} {3,-10}", student.StudentId, student.FirstName, student.LastName, student.City);
                                                            }
                                                        }

                                                        Console.WriteLine("[1]Add student");
                                                        Console.WriteLine("[2]Remove student");
                                                        Console.WriteLine("[3]Course menu");
                                                        try
                                                        {
                                                            selection = int.Parse(Console.ReadKey(true).KeyChar.ToString());
                                                        }

                                                        catch (FormatException)
                                                        {
                                                            continue;
                                                        }

                                                        switch (selection)
                                                        {
                                                            case 1:
                                                                Console.Clear();
                                                                int studentId;
                                                                Console.WriteLine("\n{0,-5} {1,-10} {2,-15} {3,-10}\n", "ID", "Firstname", "Lastname", "City");
                                                                if (dbCtx.Students is not null && crs.Students is not null)
                                                                {
                                                                    foreach (var student in dbCtx.Students)
                                                                    {
                                                                        if (!crs.Students.Contains(student))
                                                                            Console.WriteLine("{0,-5} {1,-10} {2,-15} {3,-10}", student.StudentId, student.FirstName, student.LastName, student.City);
                                                                    }

                                                                }

                                                                try
                                                                {
                                                                    studentId = int.Parse(Console.ReadLine());
                                                                }

                                                                catch
                                                                {
                                                                    Console.WriteLine("Invalid input, Press ENTER to try again");
                                                                    Console.ReadKey();
                                                                    break;
                                                                }

                                                                var std = dbCtx.Students.First(s => s.StudentId == studentId);

                                                                if (!crs.Students.Contains(std) && std != null)
                                                                {
                                                                    crs.Students.Add(std);
                                                                    dbCtx.SaveChanges();
                                                                    Console.WriteLine($"Student id:{studentId} succesfully added to Courseid:{id}");
                                                                    Console.WriteLine("Press ENTER to continue");
                                                                    Console.ReadKey();
                                                                    MainMenu();
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Invalid input, Press ENTER to try again");
                                                                    Console.ReadKey();
                                                                }
                                                                break;
                                                            case 2:
                                                                Console.WriteLine("Select the id of the student you want to remove:");
                                                                try
                                                                {
                                                                    studentId = int.Parse(Console.ReadLine());
                                                                }

                                                                catch
                                                                {
                                                                    Console.WriteLine("Invalid input, Press ENTER to try again");
                                                                    Console.ReadKey();
                                                                    break;
                                                                }

                                                                var std2 = dbCtx.Students.First(s => s.StudentId == studentId);
                                                                if (std2 != null && crs.Students.Contains(std2))
                                                                {
                                                                    crs.Students.Remove(std2);
                                                                    dbCtx.SaveChanges();
                                                                    Console.WriteLine($"Student ID{studentId} successfully removed from Course ID{id}");
                                                                    Console.WriteLine("Press ENTER to continue");
                                                                    Console.ReadKey();
                                                                    CourseStudents();

                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Invalid input, Press ENTER to try again");
                                                                    Console.ReadKey();
                                                                }

                                                                break;
                                                            case 3:
                                                                CourseMenu();
                                                                break;
                                                        }
                                                    }

                                                case 5:
                                                    Console.WriteLine("Are you sure you want to delete Course?");
                                                    Console.WriteLine("Press Y to confirm, Else press ENTER.");
                                                    var keyPressed = Console.ReadKey();
                                                    if (keyPressed.Key == ConsoleKey.Y)
                                                    {
                                                        dbCtx.Courses.Remove(crs);
                                                        dbCtx.SaveChanges();
                                                        Console.WriteLine($"\n Course ID {id} successfully removed");
                                                        Console.WriteLine("Press ENTER to continue");
                                                        Console.ReadKey();
                                                        CourseMenu();
                                                    }
                                                    break;

                                            }


                                        }

                                    }


                                }

                            }


                        case 3:
                            CourseStudents();
                            break;
                        case 4:
                            MainMenu();
                            break;
                    }
                    Console.ReadLine();
                    MainMenu();
                }

            }
            public void CourseStudents()
            {

                while (true)
                {
                    Console.Clear();
                    int selection;
                    foreach (var crs in dbCtx.Courses.Include(c => c.Students))
                    {

                        Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-10}", "ID", "CourseName", "StartDate", "EndDate");

                        Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-10}", crs.CourseId, crs.CourseName,
                                                            crs.StartDate.ToShortDateString(), crs.EndDate.ToShortDateString());

                        Console.WriteLine("-------------------------------students----------------------------------------");
                        Console.WriteLine("{0,-5} {1,-10} {2,-15} {3,-10}\n", "ID", "Firstname", "Lastname", "City");

                        foreach (var student in crs.Students)
                        {
                            Console.WriteLine("{0,-5} {1,-10} {2,-15} {3,-10}\n", student.StudentId, student.FirstName, student.LastName, student.City);
                        }
                        Console.WriteLine("--------------------------------------------------------------------------------");


                    }
                    Console.WriteLine("[1]Edit Student");
                    Console.WriteLine("[2]Course Menu");
                    Console.WriteLine("[3]Main Menu");

                    try
                    {
                        selection = int.Parse(Console.ReadKey(true).KeyChar.ToString());
                    }

                    catch (FormatException)
                    {
                        continue;
                    }

                    switch (selection)
                    {
                        case 1:
                            EditStudent();
                            break;
                        case 2:
                            CourseMenu();
                            break;
                        case 3:
                            MainMenu();
                            break;
                    }

                }

            }

        }



        static void Main(string[] args)
        {
            var studentinfo = new StudentInfo();
            studentinfo.GenerateStudents();
            studentinfo.MainMenu();
        }
    }
}