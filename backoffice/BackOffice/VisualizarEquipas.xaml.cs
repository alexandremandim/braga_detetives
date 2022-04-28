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
    /// Interaction logic for VisualizarEquipas.xaml
    /// </summary>
    public partial class VisualizarEquipas : Window
    {
        private int id, tipoUser;
        private Data.EntitiesDiretor data;
        private Data.EntitiesIC dataIC;

        public VisualizarEquipas(int id, int tipoUser)
        {
            this.id = id;
            this.tipoUser = tipoUser;
            InitializeComponent();
            inicializarDataGrid();
        }
        private void inicializarDataGrid()
        {
            if(tipoUser == 3) // IC e temos que apenas mostrar a sua equipa
            {
                dataIC = new Data.EntitiesIC();
                var listaByUser = (from t in dataIC.viewEquipas
                                   where t.IdInspetorChefe == this.id
                                   select new { t.IdUtilizador, t.Nome, t.TarefasDecorrer, t.InspetorChefe, t.IdInspetorChefe}).ToList();
                dataGridAgentes.ItemsSource = listaByUser;
            }
            else if(tipoUser == 1) // Diretor e mostramos todas as equipas
            {
                data = new Data.EntitiesDiretor();
                dataGridAgentes.ItemsSource = data.viewEquipas.ToList();
            }
        }
        private void sairButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
