using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace University
{
  public class StudentTest : IDisposable
  {
    public StudentTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=university_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_StudentsEmptyAtFirst()
    {
      //Arrange, Act
      int result = Student.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameDescription()
    {
      //Arrange, Act
      Student firstStudent = new Student("larry", new DateTime(2016, 2, 2));
      Student secondStudent = new Student("larry", new DateTime(2016, 2, 2));

      //Assert
      Assert.Equal(firstStudent, secondStudent);
    }

    [Fact]
    public void Test_Save()
    {
      //Arrange
      Student testStudent = new Student("larry", new DateTime(2016, 2, 2));
      testStudent.Save();

      //Act
      List<Student> result = Student.GetAll();
      List<Student> testList = new List<Student>{testStudent};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_SaveAssignsIdToObject()
    {
      //Arrange
      Student testStudent = new Student("larry", new DateTime(2016, 2, 2));
      testStudent.Save();

      //Act
      Student savedStudent = Student.GetAll()[0];

      int result = savedStudent.GetId();
      int testId = testStudent.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_FindFindsStudentInDatabase()
    {
      //Arrange
      Student testStudent = new Student("larry", new DateTime(2016, 2, 2));
      testStudent.Save();

      //Act
      Student result = Student.Find(testStudent.GetId());

      //Assert
      Assert.Equal(testStudent, result);
    }

    [Fact]
    public void Test_AddCourse_AddsCourseToStudent()
    {
      //Arrange
      Student testStudent = new Student("larry", new DateTime(2016, 2, 2));
      testStudent.Save();

      Course testCourse = new Course("english", 1);
      testCourse.Save();

      //Act
      testStudent.AddCourse(testCourse);

      List<Course> result = testStudent.GetCourses();
      List<Course> testList = new List<Course>{testCourse};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetCourses_ReturnsAllStudentCourses()
    {
      //Arrange
      Student testStudent = new Student("larry", new DateTime(2016, 2, 2));
      testStudent.Save();

      Course testCourse1 = new Course("english", 1);
      testCourse1.Save();

      Course testCourse2 = new Course("french", 2);
      testCourse2.Save();

      //Act
      testStudent.AddCourse(testCourse1);
      List<Course> result = testStudent.GetCourses();
      List<Course> testList = new List<Course> {testCourse1};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Delete_DeletesStudentAssociationsFromDatabase()
    {
      //Arrange
      Course testCourse = new Course("english", 1);
      testCourse.Save();

      string testName = "larry";
      Student testStudent = new Student(testName, new DateTime(2016, 2, 2));
      testStudent.Save();

      //Act
      testStudent.AddCourse(testCourse);
      testStudent.Delete();

      List<Student> resultCourseStudents = testCourse.GetStudents();
      List<Student> testStudents = new List<Student> {};

      //Assert
      Assert.Equal(testStudents, resultCourseStudents);
    }


    public void Dispose()
    {
        Course.DeleteAll();
        Student.DeleteAll();
    }
  }
}
