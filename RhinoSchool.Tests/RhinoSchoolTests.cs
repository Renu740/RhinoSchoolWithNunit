using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RhinoSchool.Tests
{
    public class RhinoSchoolTests
    {
        IRepository<Student> mockStudentRepository;
        ICourseCatelog mockCourseCatalogRepository;

        [SetUp]
        public void Setup()
        {
            mockStudentRepository = MockRepository.GenerateMock<IRepository<Student>>();
            mockCourseCatalogRepository = MockRepository.GenerateMock<ICourseCatelog>();
        }

        [Test]
        public void ApplyForCourse_NotNull_ReturnsStudents()
        {
            mockCourseCatalogRepository.Expect(d => d.HasCourse(Arg<string>.Is.NotNull)).Return(true);
            mockStudentRepository.Expect(d => d.Exist(Arg<string>.Is.NotNull)).Return(false);
            mockStudentRepository.Expect(d => d.Save(Arg<Student>.Is.NotNull)).Return(new Student());

            CourseManager courseManager = new CourseManager(mockCourseCatalogRepository, mockStudentRepository);
            var student = new Student
            {
                Id = 10,
                Name = "Renu",
                Email = "renu@gmail.com"
            };

            var course = new Course
            {
                Id = 10,
                CourseTitle = "Azure"
            };

            var studentObj = courseManager.ApplyForCourse(student, course);

            Assert.NotNull(studentObj);
        }

        [Test]
        public void ApplyForCourse_NullStudent_ThrowsException()
        {
            mockCourseCatalogRepository.Expect(d => d.HasCourse(Arg<string>.Is.Null)).Return(false);
            mockStudentRepository.Expect(d => d.Exist(Arg<string>.Is.Null)).Return(true);
            mockStudentRepository.Expect(d => d.Save(Arg<Student>.Is.Null)).Return(null);

            CourseManager courseManager = new CourseManager(mockCourseCatalogRepository, mockStudentRepository);
            var course = new Course
            {
                Id = 10,
                CourseTitle = "Azure"
            };

            Assert.Throws<Exception>(() => { courseManager.ApplyForCourse(null, course); });
        }

        [Test]
        public void ApplyCourse_ItemExist_ThrowsException()
        {
            mockCourseCatalogRepository.Expect(d => d.HasCourse(null)).IgnoreArguments().Return(true);
            mockStudentRepository.Expect(d => d.Exist(null)).IgnoreArguments().Return(true);

            mockStudentRepository.Expect(d => d.Save(Arg<Student>.Is.Null)).Return(null);

            CourseManager courseManager = new CourseManager(mockCourseCatalogRepository, mockStudentRepository);

            Assert.Throws<Exception>(() =>
            {
                courseManager.ApplyForCourse(new Student { Email = "renu@gmail.com", Name = "Renu" },
                                              new Course { CourseTitle = "Azure", });

            });
        }
        [Test]
        public void ApplyCourse_ItemDontExist_ReturnsStudent()
        {
            mockCourseCatalogRepository.Expect(d => d.HasCourse(null)).IgnoreArguments().Return(true);
            mockStudentRepository.Expect(d => d.Exist(null)).IgnoreArguments().Return(false);
            mockStudentRepository.Expect(d => d.Save(null)).IgnoreArguments().Return(new Student
            { Email = "renu@gmail.com", Name = "Renu" });

            CourseManager courseManager = new CourseManager(mockCourseCatalogRepository, mockStudentRepository);

            var studentObj = courseManager.ApplyForCourse(new Student { Email = "renu@gmail.com", Name = "Renu" },
                                              new Course { CourseTitle = "Azure", });
            Assert.IsNotNull(studentObj);
        }

        List<string> studentListData = new List<string>();

        [Test]
        public void Save_DublicateItem_ThrowsException()
        {
            mockCourseCatalogRepository.Expect(x => x.HasCourse(Arg<string>.Is.NotNull)).IgnoreArguments().Return(true);

            mockStudentRepository.Expect(d => d.Save(Arg<Student>.Is.NotNull)).WhenCalled((args) =>
            {
                var student =  (Student)args.Arguments[0];
                string name = student.Name;
                studentListData.Add((name));
            }).Return(new Student { Email = "renu@gmail.com",  Name = "Renu" });
            

            mockStudentRepository.Expect(d => d.Exist(null)).IgnoreArguments().Do((Func<string, bool>)((input) =>
            {
                return studentListData.Contains(input);
            }));

            CourseManager courseManager = new CourseManager(mockCourseCatalogRepository, mockStudentRepository);
            var studentData = new Student
            {
                Name = "Renu",
                Email = "renu@gmail.com"
            };

            var courseData = new Course
            {
                CourseTitle = "Azure"
            };
           var studentObj = courseManager.ApplyForCourse(studentData, courseData);

            Assert.IsNotNull(studentObj);
            Assert.IsTrue(studentListData.Count > 0);
            Assert.Throws<Exception>(() => courseManager.ApplyForCourse(new Student {Name = "Renu" }, courseData));
        }

        [TestCase("Renu")]
        [TestCase("renu")]
        [TestCase("RENU")]
        public void Save_ValidDataWithConstraint_ReturnsTrue(string Name)
        {
            mockStudentRepository.Expect(d => d.Save(Arg<Student>.Matches(x => (Regex.IsMatch(x.Name, "^[a-zA-Z]*$"))))).Return(new Student());

            var studentObj = mockStudentRepository.Save(new Student { Name = Name });

            Assert.IsNotNull(studentObj);

        }

        [TestCase("135545")]
        [TestCase("renu8787")]
        [TestCase("RENU878@##")]
        public void Save_InValidNameConstraint_ReturnsNull(string Name)
        {
            mockStudentRepository.Expect(d => d.Save(Arg<Student>.Matches(x => (!Regex.IsMatch(x.Name, "^[a-zA-Z]*$"))))).Return(null);

            var studentObj = mockStudentRepository.Save(new Student { Name = Name });

            Assert.IsNull(studentObj);

        }


    }
}