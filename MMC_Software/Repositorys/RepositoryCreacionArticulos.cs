using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMC_Software.Repositorys
{
    public class RepositoryCreacionArticulos
    {

        SqlTransaction _Trans;
        SqlConnection _conn;

        public RepositoryCreacionArticulos(SqlTransaction trans)
        {
            _Trans = trans; 
            _conn =trans.Connection;
        }
    }

}
