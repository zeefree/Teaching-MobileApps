using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Provider;
using Android.Content.PM;
using System.Collections.Generic;
using Google.Apis.Services;
using Google.Apis.Vision.v1.Data;
using Android.Views.InputMethods;
using System;

namespace Cloud_Photo
{
    
    [Activity(Label = "Cloud_Photo", MainLauncher = true, Icon = "@mipmap/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        bool uploaded_state = false;
        List<string> tags = new List<string>();
        List<float> percentage = new List<float>();
        int response_state = 0;
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

            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());

            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button cambtn = FindViewById<Button>(Resource.Id.camerabutton);
            Button gallybtn = FindViewById<Button>(Resource.Id.gallerybutton);
            Button upload = FindViewById<Button>(Resource.Id.cloudbutton);
            Button refresh = FindViewById<Button>(Resource.Id.refreshbutton);
            EditText editabletext = FindViewById<EditText>(Resource.Id.textfield);
            TextView maintxt = FindViewById<TextView>(Resource.Id.mainTextView);
            Button positve = FindViewById<Button>(Resource.Id.positivebutton);
            Button negative = FindViewById<Button>(Resource.Id.negativebutton);

            List<Button> cambuttons = new List<Button>();

            cambuttons.Add(cambtn);
            cambuttons.Add(gallybtn);
            cambuttons.Add(upload);

            if (IsThereAnAppToTakePictures() == true)
            {
                CreateDirectoryForPictures();
                cambtn.Click += TakePicture;
            }

            upload.Click += delegate { PromptForUpload(); };

            gallybtn.Click += delegate
            {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 1);
            };

            refresh.Click += delegate
            {
                SwitchState();
                tags.Clear();
                percentage.Clear();
                maintxt.Text = "";
                response_state = 0;
            };

            positve.Click +=  Respond;
            negative.Click += Respond;
            
