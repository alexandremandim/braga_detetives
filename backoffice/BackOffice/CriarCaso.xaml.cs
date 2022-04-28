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
    public partial class CriarCaso : Window
    {
        private Data.EntitiesDiretor data = new Data.EntitiesDiretor();
        private int idDiretor;
        public CriarCaso(int idDiretor)
        {
            this.idDiretor = idDiretor;
            InitializeComponent();
            cmbInspetores.ItemsSource = data.viewInspetoresChefe.ToList();
            cmbInspetores.DisplayMemberPath = "Nome";
            cmbInspetores.SelectedValuePath = "idUtilizador";
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if(txtBoxDescricao.Text != "" && txtBoxNomeCaso.Text != "" && txtBoxObjetivos.Text != "" && cmbInspetores.SelectedIndex > -1)
            {
                Data.Caso novoCaso = new Data.Caso();
                novoCaso.Descricao = txtBoxDescricao.Text;
                novoCaso.idUtilizadorResponsavel = ((Data.viewInspetoresChefe)(cmbInspetores.SelectedItem)).IdUtilizador;
                novoCaso.Objetivos = txtBoxObjetivos.Text;
                novoCaso.Terminado = false;
                novoCaso.Relatorio = null;
                novoCaso.Nome = txtBoxNomeCaso.Text;
                data.Caso.Add(novoCaso);
                data.SaveChanges();
                MessageBox.Show("Caso criado!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Campos inválidos!","Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
