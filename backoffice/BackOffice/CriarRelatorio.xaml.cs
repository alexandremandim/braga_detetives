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
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BackOffice
{
    
    public partial class CriarRelatorio : Window
    {
        private Data.EntitiesIC data = new Data.EntitiesIC();
        private int idIC;

        public CriarRelatorio(int idIC)
        {
            this.idIC = idIC;
            InitializeComponent();
            preencherComponentes();
            dataGridCasos.SelectedValuePath = "IdCaso"; 
        }

        private void preencherComponentes()
        {
            this.dataGridCasos.ItemsSource = (from c in data.CasosAtivos(idIC)
                                              where c.Relatorio == null && c.Terminado == true
                                              select new
                                              {
                                                  c.IdCaso,
                                                  c.Nome,
                                                  c.Descricao,
                                                  c.Objetivos,
                                                  c.Relatorio
                                              }).ToList();
            this.cmbRelatoriosGerados.ItemsSource = (from c in data.Caso
                                         where c.idUtilizadorResponsavel == this.idIC
                                             && c.Relatorio != null
                                         select new { c.IdCaso, c.Nome }).ToList();
            this.cmbRelatoriosGerados.DisplayMemberPath = "Nome";
            this.cmbRelatoriosGerados.SelectedValuePath = "IdCaso";
        }

        private void buttonSair_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void criarRelatorioButton(object sender, RoutedEventArgs e)
        {
            if (dataGridCasos.SelectedIndex > -1)
            {
                Data.Caso caso = data.Caso.Find((int)dataGridCasos.SelectedValue);
                if(caso!= null){
                    // Gerar Relatorio
                    Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
                    System.IO.Directory.CreateDirectory("relatorios");
                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream("./relatorios/"+caso.IdCaso+".pdf", FileMode.Create));

                    doc.Open();
                    // Inicio do relatorio
                    iTextSharp.text.Paragraph paragrafo1 = new iTextSharp.text.Paragraph();
                    paragrafo1.Add("Titulo: " + caso.Nome + "\nDescrição: "+caso.Descricao+"\nObjetivos: " + caso.Objetivos + "\n");
                    paragrafo1.Add("ID Inspetor responsável: " + caso.idUtilizadorResponsavel + "\nAgentes envolvidos: \n");

                    var agentesEnvolvidos = data.AgentesEnvolvidos(caso.IdCaso);
                    foreach (var agente in agentesEnvolvidos)
                    {
                        paragrafo1.Add(agente.IdUtilizador + " - " + agente.Nome + "\n");
                        foreach (var tarefas in data.dadosRelatorioAgente(agente.IdUtilizador, caso.IdCaso).ToList()){
                            paragrafo1.Add("Titulo: " + tarefas.Titulo + " Número de dados: " + tarefas.NumeroDados + "\n");
                        }
                    }
                    doc.Add(paragrafo1);
                    doc.Close();

                    // Atualizar BD (guardar caminho do pdf e savechanges)
                    var x = data.Caso.SingleOrDefault(c => c.IdCaso == caso.IdCaso);
                    if (caso != null)
                    {
                        x.Relatorio = "./relatorios/" + caso.IdCaso + ".pdf";
                        data.SaveChanges();
                        MessageBox.Show("Relatorio gerado.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        preencherComponentes();
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um caso!", "Atenção!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void enviarEmailButton(object sender, RoutedEventArgs e)
        {
            if(textBoxMail.Text != "")
            {
                Data.Caso caso = data.Caso.Find((int)cmbRelatoriosGerados.SelectedValue);
                if(caso.Relatorio != null)
                {
                    string corpo = ("Em anexo foi-lhe enviado o relatório do caso: " + caso.Nome
                        + "\n Esta mensagem foi gerada automaticamente pelo software.\nCumprimentos\n");
                    TEmail email = new TEmail();
                    email.Enviar(textBoxMail.Text, "Relatório - "+ caso.Nome, corpo, caso.Relatorio);
                }
                else
                {
                    MessageBox.Show("Relatório ainda não foi gerado!");
                }
            }
        }
    }
}
