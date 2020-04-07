using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DAL
{
    public class DbService : IDbService
    {
        public bool CheckIndex(string index)
        {
            try
            {
                using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18445; Integrated Security=True"))
                using (var com = new SqlCommand())
                {
                    com.Connection = con;
                    con.Open();

                    com.CommandText = "SELECT * FROM Student where Student.IndexNumber = " + index;
                    var dataReader = com.ExecuteReader();
                    if (dataReader.Read())
                    {
                        dataReader.Close();
                        return true;
                    }
                    else if (!dataReader.Read())
                    {
                        dataReader.Close();
                        return false;
                    }
                }
            }
            catch (SqlException ex)
            {
                return false;
            }

                //default, zawsze musi się kończyć returnem metoda, nawet jak nie wejdzie do ifa
                return false;
                
        }
    }

        //public IEnumerable<Student> GetStudents()
        //{
        //    throw new NotImplementedException();
        //}
}

