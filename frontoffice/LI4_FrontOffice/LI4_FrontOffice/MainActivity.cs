using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using LI4_FrontOffice;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Text;
using Android.Content.Res;
using System.IO;

namespace LI4_FrontOffice
{


    [Activity(Label = "Menu FrontOffice", MainLauncher = true, Icon = "@drawable/icon")]


    public class MainActivity : Activity
    {
        Button verTarefas;
        Button exportar;
        Button importar;
        Button alterarCred;
        string idAgente, pass;

        PartilhaTarefas partilha;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);


            SetContentView(Resource.Layout.Main);

            // partilha = new PartilhaTarefas();


          
            carregarTarefas();
            partilha = (PartilhaTarefas)Application;

            // partilha.Lista.Add(new Tarefa() { IdCaso = 2, Latitude = 33, Longitude = 27, NomeCaso = "Misterio em Palmela", Titulo = "Vigiar paulo", IdTarefa = 3, Descricao = "vigiar isso", LocalTexto = "Rua da estrada velha" });
            // partilha.Lista.Add(new Tarefa() { IdCaso = 1, Latitude = 5, Longitude = 88, NomeCaso = "Deter bandidos na costa", Titulo = "Gang do Maneta", IdTarefa = 11, Descricao = "Encontrar Zeca", LocalTexto = " Praia das Algas" });


            verTarefas = FindViewById<Button>(Resource.Id.VerTarefasButton);
            exportar = FindViewById<Button>(Resource.Id.InicExportButton);
            importar = FindViewById<Button>(Resource.Id.InicImportButton);
            alterarCred = FindViewById<Button>(Resource.Id.AlterCredButton);

