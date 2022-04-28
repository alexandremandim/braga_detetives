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
using Android.Locations;
using System.Globalization;

namespace LI4_FrontOffice
{
    [Activity(Label = "TirarNotas")]
    public class TirarNotas : Activity, ILocationListener
    {
        EditText textoditavel;
        PartilhaTarefas tarefas;
        Tarefa tarefa;
        int idTarefa;
        String addressText;
        String locationText;
        //  String distanceText;
       
        Location currentLocation;
        LocationManager locationManager;
        string caminho;
        string locationProvider = string.Empty;

        float latitude, longitude;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.notaEscrita);
            // Create your application here
            

            try
            {
                var MyJsonString = Intent.GetStringExtra("idtarefa");
                idTarefa = JsonConvert.DeserializeObject<int>(MyJsonString, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
              
                tarefas = (PartilhaTarefas)Application;

                tarefa = tarefas.Lista[idTarefa];

                getLocation();

            }
            catch (ArgumentNullException e)
            {
                tarefa = new Tarefa();
            }
           

            textoditavel = FindViewById<EditText>(Resource.Id.textoeditavel);
          
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();



            if (!System.IO.File.Exists("/sdcard/Li4Detetives/notas/"))
            {
                Java.IO.File notasDirectory = new Java.IO.File("/sdcard/Li4Detetives/notas/");

                notasDirectory.Mkdirs();
            }

           
            Java.IO.File nota = new Java.IO.File("/sdcard/Li4Detetives/notas/nota" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt");
            caminho = nota.AbsolutePath;
            StreamWriter sw = new StreamWriter(caminho);
            sw.Write(textoditavel.Text);
            sw.Close();

            // Intent.PutExtra(tarefa);
            tarefa.Dados.Add(new Dado()
            {
                Tipo = 2,
                Caminho = caminho,
                Latitude = latitude,
                Longitude = longitude,
                Data = DateTime.Now

            });



        }

        protected override void OnResume()
        {
            base.OnResume();

            if (locationProvider != string.Empty)
            {
                locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);

            }
        
        }

        protected override void OnPause()
        {
            base.OnPause();


            locationManager.RemoveUpdates(this);
        }


        public void getLocation()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);

            // define its Criteria
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Medium
            };

            // find a location provider (GPS, wi-fi, etc.)
            IList<string> acceptableLocationProviders =
        locationManager.GetProviders(criteriaForLocationService, true);

            // if we have any, use the first one
            if (acceptableLocationProviders.Any())
                locationProvider = acceptableLocationProviders.First();
            else
                locationProvider = String.Empty;


        }

        public void OnLocationChanged(Location location)
        {
            try
            {
                currentLocation = location;
                if (currentLocation == null)
                    locationText = "Location not found";
                else
                {


                    latitude = (float)currentLocation.Latitude;
                    longitude = (float)currentLocation.Longitude;



                    Android.Locations.Geocoder geocoder = new Geocoder(this);
                    // this gets the address from the Internet
                    IList<Address> addressList =
                geocoder.GetFromLocation(currentLocation.Latitude,
currentLocation.Longitude, 10);
                    Address address = addressList.FirstOrDefault();


                    if (address != null)
                    {
                      
                        StringBuilder deviceAddress = new StringBuilder();
                        for (int i = 0; i < address.MaxAddressLineIndex; i++)
                        {
                            deviceAddress.Append(address.GetAddressLine(i)).AppendLine(",");
                        }
                        addressText = deviceAddress.ToString();
                    } // end if
                    else addressText = "address not found";
                }
            }
            catch
            {
                addressText = "address not found";
            } // end catch
        } // end onLocationChanged

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }
    }
}