using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace University
{
  public class CourseTest : IDisposable
  {
    public CourseTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=university_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_CoursesEmptyAtFirst()
    {
      //Arrange, Act
      int result = Course.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_Equal_ReturnsTrueForSameName()
    {
      //Arrange, Act
      Course firstCourse = new Course("english", 1);
      Course secondCourse = new Course("english", 1);

      //Assert
      Assert.Equal(firstCourse, secondCourse);
    }

    [Fact]
    public void Test_Save_SavesCourseToDatabase()
    {
      //Arrange
      Course testCourse = new Course("english", 1);
      testCourse.Save();

      //Act
      List<Course> result = Course.GetAll();
      List<Course> testList = new List<Course>{testCourse};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Save_AssignsIdToCourseObject()
    {
      //Arrange
      Course testCourse = new Course("english", 1);
      testCourse.Save();

      //Act
      Course savedCourse = Course.GetAll()[0];
      int result = savedCourse.GetId();
      int testId = testCourse.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_Find_FindsCourseInDatabase()
    {
      //Arrange
      Course testCourse = new Course("english", 1);
      testCourse.Save();

      //Act
      Course foundCourse = Course.Find(testCourse.GetId());

      //Assert
      Assert.Equal(testCourse, foundCourse);
    }
    [Fact]
    public void Test_GetStudents_RetrievesAllStudentsWithCourse()
    {
      Course testCourse = new Course("english", 1);
      testCourse.Save();
      Student firstStudent = new Student("larry", new DateTime(2016,2,2));
      firstStudent.Save();
      Student secondStudent = new Student("john",new DateTime(2016,2,2));
      secondStudent.Save();
      testCourse.AddStudent(firstStudent);
      List<Student> testStudentList = new List<Student> {firstStudent};
      List<Student> resultStudentList = testCourse.GetStudents();
      Assert.Equal(testStudentList, resultStudentList);
    }
    [Fact]
    public void Test_Update_UpdatesCourseInDatabase()
    {
      //Arrange
      string courseName = "english";
      Course testCourse = new Course(courseName, 1);
      testCourse.Save();
      string newName = "economics";

      //Act
      testCourse.Update(newName, 1);

      string result = testCourse.GetName();

      //Assert
      Assert.Equal(newName, result);
    }
    public void Test_Delete_DeletesCourseFromDatabase()
    {
      //Arrange
      string courseName1 = "english";
      Course testCourse1 = new Course(courseName1, 1);
      testCourse1.Save();

      string courseName2 = "economics";
      Course testCourse2 = new Course(courseName2, 1);
      testCourse2.Save();

      //Act
      testCourse1.Delete();
      List<Course> resultCourses = Course.GetAll();
      List<Course> testCourseList = new List<Course> {testCourse2};

      //Assert
      Assert.Equal(testCourseList, resultCourses);
    }

    [Fact]
    public void Test_AddStudent_AddsStudentToCourse()
    {
      //Arrange
      Course testCourse = new Course("english", 1);
      testCourse.Save();

      Student testStudent = new Student("larry",new DateTime(2016,2,2));
      testStudent.Save();

      Student testStudent2 = new Student("john",new DateTime(2016,2,2));
      testStudent2.Save();

      //Act
      testCourse.AddStudent(testStudent);
      testCourse.AddStudent(testStudent2);

      List<Student> result = testCourse.GetStudents();
      List<Student> testList = new List<Student>{testStudent, testStudent2};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetStudents_ReturnsAllCourseStudents()
    {
      //Arrange
      Course testCourse = new Course("english", 1);
      testCourse.Save();

      Student testStudent1 = new Student("larry",new DateTime(2016,2,2));
      testStudent1.Save();

      Student testStudent2 = new Student("john",new DateTime(2016,2,2));
      testStudent2.Save();

      //Act
      testCourse.AddStudent(testStudent1);
      List<Student> savedStudents = testCourse.GetStudents();
      List<Student> testList = new List<Student> {testStudent1};

      //Assert
      Assert.Equal(testList, savedStudents);
    }

    [Fact]
    public void Test_Delete_DeletesCourseAssociationsFromDatabase()
    {
      //Arrange
      Student testStudent = new Student("larry",new DateTime(2016,2,2));
      testStudent.Save();

      string courseName = "english";
      Course testCourse = new Course(courseName, 1);
      testCourse.Save();

      //Act
      testCourse.AddStudent(testStudent);
      testCourse.Delete();

      List<Course> resultStudentCourses = testStudent.GetCourses();
      List<Course> testStudentCourses = new List<Course> {};

      //Assert
      Assert.Equal(testStudentCourses, resultStudentCourses);
    }

    public void Dispose()
    {
        Course.DeleteAll();
        Student.DeleteAll();
    }
  }
}
