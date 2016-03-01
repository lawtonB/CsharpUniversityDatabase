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

  public static Course Find(int id)
  {
    SqlConnection conn = DB.Connection();
    SqlDataReader rdr = null;
    conn.Open();

    SqlCommand cmd = new SqlCommand("SELECT * FROM courses WHERE id = @CourseId;", conn);
    SqlParameter courseIdParameter = new SqlParameter();
    courseIdParameter.ParameterName = "@CourseId";
    courseIdParameter.Value = id.ToString();
    cmd.Parameters.Add(courseIdParameter);
    rdr = cmd.ExecuteReader();

    int foundCourseId = 0;
    string foundCourseName = null;
    int foundCourseNumber = 0;

    while(rdr.Read())
    {
      foundCourseId = rdr.GetInt32(0);
      foundCourseName = rdr.GetString(1);
      foundCourseNumber = rdr.GetInt32(2);
    }
    Course foundCourse = new Course(foundCourseDescription, foundCourseNumber, foundCourseId);

    if (rdr != null)
    {
      rdr.Close();
    }
    if (conn != null)
    {
      conn.Close();
    }
    return foundCourse;
  }

  public void Update(string newCourseName, int newCourseNumber)
  {
    SqlConnection conn = DB.Connection();
    SqlDataReader rdr;
    conn.Open();

    SqlCommand cmd = new SqlCommand("UPDATE courses SET name = @CourseName OUTPUT INSERTED.name WHERE id = @CourseId; UPDATE courses SET course_id = @CourseNumber OUTPUT INSERTED.course_id WHERE id = @CourseId;", conn);

    SqlParameter newNameParameter = new SqlParameter();
    newNameParameter.ParameterName = "@CourseName";
    newNameParameter.Value = newCourseName;
    cmd.Parameters.Add(newNameParameter);

    SqlParameter newCourseNumberParameter = new SqlParameter();
    newCourseNumberParameter.ParameterName = "@CourseNumber";
    newCourseNumberParameter.Value = newCourseNumber;
    cmd.Parameters.Add(newCourseNumberParameter);


    SqlParameter courseIdParameter = new SqlParameter();
    courseIdParameter.ParameterName = "@CourseId";
    courseIdParameter.Value = this.GetId();
    cmd.Parameters.Add(courseIdParameter);
    rdr = cmd.ExecuteReader();

    while(rdr.Read())
    {
      this._name = rdr.GetString(0);
    }

    if (rdr != null)
    {
      rdr.Close();
    }

    if (conn != null)
    {
      conn.Close();
    }
  }

  public void AddStudent(Student newStudent)
  {
    SqlConnection conn = DB.Connection();
    conn.Open();

    SqlCommand cmd = new SqlCommand("INSERT INTO students_courses (course_id, student_id) VALUES (@CourseId, @StudentId)", conn);
    SqlParameter courseIdParameter = new SqlParameter();
    courseIdParameter.ParameterName = "@CourseId";
    courseIdParameter.Value = this.GetId();
    cmd.Parameters.Add(courseIdParameter);

    SqlParameter studentIdParameter = new SqlParameter();
    studentIdParameter.ParameterName = "@StudentId";
    studentIdParameter.Value = newStudent.GetId();
    cmd.Parameters.Add(studentIdParameter);

    cmd.ExecuteNonQuery();

    if (conn != null)
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
      if (conn != null)
      {
        conn.Close();
      }
        return students;
      }
    }


    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM courses WHERE id = @CourseId; DELETE FROM students_courses WHERE course_id = @CourseId;", conn);

      SqlParameter courseIdParameter = new SqlParameter();
      courseIdParameter.ParameterName = "@CourseId";
      courseIdParameter.Value = this.GetId();

      cmd.Parameters.Add(courseIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM courses;", conn);
      cmd.ExecuteNonQuery();
    }

    public static List<Course> GetAll()
    {
      List<Course> allCourses = new List<Course>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM courses;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int courseId = rdr.GetInt32(0);
        string courseName = rdr.GetString(1);
        int courseNumber = rdr.GetInt32(2);
        Course newCourse = new Course(courseName, courseNumber, courseId);
        allCourses.Add(newCourse);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allCourses;
    }
  }
}
