using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Newtonsoft.Json;
using System.IO;
using Android.Locations;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Android.Util;
using System.Threading.Tasks;

namespace LI4_FrontOffice
{
    [Activity(Label = "IndicarCaminho")]
    public class IndicarCaminho : Activity, IOnMapReadyCallback, ILocationListener
    {
        private GoogleMap mMap;
        Button normal, terreno, satelite, hibrido;
       
        PartilhaTarefas tarefas;
        Tarefa tar;
        int idTarefa;
        string _addressText;
        Location _currentLocation;
        LocationManager _locationManager;
        bool tabo = false;
        static readonly string TAG = "X:" + typeof(IndicarCaminho).Name;
        string _locationProvider;
        AlertDialog.Builder callDialog;
        Button continuar;
        Button yesBtn;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
           
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.indicarCaminho);

            // Get our button from the layout resource,
            // and attach an event to it

            try
            {
                var MyJsonString = Intent.GetStringExtra("idtarefa");
                idTarefa = JsonConvert.DeserializeObject<int>(MyJsonString, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
                tarefas = (PartilhaTarefas)Application;

                tar = tarefas.Lista[idTarefa];
            }
            catch (ArgumentNullException e)
            {
                
            }

            InitializeLocationManager();

             callDialog = new AlertDialog.Builder(this);
            
            callDialog.SetMessage("Aguarde... A obter a sua localização");
            callDialog.SetPositiveButton("Continuar", (EventHandler<DialogClickEventArgs>)null);
            var dialog = callDialog.Create();
            dialog.Show();
             yesBtn = dialog.GetButton((int)DialogButtonType.Positive);
            yesBtn.Enabled = false;
            // Assign our handlers.
            yesBtn.Click += (sender, args) =>
            {
                SetUpMap();
                dialog.Dismiss();
            };

            

            normal = FindViewById<Button>(Resource.Id.bnormal);
            terreno = FindViewById<Button>(Resource.Id.bterreno);
            hibrido = FindViewById<Button>(Resource.Id.bhibrido);
            satelite = FindViewById<Button>(Resource.Id.bsatelite);

            normal.Click += Normal_Click;
            terreno.Click += Terreno_Click;
            hibrido.Click += Hibrido_Click;
            satelite.Click += Satelite_Click;
        }

        private void Satelite_Click(object sender, EventArgs e)
        {
           
            mMap.MapType = GoogleMap.MapTypeSatellite;
        }

        private void Hibrido_Click(object sender, EventArgs e)
        {
            
            mMap.MapType = GoogleMap.MapTypeHybrid;
        }

        private void Terreno_Click(object sender, EventArgs e)
        {
           
            mMap.MapType = GoogleMap.MapTypeTerrain;
        }

        private void Normal_Click(object sender, EventArgs e)
        {
           
            mMap.MapType = GoogleMap.MapTypeNormal;
        }

        private void SetUpMap()
        {
            if (mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;
            System.Threading.Thread.Sleep(3000);
           
            
            LatLng destino = new LatLng(tar.Latitude, tar.Longitude);
            MarkerOptions op = new MarkerOptions()
                .SetPosition(destino)
                .SetTitle(tar.Titulo)
                .SetSnippet(tar.Descricao);

            mMap.AddMarker(op);

            LatLng localAtual = new LatLng(_currentLocation.Latitude, _currentLocation.Longitude);
            MarkerOptions op1 = new MarkerOptions()
                .SetPosition(localAtual)
                .SetTitle("Local atual")
                .SetSnippet("Você encontra-se neste local");

            mMap.AddMarker(op1);

            PolylineOptions poliop = new PolylineOptions() { };
            poliop.Add(destino);
            poliop.Add(localAtual);

            mMap.AddPolyline(poliop);

            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(destino, 10);
            mMap.MoveCamera(camera);
        }

        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }
            Log.Debug(TAG, "Using " + _locationProvider + ".");
        }
        protected override void OnResume()
        {
            base.OnResume();
            _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
        }      // end OnResume

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }


        public async void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation != null)
            {
                   
                yesBtn.Enabled = true;
                callDialog.SetMessage("Pode continuar");
                  Address address = await ReverseGeocodeCurrentLocation();
                DisplayAddress(address);
            }
        }




        async void AddressButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (_currentLocation == null)
            {
                _addressText = "Can't determine the current address. Try again in a few minutes.";
                return;
            }

            Address address = await ReverseGeocodeCurrentLocation();
            DisplayAddress(address);
        }

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(tar.Latitude, tar.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                // Remove the last comma from the end of the address.
                _addressText = deviceAddress.ToString();
                tar.LocalTexto = _addressText;

            }
          
        }





        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }



    }
}