using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.IO;
using Android.Graphics;
using Android.Provider;
using System.Collections.Generic;
using Android.Content.PM;
using static Android.Net.Uri;
using Newtonsoft.Json;
using Android.Locations;
using System.Linq;
using System.Text;
using System.Globalization;

namespace LI4_FrontOffice
{
    [Activity(Label = "Fotografar")]
    public class Fotografar : Activity, ILocationListener
    {
        ImageView _imageView;
        int idTarefa;
        Tarefa tarefa;
        String addressText;
        String locationText;
        //  String distanceText;
        PartilhaTarefas tarefas;
        Location currentLocation;
        LocationManager locationManager;
        string caminho;
        string locationProvider = string.Empty;

        float latitude, longitude;

        protected override void OnCreate(Bundle bundle)
        {

            var MyJsonString = Intent.GetStringExtra("idtarefa");
            idTarefa = JsonConvert.DeserializeObject<int>(MyJsonString, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.fotografar);

            tarefas = (PartilhaTarefas)Application;

            tarefa = tarefas.Lista[idTarefa];

            getLocation();

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button button = FindViewById<Button>(Resource.Id.botaoTirar);
                _imageView = FindViewById<ImageView>(Resource.Id.imageViewFoto);
                button.Click += TakeAPicture;
            }
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

        private void CreateDirectoryForPictures()
        { //
            App._dir = new File("/sdcard/Li4Detetives/Fotos/");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            caminho = App._file.AbsolutePath;
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = _imageView.Height;
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
            if (App.bitmap != null)
            {
                _imageView.SetImageBitmap(App.bitmap);
                App.bitmap = null;
            }
            

            // Dispose of the Java side bitmap.
            GC.Collect();



            tarefa.Dados.Add(new Dado()
            {
                Tipo = 1,
                Caminho = caminho,
                Latitude = latitude,
                Longitude = longitude,
                Data = DateTime.Now

            });

           
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


    public static class App
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }


    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }

    }


}

