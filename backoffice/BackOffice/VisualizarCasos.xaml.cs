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

    public partial class VisualizarCasos : Window
    {
        private Data.EntitiesIC data = new Data.EntitiesIC();
        private int idIC;
        public VisualizarCasos(int idIC)
        {
            this.idIC = idIC;
            InitializeComponent();
            var listaCasos = data.CasosAtivos(idIC);
            dataGridCasos.ItemsSource = listaCasos;

        }

        private void buttonSair_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
