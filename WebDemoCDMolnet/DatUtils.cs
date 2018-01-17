using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace WebDemoCDMolnet
{
    public class DatUtils
    {
        public static IEnumerable<Artist> GetGrupper()
        {
            string sConn = "Data Source=WIN8-HORJA\\SQLEXPRESS;database=CDBD;uid=sapjappl;pwd=gunners";
            List<Artist> list = new List<Artist>();
            using (SqlConnection con = new SqlConnection(sConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM vArtister WHERE UserId = 1";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Artist A = new Artist();
                        A.artist = reader["Artist"].ToString().Trim();
                        list.Add(A);
                    }
                }
            }
            return list;
        }
        public static IEnumerable<T> GetPage<T>(int pageNumber, int pageSize, IEnumerable<T> list)
        {
            var output = list
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize);
            return output;
        }
    }
}