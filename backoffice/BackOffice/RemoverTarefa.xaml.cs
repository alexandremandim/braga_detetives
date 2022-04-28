using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for RemoverTarefa.xaml
    /// </summary>
    public partial class RemoverTarefa : Window
    {
        private Data.EntitiesIC data = new Data.EntitiesIC();
        private int id;
        public RemoverTarefa(int idIC)
        {
            this.id = idIC;
            InitializeComponent();

            gridTarefas.ItemsSource = data.TarefasNaoSincronizadas(id).ToList();
        }

        private void cancelarButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void removerButton(object sender, RoutedEventArgs e)
        {
            if(gridTarefas.SelectedIndex > -1)
            {
                int idTarefa = ((Data.TarefasNaoSincronizadas_Result)gridTarefas.SelectedItems[0]).IdTarefa;

                var tarefaARemover = (from tarefa in data.Tarefa where tarefa.IdTarefa == idTarefa select tarefa).SingleOrDefault();
                if(tarefaARemover != null)
                {
                    data.Tarefa.Remove(tarefaARemover);
                    data.SaveChanges();
                    MessageBox.Show("Tarefa removida com sucesso!", "Removida", MessageBoxButton.OK, MessageBoxImage.None);
                    gridTarefas.ItemsSource = data.TarefasNaoSincronizadas(id).ToList();
                }
                else
                {
                    MessageBox.Show("Erro!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            else
            {
                MessageBox.Show("Selecione uma tarefa!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
