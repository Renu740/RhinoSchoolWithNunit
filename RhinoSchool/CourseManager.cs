using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoSchool
{
    public class CourseManager
    {
        ICourseCatelog courseCatelog;
        IRepository<Student> studentRepository;

        public CourseManager(ICourseCatelog courseCatelog
                           , IRepository<Student> studentRepository)
        {
            this.courseCatelog = courseCatelog;
            this.studentRepository = studentRepository;
        }

        public Student ApplyForCourse(Student student,Course course)
        {
            Student s = null;
            if ( (course!=null) &&  (courseCatelog.HasCourse(course.CourseTitle)))
            {
                if (!studentRepository.Exist(student.Name))
                {
                    s = studentRepository.Save(student);
                }
                else
                    throw new Exception("You have aready enrolled for the course");
            }
            else
                throw new Exception($"Sorry we dont offer this coruse {course.CourseTitle}");


            return s;
        }
    }
}
