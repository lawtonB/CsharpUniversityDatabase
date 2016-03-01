using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace University
{
  public class Student
  {
    private int _id;
    private string _name;
    private DateTime _enrollmentDate;

    public Student(string Name, DateTime EnrollmentDate, int Id = 0)
    {
      _id = Id;
      _name = Name;
      _enrollmentDate = EnrollmentDate;
    }

    public override bool Equals(System.Object otherStudent)
    {
      if (!(otherStudent is Student))
      {
        return false;
      }
      else
      {
        Student newStudent = (Student) otherStudent;
          bool idEquality = this.GetId() == newStudent.GetId();
          bool nameEquality = this.GetName() == newStudent.GetName();
          bool EnrollmentDateEquality = this.GetEnrollmentDate() == newStudent.GetEnrollmentDate();
          return (idEquality && nameEquality && EnrollmentDateEquality);
      }
    }

    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }
    public DateTime GetEnrollmentDate()
    {
      return _enrollmentDate;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }
    public void SetEnrollmentDate(DateTime newEnrollmentDate)
    {
      _enrollmentDate = newEnrollmentDate;
    }

    public static List<Student> GetAll()
    {
      List<Student> allStudents = new List<Student>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM students;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int studentId = rdr.GetInt32(0);
        string studentName = rdr.GetString(1);
        DateTime studentEnrollmentDate = rdr.GetDateTime(2);
        Student newStudent = new Student(studentName, studentEnrollmentDate, studentId);
        allStudents.Add(newStudent);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allStudents;
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM students", conn);
      cmd.ExecuteNonQuery();
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO students (name, enrollment_date) OUTPUT INSERTED.id VALUES (@StudentName, @EnrollmentDate);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@StudentName";
      nameParameter.Value = this.GetName();


      SqlParameter EnrollmentDateParameter = new SqlParameter();
      EnrollmentDateParameter.ParameterName = "@EnrollmentDate";
      EnrollmentDateParameter.Value = this.GetEnrollmentDate();

      cmd.Parameters.Add(nameParameter);
      cmd.Parameters.Add(EnrollmentDateParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
    }
  }

  public static Student Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM students WHERE id = @StudentId;", conn);
      SqlParameter StudentIdParameter = new SqlParameter();
      StudentIdParameter.ParameterName = "@StudentId";
      StudentIdParameter.Value = id.ToString();
      cmd.Parameters.Add(StudentIdParameter);
      rdr = cmd.ExecuteReader();

      int foundStudentId = 0;
      string foundStudentDescription = null;
      DateTime foundStudentEnrollmentDate = new DateTime(2000,1,1);

      while(rdr.Read())
      {
        foundStudentId = rdr.GetInt32(0);
        foundStudentDescription = rdr.GetString(1);
        foundStudentEnrollmentDate = rdr.GetDateTime(2);
      }
      Student foundStudent = new Student(foundStudentDescription, foundStudentEnrollmentDate, foundStudentId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundStudent;
    }

    public void AddCourse(Course newCourse)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO students_courses (course_id, student_id) VALUES (@CourseId, @StudentId);", conn);

      SqlParameter courseIdParameter = new SqlParameter();
      courseIdParameter.ParameterName = "@CourseId";
      courseIdParameter.Value = newCourse.GetId();
      cmd.Parameters.Add(courseIdParameter);

      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = this.GetId();
      cmd.Parameters.Add(studentIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Course> GetCourses()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT course_id FROM students_courses WHERE student_id = @StudentId;", conn);

      SqlParameter studentIdParameter = new SqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = this.GetId();
      cmd.Parameters.Add(studentIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> courseIds = new List<int> {};

      while (rdr.Read())
      {
        int courseId = rdr.GetInt32(0);
        courseIds.Add(courseId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<Course> courses = new List<Course> {};

      foreach (int courseId in courseIds)
      {
        SqlDataReader queryReader = null;
        SqlCommand courseQuery = new SqlCommand("SELECT * FROM courses WHERE id = @CourseId;", conn);

        SqlParameter courseIdParameter = new SqlParameter();
        courseIdParameter.ParameterName = "@CourseId";
        courseIdParameter.Value = courseId;
        courseQuery.Parameters.Add(courseIdParameter);

        queryReader = courseQuery.ExecuteReader();
        while (queryReader.Read())
        {
          int CourseId = queryReader.GetInt32(0);
          string courseName = queryReader.GetString(1);
          int courseNumber = queryReader.GetInt32(2);
          Course foundCourse = new Course(courseName, courseNumber, CourseId);
          courses.Add(foundCourse);
        }
        if (queryReader != null)
        {
          queryReader.Close();
        }
      }
      if (conn != null)
      {
        conn.Close();
      }
      return courses;
    }



  }
}