            editabletext.KeyPress += (object sender, Android.Views.View.KeyEventArgs e) =>
            {
                if ((e.Event.Action == Android.Views.KeyEventActions.Down) && (e.KeyCode == Android.Views.Keycode.Enter))
                {
                    editabletext.Visibility = Android.Views.ViewStates.Gone;
                    maintxt.Text = maintxt.Text + "\n A " + editabletext.Text + "?";
                    CheckTags();
                   
                }
                else
                {
                   
                    e.Handled = false;
                   
                }
            };
        }

        private void CheckTags()
        {
            
            EditText editabletext = FindViewById<EditText>(Resource.Id.textfield);
            TextView maintxt = FindViewById<TextView>(Resource.Id.mainTextView);
            bool found = false;
            for (int i = 0; i < tags.Count; i++)
            {
                if (tags[i] == editabletext.Text)
                {
                    maintxt.Text = maintxt.Text + "\nOh a " + tags[i] + " my source only gave me a " + percentage[i] + " chance of it being that...";
                    found = true;
                }
            }
            if(!found)
            {
                maintxt.Text = maintxt.Text + "\nGuess my sources were off I didn't even know it was a " + editabletext.Text;
            }

            maintxt.Text = maintxt.Text + "\nWell if you want to upload a new picture press restart.";
        }

        

        private void PromptForUpload()
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Upload to Cloud");
            alert.SetMessage("You have limited photos to upload to cloud, is this the one you want to upload?");
            alert.SetPositiveButton("Yes", (senderAlert, args) => 
                                              { Toast.MakeText(this, "Uploading!", ToastLength.Short).Show();
                                                UploadToCloud();
                                              }
                                   );
            alert.SetNegativeButton("No", (senderAlert, args) =>
                                              { Toast.MakeText(this, "Canceled!", ToastLength.Short).Show();}
                                   );

            Dialog dialog = alert.Create();
            dialog.Show();
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
                    Android.OS.Environment.DirectoryPictures), "CloudPhoto");
            if (!_dir.Exists())
            {
                _dir.Mkdirs();
            }
        }

        private void TakePicture(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            _file = new Java.IO.File(_dir, string.Format("myPhoto_{0}.jpg", System.Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_file));
            StartActivityForResult(intent, 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            Button upload = FindViewById<Button>(Resource.Id.cloudbutton);
            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    var imageView = FindViewById<ImageView>(Resource.Id.mainImage);

                    imageView.SetImageURI(data.Data);

                    Android.Graphics.Drawables.BitmapDrawable bd = (Android.Graphics.Drawables.BitmapDrawable)imageView.Drawable;
                    Android.Graphics.Bitmap bitmap = bd.Bitmap;

                    //Gotta shrink down the bitmaps because they tend to be too big



                    int height = bitmap.Height;
                    int width = bitmap.Width;

                    ScaleBounds(ref height, ref width);

                    Android.Graphics.Bitmap smallBitmap =
                    Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, width, height, true);

                    imageView.SetImageBitmap(smallBitmap);

                    imageView.Visibility = Android.Views.ViewStates.Visible;
                }

            }
            else if (requestCode == 0)
            {
                if (resultCode == Result.Ok)
                {
                    //send to image gallery
                    Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                    var contentUri = Android.Net.Uri.FromFile(_file);
                    mediaScanIntent.SetData(contentUri);
                    SendBroadcast(mediaScanIntent);

                    // Display in ImageView. We will resize the bitmap to fit the display.
                    // Loading the full sized image will consume too much memory
                    // and cause the application to crash.
                    ImageView imageView = FindViewById<ImageView>(Resource.Id.mainImage);
                    int height = Resources.DisplayMetrics.HeightPixels;
                    int width = imageView.Height;

                    //load picture from file
                    Android.Graphics.Bitmap bitmap = _file.Path.LoadAndResizeBitmap(width, height);


                    if (bitmap != null)
                    {
                        imageView.SetImageBitmap(bitmap);
                        imageView.Visibility = Android.Views.ViewStates.Visible;
                        bitmap = null;
                    }

                    // Dispose of the Java side bitmap.
                    System.GC.Collect();
                }
            }
            upload.Visibility = Android.Views.ViewStates.Visible;
        }

        //Most images are going to be too big so we need to shrink them down
        // This is an attempt to properly scale down images so they keep resolution
        //The point of acceptable_resoultion is to act as a point of refrence to scale images down to
        private void ScaleBounds(ref int height, ref int width)
        {

            /*
            int acceptal_resoultion = 1024 * 768;
            int cur_resoultion = height * width;

            if (cur_resoultion > acceptal_resoultion)
            {
                height = height / (2 + (cur_resoultion / acceptal_resoultion));
                width = width / (2 + (cur_resoultion / acceptal_resoultion));
            }
            */

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
         
            foreach (var item in apiResult.Responses[0].LabelAnnotations)
            {
                tags.Add(item.Description);
                percentage.Add((float)(item.Score));
            }
            AdjustTextView();

            SwitchState();
        }

        private void SwitchState()
        {
            Button cambtn = FindViewById<Button>(Resource.Id.camerabutton);
            Button gallybtn = FindViewById<Button>(Resource.Id.gallerybutton);
            Button upload = FindViewById<Button>(Resource.Id.cloudbutton);
            Button refresh = FindViewById<Button>(Resource.Id.refreshbutton);
            Button positve = FindViewById<Button>(Resource.Id.positivebutton);
            Button negative = FindViewById<Button>(Resource.Id.negativebutton);

            if (uploaded_state == false)
            {
                cambtn.Visibility = Android.Views.ViewStates.Gone;
                gallybtn.Visibility = Android.Views.ViewStates.Gone;
                upload.Visibility = Android.Views.ViewStates.Gone;
                refresh.Visibility = Android.Views.ViewStates.Visible;
                negative.Visibility = Android.Views.ViewStates.Visible;
                positve.Visibility = Android.Views.ViewStates.Visible;

                uploaded_state = true;
            }
            else
            {
                cambtn.Visibility = Android.Views.ViewStates.Visible;
                gallybtn.Visibility = Android.Views.ViewStates.Visible;
                refresh.Visibility = Android.Views.ViewStates.Gone;
                negative.Visibility = Android.Views.ViewStates.Gone;
                positve.Visibility = Android.Views.ViewStates.Gone;

                uploaded_state = false;
            }
            
        }

        public void Respond(object sender, EventArgs e)
        {
            Button response = sender as Button;
            TextView maintxt = FindViewById<TextView>(Resource.Id.mainTextView);
            EditText editabletext = FindViewById<EditText>(Resource.Id.textfield);
            Button positve = FindViewById<Button>(Resource.Id.positivebutton);
            Button negative = FindViewById<Button>(Resource.Id.negativebutton);

            if (response.Id == (Resource.Id.positivebutton))
            {
                
                 maintxt.Text = maintxt.Text + "\nYour welcome! If you want to upload a new photo feel free to press the restart button.";
               
            }
            else
            {
                maintxt.Text = maintxt.Text + "\n it's not a " + tags[0] + "?\nThen what is it?";
                editabletext.Visibility = Android.Views.ViewStates.Visible;
            }

            negative.Visibility = Android.Views.ViewStates.Gone;
            positve.Visibility = Android.Views.ViewStates.Gone;
          
        }

        private void AdjustTextView()
        {
            TextView maintxt = FindViewById<TextView>(Resource.Id.mainTextView);
            maintxt.Text = "Wow, that's a pretty cool " + tags[0];
            
        }

    }
}

