using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Provider;
using Android.Content.PM;
using System.Collections.Generic;
using Google.Apis.Services;
using Google.Apis.Vision.v1.Data;

namespace Cloud_Photo
{
    [Activity(Label = "Cloud_Photo", MainLauncher = true, Icon = "@mipmap/icon")]
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

            // Get our button from the layout resource,
            // and attach an event to it
            Button cambtn = FindViewById<Button>(Resource.Id.camerabutton);
            Button upload = FindViewById<Button>(Resource.Id.cloudbutton);

            if (IsThereAnAppToTakePictures() == true)
            {
                CreateDirectoryForPictures();
                cambtn.Click += TakePicture;
            }

            upload.Click += delegate { UploadToCloud(); };
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
            if (resultCode == Result.Ok)
            {
                ImageView imageView = FindViewById<ImageView>(Resource.Id.mainImage);
                int height = Resources.DisplayMetrics.HeightPixels;
                int width = imageView.Height;

                //AC: workaround for not passing actual files
                Android.Graphics.Bitmap bitmap = (Android.Graphics.Bitmap)data.Extras.Get("data");
                Android.Graphics.Bitmap copyBitmap =
                bitmap.Copy(Android.Graphics.Bitmap.Config.Argb8888, true);


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

        private void UploadToCloud()
        {
            ImageView imageView = FindViewById<ImageView>(Resource.Id.mainImage);
            Android.Graphics.Drawables.BitmapDrawable bd = (Android.Graphics.Drawables.BitmapDrawable)imageView.Drawable;
            Android.Graphics.Bitmap bitmap = bd.Bitmap;

            string credPath = "google_api.json";
            Google.Apis.Auth.OAuth2.GoogleCredential credential;

            //load our cert into the credential
            using (var stream = Assets.Open(credPath))
            {
                credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromStream(stream);
            }
            credential = credential.CreateScoped(Google.Apis.Vision.v1.VisionService.Scope.CloudPlatform);
            var client = new Google.Apis.Vision.v1.VisionService(
               new BaseClientService.Initializer()
               {
                   ApplicationName = "cs480firsttest-195221",
                   HttpClientInitializer = credential
               }
               );

            // tell google that we want to perform feature analysis
            var request = new AnnotateImageRequest();
            request.Image = new Image();

            //converts our bitmap object into a byte[] to send to google
            using (var stream = new System.IO.MemoryStream())
            {
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 0, stream);
                request.Image.Content = System.Convert.ToBase64String(stream.ToArray());
            }

            //say we want google to us label detection
            request.Features = new List<Feature>();
            request.Features.Add(new Feature() { Type = "LABEL_DETECTION" });

            //add to list of items to send to goolge
            var batch = new BatchAnnotateImagesRequest();
            batch.Requests = new List<AnnotateImageRequest>();
            batch.Requests.Add(request);

            //Finally, make the call
            var apiResult = client.Images.Annotate(batch).Execute();
            List<string> tags = new List<string>();
            foreach (var item in apiResult.Responses[0].LabelAnnotations)
            {
                tags.Add(item.Description);
            }
        }

        private void AdjustTextView(List<string> tags)
        {
            TextView maintxt = FindViewById<TextView>(Resource.Id.mainTextView);
            foreach (var tag in tags)
            {

            }
        }

    }
}

