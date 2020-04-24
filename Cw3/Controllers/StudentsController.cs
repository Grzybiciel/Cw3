using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Cw3.DAL;
using Cw3.DTOs.Requests;
using Cw3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        public IConfiguration Configuration { get; set; }
        public StudentsController(IDbService dbService, IConfiguration configuration)
        {
            Configuration = configuration;
            _dbService = dbService;
        }

        [HttpGet]
   //     [Authorize(Roles = "Lucas")]
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
        public IActionResult Login(LoginRequestDTO request)
        {
            var login = request.Login;
            var haslo = request.Haslo;
            String FirstName;
            String LastName;
            String Id;
            String Password;
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18445; Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT * FROM Student where IndexNumber = @indexNum AND Password = @pass";
                com.Parameters.AddWithValue("indexNum", login);
                com.Parameters.AddWithValue("pass", haslo);

                client.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    FirstName = dr["FirstName"].ToString();
                    LastName = dr["LastName"].ToString();
                    Id = request.Login;
                    Password = request.Haslo;
                }
                else
                { 
                    return BadRequest("Nie ma takiego loginu lub hasła");
                }

            }
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Id),
                new Claim(ClaimTypes.Name, FirstName+" "+LastName),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role,"student"),
                new Claim(ClaimTypes.Role,"employee"),
    //            new Claim(ClaimTypes.Role,login)//można podać jako login Lucas i potem httpGet będzie lub nie będzie działać
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );
            
            return Ok(new
            {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken = Guid.NewGuid()
            });
        }

  /*      [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            //... add to database
            //... generating index number
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }
  */
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