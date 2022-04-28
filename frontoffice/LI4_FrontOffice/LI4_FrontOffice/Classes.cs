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
using System.Data.SqlTypes;

namespace LI4_FrontOffice
{
    [Application]
    class PartilhaTarefas:Application
    {
       private  List<Tarefa> lista;

        public List<Tarefa> Lista
        {
            get { return lista; }
            set { lista = value; }
        }

        public PartilhaTarefas (IntPtr handle, JniHandleOwnership transfer):base(handle,transfer)

        {
            lista = new List<Tarefa>();
        }

        public PartilhaTarefas()

        {
            lista = new List<Tarefa>();
        }


        public override void OnCreate()
        {
            base.OnCreate();
        }


    }


    class Dado
    {
        public int Tipo { get; set; } // 1 -> Imagem, 4-> Gravação, 2 -> NotaEscrita
        public string Caminho { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public SqlDateTime Data { get; set; }
        

        public override string ToString()
        {
            return "Tipo: " + Tipo + "  Caminho: " + Caminho + "  Latitude: " + Latitude + "  Longitude: " + Longitude;
        }

    }
    class Tarefa
    {
        public int IdTarefa { get; set; }
        public int IdCaso { get; set; }
        public string NomeCaso { get; set; }
        public string Titulo { get; set; }
        public bool Realizada { get; set; }
        public bool Suspensa{ get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
       
        public string LocalTexto { get; set; }
        public string Descricao { get; set; }
        private List<string> listaObjetivos;
        private List<Dado> listaDados;

        public List<string> Objetivos
        {
            get { return listaObjetivos; }
            set { listaObjetivos = value; }
        }
        public List<Dado> Dados
        {
            get { return listaDados; }
            set { listaDados = value; }
        }

        public override string ToString()
        {
            return "Id: " + IdTarefa + "  Tarefa: " + Titulo + "  Caso:" +IdCaso;
        }

        public string ToString2()
        {
            return "Id:" + IdTarefa + "  Tarefa" + Titulo + "IdCaso: " + IdCaso + "NomeCaso: " + NomeCaso + "Realizada: " + Realizada
                + "Latitude: " + Latitude + "Longitude: " + Longitude + "LocalTexto: " + LocalTexto
                + "Descricao: " + Descricao;


        }

        public string getObjetivos()
        {
            string s = "";
            foreach (string item in listaObjetivos)
            {
                s += "- " + item + ";\n";
            }
            return s;
        }

    }




}