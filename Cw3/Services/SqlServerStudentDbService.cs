using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace Cw3.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            var st = new Student();
            st.FirstName = request.FirstName;
            st.LastName = request.LastName;
            st.IndexNumber = request.IndexNumber;
            st.Birthdate = DateTime.Now.Date;
            //Console.WriteLine(DateTime.Now.Date);
            int sem = -1, idS = -1; DateTime sDate = DateTime.Now; int idEnrollment = -1;

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18445; Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                try
                {
                    com.Transaction = tran;
                    com.CommandText = "select IdStudy from studies where name = @name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    var dr = com.ExecuteReader();

                    if (!dr.Read())
                    {
                        tran.Rollback();
                        //return BadRequest("Studia nie istnieja");

                    }
                    int idStudies = (int)dr["IdStudy"];

                    com.CommandText = "Select TOP (1) IDENROLLMENT, SEMESTER, ENROLLMENT.IDSTUDY, STARTDATE FROM ENROLLMENT, STUDIES " +
                                        "WHERE ENROLLMENT.IDSTUDY = STUDIES.IDSTUDY " +
                                        "AND SEMESTER = 1 " +
                                        "AND NAME = @nNAME " +
                                        "ORDER BY STARTDATE DESC";
                    com.Parameters.AddWithValue("nNAME", request.Studies);
                    dr.Close();
                    dr = com.ExecuteReader();

                    if (!dr.Read())
                    {
                        com.CommandText = "INSERT INTO ENROLLMENT VALUES( (SELECT MAX(IDENROLLMENT)+1 FROM ENROLLMENT),1,@idStudies,@dateNow)";
                        com.Parameters.AddWithValue("idStudies", idStudies);
                        com.Parameters.AddWithValue("dateNow", DateTime.Now.Date);
                        dr.Close();
                        com.ExecuteNonQuery();

                        com.CommandText = "SELECT TOP(1) * FROM ENROLLMENT ORDER BY IDENROLLMENT DESC";
                        dr = com.ExecuteReader();
                        dr.Read();
                        Console.WriteLine(dr.ToString());
                        idEnrollment = (int)dr["IDENROLLMENT"];

                    }
                    else
                    {
                        idEnrollment = (int)dr["IDENROLLMENT"];
                    }

                    //dodanie studenta
                    com.CommandText = "SELECT * FROM STUDENT WHERE INDEXNUMBER = @indexnumber";
                    com.Parameters.AddWithValue("indexnumber", request.IndexNumber);
                    dr.Close();
                    dr = com.ExecuteReader();

                    if (dr.Read())
                    {
                        //return BadRequest("Student o tym indeksie juz istnieje");
                    }
                    dr.Close();
                    com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES(@Index, @Fname, @LName, @Bday, @IdEn)";
                    com.Parameters.AddWithValue("Index", request.IndexNumber);
                    com.Parameters.AddWithValue("Fname", request.FirstName);
                    com.Parameters.AddWithValue("Lname", request.LastName);
                    string dt = request.Birthdate.Replace(".", "-");
                    string tmp = dt.Substring(6, 4) + "-" + dt.Substring(3, 2) + "-" + dt.Substring(0, 2);
                    //10.10.1999 -> 1999-01-01
                    com.Parameters.AddWithValue("Bday", tmp);
                    com.Parameters.AddWithValue("IdEn", idEnrollment);
                    com.ExecuteNonQuery();

                    com.CommandText = "Select * from Enrollment where idenrollment = @IdEnz";
                    com.Parameters.AddWithValue("IdEnz", idEnrollment);
                    dr = com.ExecuteReader();
                    dr.Read();
                    sem = (int)dr["Semester"];
                    idS = (int)dr["IdStudy"];
                    sDate = ((DateTime)dr["StartDate"]);
                    dr.Close();

                    tran.Commit();


                }
                catch (SqlException exc)
                {

                    tran.Rollback();

                    // return BadRequest(exc.Message);
                }

            }
            var response = new EnrollStudentResponse();
            response.idEnrollment = idEnrollment;
            response.Semester = sem;
            response.IdStudy = idS;
            response.StartDate = sDate;

            return response;
        }


        public EnrollStudentResponse PromoteStudent(PromotionRequest request)
        {
            var response = new EnrollStudentResponse();
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18445; Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                com.Transaction = tran;
                com.CommandText = "EXEC PromoteStudents @stud,@sem";
                com.Parameters.AddWithValue("stud", request.Studies);
                com.Parameters.AddWithValue("sem", request.Semester);
                try
                {
                    com.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                com.CommandText = "Select * from Enrollment, Studies where Enrollment.IdStudy = Studies.IdStudy and name = @stud and Semester = @sem+1";
                var dr = com.ExecuteReader();
                dr.Read();
                var idEnrollment = (int)dr["idEnrollment"];
                var sem = (int)dr["Semester"];
                var idS = (int)dr["IdStudy"];
                var sDate = ((DateTime)dr["StartDate"]);
                dr.Close();
                response.idEnrollment = idEnrollment;
                response.Semester = sem;
                response.IdStudy = idS;
                response.StartDate = sDate;
            }
            return response;
        }
    }
}