            verTarefas.Click += VerTarefas_Click;
            importar.Click += Importar_Click;
            exportar.Click += Exportar_Click;
          

        }

        class Par
        {
            public int Id { get; set; }
            private Dado dado;

            public Dado Dado { get; set; }


        }

        private void Exportar_Click(object sender, EventArgs e)
        {
            getCredenciais();


            partilha = (PartilhaTarefas)Application;
            List<Par> listaPares = new List<Par>();




            foreach (Tarefa tar in partilha.Lista)
            {
                int id = tar.IdTarefa;
                if (tar.Realizada == true)
                {
                    foreach (Dado dado in tar.Dados)
                    {
                        listaPares.Add(new Par() { Id = id, Dado = dado });
                    }

                }

            }

            StringBuilder sb = new StringBuilder();
            StringBuilder sbdados = new StringBuilder();

            if (listaPares.Count == 0)
            {
                var callDialog = new AlertDialog.Builder(this);
                callDialog.SetMessage("Sem tarefas concluídas para exportar");
                callDialog.SetNegativeButton("Ok", delegate {

                });

                callDialog.Show();

                return;
            }

            else if (listaPares.Count > 0)
            {
               
                sb.Append(listaPares[0].Id);
                Par par = listaPares[0];
                sbdados.Append("(" + "'" + par.Dado.Caminho + "'" + "," + "'" + ((DateTime)par.Dado.Data).ToString("MM/dd/yyyy HH:mm:ss") + "'" + "," + par.Dado.Latitude.ToString().Replace(',', '.') + "," + par.Dado.Longitude.ToString().Replace(',', '.') + "," + par.Id + "," + par.Dado.Tipo + ")");
                for (int i = 1; i < listaPares.Count; i++)
                {
                    par = listaPares[i];
                   
                    sb.Append("," + listaPares[i].Id);
                    sbdados.Append(",(" + "'" + par.Dado.Caminho + "'" + "," + "'" + ((DateTime)par.Dado.Data).ToString("MM/dd/yyyy HH:mm:ss") + "'" + "," + par.Dado.Latitude.ToString().Replace(',', '.') + "," + par.Dado.Longitude.ToString().Replace(',', '.') + "," + par.Id + "," + par.Dado.Tipo + ")");
                }
            }
            
            try
            {
                string connectionString = "Server=192.168.111.110,1433;Database=LI4-Agentes;User ID=IC;Password=inspetorchefe;";

                IDbConnection condb;
                using (condb = new SqlConnection(connectionString))
                {

                    condb.Open();
                    using (IDbCommand cmddb = condb.CreateCommand())
                    {   //validação de credenciais
                        string sql = "SELECT * FROM [LI4-Agentes].[dbo].[Utilizador]  WHERE IdUtilizador =" + idAgente;
                      
                        cmddb.CommandText = sql;
                        using (IDataReader reader = cmddb.ExecuteReader())
                        {
                            string palavraChave = null;
                            while (reader.Read())
                            {
                                palavraChave = (string)reader["Password"];
                            }

                            if (!palavraChave.Equals(pass)) {
                                var callDialog = new AlertDialog.Builder(this);
                                callDialog.SetMessage("Credenciais inválidas");
                                callDialog.SetNegativeButton("Ok", delegate {
                                });

                                callDialog.Show();
                                return; }

                            reader.Close();
                           // marcar como concluídas
                            sql = "UPDATE [LI4-Agentes].[dbo].[Tarefa] SET Realizada=1 WHERE IdTarefa IN (" + sb.ToString() + ")";
                            cmddb.CommandText = sql;
                            IDataReader reader3 = cmddb.ExecuteReader();
                            reader3.Close();
                            //enviar os dados das tarefas
                            sql = "INSERT INTO [LI4-Agentes].[dbo].[Dados] VALUES " + sbdados.ToString();
                           
                            cmddb.CommandText = sql;

                            IDataReader reader2 = cmddb.ExecuteReader();
                            reader2.Close();
                            condb.Close();

                        }
                    }

                }


            }
            catch (Exception)
            {

                throw;
            }

            for (int i = 0; i < partilha.Lista.Count; i++)
            {
                int id = partilha.Lista[i].IdTarefa;
                if (partilha.Lista[i].Realizada == true)
                {
                    partilha.Lista.Remove(partilha.Lista[i]);
                }

            }

            escreverTarefas();
        }

        private void getCredenciais()
        {


            XmlDocument doc = new XmlDocument();
            doc.Load("/sdcard/Li4Detetives/plano.xml");

            idAgente = doc.DocumentElement.Attributes["IdAgente"].Value;
            pass = doc.DocumentElement.Attributes["Password"].Value;
        }

        


        private void Importar_Click(object sender, EventArgs e)
        {
           
            getCredenciais();
           
            partilha = (PartilhaTarefas)Application;
            string connectionString = "Server=192.168.111.110,1433;Database=LI4-Agentes;User ID=IC;Password=inspetorchefe;";

            try
            {
                IDbConnection condb;
                using (condb = new SqlConnection(connectionString)) {

                    condb.Open();
                    using(IDbCommand cmddb = condb.CreateCommand())
                    {
                        
                            string sql = "SELECT * FROM [LI4-Agentes].[dbo].[Utilizador]  WHERE IdUtilizador =" + idAgente;
                           
                            cmddb.CommandText = sql;
                            using (IDataReader reader = cmddb.ExecuteReader())
                        {
                            string palavraChave = null;
                                while (reader.Read())
                                {
                                    palavraChave = (string)reader["Password"];

                                }

                                if (!palavraChave.Equals(pass))
                                {
                                    var callDialog = new AlertDialog.Builder(this);
                                    callDialog.SetMessage("Credenciais inválidas");
                                    callDialog.SetNegativeButton("Ok", delegate {

                                    });

                                    callDialog.Show();

                                    return;
                                }
                            reader.Close();
                            sql = "SELECT * FROM [LI4-Agentes].[dbo].[Tarefa] as T INNER JOIN  [LI4-Agentes].[dbo].[Caso] as C ON T.IdCaso=C.IdCaso WHERE IdAgente=" + idAgente + " AND Exportada=0";
                                cmddb.CommandText = sql;
                                IDataReader reader3 = cmddb.ExecuteReader();
                          
                            while (reader3.Read())
                            {
                                // tokenizar objetivos
                                string objetivos = (string)reader3["Objetivos"];
                                char[] c = { ';' };
                               string [] objs =  objetivos.Split(c);
                                List<string> listaObjetivos = new List<string>();
                                foreach (string s in objs)
                                {
                                    listaObjetivos.Add(s);
                                  
                                }
                                

                                Tarefa t = new Tarefa()
                                {
                                    IdTarefa = (int)reader3["IdTarefa"],
                                    Titulo = (string)reader3["Titulo"],
                                    IdCaso = (int)reader3["IdCaso"],
                                    NomeCaso = (string)reader3["Nome"],
                                    Descricao = (string)reader3["Descricao"],
                                    Latitude = (float)(double)reader3["Latitude"],
                                    Longitude = (float)(double)reader3["Longitude"],
                                    Realizada = (bool)(Boolean)reader3["Realizada"],
                                    Suspensa = false,
                                    Dados = new List<Dado>(),
                                    Objetivos = listaObjetivos
                                    
                                };

                                t.ToString2();
                                partilha.Lista.Add(t);
                            }
                            reader3.Close();
                            
                            sql = "UPDATE [LI4-Agentes].[dbo].[Tarefa] SET Exportada=1 WHERE IdAgente=" + idAgente + " AND Exportada=0";
                            cmddb.CommandText = sql;
                            
                            IDataReader reader2 = cmddb.ExecuteReader();
                            reader2.Close();
                            condb.Close();
                        }
                    }

                }


            }
            catch (Exception exzc)
            {

               
            }


            escreverTarefas();

        }

        private void VerTarefas_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(VerTarefas));
            this.StartActivity(intent);

        }



        public void escreverTarefas()
        {
            partilha = (PartilhaTarefas)Application;
            XmlDocument xml = new XmlDocument();
            XmlNode docNode = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
            xml.AppendChild(docNode);

            XmlElement plano = xml.CreateElement("PlanoAtividades");
            plano.SetAttribute("IdAgente", idAgente);
            plano.SetAttribute("Password", pass);


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

                    switch (d.Tipo)
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

            //StreamWriter sw = new StreamWriter("/sdcard/Li4Detetives/plano.xml");
            //// StreamWriter sw = new StreamWriter("/storage/emulated/0/plano.xml");
            //sw.Write(sr.ReadToEnd());
            //sr.Close();
            //sw.Close();


            //var caminhoFicheiro = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            //var nomeFicheiro = Path.Combine(caminhoFicheiro.ToString(), "plano.xml");
          

            XmlDocument doc = new XmlDocument();
            doc.Load("/sdcard/Li4Detetives/plano.xml");

            idAgente = doc.DocumentElement.Attributes["IdAgente"].Value;
            pass= doc.DocumentElement.Attributes["Password"].Value;

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



    }
}

