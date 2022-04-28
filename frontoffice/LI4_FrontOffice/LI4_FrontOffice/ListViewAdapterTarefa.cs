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

namespace LI4_FrontOffice
{
    class ListViewAdapterTarefa : BaseAdapter<Tarefa>
    {
        private List<Tarefa> listaTarefas;
        private Context contexto;



        public ListViewAdapterTarefa(Context cont, List<Tarefa> lista)
            {
                this.listaTarefas = lista;
                this.contexto = cont;
            }


        public override int Count
        {
            get
            {
                return listaTarefas.Count;
            }
        }

        public override Tarefa this[int position]
        {
            get { return listaTarefas[position]; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if(row==null)
            {
                row = LayoutInflater.From(contexto).Inflate(Resource.Layout.linhaListView,null,false);
            }

            TextView  tvi= row.FindViewById<TextView>(Resource.Id.textViewTarefa);
            tvi.Text = listaTarefas[position].ToString();

            return row;
        }
    }
}