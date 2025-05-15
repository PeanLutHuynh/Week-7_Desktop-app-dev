using System;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StudentSystem.Data;
using StudentSystem.Models;

namespace StudentSystem.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            var context = new StudentSystemContext();

            // Ensure database is created
            context.Database.EnsureCreated();

            // Synchronize data with code definitions
            SeedData(context);

            // Menu to execute queries
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Student Management System - Choose an option:");
                Console.WriteLine("1. List all students and their homework submissions");
                Console.WriteLine("2. List all courses with their resources");
                Console.WriteLine("3. List courses with more than 5 resources");
                Console.WriteLine("4. List active courses on a specific date");
                Console.WriteLine("5. Calculate student course statistics");
                Console.WriteLine("6. List courses with resources and licenses");
                Console.WriteLine("7. List student statistics with resources and licenses");
                Console.WriteLine("0. Exit");

                Console.Write("\nYour choice: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ListStudentsWithHomework(context);
                        break;
                    case "2":
                        ListCoursesWithResources(context);
                        break;
                    case "3":
                        ListCoursesWithManyResources(context);
                        break;
                    case "4":
                        ListActiveCoursesOnDate(context);
                        break;
                    case "5":
                        CalculateStudentStatistics(context);
                        break;
                    case "6":
                        ListCoursesWithResourcesAndLicenses(context);
                        break;
                    case "7":
                        ListStudentStatisticsWithResourcesAndLicenses(context);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private static void SeedData(StudentSystemContext context)
        {
            try
            {
                // Students
                Console.WriteLine("Synchronizing student data...");
                var studentsToKeep = new[]
                {
                    new Student { Name = "Nguyễn Văn A", PhoneNumber = "0123456789", RegistrationDate = DateTime.Now.AddMonths(-6), Birthday = new DateTime(1995, 5, 15) },
                    new Student { Name = "Huỳnh Văn B", PhoneNumber = "0132547698", RegistrationDate = DateTime.Now.AddMonths(-3), Birthday = new DateTime(1998, 8, 21) },
                    new Student { Name = "Đặng Văn C", RegistrationDate = DateTime.Now.AddMonths(-1) },
                    new Student { Name = "Trần Thị D", RegistrationDate = DateTime.Now.AddMonths(-2), Birthday = new DateTime(1997, 12, 30) },
                    // Additional students
                    new Student { Name = "Lê Thị E", PhoneNumber = "0987654321", RegistrationDate = DateTime.Now.AddMonths(-4), Birthday = new DateTime(1999, 3, 12) },
                    new Student { Name = "Phạm Văn F", RegistrationDate = DateTime.Now.AddMonths(-5) },
                };

                SynchronizeEntities(context, context.Students,
                    studentsToKeep,
                    s => s.Name,
                    (existing, updated) =>
                    {
                        if (updated.PhoneNumber != null && existing.PhoneNumber != updated.PhoneNumber)
                        {
                            existing.PhoneNumber = updated.PhoneNumber;
                            return true;
                        }
                        if (existing.RegistrationDate != updated.RegistrationDate)
                        {
                            existing.RegistrationDate = updated.RegistrationDate;
                            return true;
                        }
                        if (updated.Birthday != null && existing.Birthday != updated.Birthday)
                        {
                            existing.Birthday = updated.Birthday;
                            return true;
                        }
                        return false;
                    },
                    "Student");

                // Courses
                Console.WriteLine("Synchronizing course data...");
                var coursesToKeep = new[]
                {
                    new Course { Name = "C# Basics", Description = "Introduction to C#", StartDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddMonths(1), Price = 350.00m },
                    new Course { Name = "C# Advanced", Description = "Advanced C# concepts", StartDate = DateTime.Now.AddMonths(-1), EndDate = DateTime.Now.AddMonths(2), Price = 400.00m },
                    new Course { Name = "SQL Basics", StartDate = DateTime.Now.AddMonths(-3), EndDate = DateTime.Now.AddMonths(-1), Price = 300.00m },
                    new Course { Name = "Flutter Basics", Description = "Introduction to Flutter", StartDate = DateTime.Now.AddDays(-12), EndDate = DateTime.Now.AddDays(+4), Price = 350.00m },
                    new Course { Name = "Java Basics", StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(+5), Price = 300.00m },
                    // Additional courses
                    new Course { Name = "Python Basics", Description = "Introduction to Python", StartDate = DateTime.Now.AddDays(-20), EndDate = DateTime.Now.AddDays(+15), Price = 320.00m },
                    new Course { Name = "Web Development", Description = "HTML, CSS and JavaScript", StartDate = DateTime.Now.AddDays(-15), EndDate = DateTime.Now.AddDays(+20), Price = 380.00m },
                    new Course { Name = "Data Science", Description = "Introduction to Data Analysis", StartDate = DateTime.Now.AddDays(-30), EndDate = DateTime.Now.AddDays(+30), Price = 450.00m },
                };

                SynchronizeEntities(context, context.Courses,
                    coursesToKeep,
                    c => c.Name,
                    (existing, updated) =>
                    {
                        bool hasChanges = false;
                        if (updated.Description != null && existing.Description != updated.Description)
                        {
                            existing.Description = updated.Description;
                            hasChanges = true;
                        }
                        if (existing.StartDate != updated.StartDate)
                        {
                            existing.StartDate = updated.StartDate;
                            hasChanges = true;
                        }
                        if (existing.EndDate != updated.EndDate)
                        {
                            existing.EndDate = updated.EndDate;
                            hasChanges = true;
                        }
                        if (existing.Price != updated.Price)
                        {
                            existing.Price = updated.Price;
                            hasChanges = true;
                        }
                        return hasChanges;
                    },
                    "Course");

                // Get ID dictionaries for relations
                var studentDict = context.Students.AsNoTracking().ToDictionary(s => s.Name, s => s.StudentId);
                var courseDict = context.Courses.AsNoTracking().ToDictionary(c => c.Name, c => c.CourseId);

                // Enrollments
                Console.WriteLine("Synchronizing enrollment data...");
                var enrollmentsToKeep = new[]
                {
                    new { StudentName = "Trần Thị D", CourseName = "C# Basics" },
                    new { StudentName = "Nguyễn Văn A", CourseName = "SQL Basics" },
                    new { StudentName = "Huỳnh Văn B", CourseName = "C# Basics" },
                    new { StudentName = "Đặng Văn C", CourseName = "C# Advanced" },
                    new { StudentName = "Nguyễn Văn A", CourseName = "Flutter Basics" },
                    // Additional enrollments
                    new { StudentName = "Lê Thị E", CourseName = "Python Basics" },
                    new { StudentName = "Phạm Văn F", CourseName = "Web Development" },
                    new { StudentName = "Lê Thị E", CourseName = "Data Science" },
                    new { StudentName = "Trần Thị D", CourseName = "Java Basics" },
                    new { StudentName = "Đặng Văn C", CourseName = "Python Basics" },
                    new { StudentName = "Huỳnh Văn B", CourseName = "Web Development" },
                    new { StudentName = "Nguyễn Văn A", CourseName = "Data Science" },
                };

                // Process enrollments
                var existingEnrollments = context.StudentCourses.ToList();
                var keepEnrollmentKeys = new List<(int, int)>();

                foreach (var enrollment in enrollmentsToKeep)
                {
                    if (studentDict.TryGetValue(enrollment.StudentName, out int studentId) &&
                        courseDict.TryGetValue(enrollment.CourseName, out int courseId))
                    {
                        keepEnrollmentKeys.Add((studentId, courseId));
                    }
                }

                // Remove enrollments not in the list
                foreach (var existingEnrollment in existingEnrollments)
                {
                    if (!keepEnrollmentKeys.Contains((existingEnrollment.StudentId, existingEnrollment.CourseId)))
                    {
                        context.StudentCourses.Remove(existingEnrollment);
                        var studentName = context.Students.FirstOrDefault(s => s.StudentId == existingEnrollment.StudentId)?.Name;
                        var courseName = context.Courses.FirstOrDefault(c => c.CourseId == existingEnrollment.CourseId)?.Name;
                        Console.WriteLine($"Removed unused enrollment: {studentName} - {courseName}");
                    }
                }
                context.SaveChanges();

                // Add new enrollments
                foreach (var enrollment in enrollmentsToKeep)
                {
                    if (studentDict.TryGetValue(enrollment.StudentName, out int studentId) &&
                        courseDict.TryGetValue(enrollment.CourseName, out int courseId))
                    {
                        var exists = context.StudentCourses.Any(sc =>
                            sc.StudentId == studentId && sc.CourseId == courseId);

                        if (!exists)
                        {
                            context.StudentCourses.Add(new StudentCourse
                            {
                                StudentId = studentId,
                                CourseId = courseId
                            });
                            Console.WriteLine($"Enrolled student {enrollment.StudentName} in course {enrollment.CourseName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Cannot enroll: Student {enrollment.StudentName} or course {enrollment.CourseName} not found");
                    }
                }
                context.SaveChanges();

                // Resources
                Console.WriteLine("Synchronizing resource data...");
                var resourcesToKeep = new[]
                {
                    new { Name = "C# Slides", ResourceType = ResourceType.Presentation, Url = "https://example.com/slides1", CourseName = "C# Basics" },
                    new { Name = "C# Video", ResourceType = ResourceType.Video, Url = "https://example.com/video1", CourseName = "C# Basics" },
                    new { Name = "C# Advanced PDF", ResourceType = ResourceType.Document, Url = "https://example.com/pdf1", CourseName = "C# Advanced" },
                    // Additional resources
                    new { Name = "C# Practice Exercises", ResourceType = ResourceType.Document, Url = "https://example.com/exercises1", CourseName = "C# Basics" },
                    new { Name = "C# Tutorial Videos", ResourceType = ResourceType.Video, Url = "https://example.com/tutorial1", CourseName = "C# Basics" },
                    new { Name = "C# Cheat Sheet", ResourceType = ResourceType.Document, Url = "https://example.com/cheatsheet", CourseName = "C# Basics" },
                    new { Name = "Design Patterns PDF", ResourceType = ResourceType.Document, Url = "https://example.com/patterns", CourseName = "C# Advanced" },
                    new { Name = "SQL Basics Slides", ResourceType = ResourceType.Presentation, Url = "https://example.com/sql_slides", CourseName = "SQL Basics" },
                    new { Name = "SQL Tutorial Videos", ResourceType = ResourceType.Video, Url = "https://example.com/sql_videos", CourseName = "SQL Basics" },
                    new { Name = "Flutter Installation Guide", ResourceType = ResourceType.Document, Url = "https://example.com/flutter_install", CourseName = "Flutter Basics" },
                    new { Name = "Flutter UI Components", ResourceType = ResourceType.Presentation, Url = "https://example.com/flutter_ui", CourseName = "Flutter Basics" },
                    new { Name = "Python Setup Guide", ResourceType = ResourceType.Document, Url = "https://example.com/python_setup", CourseName = "Python Basics" },
                    new { Name = "Python Practice Problems", ResourceType = ResourceType.Document, Url = "https://example.com/python_problems", CourseName = "Python Basics" },
                };

                // Process resources
                var existingResources = context.Resources.ToList();
                var keepResourceKeys = new List<(string, int)>();

                foreach (var res in resourcesToKeep)
                {
                    if (courseDict.TryGetValue(res.CourseName, out int courseId))
                    {
                        keepResourceKeys.Add((res.Name, courseId));
                    }
                }

                // Remove resources not in the list
                foreach (var existingResource in existingResources)
                {
                    if (!keepResourceKeys.Contains((existingResource.Name, existingResource.CourseId)))
                    {
                        context.Resources.Remove(existingResource);
                        var courseName = context.Courses.FirstOrDefault(c => c.CourseId == existingResource.CourseId)?.Name;
                        Console.WriteLine($"Removed unused resource: {existingResource.Name} (course: {courseName})");
                    }
                }
                context.SaveChanges();

                // Add/update resources
                foreach (var res in resourcesToKeep)
                {
                    if (courseDict.TryGetValue(res.CourseName, out int courseId))
                    {
                        var existingResource = context.Resources.FirstOrDefault(r =>
                            r.Name == res.Name && r.CourseId == courseId);

                        if (existingResource != null)
                        {
                            // Update existing resource
                            bool hasChanges = false;

                            if (existingResource.ResourceType != res.ResourceType)
                            {
                                existingResource.ResourceType = res.ResourceType;
                                hasChanges = true;
                            }

                            if (existingResource.Url != res.Url)
                            {
                                existingResource.Url = res.Url;
                                hasChanges = true;
                            }

                            if (hasChanges)
                            {
                                Console.WriteLine($"Updated resource: {res.Name}");
                            }
                        }
                        else
                        {
                            // Add new resource
                            context.Resources.Add(new Resource
                            {
                                Name = res.Name,
                                ResourceType = res.ResourceType,
                                Url = res.Url,
                                CourseId = courseId
                            });
                            Console.WriteLine($"Added new resource: {res.Name}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Cannot add resource: Course {res.CourseName} not found");
                    }
                }
                context.SaveChanges();

                // Homework
                Console.WriteLine("Synchronizing homework data...");
                var homeworksToKeep = new[]
                {
                    new { Content = "C# Basic Assignment", ContentType = ContentType.Pdf, SubmissionDate = DateTime.Now.AddDays(-10), StudentName = "Trần Thị D", CourseName = "C# Basics" },
                    new { Content = "SQL Exercise 1", ContentType = ContentType.Zip, SubmissionDate = DateTime.Now.AddDays(-5), StudentName = "Nguyễn Văn A", CourseName = "SQL Basics" },
                    new { Content = "C# OOP Exercise", ContentType = ContentType.Pdf, SubmissionDate = DateTime.Now.AddDays(-3), StudentName = "Huỳnh Văn B", CourseName = "C# Basics" },
                    new { Content = "LINQ Assignment", ContentType = ContentType.Application, SubmissionDate = DateTime.Now.AddDays(-2), StudentName = "Đặng Văn C", CourseName = "C# Advanced" },
                    new { Content = "Flutter UI Assignment", ContentType = ContentType.Zip, SubmissionDate = DateTime.Now.AddDays(-1), StudentName = "Nguyễn Văn A", CourseName = "Flutter Basics" },
                };

                // Process homework
                var existingHomeworks = context.HomeworkSubmissions.ToList();
                var keepHomeworkKeys = new List<(string, int, int)>();

                foreach (var hw in homeworksToKeep)
                {
                    if (studentDict.TryGetValue(hw.StudentName, out int studentId) &&
                        courseDict.TryGetValue(hw.CourseName, out int courseId))
                    {
                        keepHomeworkKeys.Add((hw.Content, studentId, courseId));
                    }
                }

                // Remove homework not in the list
                foreach (var existingHomework in existingHomeworks)
                {
                    if (!keepHomeworkKeys.Contains((existingHomework.Content, existingHomework.StudentId, existingHomework.CourseId)))
                    {
                        context.HomeworkSubmissions.Remove(existingHomework);
                        var studentName = context.Students.FirstOrDefault(s => s.StudentId == existingHomework.StudentId)?.Name;
                        var courseName = context.Courses.FirstOrDefault(c => c.CourseId == existingHomework.CourseId)?.Name;
                        Console.WriteLine($"Removed unused homework: {existingHomework.Content} (student: {studentName}, course: {courseName})");
                    }
                }
                context.SaveChanges();

                // Add/update homework
                foreach (var hw in homeworksToKeep)
                {
                    if (studentDict.TryGetValue(hw.StudentName, out int studentId) &&
                        courseDict.TryGetValue(hw.CourseName, out int courseId))
                    {
                        var existingHomework = context.HomeworkSubmissions.FirstOrDefault(h =>
                            h.Content == hw.Content &&
                            h.StudentId == studentId &&
                            h.CourseId == courseId);

                        if (existingHomework != null)
                        {
                            // Update existing homework
                            bool hasChanges = false;

                            if (existingHomework.ContentType != hw.ContentType)
                            {
                                existingHomework.ContentType = hw.ContentType;
                                hasChanges = true;
                            }

                            if (existingHomework.SubmissionDate != hw.SubmissionDate)
                            {
                                existingHomework.SubmissionDate = hw.SubmissionDate;
                                hasChanges = true;
                            }

                            if (hasChanges)
                            {
                                Console.WriteLine($"Updated homework: {hw.Content}");
                            }
                        }
                        else
                        {
                            // Add new homework
                            context.HomeworkSubmissions.Add(new Homework
                            {
                                Content = hw.Content,
                                ContentType = hw.ContentType,
                                SubmissionDate = hw.SubmissionDate,
                                StudentId = studentId,
                                CourseId = courseId
                            });
                            Console.WriteLine($"Added new homework: {hw.Content}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Cannot add homework: Student {hw.StudentName} or course {hw.CourseName} not found");
                    }
                }
                context.SaveChanges();

                // Licenses
                Console.WriteLine("Synchronizing license data...");
                var licensesToKeep = new[]
                {
                new License { Name = "MIT License" },
                new License { Name = "Apache License 2.0" },
                new License { Name = "GNU GPL v3" },
                new License { Name = "Creative Commons" }
                };

                SynchronizeEntities(context, context.Licenses,
                    licensesToKeep,
                    l => l.Name,
                    (existing, updated) => false, // Licenses don't have properties to update
                    "License");

                // Resource Licenses
                Console.WriteLine("Synchronizing resource licenses...");
                var resourceLicensesToKeep = new[]
                {
                new { ResourceName = "C# Slides", LicenseName = "MIT License" },
                new { ResourceName = "C# Video", LicenseName = "Creative Commons" },
                new { ResourceName = "C# Advanced PDF", LicenseName = "GNU GPL v3" },
                new { ResourceName = "C# Practice Exercises", LicenseName = "MIT License" },
                new { ResourceName = "C# Tutorial Videos", LicenseName = "Creative Commons" },
                new { ResourceName = "SQL Basics Slides", LicenseName = "Apache License 2.0" },
                new { ResourceName = "Python Setup Guide", LicenseName = "MIT License" },
                new { ResourceName = "Flutter UI Components", LicenseName = "Creative Commons" },
                // Add more resource-license relationships as needed
                };

                SynchronizeResourceLicenses(context, resourceLicensesToKeep);
                Console.WriteLine("Database synchronization completed successfully.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database update error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Details: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Details: {ex.InnerException.Message}");
                }
            }
        }

        // Generic method to synchronize entities between code and database
        private static void SynchronizeEntities<T, TKey>(
            StudentSystemContext context,
            DbSet<T> dbSet,
            T[] entitiesToKeep,
            Func<T, TKey> keySelector,
            Func<T, T, bool> updateAction,
            string entityName) where T : class
        {
            // Get existing entities
            var existingEntities = dbSet.ToList();
            var keysToKeep = entitiesToKeep.Select(keySelector).ToList();

            // Remove entities not in list
            foreach (var existingEntity in existingEntities)
            {
                var key = keySelector(existingEntity);
                if (!keysToKeep.Contains(key))
                {
                    dbSet.Remove(existingEntity);
                    Console.WriteLine($"Removed unused {entityName}: {key}");
                }
            }
            context.SaveChanges();

            // Update or add entities
            foreach (var entity in entitiesToKeep)
            {
                var key = keySelector(entity);
                var existingEntity = existingEntities.FirstOrDefault(e => keySelector(e).Equals(key));

                if (existingEntity != null)
                {
                    // Update if needed
                    if (updateAction(existingEntity, entity))
                    {
                        Console.WriteLine($"Updated {entityName}: {key}");
                    }
                }
                else
                {
                    // Add new entity
                    dbSet.Add(entity);
                    Console.WriteLine($"Added new {entityName}: {key}");
                }
            }
            context.SaveChanges();
        }

        private static void ListStudentsWithHomework(StudentSystemContext context)
        {
            // List all students and their homework submissions
            Console.WriteLine("\n--- Students and Their Homework Submissions ---");

            var studentsWithHomework = context.Students
                .Include(s => s.HomeworkSubmissions)
                .OrderBy(s => s.Name)
                .ToList();

            foreach (var student in studentsWithHomework)
            {
                Console.WriteLine($"Student: {student.Name}");

                if (student.HomeworkSubmissions.Any())
                {
                    Console.WriteLine("  Homework submissions:");
                    foreach (var homework in student.HomeworkSubmissions)
                    {
                        Console.WriteLine($"  - Content: {homework.Content}, Type: {homework.ContentType}");
                    }
                }
                else
                {
                    Console.WriteLine("  No homework submissions");
                }

                Console.WriteLine();
            }
        }

        private static void ListCoursesWithResources(StudentSystemContext context)
        {
            // List all courses with their resources
            Console.WriteLine("\n--- Courses With Their Resources ---");

            var coursesWithResources = context.Courses
                .Include(c => c.Resources)
                .OrderBy(c => c.StartDate)
                .ThenByDescending(c => c.EndDate)
                .ToList();

            foreach (var course in coursesWithResources)
            {
                Console.WriteLine($"Course: {course.Name}");
                Console.WriteLine($"Description: {course.Description ?? "N/A"}");

                if (course.Resources.Any())
                {
                    Console.WriteLine("  Resources:");
                    foreach (var resource in course.Resources)
                    {
                        Console.WriteLine($"  - Name: {resource.Name}, Type: {resource.ResourceType}, URL: {resource.Url}");
                    }
                }
                else
                {
                    Console.WriteLine("  No resources");
                }

                Console.WriteLine();
            }
        }

        private static void ListCoursesWithManyResources(StudentSystemContext context)
        {
            // List courses with more than 5 resources
            Console.WriteLine("\n--- Courses With More Than 5 Resources ---");

            var coursesWithManyResources = context.Courses
                .Include(c => c.Resources)
                .Where(c => c.Resources.Count > 5)
                .OrderByDescending(c => c.Resources.Count)
                .ThenByDescending(c => c.StartDate)
                .ToList();

            if (coursesWithManyResources.Any())
            {
                foreach (var course in coursesWithManyResources)
                {
                    Console.WriteLine($"Course: {course.Name}, Resources: {course.Resources.Count}");
                }
            }
            else
            {
                Console.WriteLine("No courses with more than 5 resources found.");
            }
        }

        private static void ListActiveCoursesOnDate(StudentSystemContext context)
        {
            // List active courses on a specific date
            Console.WriteLine("\n--- Active Courses On Specific Date ---");
            Console.Write("Enter date (yyyy-MM-dd): ");
            try
            {
                string? dateInput = Console.ReadLine();
                if (DateTime.TryParse(dateInput, out DateTime date))
                {
                    var activeCourses = context.Courses
                        .Include(c => c.StudentsEnrolled)
                        .ThenInclude(sc => sc.Student)
                        .Where(c => c.StartDate <= date && c.EndDate >= date)
                        .AsEnumerable() // Convert to IEnumerable to use LINQ to Objects
                        .OrderByDescending(c => c.StudentsEnrolled.Count)
                        .ThenByDescending(c => (c.EndDate - c.StartDate).Days)
                        .ToList();

                    if (activeCourses.Any())
                    {
                        foreach (var course in activeCourses)
                        {
                            int duration = (course.EndDate - course.StartDate).Days;
                            int studentCount = course.StudentsEnrolled.Count;

                            Console.WriteLine($"Course: {course.Name}");
                            Console.WriteLine($"Period: {course.StartDate:yyyy-MM-dd} to {course.EndDate:yyyy-MM-dd} ({duration} days)");
                            Console.WriteLine($"Students enrolled: {studentCount}");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No active courses found on {date:yyyy-MM-dd}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid date format!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void ListCoursesWithResourcesAndLicenses(StudentSystemContext context)
        {
            Console.WriteLine("\n--- Courses With Resources And Licenses ---");

            var courses = context.Courses
                .Include(c => c.Resources)
                    .ThenInclude(r => r.Licenses)
                        .ThenInclude(rl => rl.License)
                .OrderByDescending(c => c.Resources.Count)
                .ThenBy(c => c.Name)
                .ToList();

            foreach (var course in courses)
            {
                Console.WriteLine($"Course: {course.Name}");

                // Sắp xếp resources theo số lượng licenses và tên
                var orderedResources = course.Resources
                    .OrderByDescending(r => r.Licenses.Count)
                    .ThenBy(r => r.Name);

                foreach (var resource in orderedResources)
                {
                    Console.WriteLine($"  Resource: {resource.Name}");

                    if (resource.Licenses.Any())
                    {
                        Console.WriteLine("    Licenses:");
                        foreach (var resourceLicense in resource.Licenses)
                        {
                            Console.WriteLine($"      - {resourceLicense.License.Name}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("    No licenses");
                    }
                }

                Console.WriteLine();
            }
        }

        private static void ListStudentStatisticsWithResourcesAndLicenses(StudentSystemContext context)
        {
            Console.WriteLine("\n--- Student Statistics With Resources And Licenses ---");

            var studentStats = context.Students
                .Include(s => s.CourseEnrollments)
                    .ThenInclude(sc => sc.Course)
                        .ThenInclude(c => c.Resources)
                            .ThenInclude(r => r.Licenses)
                .Select(s => new
                {
                    StudentName = s.Name,
                    CourseCount = s.CourseEnrollments.Count,
                    ResourcesCount = s.CourseEnrollments.SelectMany(sc => sc.Course.Resources).Count(),
                    LicensesCount = s.CourseEnrollments
                        .SelectMany(sc => sc.Course.Resources)
                        .SelectMany(r => r.Licenses)
                        .Count()
                })
                .OrderByDescending(x => x.CourseCount)
                .ThenByDescending(x => x.ResourcesCount)
                .ThenBy(x => x.StudentName)
                .ToList();

            foreach (var stat in studentStats)
            {
                Console.WriteLine($"Student: {stat.StudentName}");
                Console.WriteLine($"  Courses: {stat.CourseCount}");
                Console.WriteLine($"  Total Resources: {stat.ResourcesCount}");
                Console.WriteLine($"  Total Licenses: {stat.LicensesCount}");
                Console.WriteLine();
            }
        }

        private static void CalculateStudentStatistics(StudentSystemContext context)
        {
            // Calculate statistics for students and their courses
            Console.WriteLine("\n--- Student Course Statistics ---");

            var studentStats = context.Students
                .Include(s => s.CourseEnrollments)
                .ThenInclude(sc => sc.Course)
                .Select(s => new
                {
                    StudentName = s.Name,
                    CourseCount = s.CourseEnrollments.Count,
                    TotalPrice = s.CourseEnrollments.Sum(sc => sc.Course.Price),
                    AveragePrice = s.CourseEnrollments.Any() ? s.CourseEnrollments.Average(sc => sc.Course.Price) : 0
                })
                .AsEnumerable() // Convert to IEnumerable because SQLite doesn't support decimal in LINQ
                .OrderByDescending(x => x.TotalPrice)
                .ThenByDescending(x => x.CourseCount)
                .ThenBy(x => x.StudentName)
                .ToList();

            foreach (var stat in studentStats)
            {
                Console.WriteLine($"Student: {stat.StudentName}");
                Console.WriteLine($"  Courses: {stat.CourseCount}");
                Console.WriteLine($"  Total Price: ${stat.TotalPrice:F2}");
                Console.WriteLine($"  Average Price: ${stat.AveragePrice:F2}");
                Console.WriteLine();
            }
        }

        private static void SynchronizeResourceLicenses(
            StudentSystemContext context,
            dynamic[] resourceLicensesToKeep)
        {
            var existingResourceLicenses = context.ResourceLicenses.ToList();
            var resourceDict = context.Resources.AsNoTracking().ToDictionary(r => r.Name, r => r.ResourceId);
            var licenseDict = context.Licenses.AsNoTracking().ToDictionary(l => l.Name, l => l.LicenseId);

            var keepResourceLicenseKeys = new List<(int, int)>();

            // Build the list of resource-license pairs to keep
            foreach (var rl in resourceLicensesToKeep)
            {
                bool resourceFound = resourceDict.TryGetValue(rl.ResourceName, out int resourceId);
                bool licenseFound = licenseDict.TryGetValue(rl.LicenseName, out int licenseId);

                if (resourceFound && licenseFound)
                {
                    keepResourceLicenseKeys.Add((resourceId, licenseId));
                }
                else
                {
                    Console.WriteLine($"Cannot link: Resource {rl.ResourceName} or license {rl.LicenseName} not found");
                }
            }

            // Remove resource-license relationships not in the list
            foreach (var existingResourceLicense in existingResourceLicenses)
            {
                if (!keepResourceLicenseKeys.Contains((existingResourceLicense.ResourceId, existingResourceLicense.LicenseId)))
                {
                    context.ResourceLicenses.Remove(existingResourceLicense);
                    var resourceName = context.Resources.FirstOrDefault(r => r.ResourceId == existingResourceLicense.ResourceId)?.Name;
                    var licenseName = context.Licenses.FirstOrDefault(l => l.LicenseId == existingResourceLicense.LicenseId)?.Name;
                    Console.WriteLine($"Removed unused resource-license link: {resourceName} - {licenseName}");
                }
            }
            context.SaveChanges();

            // Add new resource-license relationships
            foreach (var rl in resourceLicensesToKeep)
            {
                bool resourceFound = resourceDict.TryGetValue(rl.ResourceName, out int resourceId);
                bool licenseFound = licenseDict.TryGetValue(rl.LicenseName, out int licenseId);

                if (resourceFound && licenseFound)
                {
                    var exists = context.ResourceLicenses.Any(rl =>
                        rl.ResourceId == resourceId && rl.LicenseId == licenseId);

                    if (!exists)
                    {
                        context.ResourceLicenses.Add(new ResourceLicense
                        {
                            ResourceId = resourceId,
                            LicenseId = licenseId
                        });
                        Console.WriteLine($"Added resource-license link: {rl.ResourceName} - {rl.LicenseName}");
                    }
                }
            }
            context.SaveChanges();
        }
    }
}