using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using Android.Content.Res;

namespace LI4_FrontOffice
{
    [Activity(Label = " Plano de Tarefas ")]
    public class VerTarefas : Activity
    {
        ListView listaV;
       

        PartilhaTarefas partilha;
        string idAgente;
        string password;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.VerTarefas);
            listaV = FindViewById<ListView>(Resource.Id.listViewVerTarefas);
          
             carregarTarefas();

           // partilha = (PartilhaTarefas)Application;

            //listaTarefas= partilha.Lista;
        
          
            listaV.ItemClick += ListaV_ItemClick;

        }

        protected override void OnResume()
        {
           

            base.OnResume();
            partilha = (PartilhaTarefas)Application;
            ListViewAdapterTarefa adapter = new ListViewAdapterTarefa(this, partilha.Lista);
            listaV.Adapter = adapter;


        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            escreverTarefas();
        }



        public void escreverTarefas()
        {
            partilha = (PartilhaTarefas)Application;
            XmlDocument xml = new XmlDocument();
            XmlNode docNode = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
            xml.AppendChild(docNode);

            XmlElement plano = xml.CreateElement("PlanoAtividades");
            plano.SetAttribute("IdAgente", idAgente);
            plano.SetAttribute("Password", password);


            foreach (Tarefa t in partilha.Lista)
            {
                XmlElement tarefa = xml.CreateElement("Tarefa");
                tarefa.SetAttribute("IdTarefa", (t.IdTarefa).ToString());
                tarefa.SetAttribute("NomeCaso", t.NomeCaso);
                tarefa.SetAttribute("IdCaso", (t.IdCaso).ToString());
                tarefa.SetAttribute("Titulo", t.Titulo);
                tarefa.SetAttribute("Realizada", t.Realizada.ToString());
                tarefa.SetAttribute("Suspensa", t.Suspensa.ToString());
                tarefa.SetAttribute("Latitude", (t.Latitude).ToString());
                tarefa.SetAttribute("Longitude", (t.Longitude).ToString());

                XmlElement localizacaoTexto = xml.CreateElement("LocalizacaoTexto");
                localizacaoTexto.InnerText = t.LocalTexto;
                XmlElement descricao = xml.CreateElement("Descricao");
                descricao.InnerText = t.Descricao;
                XmlElement objetivos = xml.CreateElement("Objetivos");

                foreach (string obj in t.Objetivos)
                {
                    XmlElement objetivo = xml.CreateElement("Objetivo");
                    objetivo.InnerText = obj;
                    objetivos.AppendChild(objetivo);
                }

                XmlElement dados = xml.CreateElement("Dados");
                XmlElement dado = null;
                foreach (Dado d in t.Dados)
                {

                    switch(d.Tipo)
                    {
                        case 1:
                             dado = xml.CreateElement("Imagem");
                            break;
                        case 4:
                            dado = xml.CreateElement("Gravacao");
                            break;

                        case 2:
                            dado = xml.CreateElement("NotaEscrita");
                            break;
                        default:
                            break;
                    }
                        
                    // XmlElement dado = xml.CreateElement("");
                    dado.SetAttribute("Caminho", d.Caminho);
                    dado.SetAttribute("Latitude", d.Latitude.ToString());
                    dado.SetAttribute("Longitude", d.Longitude.ToString());
                    dado.SetAttribute("Data", d.Data.ToString());

                    dados.AppendChild(dado);

                }

                tarefa.AppendChild(localizacaoTexto);
                tarefa.AppendChild(descricao);
                tarefa.AppendChild(objetivos);
                tarefa.AppendChild(dados);


                plano.AppendChild(tarefa);
            }
            xml.AppendChild(plano);
            xml.Save("/sdcard/Li4Detetives/plano.xml");

        }
        
        public void carregarTarefas()
        {
            int tipoDado = 0;
            string caminho = null, dataString = null;
            DateTime data;


            
            partilha = (PartilhaTarefas)Application;

            partilha.Lista = new List<Tarefa>();


            //AssetManager am = this.Assets;
            //Stream stream = am.Open("plano.xml");
            //StreamReader sr = new StreamReader(stream);

            //StreamWriter sw = new StreamWriter("/sdcard/planoAtividades/plano.xml");
            //// StreamWriter sw = new StreamWriter("/storage/emulated/0/plano.xml");
            //sw.Write(sr.ReadToEnd());
            //sr.Close();
            //sw.Close();


            //var caminhoFicheiro = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            //var nomeFicheiro = Path.Combine(caminhoFicheiro.ToString(), "plano.xml");
            //Console.WriteLine(nomeFicheiro);

            XmlDocument doc = new XmlDocument();
            doc.Load("/sdcard/Li4Detetives/plano.xml");

             idAgente = doc.DocumentElement.Attributes["IdAgente"].Value;
             password = doc.DocumentElement.Attributes["Password"].Value;

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
                       
                        caminho = nodeDado.Attributes["Caminho"].Value;
                    }

                    //se for uma gravação, atribui tipo = 2, nota é null (não existe) e recolhe o caminho
                    else if (nodeDado.Name.Equals("Gravacao"))
                    {
                        tipoDado = 4;
                       
                        caminho = nodeDado.Attributes["Caminho"].Value;
                    }

                    //se for uma nota escrita, atribui tipo = 3, recolhe a nota e caminho é null
                    else if (nodeDado.Name.Equals("NotaEscrita"))
                    {
                        tipoDado = 2;
                       
                        caminho = nodeDado.Attributes["Caminho"].Value;

                    }

                    dataString = nodeDado.Attributes["Data"].Value;
                    data = DateTime.ParseExact(dataString, "dd/MM/yyyy HH:mm:ss", null);
                    float latitude = float.Parse(nodeDado.Attributes["Latitude"].Value);
                    float longitude = float.Parse(nodeDado.Attributes["Longitude"].Value);

                    //cria um Dado
                    Dado d = new Dado()
                    {
                        Caminho = caminho,
                        Latitude = latitude,
                        Longitude = longitude,
                        Tipo = tipoDado,
                      
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

                partilha.Lista.Add(t);
            }

           
        }



        private void ListaV_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(Tarefas));
            // var MySerializedObject = JsonConvert.SerializeObject(listaTarefas[e.Position]);



            string MySerializedObject = JsonConvert.SerializeObject(e.Position, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            intent.PutExtra("idtarefa", MySerializedObject);

            this.StartActivity(intent);

            //var MyJsonString = Intent.GetStringExtra("listaTarefas");
            //var varT = JsonConvert.DeserializeObject<string>(MyJsonString);


        }
    }


}