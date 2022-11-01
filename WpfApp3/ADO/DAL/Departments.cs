using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WpfApp3.ADO.Entities;

namespace WpfApp3.ADO.DAL
{
    // DAL - Data Access Layer - набор "инструментов" для доступа к данным
    // из таблицы Departments
    public class Departments
    {
        private readonly SqlConnection _connection;

        public Departments(SqlConnection connection)
        {
            _connection = connection;
        }      

        public void Delete([NotNull] Entities.Department department)
        {
            using (SqlCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = String.Format(
                    "DELETE FROM Departments WHERE Id = '{0}' ",
                    department.Id);
                cmd.ExecuteNonQuery();
            }
        }
        public Guid Create([NotNull] Entities.Department department)
        {
            Guid id = Guid.NewGuid();
            using (SqlCommand cmd = _connection.CreateCommand())
            {               
                    cmd.CommandText = String.Format(
                        "INSERT INTO Departments(Id, Name) VALUES('{0}', N'{1}')",
                        id, department.Name);
                    cmd.ExecuteNonQuery();              
            }
            return id;
        }

        public void Update( [NotNull] Entities.Department department)
        {
            using (SqlCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = String.Format(
                    "UPDATE Departments SET Name = N'{0}' WHERE Id = '{1}'",
                    department.Name, department.Id);  
                cmd.ExecuteNonQuery();
            }
        }
    
        public List<Entities.Department> GetList()
        {
           using (SqlCommand cmd = _connection.CreateCommand())
            {
                List<Entities.Department> departments = new();
              
                cmd.CommandText = "SELECT Id, Name FROM Departments";

               
                SqlDataReader res = cmd.ExecuteReader();

                while (res.Read())
                {
                    departments.Add(new Entities.Department
                    {
                        Id = res.GetGuid(0),
                        Name = res.GetString(1)
                    });
                    CountManagers();
                
                }
               
              
                res.Close();
                return departments;
            }
        }


        public void CountManagers()
        {
            //var sqlCount = _connection.CreateCommand();
            var sqlCount = "SELECT COUNT(*) FROM Managers";
            
        }
    }
}
