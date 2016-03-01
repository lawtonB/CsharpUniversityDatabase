using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace University
{
  public class Course
  {
    private int _id;
    private string _name;
    private int _courseNumber;

    public Course(string Name, int CourseNumber, int Id = 0)
    {
      _id = Id;
      _name = Name;
      _courseNumber = CourseNumber;
    }

    public int GetId()
    {
      return _id;
    }

    public string GetName()
    {
      return _name;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }

    public int GetCourseNumber()
    {
      return _courseNumber;
    }
    public void SetCourseNumber(int newCourseNumber)
    {
      _courseNumber = newCourseNumber;
    }

    public override bool Equals(System.Object otherCourse)
    {
      if (!(otherCourse is Course))
      {
        return false;
      }
      else
      {
        Course newCourse = (Course) otherCourse;
        bool idEquality = this.GetId() == newCourse.GetId();
        bool nameEquality = this.GetName() == newCourse.GetName();
        bool courseNumberEquality = this.GetCourseNumber() == newCourse.GetCourseNumber();
        return (idEquality && nameEquality && courseNumberEquality);
      }
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO courses (name, course_number) OUTPUT INSERTED.id VALUES (@StudentName, @CourseNumber);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@StudentName";
      nameParameter.Value = this.GetName();


      SqlParameter CourseNumberParameter = new SqlParameter();
      CourseNumberParameter.ParameterName = "@CourseNumber";
      CourseNumberParameter.Value = this.GetCourseNumber();

      cmd.Parameters.Add(nameParameter);
      cmd.Parameters.Add(CourseNumberParameter);
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

  public List<Student> GetStudents()
{
  SqlConnection conn = DB.Connection();
  SqlDataReader rdr = null;
  conn.Open();

  SqlCommand cmd = new SqlCommand("SELECT student_id FROM students_courses WHERE course_id = @CourseId;", conn);
  SqlParameter courseIdParameter = new SqlParameter();
  courseIdParameter.ParameterName = "@CourseId";
  courseIdParameter.Value = this.GetId();
  cmd.Parameters.Add(courseIdParameter);

  rdr = cmd.ExecuteReader();

  List<int> studentIds = new List<int> {};
  while(rdr.Read())
  {
    int studentId = rdr.GetInt32(0);
    studentIds.Add(studentId);
  }
  if (rdr != null)
  {
    rdr.Close();
  }

  List<Student> students = new List<Student> {};
  foreach (int studentId in studentIds)
  {
    SqlDataReader queryReader = null;
    SqlCommand studentQuery = new SqlCommand("SELECT * FROM students WHERE id = @StudentId;", conn);

    SqlParameter studentIdParameter = new SqlParameter();
    studentIdParameter.ParameterName = "@StudentId";
    studentIdParameter.Value = studentId;
    studentQuery.Parameters.Add(studentIdParameter);

    queryReader = studentQuery.ExecuteReader();
    while(queryReader.Read())
    {
      int studentStudentId = queryReader.GetInt32(0);
      string studentName = queryReader.GetString(1);
      DateTime studentEnrollmentDate = queryReader.GetDateTime(2);
      Student foundStudent = new Student(studentName, studentEnrollmentDate, studentStudentId);
      students.Add(foundStudent);
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
  return students;
  }


  }
}
