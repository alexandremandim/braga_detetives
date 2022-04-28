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
using Android.Media;
using Java.IO;
using Newtonsoft.Json;
using Android.Locations;
using System.Globalization;

namespace LI4_FrontOffice
{
    [Activity(Label = "Gravador")]
    public class Gravador : Activity, ILocationListener
    {
        MediaRecorder _recorder;
        MediaPlayer _player;
        Button _start;
        Button _stop;
        Button _rep;
        PartilhaTarefas tarefas;
        Tarefa tarefa;
        int idTarefa;

        String addressText;
        String locationText;
      //  String distanceText;
        Location currentLocation;
        LocationManager locationManager;
        string locationProvider = string.Empty;

        float latitude, longitude;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var MyJsonString = Intent.GetStringExtra("idtarefa");
            idTarefa = JsonConvert.DeserializeObject<int>(MyJsonString, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
          
            tarefas = (PartilhaTarefas)Application;

            tarefa = tarefas.Lista[idTarefa];
          

            SetContentView(Resource.Layout.gravadeiro);
            _start = FindViewById<Button>(Resource.Id.start);
            _stop = FindViewById<Button>(Resource.Id.stop);
            _rep = FindViewById<Button>(Resource.Id.reproduzir);
            _rep.Enabled = false;

            getLocation();
            // create a File object for the parent directory

            if (!System.IO.File.Exists("/sdcard/Li4Detetives/gravacoes/"))
            {
                File gravacoesDirectory = new File("/sdcard/Li4Detetives/gravacoes/");

                gravacoesDirectory.Mkdirs();
            }
           
            
            string path = "/sdcard/Li4Detetives/gravacoes/test" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".wav";
            
            _start.Click += delegate {

                _stop.Enabled = !_stop.Enabled;
                _start.Enabled = !_start.Enabled;
                _rep.Enabled = false;
                _recorder.SetAudioSource(AudioSource.Mic);
                _recorder.SetOutputFormat(OutputFormat.ThreeGpp);
                _recorder.SetAudioEncoder(AudioEncoder.AmrNb);
                _recorder.SetOutputFile(path);
                _recorder.Prepare();
                _recorder.Start();
            };

            _rep.Click += delegate {
                
                _player.SetDataSource(path);
                _player.Prepare();
                _player.Start();
                _start.Enabled = true;
            };


            _stop.Click += delegate {
                _stop.Enabled = !_stop.Enabled;

                _recorder.Stop();
                _recorder.Reset();

                _start.Enabled = true;
                _rep.Enabled = true;

               
                
                tarefa.Dados.Add(new Dado() { Tipo=4, Caminho = path, Latitude = latitude, Longitude = longitude,
                    Data = DateTime.Now
                });
                
            };
        }

        
        protected override void OnResume()
        {
            base.OnResume();

            _recorder = new MediaRecorder();
            _player = new MediaPlayer();

            _player.Completion += (sender, e) =>
            {
                _player.Reset();
                _start.Enabled = !_start.Enabled;
            };

            if (locationProvider != string.Empty)
            {
                locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);

            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            _player.Release();
            _recorder.Release();
            _player.Dispose();
            _recorder.Dispose();
            _player = null;
            _recorder = null;

            locationManager.RemoveUpdates(this);
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

    }
}