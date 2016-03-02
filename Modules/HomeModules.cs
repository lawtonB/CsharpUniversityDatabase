using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ViewEngines.Razor;

namespace University
{
    public class HomeModule : NancyModule
    {
      public HomeModule()
      {
        Get["/"] = _ => {
          return View["index.cshtml"];
        };
        Get["/students"] = _ => {
          List<Student> allStudents = Student.GetAll();
          return View["student.cshtml", allStudents];
        };
        Get["/courses"] = _ => {
          List<Course> allCourses = Course.GetAll();
          return View["courses.cshtml", allCourses];
        };
        //make new student
        Get["/students/new"] = _ => {
          return View["student.cshtml"];
        };
        Post["/students/new"] = _ => {
          DateTime newDateTime = Convert.ToDateTime((string)Request.Form["enrollment-date"]);
          Student newStudent = new Student(Request.Form["student-name"], newDateTime);
          newStudent.Save();
          return View["student.cshtml"];
        };
        //make new course
        Get["/courses/new"] = _ => {
          Dictionary<string, object> model = new Dictionary<string, object>();
          student newStudent = new Student(Request.Form["student-name"], newDateTime);
          Course newcourse = Course.Find(parameters.id);
          var studentClass = newStudent.GetClass();
          model.Add("student", newStudent);
          model.Add("course", newcourse);
          model.Add("class", studentClass);

          return View["courses_form.cshtml", model];
        };
        Post["/courses/new"] = _ => {
          Course newCourse = new Course(Request.Form["course-name"], Request.Form["course-number"]);
          newCourse.Save();
          return View["success.cshtml"];
        };
        Get["/courses/delete/{id}"] = parameters => {
          Course newCourse = Course.Find(parameters.id);
          newCourse.Delete();
          List<Course> AllCourses = Course.GetAll();
          return View["courses.cshtml", AllCourses];
        };
        Get["students/{id}"] = parameters => {
          Dictionary<string, object> model = new Dictionary<string, object>();
          Student selectedStudent = Student.Find(parameters.id);
          List<Course> StudentCourses = selectedStudent.GetCourses();
          List<Course> AllCourses = Course.GetAll();
          model.Add("student", selectedStudent);
          model.Add("courses", StudentCourses);
          model.Add("allcourses", AllCourses);
          return View["Courses.cshtml"];
        };
        Post["students/{id}"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>();
        DateTime newDateTime = Convert.ToDateTime((string)Request.Form["enrollment-date"]);
        Student SelectedStudent = Student.Find(parameters.id);
        SelectedStudent.Update(Request.Form["name"], newDateTime);
        List<Course> StudentCourses = SelectedStudent.GetCourses();
        List<Course> AllCourses = Course.GetAll();
        model.Add("student", SelectedStudent);
        model.Add("studentCourses", StudentCourses);
        model.Add("allCourses", AllCourses);
        return View["student.cshtml", model];
      };

      Get["courses/{id}"] = parameters => {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Course SelectedCourse = Course.Find(parameters.id);
      List<Student> CourseStudents = SelectedCourse.GetStudents();
      List<Student> AllStudents = Student.GetAll();
      model.Add("course", SelectedCourse);
      model.Add("courseStudents", CourseStudents);
      model.Add("allStudents", AllStudents);
      return View["course.cshtml", model];
      };

      Post["task/add_course"] = _ => {
      Course newcourse = (Request.Form["course-number"]);
      newcourse.Save();
      // Student newStudent = Student.Find(Request.Form["student-name"], Request.Form["course-number"]);
      // Student.AddCourse(course);
      return View["success.cshtml"];
    };


      }
    }
}
