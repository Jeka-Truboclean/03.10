using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace _03._10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var group = new Group() { Name = "Group A" };
                var student = new Student() { Name = "John", Age = 20, Groups = new List<Group> { group } };

                db.Groups.Add(group);
                db.Students.Add(student);
                db.SaveChanges();

                Console.WriteLine("Student and group are added.");

                var studentWithGroups = db.Students.Include(s => s.Groups).FirstOrDefault(s => s.Name == "John");
                if (studentWithGroups != null)
                {
                    Console.WriteLine($"Student: {studentWithGroups.Name}, Group: {string.Join(", ", studentWithGroups.Groups.Select(g => g.Name))}");
                }

                var groupWithStudents = db.Groups.Include(g => g.Students).FirstOrDefault(g => g.Name == "Group A");
                if (groupWithStudents != null)
                {
                    Console.WriteLine($"Group: {groupWithStudents.Name}, Students: {string.Join(", ", groupWithStudents.Students.Select(s => s.Name))}");
                }

                var studentToUpdate = db.Students.FirstOrDefault(s => s.Id == student.Id);
                if (studentToUpdate != null)
                {
                    studentToUpdate.Name = "John Updated";
                    db.SaveChanges();
                    Console.WriteLine("Student Name is changed.");
                }

                var studentToDelete = db.Students.FirstOrDefault(s => s.Id == student.Id);
                if (studentToDelete != null)
                {
                    db.Students.Remove(studentToDelete);
                    db.SaveChanges();
                    Console.WriteLine("Student is deleted.");
                }
            }
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public List<Group> Groups { get; set; } = new List<Group>();
    }

    public class Group
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Student> Students { get; set; } = new List<Student>();
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }

        public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\ProjectModels;Database=StudentGroupDb;Trusted_Connection=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Groups)
                .WithMany(g => g.Students)
                .UsingEntity(j => j.ToTable("StudentGroups"));
        }
    }
}
