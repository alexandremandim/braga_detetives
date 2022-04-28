using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BackOffice
{
    /// <summary>
    /// Interaction logic for VerificarTarefas.xaml
    /// </summary>
    public partial class VerificarTarefas : Window
    {
        private Data.EntitiesIC dataIC = new Data.EntitiesIC();
        private int id;
        public VerificarTarefas(int id)
        {
            this.id = id;
            InitializeComponent();

            comboBox.ItemsSource = (from u in dataIC.Utilizador where u.IdChefe == this.id
                                    select new {u.IdUtilizador, u.Nome}).ToList();
            comboBox.DisplayMemberPath = "Nome";
            comboBox.SelectedValuePath = "IdUtilizador";
        }

        private void sairButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedIndex > -1)
            {
                int idUtilizador = (int)comboBox.SelectedValue;
                this.dataGridTarefa.ItemsSource = (from t in dataIC.TarefasIC(id) where t.IdAgente == idUtilizador
                                             select new { t.IdTarefa, t.Titulo, t.Descricao,t.Objetivos,t.Latitude,t.Longitude,t.NomeCaso,t.IdAgente,
                                             t.Agente, t.Realizada, t.Exportada}).ToList();
            }
        }
    }
}
