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
using System.IO;

namespace LI4_FrontOffice
{
    [Activity(Label = "Tarefas")]
    public class Tarefas : Activity
    {
        Button caminho, foto, notaVoz, notaEscrita, obter, concluir, suspender;
        TextView caso, descricao, objetivos, localizacao, realizada, suspensa;
        Tarefa tarefa;
        PartilhaTarefas tarefas;
        int idTarefa;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Tarefa);
           
            var MyJsonString = Intent.GetStringExtra("idtarefa");
             idTarefa = JsonConvert.DeserializeObject<int>(MyJsonString,   new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

            tarefas = (PartilhaTarefas)Application;

            tarefa = tarefas.Lista[idTarefa];

           

            caminho = FindViewById<Button>(Resource.Id.botaoCaminho);
            foto = FindViewById<Button>(Resource.Id.botaoFoto);
            notaVoz = FindViewById<Button>(Resource.Id.botaoVoz);
            notaEscrita = FindViewById<Button>(Resource.Id.botaoEscrita);
            obter = FindViewById<Button>(Resource.Id.botaoInfo);
            concluir = FindViewById<Button>(Resource.Id.botaoConcluir);
            suspender = FindViewById<Button>(Resource.Id.botaoSuspender);
            caso = FindViewById<TextView>(Resource.Id.textViewCaso);
            descricao = FindViewById<TextView>(Resource.Id.textViewDescricao);
            objetivos = FindViewById<TextView>(Resource.Id.textViewObj);
            localizacao = FindViewById<TextView>(Resource.Id.textViewLocal);
            realizada = FindViewById<TextView>(Resource.Id.textViewConcluida);
            suspensa = FindViewById<TextView>(Resource.Id.textViewSuspensa);

            
            obter.Click += Obter_Click;
            caminho.Click += Caminho_Click;
            foto.Click += Foto_Click;
            notaVoz.Click += NotaVoz_Click;
            notaEscrita.Click += NotaEscrita_Click;
            concluir.Click += Concluir_Click;
            suspender.Click += Suspender_Click;

        }
        protected override void OnResume()
        {
            base.OnResume();
            caso.Text = "Caso: "+ tarefa.NomeCaso;
            descricao.Text = "Descricão: " +tarefa.Descricao;
            objetivos.Text = "Objetivos: " + tarefa.getObjetivos();
            // objetivos.Text = "ver tudo";
            localizacao.Text = "Localização: " + tarefa.LocalTexto;

            if (tarefa.Realizada)
            {
                realizada.Text = "Concluida: Sim";
            }
            else
            {
                realizada.Text = "Concluida: Não";

            }

            if (tarefa.Suspensa)
            {
                suspensa.Text = "Suspensa: Sim";
            }
            else
            {
                suspensa.Text = "Suspensa: Não";

            }

          

        }


        protected override void OnRestart()
        {
            base.OnRestart();

          

        }


        private void Obter_Click(object sender, EventArgs e)
        {
           
                var uri = Android.Net.Uri.Parse("http://www.google.com");
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            

        }

        private void Suspender_Click(object sender, EventArgs e)
        {
            var callDialog = new AlertDialog.Builder(this);
            callDialog.SetMessage("Confirma que quer dar como suspensa a tarefa?");
            callDialog.SetPositiveButton("Sim", delegate {

                suspensa.Text = "Suspensa: Sim";
                tarefa.Suspensa = true;
            });

            callDialog.SetNeutralButton("Retomar", delegate {

                suspensa.Text = "Suspensa: Não";
                tarefa.Suspensa = false;
            });
            callDialog.SetNegativeButton("Cancelar", delegate { });

           


            // Show the alert dialog to the user and wait for response.
            callDialog.Show();

        }

        private void Concluir_Click(object sender, EventArgs e)
        {
            var callDialog = new AlertDialog.Builder(this);
            callDialog.SetMessage("Confirma que quer dar como concluida a tarefa?");
            callDialog.SetPositiveButton("Sim", delegate {

                realizada.Text = "Concluida: Sim";
                tarefa.Realizada = true;
            });
           
            callDialog.SetNegativeButton("Cancelar", delegate { });

            // Show the alert dialog to the user and wait for response.
            callDialog.Show();

            
        }

        private void NotaEscrita_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TirarNotas));
            string MySerializedObject = JsonConvert.SerializeObject(idTarefa, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            intent.PutExtra("idtarefa", MySerializedObject);

            this.StartActivity(intent);
           
        }

        private void NotaVoz_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(Gravador));
            string MySerializedObject = JsonConvert.SerializeObject(idTarefa, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            intent.PutExtra("idtarefa", MySerializedObject);


            this.StartActivity(intent);
           
           
        }

        private void Foto_Click(object sender, EventArgs e)
        {
            string MySerializedObject = JsonConvert.SerializeObject(idTarefa, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
          
            Intent intent = new Intent(this, typeof(Fotografar));
            intent.PutExtra("idtarefa", MySerializedObject);
           
            this.StartActivity(intent);
        }

        private void Caminho_Click(object sender, EventArgs e)
        {

            Intent intent = new Intent(this, typeof(IndicarCaminho));
            string MySerializedObject = JsonConvert.SerializeObject(idTarefa, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            intent.PutExtra("idtarefa", MySerializedObject);
            this.StartActivity(intent);
        }
    }
}