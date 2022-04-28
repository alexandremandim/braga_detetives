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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;

namespace BackOffice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtLogIN.Focus();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(txtLogIN.Text);
                using (var dados = new Data.EntitiesDiretor())
                {
                    if (!dados.Utilizador.Any(u => u.IdUtilizador == id))
                    {
                        MessageBox.Show("O utilizador não existe!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtLogIN.Focus();
                        return;
                    }
                    if (!dados.Utilizador.Any(u => u.IdUtilizador == id && u.Password == txtPass.Password))
                    {
                        MessageBox.Show("A password está incorreta", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtPass.Focus();
                    }
                    else {
                        int tipoUtilizador = dados.Utilizador.First(u => u.IdUtilizador == id).IdTipoUtilizador;
                        Menu menu = new Menu(id, tipoUtilizador);
                        this.Close();
                        menu.Show();
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Tem que inserir o ID", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
