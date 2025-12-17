using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MMC_Software
{
    public class RepositoryTarifas
    {
        private string _ConexionSql;

        public RepositoryTarifas(string conexionSql)
        {
            _ConexionSql = conexionSql;
        }

    }
}
