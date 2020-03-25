using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DAL;
using Cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            List<Object> listQ = new List<Object>();
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18445; Integrated Security=True"))
            using (var com = new SqlCommand())
                
            {
                com.Connection = client;
                com.CommandText = "select FirstName, LastName, BirthDate, Name, Semester " +
                    "from Student, Studies, Enrollment " +
                    "where Student.IdEnrollment = Enrollment.IdEnrollment and Studies.IdStudy = Enrollment.IdStudy";


                client.Open();
                var dr = com.ExecuteReader();
                while(dr.Read())
                {
                    var st = new
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = DateTime.Parse(dr["BirthDate"].ToString()),
                        Name = dr["Name"].ToString(),
                        Semester = dr["Semester"].ToString(),
                    };
                    listQ.Add(st);
                }

            }

            return Ok(listQ);
                    
        }

        [HttpGet("{id}")]
        public IActionResult GetStudents(int id)
        {
            List<Object> listQ = new List<Object>();
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18445; Integrated Security=True"))
            using (var com = new SqlCommand())

            {
                com.Connection = client;
                com.CommandText = "SELECT FirstName, LastName, BirthDate, Name, Semester " +
                    "FROM Student, Studies, Enrollment " +
                    "WHERE Student.IdEnrollment = Enrollment.IdEnrollment " +
                    "AND Studies.IdStudy = Enrollment.IdStudy " +
                    "AND IndexNumber = @id";

                com.Parameters.AddWithValue("id", id);

                client.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = DateTime.Parse(dr["BirthDate"].ToString()),
                        Name = dr["Name"].ToString(),
                        Semester = dr["Semester"].ToString(),
                    };
                    listQ.Add(st);
                }

            }
            if (listQ.Count != 0)
            {
                return Ok(listQ);
            }
            else
            {
                return NotFound("Nie znaleziono studenta");
            }
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            //... add to database
            //... generating index number
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }
        [HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            return Ok("Aktualizacja dokonczona");
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie ukończone");
        }


    }
}