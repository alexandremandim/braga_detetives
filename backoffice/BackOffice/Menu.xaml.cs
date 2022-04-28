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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        private int id, tipoUser;
        private List<String> listaOpcoes;
        public Menu(int id, int idTipoUser)
        {
            InitializeComponent();
            this.id = id;
            this.tipoUser = idTipoUser;
            this.listaOpcoes = new List<string>();

            inicializarMenu();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch ((string)listBox.SelectedItem.ToString())
            {
                case "Criar Caso":
                    CriarCaso criarCaso = new CriarCaso(this.id);
                    criarCaso.Show();
                    break;
                case "Criar Perfil":
                    CriarPerfil criarPerfil = new CriarPerfil(this.id);
                    criarPerfil.Show();
                    break;
                case "Visualizar Equipas":
                    VisualizarEquipas verEquipas = new VisualizarEquipas(this.id, this.tipoUser);
                    verEquipas.Show();
                    break;
                case "Remover Tarefa":
                    RemoverTarefa removerTarefa = new RemoverTarefa(id);
                    removerTarefa.Show();
                    break;
                case "Concluir caso":
                    ConcluirCaso concluirCaso = new ConcluirCaso(id);
                    concluirCaso.Show();
                    break;
                case "Criar relatorio":
                    CriarRelatorio criarRelatorio = new CriarRelatorio(id);
                    criarRelatorio.Show();
                    break;
                case "Verificar Tarefas":
                    VerificarTarefas verificarTarefas = new VerificarTarefas(id);
                    verificarTarefas.Show();
                    break;
                case "Visualizar Casos":
                    VisualizarCasos visualizarCasos = new VisualizarCasos(id);
                    visualizarCasos.Show();
                    break; 
                case "Criar Tarefa":
                    CriarTarefa criarTarefa = new CriarTarefa(id);
                    criarTarefa.Show();
                    break;
                case "Transformar wav em XML":
                    TransformarVoz transformar = new TransformarVoz(this.id);
                    transformar.Show();
                    break;
            }
        }

        void inicializarMenu()
        {
            switch (tipoUser)
            {
                case 1: // Diretor
                    listaOpcoes.Add("Criar Caso");
                    listaOpcoes.Add("Criar Perfil");
                    listaOpcoes.Add("Visualizar Equipas");
                    break;
                case 2: // Agente
                    break;
                case 3: // IC
                    listaOpcoes.Add("Visualizar Equipas");
                    listaOpcoes.Add("Remover Tarefa");
                    listaOpcoes.Add("Concluir caso");
                    listaOpcoes.Add("Criar relatorio");
                    listaOpcoes.Add("Verificar Tarefas");
                    listaOpcoes.Add("Visualizar Casos");
                    listaOpcoes.Add("Criar Tarefa");
                    listaOpcoes.Add("Transformar wav em XML");
                    break;
            }
            listBox.ItemsSource = listaOpcoes;
        }
    }
}
