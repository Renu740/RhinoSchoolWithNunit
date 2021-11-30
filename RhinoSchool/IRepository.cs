using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoSchool
{
    public interface IRepository<T>  where T  : class
    {
        T Save(T t);
        bool Exist(string name);
    }

    public interface ICourseCatelog
    {
        IList<Course> GetCourseCatelog();
        bool HasCourse(string title);
    }


    public class Course
    {
        public int Id { get; set; }
        public string CourseTitle { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
