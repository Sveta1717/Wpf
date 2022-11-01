using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WpfApp3.ADO.Entities;

namespace WpfApp3.ADO.DAL
{
    public class Managers        
    {
        private readonly SqlConnection _connection;

        public Managers(SqlConnection connection)
        {
            _connection = connection;
        }

        public List<Entities.Manager> GetList()
        {
            List<Entities.Manager> managers = new();
            using (SqlCommand cmd = _connection.CreateCommand())
            {               
                cmd.CommandText = "SELECT Id, Name, Surname, Secname, Id_main_dep, Id_sec_dep, Id_chief FROM Managers";
                using SqlDataReader res = cmd.ExecuteReader();
                while (res.Read())
                {
                    managers.Add(new Entities.Manager
                    {
                        Id          = res.GetGuid(0),
                        Name        = res.GetString(1),
                        Surname     = res.GetString(3),
                        SecName     = res.GetString(2),
                        Id_main_dep = res.GetGuid(4),
                        Id_sec_dep  = res.GetValue(5) == DBNull.Value ? null : res.GetGuid(5),
                        Id_chief    = res.GetValue(6) == DBNull.Value ? null : res.GetGuid(6)                        
                    });
                }               
            }
            return managers;
        }
    }
}
