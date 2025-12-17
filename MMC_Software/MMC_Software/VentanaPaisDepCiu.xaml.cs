using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MMC_Software
{
    public partial class VentanaPaisDepCiu : UserControl
    {
        public VentanaPaisDepCiu()
        {
            InitializeComponent();
            CargarTodo();
        }

        #region --- Helpers / Conexión SQL ---

        private SqlConnection CrearConexion()
        {
            string cadena = ConfiguracionConexion.ObtenerCadenaConexion(ConexionEmpresaActual.BaseDeDatosSeleccionada);
            return new SqlConnection(cadena);
        }

        private void CargarTodo()
        {
            try
            {
                MuestraPais();
                MuestraDepartamento();
                MuestraCiudad();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Error al cargar los datos iniciales.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region --- Países ---

        private void MuestraPais()
        {
            try
            {
                listPaises.Items.Clear();

                using (SqlConnection conexion = CrearConexion())
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SELECT PaisCodigo, PaisNombre FROM ConfPais ORDER BY PaisNombre", conexion);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        listPaises.Items.Add($"{dr["PaisCodigo"]} - {dr["PaisNombre"]}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Error al mostrar los países.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonCrear_Click(object sender, RoutedEventArgs e)
        {
            txtCodigoPais.Clear();
            txtNombrePais.Clear();
            txtCodigoPais.IsEnabled = true;
            txtNombrePais.IsEnabled = true;
            txtCodigoPais.Focus();
        }

        private void ButtonModificar_Click(object sender, RoutedEventArgs e)
        {
            txtCodigoPais.IsEnabled = false;
            txtNombrePais.IsEnabled = true;
            txtNombrePais.Focus();
        }

        private void ButtonGuarda_Click(object sender, RoutedEventArgs e)
        {
            GuardarActualizarPais();
        }

        private void GuardarActualizarPais()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCodigoPais.Text) || txtCodigoPais.Text.Length > 5)
                {
                    MessageBox.Show("Código de país inválido (máx. 5 caracteres).", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNombrePais.Text))
                {
                    MessageBox.Show("Debe ingresar el nombre del país.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (SqlConnection conexion = CrearConexion())
                {
                    conexion.Open();

                    SqlCommand existe = new SqlCommand("SELECT COUNT(*) FROM ConfPais WHERE PaisCodigo=@codigo", conexion);
                    existe.Parameters.Add("@codigo", SqlDbType.VarChar, 10).Value = txtCodigoPais.Text;
                    int count = Convert.ToInt32(existe.ExecuteScalar());

                    if (count > 0)
                    {
                        SqlCommand update = new SqlCommand("UPDATE ConfPais SET PaisNombre=@nombre WHERE PaisCodigo=@codigo", conexion);
                        update.Parameters.Add("@nombre", SqlDbType.VarChar, 50).Value = txtNombrePais.Text;
                        update.Parameters.Add("@codigo", SqlDbType.VarChar, 10).Value = txtCodigoPais.Text;
                        update.ExecuteNonQuery();

                        MessageBox.Show("✅ País actualizado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        SqlCommand insert = new SqlCommand("INSERT INTO ConfPais (PaisCodigo, PaisNombre) VALUES (@codigo, @nombre)", conexion);
                        insert.Parameters.Add("@codigo", SqlDbType.VarChar, 10).Value = txtCodigoPais.Text;
                        insert.Parameters.Add("@nombre", SqlDbType.VarChar, 50).Value = txtNombrePais.Text;
                        insert.ExecuteNonQuery();

                        MessageBox.Show("✅ País creado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                MuestraPais();
                txtCodigoPais.IsEnabled = false;
                txtNombrePais.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Error al guardar el país.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (listPaises.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un país para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string paisSeleccionado = listPaises.SelectedItem.ToString().Split('-')[0].Trim();

            if (MessageBox.Show("¿Está seguro de eliminar el país seleccionado?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection conexion = CrearConexion())
                    {
                        conexion.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM ConfPais WHERE PaisCodigo=@codigo", conexion);
                        cmd.Parameters.Add("@codigo", SqlDbType.VarChar, 10).Value = paisSeleccionado;
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("✅ País eliminado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    MuestraPais();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    MessageBox.Show("Error al eliminar el país.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void listPaises_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listPaises.SelectedItem != null)
            {
                string pais = listPaises.SelectedItem.ToString();
                string[] partes = pais.Split('-');
                txtCodigoPais.Text = partes[0].Trim();
                txtNombrePais.Text = partes[1].Trim();
            }
        }

        #endregion

        #region --- Departamentos ---

        private void MuestraDepartamento()
        {
            try
            {
                listDepartamentos.Items.Clear();
                cmbPaisDepartamento.Items.Clear();

                using (SqlConnection conexion = CrearConexion())
                {
                    conexion.Open();

                    SqlCommand cmd = new SqlCommand("SELECT DepartamentoCodigo, DepartamentoNombre, PaisID FROM ConfDepartamento ORDER BY DepartamentoNombre", conexion);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        listDepartamentos.Items.Add($"{dr["DepartamentoCodigo"]} - {dr["DepartamentoNombre"]}");
                    }
                    dr.Close();

                    SqlCommand paises = new SqlCommand("SELECT PaisID, PaisNombre FROM ConfPais ORDER BY PaisNombre", conexion);
                    SqlDataAdapter da = new SqlDataAdapter(paises);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbPaisDepartamento.ItemsSource = dt.DefaultView;
                    cmbPaisDepartamento.DisplayMemberPath = "PaisNombre";
                    cmbPaisDepartamento.SelectedValuePath = "PaisID";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Error al mostrar los departamentos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // <-- Evento restaurado que pediste -->
        private void cmbPaisDepartamentoList_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = CrearConexion())
                {
                    conexion.Open();
                    SqlDataAdapter adp = new SqlDataAdapter("SELECT PaisID, PaisNombre FROM ConfPais ORDER BY PaisNombre", conexion);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);

                    cmbPaisDepartamento.ItemsSource = dt.DefaultView;
                    cmbPaisDepartamento.DisplayMemberPath = "PaisNombre";
                    cmbPaisDepartamento.SelectedValuePath = "PaisID";
                    cmbPaisDepartamento.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Error cargando países.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonCrearDep_Click(object sender, RoutedEventArgs e)
        {
            txtCodigoDepartamento.Clear();
            txtNombreDepartamento.Clear();
            txtIsoDepartamento.Clear();

            txtCodigoDepartamento.IsEnabled = true;
            txtNombreDepartamento.IsEnabled = true;
            txtIsoDepartamento.IsEnabled = true;
            cmbPaisDepartamento.IsEnabled = true;
        }

        private void ButtonModificadep_Click(object sender, RoutedEventArgs e)
        {
            txtCodigoDepartamento.IsEnabled = false;
            txtNombreDepartamento.IsEnabled = true;
            txtIsoDepartamento.IsEnabled = true;
            cmbPaisDepartamento.IsEnabled = true;
        }

        private void ButtonGuardaDepartamento_Click(object sender, RoutedEventArgs e)
        {
            GuardarDepartamento();
        }

        private void GuardarDepartamento()
        {
            try
            {
                if (cmbPaisDepartamento.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un país.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (SqlConnection conexion = CrearConexion())
                {
                    conexion.Open();

                    SqlCommand cmd = new SqlCommand(@"
                        IF EXISTS (SELECT 1 FROM ConfDepartamento WHERE DepartamentoCodigo=@codigo)
                            UPDATE ConfDepartamento SET DepartamentoNombre=@nombre, DepartamentoISO=@iso, PaisID=@pais WHERE DepartamentoCodigo=@codigo
                        ELSE
                            INSERT INTO ConfDepartamento (PaisID, DepartamentoCodigo, DepartamentoNombre, DepartamentoISO)
                            VALUES (@pais, @codigo, @nombre, @iso)", conexion);

                    cmd.Parameters.Add("@codigo", SqlDbType.VarChar, 10).Value = txtCodigoDepartamento.Text;
                    cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 80).Value = txtNombreDepartamento.Text;
                    cmd.Parameters.Add("@iso", SqlDbType.VarChar, 10).Value = txtIsoDepartamento.Text;
                    cmd.Parameters.Add("@pais", SqlDbType.Int).Value = cmbPaisDepartamento.SelectedValue;

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Departamento guardado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                MuestraDepartamento();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Error al guardar el departamento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonEliminarDep_Click(object sender, RoutedEventArgs e)
        {
            if (listDepartamentos.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un departamento para eliminar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string codigo = listDepartamentos.SelectedItem.ToString().Split('-')[0].Trim();

            if (MessageBox.Show("¿Eliminar este departamento?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection conexion = CrearConexion())
                    {
                        conexion.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM ConfDepartamento WHERE DepartamentoCodigo=@codigo", conexion);
                        cmd.Parameters.Add("@codigo", SqlDbType.VarChar, 10).Value = codigo;
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("✅ Departamento eliminado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    MuestraDepartamento();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    MessageBox.Show("Error al eliminar el departamento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void listDepartamentos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listDepartamentos.SelectedItem != null)
            {
                string dep = listDepartamentos.SelectedItem.ToString();
                string[] partes = dep.Split('-');
                txtCodigoDepartamento.Text = partes[0].Trim();
                txtNombreDepartamento.Text = partes[1].Trim();
            }
        }

        #endregion

        #region --- Ciudades ---

        private void MuestraCiudad()
        {
            try
            {
                listCiudades.Items.Clear();
                cmbDepartamentoCiudad.Items.Clear();
                cmbPaisCiudad.Items.Clear();

                using (SqlConnection conexion = CrearConexion())
                {
                    conexion.Open();

                    SqlCommand cmd = new SqlCommand("SELECT CiudadCodigo, CiudadNombre FROM ConfCiudad ORDER BY CiudadNombre", conexion);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        listCiudades.Items.Add($"{dr["CiudadCodigo"]} - {dr["CiudadNombre"]}");
                    }
                    dr.Close();

                    SqlDataAdapter daDep = new SqlDataAdapter("SELECT DepartamentoID, DepartamentoNombre FROM ConfDepartamento ORDER BY DepartamentoNombre", conexion);
                    DataTable dtDep = new DataTable();
                    daDep.Fill(dtDep);
                    cmbDepartamentoCiudad.ItemsSource = dtDep.DefaultView;
                    cmbDepartamentoCiudad.DisplayMemberPath = "DepartamentoNombre";
                    cmbDepartamentoCiudad.SelectedValuePath = "DepartamentoID";

                    SqlDataAdapter daPais = new SqlDataAdapter("SELECT PaisID, PaisNombre FROM ConfPais ORDER BY PaisNombre", conexion);
                    DataTable dtPais = new DataTable();
                    daPais.Fill(dtPais);
                    cmbPaisCiudad.ItemsSource = dtPais.DefaultView;
                    cmbPaisCiudad.DisplayMemberPath = "PaisNombre";
                    cmbPaisCiudad.SelectedValuePath = "PaisID";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Error al mostrar las ciudades.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void listCiudades_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listCiudades.SelectedItem != null)
            {
                string ciudad = listCiudades.SelectedItem.ToString();
                string[] partes = ciudad.Split('-');
                txtCodigoCiudad.Text = partes[0].Trim();
                txtNombreCiudad.Text = partes[1].Trim();
                // Si quieres también seleccionar departamento/pais al hacer doble click,
                // podríamos cargar eso acá (lo dejamos simple por ahora).
            }
        }

        #endregion
    }
}
