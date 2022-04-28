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
    /// Interaction logic for CriarPerfil.xaml
    /// </summary>
    public partial class CriarPerfil : Window
    {
        private Data.EntitiesDiretor data = new Data.EntitiesDiretor();
        private int idDiretor;

        public CriarPerfil(int idDiretor)
        {
            this.idDiretor = idDiretor;

            InitializeComponent();

            cmbInspetores.ItemsSource = data.viewInspetoresChefe.ToList();
            cmbInspetores.DisplayMemberPath = "Nome";
            cmbInspetores.SelectedValuePath = "idUtilizador";
        }

        private void cancelarButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void criarPerfilButton(object sender, RoutedEventArgs e)
        {
            if (txtBoxNome.Text != "" && passwordBox.Password != "" && (radioButtonIC.IsChecked == true || (radioButtonAgente.IsChecked == true && cmbInspetores.SelectedIndex > -1))){
                Data.Utilizador novoUtilizador = new Data.Utilizador();
                novoUtilizador.Nome = txtBoxNome.Text;
                novoUtilizador.Password = passwordBox.Password;
                if (radioButtonAgente.IsChecked == true)
                {
                    novoUtilizador.IdChefe = ((Data.viewInspetoresChefe)cmbInspetores.SelectedItem).IdUtilizador;
                    novoUtilizador.IdTipoUtilizador = 2; // Agente
                }
                else
                {
                    novoUtilizador.IdChefe = null;
                    novoUtilizador.IdTipoUtilizador = 3; // IC
                }
                data.Utilizador.Add(novoUtilizador);
                MessageBox.Show("Perfil criado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                data.SaveChanges();
                this.Close();
            }
            else
            {
                MessageBox.Show("Campos inválidos!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
