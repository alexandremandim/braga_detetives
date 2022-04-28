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
    /// Interaction logic for ConcluirCaso.xaml
    /// </summary>
    public partial class ConcluirCaso : Window
    {
        private Data.EntitiesIC dataIC = new Data.EntitiesIC();
        private int id;

        public ConcluirCaso(int id)
        {
            this.id = id;
            InitializeComponent();

            cmbCaso.ItemsSource = (from c in dataIC.CasosAtivos(id)
                                   where c.Terminado == false
                                   select new { c.IdCaso, c.Nome, c.Descricao, c.Objetivos, c.Relatorio }).ToList();
            cmbCaso.DisplayMemberPath = "Nome";
            cmbCaso.SelectedValuePath = "IdCaso";
        }

        private void sairButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void okButton(object sender, RoutedEventArgs e)
        {
            if (cmbCaso.SelectedIndex > -1)
            {
                var caso = dataIC.Caso.SingleOrDefault(c => c.IdCaso == (int)cmbCaso.SelectedValue);
                if (caso != null)
                {
                    //Verificar se caso tem tarefas por concluir
                    var listaTarefas = (from t in dataIC.Tarefa
                                        where t.IdCaso == (int)cmbCaso.SelectedValue
                                        select new { t.IdTarefa, t.Titulo, t.Descricao, t.Objetivos, t.Realizada, t.Exportada, t.Latitude, t.Longitude, t.IdAgente, t.IdCaso }).ToList();

                    if (!listaTarefas.Any(t => t.Realizada == false))
                    {
                        caso.Terminado = true;
                        dataIC.SaveChanges();
                        MessageBox.Show("Caso concluido.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Caso não pode ser concluído! Ainda tem tarefas por concluir!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um caso!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void verCasosButton(object sender, RoutedEventArgs e)
        {
            VisualizarCasos vercasos = new VisualizarCasos(this.id);
            vercasos.Show();
        }
    }
}
