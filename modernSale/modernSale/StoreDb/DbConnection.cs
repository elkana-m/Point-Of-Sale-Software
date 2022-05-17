using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace modernSale.StoreDb
{
    public abstract class DbConnection
    {
        private readonly string connectionStr;                // connection string to the local database

        public DbConnection()
        {
            connectionStr = @"Server=(local); DataBase=MORANNSTORE; Integrated Security = true"; //"Server=(local); DataBase=MORANNSTORE; Integrated Security=true"Data Source=DESKTOP - T3B6O70; Initial Catalog ;User Instance=false;Connect Timeout=100 ;
        }
        
        protected SqlConnection GetConnection()     
        {
            // return connection to the database

            return new SqlConnection(connectionStr);
        }
        

    }
}
