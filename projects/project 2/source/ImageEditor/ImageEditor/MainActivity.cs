using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Provider;

namespace ImageEditor
{
    [Activity(Label = "ImageEditor", MainLauncher = true)]
    public class MainActivity : Activity
    {

        /// <summary>
        /// Used to track the file that we're manipulating between functions
        /// </summary>
        public static Java.IO.File _file;

        /// <summary>
        /// Used to track the directory that we'll be writing to between functions
        /// </summary>
        public static Java.IO.File _dir;


        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button gallybtn = FindViewById<Button>(Resource.Id.gallerybutton);
            Button cambtn = FindViewById<Button>(Resource.Id.camerabutton);

            var imageView = FindViewById<ImageView>(Resource.Id.imageoutput);
            imageView.Visibility = Android.Views.ViewStates.Invisible;

            if (IsThereAnAppToTakePictures() == true)
            {
                CreateDirectoryForPictures();
                cambtn.Click += TakePicture;
            }

            gallybtn.Click += delegate 
            {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 1);
            };
        }


        //Need to make sure that the device actually has a camera before trying to use it.
        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities
                (intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void CreateDirectoryForPictures()
        {
            _dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "CameraExample");
            if (!_dir.Exists())
            {
                _dir.Mkdirs();
            }
        }

        private void TakePicture(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            _file = new Java.IO.File(_dir, string.Format("myPhoto_{0}.jpg", System.Guid.NewGuid()));
            //android.support.v4.content.FileProvider
            //getUriForFile(getContext(), "com.mydomain.fileprovider", newFile);
            //FileProvider.GetUriForFile

            //The line is a problem line for Android 7+ development
            //intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_file));
            StartActivityForResult(intent, 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    var imageView = FindViewById<ImageView>(Resource.Id.imageoutput);

                    imageView.SetImageURI(data.Data);
                    imageView.Visibility = Android.Views.ViewStates.Visible;
                }
                     
            }
            else if (requestCode == 0)
            {
                if (resultCode == Result.Ok)
                {
                    ImageView imageView = FindViewById<ImageView>(Resource.Id.imageoutput);
                    int height = Resources.DisplayMetrics.HeightPixels;
                    int width = imageView.Height;

                    //AC: workaround for not passing actual files
                    Android.Graphics.Bitmap bitmap = (Android.Graphics.Bitmap)data.Extras.Get("data");
                    Android.Graphics.Bitmap copyBitmap =
                    bitmap.Copy(Android.Graphics.Bitmap.Config.Argb8888, true);

                    //this code removes all red from a picture

                    if (copyBitmap != null)
                    {
                        imageView.SetImageBitmap(copyBitmap);
                        imageView.Visibility = Android.Views.ViewStates.Visible;
                        bitmap = null;
                        copyBitmap = null;
                    }

                    // Dispose of the Java side bitmap.
                    System.GC.Collect();
                }
                
            }
        }
    }
}

