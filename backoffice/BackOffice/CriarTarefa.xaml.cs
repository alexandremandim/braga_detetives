using System;
using System.Data;
using System.Linq;
using System.Windows;

namespace BackOffice
{
    public partial class CriarTarefa : Window
    {
        private Data.EntitiesIC data = new Data.EntitiesIC();
        private int idIC;
        public CriarTarefa(int idIC)
        {
            this.idIC = idIC;
            InitializeComponent();
            var  listaCasos = (from c in data.CasosAtivos(this.idIC)
                               where c.Terminado == false
                               select new { c.IdCaso, c.Nome, c.Descricao, c.Objetivos, c.Relatorio, c.Terminado }).ToList();
            comboBoxCaso.ItemsSource = listaCasos;
            comboBoxCaso.DisplayMemberPath = "Nome";
            comboBoxCaso.SelectedValuePath = "IdCaso";

            var listaAgentes = (from t in data.Utilizador 
                              where t.IdChefe == this.idIC
                              select new { t.IdUtilizador, t.IdTipoUtilizador, t.Nome, t.Password, t.IdChefe }).ToList();
            comboBoxAgente.ItemsSource = listaAgentes;
            comboBoxAgente.DisplayMemberPath = "Nome";
            comboBoxAgente.SelectedValuePath = "IdUtilizador";
            

        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if(txtBoxTitulo.Text != "" && txtBoxDescricao.Text != "" && txtBoxObjetivos.Text != "" && txtBoxLocal.Text != "" && comboBoxCaso.SelectedIndex > -1 && comboBoxAgente.SelectedIndex > -1)
            {
                string url = "http://maps.googleapis.com/maps/api/geocode/json?sensor=true&address=";

                Data.Tarefa novaTarefa = new Data.Tarefa();
                dynamic googleResults = new Uri(url + txtBoxLocal.Text).GetDynamicJsonObject();
                foreach (var result in googleResults.results)
                {
                    Console.WriteLine("[" + result.geometry.location.lat + "," + result.geometry.location.lng + "] " + result.formatted_address);
                    novaTarefa.Latitude = (double)result.geometry.location.lat;
                    novaTarefa.Longitude = (double)result.geometry.location.lng;
                }

                novaTarefa.Titulo = txtBoxTitulo.Text;
                novaTarefa.Descricao = txtBoxDescricao.Text;
                novaTarefa.Objetivos = txtBoxObjetivos.Text;
                novaTarefa.Realizada = false;
                novaTarefa.Exportada = false;
                novaTarefa.IdCaso = Convert.ToInt32(comboBoxCaso.SelectedValue);
                novaTarefa.IdAgente = Convert.ToInt32(comboBoxAgente.SelectedValue);
                data.Tarefa.Add(novaTarefa);
                data.SaveChanges();
                MessageBox.Show("Tarefa criada");

            }
            else
            {
                MessageBox.Show("Campos inválidos!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
               
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonVerCasos_Click(object sender, RoutedEventArgs e)
        {
            VisualizarCasos visualizarCasos = new VisualizarCasos(this.idIC);
            visualizarCasos.Show();
        }
    }
}
