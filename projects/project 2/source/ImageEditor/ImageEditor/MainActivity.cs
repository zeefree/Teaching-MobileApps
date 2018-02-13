using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Provider;
using Android.Graphics.Drawables;

using System.IO;


namespace ImageEditor
{
    [Activity(Label = "ImageEditor", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
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



            // Button gallybtn = FindViewById<Button>(Resource.Id.gallerybutton);
            Button cambtn = FindViewById<Button>(Resource.Id.camerabutton);
            Button savebtn = FindViewById<Button>(Resource.Id.savebutton);
            Button clrbtn = FindViewById<Button>(Resource.Id.clearbutton);
            Button gallybtn = FindViewById<Button>(Resource.Id.gallerybutton);

            // Effect Buttons
            Button rmvred = FindViewById<Button>(Resource.Id.removeredbutton);
            Button rmvblue = FindViewById<Button>(Resource.Id.removebluebutton);
            Button rmvgreen = FindViewById<Button>(Resource.Id.removegreenbutton);

            Button negatered = FindViewById<Button>(Resource.Id.negateredbutton);
            Button negateblue = FindViewById<Button>(Resource.Id.negatebluebutton);
            Button negategreen = FindViewById<Button>(Resource.Id.negategreenbutton);

            Button noisebtn = FindViewById<Button>(Resource.Id.noisebutton);
            Button contrastbtn = FindViewById<Button>(Resource.Id.contrastbutton);
            Button greyscale = FindViewById<Button>(Resource.Id.greyscalebutton);

            ImageView imageView = FindViewById<ImageView>(Resource.Id.imageoutput);
            imageView.Visibility = Android.Views.ViewStates.Invisible;


            rmvred.Click += delegate { RemoveColor('r', imageView); };
            rmvblue.Click += delegate { RemoveColor('b', imageView); };
            rmvgreen.Click += delegate { RemoveColor('g', imageView); };

            negatered.Click += delegate { NegateColor('r', imageView); };
            negateblue.Click += delegate { NegateColor('b', imageView); };
            negategreen.Click += delegate { NegateColor('g', imageView); };

            noisebtn.Click += delegate { RandomNoise(imageView); };

            contrastbtn.Click += delegate { HighContrast(imageView); };

            greyscale.Click += delegate { GreyScale(imageView); };
            
            clrbtn.Click += delegate { imageView.SetImageBitmap(null); };

            savebtn.Click += delegate
            {
                Android.Graphics.Drawables.BitmapDrawable bd = (Android.Graphics.Drawables.BitmapDrawable)imageView.Drawable;
                Android.Graphics.Bitmap bitmap = bd.Bitmap;

                ExportBitmapAsPNG(bitmap);

            };

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

        private void GreyScale(ImageView imageoutput)
        {
            Android.Graphics.Drawables.BitmapDrawable bd = (Android.Graphics.Drawables.BitmapDrawable)imageoutput.Drawable;
            Android.Graphics.Bitmap bitmap = bd.Bitmap;

            if (bitmap != null)
            {
                Android.Graphics.Bitmap copyBitmap = bitmap.Copy(Android.Graphics.Bitmap.Config.Argb8888, true);
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        int p = bitmap.GetPixel(i, j);
                        Android.Graphics.Color c = new Android.Graphics.Color(p);

                        c.R = (byte)((c.R + c.G + c.B) / 3);
                        c.G = (byte)((c.R + c.G + c.B) / 3);
                        c.B = (byte)((c.R + c.G + c.B) / 3);

                        copyBitmap.SetPixel(i, j, c);
                    }
                }
                if (copyBitmap != null)
                {
                    imageoutput.SetImageBitmap(copyBitmap);
                    bitmap = null;
                    copyBitmap = null;
                }
                System.GC.Collect();
            }
        }

        

