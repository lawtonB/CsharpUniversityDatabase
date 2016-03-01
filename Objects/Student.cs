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



  }
}
