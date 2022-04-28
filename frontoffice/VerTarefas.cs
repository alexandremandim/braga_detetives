using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.Res;


namespace LI4_FrontOffice
{
    [Activity(Label = "VerTarefas")]
    public class VerTarefas : Activity
    {
        ListView lista;
        List<Tarefa> listaTarefas = new List<Tarefa>();
        

        public void carregarTarefas()
        {
            int tipoDado = 0;
            string nota = null, caminho = null, dataString = null;
            DateTime data;

            AssetManager am = this.Assets;
            Stream stream = am.Open("plano.xml");
            StreamReader sr = new StreamReader(stream);

            StreamWriter sw = new StreamWriter("/storage/emulated/0/plano.xml");
            sw.Write(sr.ReadToEnd());
            sr.Close();
            sw.Close();


            var caminhoFicheiro = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var nomeFicheiro = Path.Combine(caminhoFicheiro.ToString(), "plano.xml");
            Console.WriteLine(nomeFicheiro);

            XmlDocument doc = new XmlDocument();
            doc.Load(nomeFicheiro);

            string nomeAgente = doc.DocumentElement.Attributes["NomeAgente"].Value;
            string password = doc.DocumentElement.Attributes["Password"].Value;

            //itera sobre todas as tarefas
            foreach (XmlNode node in doc.DocumentElement)
            {
                List<string> listaObjetivos = new List<string>();
                List<Dado> listaDados = new List<Dado>();

                //recolhe todos os objetivos e coloca-os na lista de objetivos
                foreach (XmlNode nodeObjetivo in node["Objetivos"])
                {
                    string objetivo = nodeObjetivo.InnerText;
                    listaObjetivos.Add(objetivo);
                }

                //itera sobre os dados, recolhe a informação e coloca um Dado na lista de dados
                foreach (XmlNode nodeDado in node["Dados"])
                {
                    //se for uma imagem, atribui tipo = 1, nota é null (não existe) e recolhe o caminho
                    if (nodeDado.Name.Equals("Imagem"))
                    {
                        tipoDado = 1;
                        nota = null;
                        caminho = nodeDado.Attributes["caminho"].Value;
                    }

                    //se for uma gravação, atribui tipo = 2, nota é null (não existe) e recolhe o caminho
                    else if (nodeDado.Name.Equals("Gravacao"))
                    {
                        tipoDado = 2;
                        nota = null;
                        caminho = nodeDado.Attributes["caminho"].Value;
                    }

                    //se for uma nota escrita, atribui tipo = 3, recolhe a nota e caminho é null
                    else if (nodeDado.Name.Equals("NotaEscrita"))
                    {
                        tipoDado = 3;
                        nota = nodeDado.InnerText;
                        caminho = null;

                    }

                    dataString = nodeDado.Attributes["Data"].Value;
                    data = DateTime.ParseExact(dataString, "yyyy-MM-dd HH:mm:ss", null);
                    float latitude = float.Parse(nodeDado.Attributes["Latitude"].Value);
                    float longitude = float.Parse(nodeDado.Attributes["Longitude"].Value);

                    //cria um Dado
                    Dado d = new Dado()
                    {
                        Caminho = caminho,
                        Latitude = latitude,
                        Longitude = longitude,
                        Tipo = tipoDado,
                        Nota = nota,
                        Data = data
                    };

                    //adiciona-o à lista de dados
                    listaDados.Add(d);

                }

                int idTarefa = int.Parse(node.Attributes["IdTarefa"].Value);
                string nomeCaso = node.Attributes["NomeCaso"].Value;
                int idCaso = int.Parse(node.Attributes["IdCaso"].Value);
                string titulo = node.Attributes["Titulo"].Value;
                bool realizada = bool.Parse(node.Attributes["Realizada"].Value);
                float latitudeTarefa = float.Parse(node.Attributes["Latitude"].Value);
                float longitudeTarefa = float.Parse(node.Attributes["Longitude"].Value);
                string localTexto = node["LocalizacaoTexto"].InnerText;
                string descricao = node["Descricao"].InnerText;


                Tarefa t = new Tarefa()
                {
                    IdTarefa = idTarefa,
                    NomeCaso = nomeCaso,
                    IdCaso = idCaso,
                    Titulo = titulo,
                    Realizada = realizada,
                    Latitude = latitudeTarefa,
                    Longitude = longitudeTarefa,
                    LocalTexto = localTexto,
                    Descricao = descricao,
                    Objetivos = listaObjetivos,
                    Dados = listaDados
                };

                listaTarefas.Add(t);
            }

            foreach(Tarefa tarefa in listaTarefas)
            {
                Console.WriteLine(tarefa.ToString());
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            lista = FindViewById<ListView>(Resource.Id.listViewVerTarefas);

            carregarTarefas();

            


        }
    



    }


}