        private void RemoveColor(char color, ImageView imageoutput)
        {
            //Weird hoops you gotta go through to get some bitmaps :/
            Android.Graphics.Drawables.BitmapDrawable bd = (Android.Graphics.Drawables.BitmapDrawable)imageoutput.Drawable;
            Android.Graphics.Bitmap bitmap = bd.Bitmap;
            if(bitmap != null)
            {
                Android.Graphics.Bitmap copyBitmap = bitmap.Copy(Android.Graphics.Bitmap.Config.Argb8888, true);

                switch (color)
                {
                    case 'r':
                        {
                            for (int i = 0; i < bitmap.Width; i++)
                            {
                                for (int j = 0; j < bitmap.Height; j++)
                                {
                                    int p = bitmap.GetPixel(i, j);
                                    Android.Graphics.Color c = new Android.Graphics.Color(p);
                                    c.R = 0;
                                    copyBitmap.SetPixel(i, j, c);
                                }
                            }
                            break;
                        }
                    case 'g':
                        {
                            for (int i = 0; i < bitmap.Width; i++)
                            {
                                for (int j = 0; j < bitmap.Height; j++)
                                {
                                    int p = bitmap.GetPixel(i, j);
                                    Android.Graphics.Color c = new Android.Graphics.Color(p);
                                    c.G = 0;
                                    copyBitmap.SetPixel(i, j, c);
                                }
                            }
                            break;
                        }
                    case 'b':
                        {
                            for (int i = 0; i < bitmap.Width; i++)
                            {
                                for (int j = 0; j < bitmap.Height; j++)
                                {
                                    int p = bitmap.GetPixel(i, j);
                                    Android.Graphics.Color c = new Android.Graphics.Color(p);
                                    c.B = 0;
                                    copyBitmap.SetPixel(i, j, c);
                                }
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                if (copyBitmap != null)
                {
                    imageoutput.SetImageBitmap(copyBitmap);
                    bitmap = null;
                    copyBitmap = null;
                }
                System.GC.Collect();
            }
        }
            
        private void NegateColor(char color, ImageView imageoutput)
        {
            //Weird hoops you gotta go through to get some bitmaps :/
            Android.Graphics.Drawables.BitmapDrawable bd = (Android.Graphics.Drawables.BitmapDrawable)imageoutput.Drawable;
            Android.Graphics.Bitmap bitmap = bd.Bitmap;

            Android.Graphics.Bitmap copyBitmap = bitmap.Copy(Android.Graphics.Bitmap.Config.Argb8888, true);

            if(bitmap != null)
            {
                switch (color)
                {
                    case 'r':
                        {
                            for (int i = 0; i < bitmap.Width; i++)
                            {
                                for (int j = 0; j < bitmap.Height; j++)
                                {
                                    int p = bitmap.GetPixel(i, j);
                                    Android.Graphics.Color c = new Android.Graphics.Color(p);
                                    c.R = (byte)(255 - c.R);
                                    copyBitmap.SetPixel(i, j, c);
                                }
                            }
                            break;
                        }
                    case 'g':
                        {
                            for (int i = 0; i < bitmap.Width; i++)
                            {
                                for (int j = 0; j < bitmap.Height; j++)
                                {
                                    int p = bitmap.GetPixel(i, j);
                                    Android.Graphics.Color c = new Android.Graphics.Color(p);
                                    c.G = (byte)(255 - c.G);
                                    copyBitmap.SetPixel(i, j, c);
                                }
                            }
                            break;
                        }
                    case 'b':
                        {
                            for (int i = 0; i < bitmap.Width; i++)
                            {
                                for (int j = 0; j < bitmap.Height; j++)
                                {
                                    int p = bitmap.GetPixel(i, j);
                                    Android.Graphics.Color c = new Android.Graphics.Color(p);
                                    c.B = (byte)(255 - c.B);
                                    copyBitmap.SetPixel(i, j, c);
                                }
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                if (copyBitmap != null)
                {
                    imageoutput.SetImageBitmap(copyBitmap);
                    bitmap = null;
                    copyBitmap = null;
                }
                System.GC.Collect();
            }
        }
            
        private void RandomNoise(ImageView imageoutput)
        
        {
            Android.Graphics.Drawables.BitmapDrawable bd = (Android.Graphics.Drawables.BitmapDrawable)imageoutput.Drawable;
            Android.Graphics.Bitmap bitmap = bd.Bitmap;

            System.Random rnd = new Random();

            if(bitmap != null)
            {
                Android.Graphics.Bitmap copyBitmap = bitmap.Copy(Android.Graphics.Bitmap.Config.Argb8888, true);
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        int noise = rnd.Next(-7, 7);
                        int p = bitmap.GetPixel(i, j);
                        Android.Graphics.Color c = new Android.Graphics.Color(p);
                       // int red = c.R + noise;
                       // int green = c.G + noise;
                       // int blue = c.B + noise;

                        c.R = (byte) (AdjustPixelValue(c.R + noise));
                        c.G = (byte)(AdjustPixelValue(c.G + noise));
                        c.B = (byte)(AdjustPixelValue(c.B + noise));


                        copyBitmap.SetPixel(i, j, c);
                    }
                }
                if (copyBitmap != null)
                {
                    imageoutput.SetImageBitmap(copyBitmap);
                    bitmap = null;
                    copyBitmap = null;
                }
                System.GC.Collect();
            }
        }

        private int AdjustPixelValue(int to_adjust)
        {
            if(to_adjust < 0)
            {
                return 0;
            }
            else if(to_adjust > 255)
            {
                return 255;
            }

            return to_adjust;
        }

        private void HighContrast(ImageView imageoutput)
        {
            Android.Graphics.Drawables.BitmapDrawable bd = (Android.Graphics.Drawables.BitmapDrawable)imageoutput.Drawable;
            Android.Graphics.Bitmap bitmap = bd.Bitmap;

            if (bitmap != null)
            {
                Android.Graphics.Bitmap copyBitmap = bitmap.Copy(Android.Graphics.Bitmap.Config.Argb8888, true);
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        int p = bitmap.GetPixel(i, j);
                        Android.Graphics.Color c = new Android.Graphics.Color(p);
        
                        c.R = (byte)(ContrastPixelValue(c.R));
                        c.G = (byte)(ContrastPixelValue(c.G));
                        c.B = (byte)(ContrastPixelValue(c.B));

                        copyBitmap.SetPixel(i, j, c);
                    }
                }
                if (copyBitmap != null)
                {
                    imageoutput.SetImageBitmap(copyBitmap);
                    bitmap = null;
                    copyBitmap = null;
                }
                System.GC.Collect();
            }
        }

        private int ContrastPixelValue(int to_check)
        {
            if(to_check >= (255/2))
            {
                return 255;
            }
            else
            {
                return 0;
            }
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

                    Android.Graphics.Drawables.BitmapDrawable bd = (Android.Graphics.Drawables.BitmapDrawable)imageView.Drawable;
                    Android.Graphics.Bitmap bitmap = bd.Bitmap;

                    //Gotta shrink down the bitmaps because they tend to be too big



                    int height = bitmap.Height;
                    int width = bitmap.Width;

                    ScaleBounds(ref height,ref width);

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
                    ImageView imageView = FindViewById<ImageView>(Resource.Id.imageoutput);
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
        }

        //Most images are going to be too big so we need to shrink them down
        // This is an attempt to properly scale down images so they keep resolution
        //The point of acceptable_resoultion is to act as a point of refrence to scale images down to
        private void ScaleBounds(ref int height,ref int width)
        {
            int acceptal_resoultion = 1024 * 768;
            int cur_resoultion = height * width;

            if (cur_resoultion > acceptal_resoultion)
            {
                height = height / (2 + (cur_resoultion / acceptal_resoultion));
                width = width / (2 + (cur_resoultion / acceptal_resoultion));
            }
        }
       
        void ExportBitmapAsPNG(Android.Graphics.Bitmap bitmap)
        {

            var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            var filename = Path.Combine(path, "newFile.png");

            var stream = new FileStream(filename, FileMode.Create);
            bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, stream);
            stream.Close();
        }
    }
}

