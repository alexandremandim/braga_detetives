using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
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
using System.Xml;

namespace BackOffice
{
    
    public partial class TransformarVoz : Window
    {
        private SpeechRecognitionEngine sre = new SpeechRecognitionEngine();
        private Grammar gr = new DictationGrammar();
        private Data.EntitiesIC data = new Data.EntitiesIC();
        private List<String> palavrasChave;
        private int id;
        
        public TransformarVoz(int idIC)
        {
            this.id = idIC;
            InitializeComponent();
            inicializarPalavrasChave();
            iniciarComponentes();
        }

        private void iniciarComponentes()
        {
            dataGrid.ItemsSource = (from d in data.tarefasDosAgentesDeUmIC(this.id)
                                    where d.IdTipoDado == 4
                                    select new { d.IdUtilizador, d.Nome, d.IdTarefa, d.IdDado, d.Caminho }).ToList();
        }

        private void inicializarPalavrasChave()
        {
            palavrasChave = new List<String>();
            palavrasChave.Add("Terminate");
            palavrasChave.Add("Atention");
            palavrasChave.Add("Clothing");
            palavrasChave.Add("Danger");
            palavrasChave.Add("Status");
            palavrasChave.Add("Crime");

        }

        private void butSair_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void butTransformar_Click(object sender, RoutedEventArgs e)
        {
            
            if (dataGrid.Items.Count > 0)
             {
                 var aux = (from d in data.tarefasDosAgentesDeUmIC(this.id)
                                        where d.IdTipoDado == 4
                                        select new { d.IdUtilizador, d.Nome, d.IdTarefa, d.IdDado, d.Caminho }).ToList();

                foreach (var dr in aux)
                {
                    string caminho = ".\\sounds" + dr.Caminho.ToString();
                    List<String> todasPalavras = palavras(caminho);
                    bool gerou = gerarXML(todasPalavras, dr.IdDado);
                    if (gerou)
                    {
                        //Actualizar na BD na tabela Dados, este registo (caminho novo e tipodado novo)
                        var dado = data.Dados.SingleOrDefault(d => d.IdDado == (int)dr.IdDado);
                        dado.Caminho = caminho;
                        dado.IdTipoDado = 3;
                        
                        //Eliminar ficheiro audio
                        File.Delete(caminho);
                        
                        MessageBox.Show("Ficheiro xml criado!");

                        data.SaveChanges();
                        iniciarComponentes(); // atualizar a grid
                    }
                    else
                    {
                        MessageBox.Show("Erro.");
                    }
                }
                
            }
             else
             {
                 MessageBox.Show("Nao existem itens para converter!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
             }
        }

        private List<String> palavras(string caminho)
        {
            List<String> palavras = new List<string>();

            sre.LoadGrammar(gr);
            sre.SetInputToWaveFile(caminho);
            sre.BabbleTimeout = new TimeSpan(Int32.MaxValue);
            sre.InitialSilenceTimeout = new TimeSpan(Int32.MaxValue);
            sre.EndSilenceTimeout = new TimeSpan(100000000);
            sre.EndSilenceTimeoutAmbiguous = new TimeSpan(100000000);

            while (true)
            {
                try
                {
                    var recText = sre.Recognize();
                    if (recText == null)
                    {
                        break;
                    }

                    palavras.Add(recText.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro!");
                    break;
                }
            }
            return palavras;
        }

        bool gerarXML(List<String> palavras, int idDado)
        {
            
            bool gerou = false;

            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement element = doc.CreateElement(string.Empty, "body", string.Empty);
            doc.AppendChild(element);

            string palavraC = "", frase = "";
            bool first = true;
            XmlElement element1 = null, element2 = null;
            XmlAttribute atributo = null;
            foreach (var palavra in palavras)
            {

                if ( this.palavrasChave.Any(x => x.Equals(palavra)) && !palavraC.Equals(palavra) && first == true)
                {//é primeira palavra chave a aparecer
                    first = false;
                    frase = "";
                    palavraC = palavra;
                    element1 = doc.CreateElement(string.Empty, "pr", string.Empty);
                    atributo = doc.CreateAttribute("pal");
                    atributo.Value = palavra;
                    element1.Attributes.Append(atributo);
                    element.AppendChild(element1);
                }
                else if (palavra.Equals("Terminate") && first==false)
                { //palavra reservada de fecho
                    palavraC = palavra;
                    element2 = doc.CreateElement(string.Empty, "texto", string.Empty);
                    atributo = doc.CreateAttribute("txt");
                    atributo.Value = frase;
                    element2.Attributes.Append(atributo);
                    element1.AppendChild(element2); //element2 filho de element1
                    
                    frase = "";
                    gerou = true;
                    break;
                }
                else if ( this.palavrasChave.Any(x => x.Equals(palavra)) && !palavraC.Equals(palavra) )
                { //Palavra chave no meio da sequencia
                    palavraC = palavra;
                    element2 = doc.CreateElement(string.Empty, "texto", string.Empty);
                    atributo = doc.CreateAttribute("txt");
                    atributo.Value = frase;
                    element2.Attributes.Append(atributo);
                    element1.AppendChild(element2); //element1 filho de element
                    frase = "";

                    //criar novo elemento
                    element1 = doc.CreateElement(string.Empty, "pr", string.Empty);
                    atributo = doc.CreateAttribute("pal");
                    atributo.Value = palavra;
                    element1.Attributes.Append(atributo);
                    element.AppendChild(element1);
                    
                }
                else //palavras que nao sao reservadas
                {
                    frase += palavra + " ";
                }
            }
            doc.Save(".\\xmls" + "\\" + idDado+".xml");
            return gerou;
        }
    }
}
