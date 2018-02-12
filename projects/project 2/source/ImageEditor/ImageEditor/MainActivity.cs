﻿using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Provider;
using Android.Graphics.Drawables;

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

            // Button gallybtn = FindViewById<Button>(Resource.Id.gallerybutton);
            Button cambtn = FindViewById<Button>(Resource.Id.camerabutton);
            Button savebtn = FindViewById<Button>(Resource.Id.savebutton);
            Button clrbtn = FindViewById<Button>(Resource.Id.clearbutton);

            // Effect Buttons
            Button rmvred = FindViewById<Button>(Resource.Id.removeredbutton);
            Button rmvblue = FindViewById<Button>(Resource.Id.removebluebutton);
            Button rmvgreen = FindViewById<Button>(Resource.Id.removegreenbutton);

            Button negatered = FindViewById<Button>(Resource.Id.negateredbutton);
            Button negateblue = FindViewById<Button>(Resource.Id.negatebluebutton);
            Button negategreen = FindViewById<Button>(Resource.Id.negategreenbutton);


            var imageView = FindViewById<ImageView>(Resource.Id.imageoutput);
            imageView.Visibility = Android.Views.ViewStates.Invisible;


            rmvred.Click += delegate { RemoveColor('r', imageView); };
            rmvblue.Click += delegate { RemoveColor('b', imageView); };
            rmvgreen.Click += delegate { RemoveColor('g', imageView); };

            negatered.Click += delegate { NegateColor('r', imageView); };
            negateblue.Click += delegate { NegateColor('b', imageView); };
            negategreen.Click += delegate { NegateColor('g', imageView); };

            if (IsThereAnAppToTakePictures() == true)
            {
                CreateDirectoryForPictures();
                cambtn.Click += TakePicture;
            }

            /*
            gallybtn.Click += delegate 
            {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 1);
            };
            */
        }

        public void RemoveColor(char color, ImageView imageoutput)
        {
            //Weird hoops you gotta go through to get some bitmaps :/
            Android.Graphics.Drawables.BitmapDrawable bd = (Android.Graphics.Drawables.BitmapDrawable)imageoutput.Drawable;
            Android.Graphics.Bitmap bitmap = bd.Bitmap;

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

        private void NegateColor(char color, ImageView imageoutput)
        {
            //Weird hoops you gotta go through to get some bitmaps :/
            Android.Graphics.Drawables.BitmapDrawable bd = (Android.Graphics.Drawables.BitmapDrawable)imageoutput.Drawable;
            Android.Graphics.Bitmap bitmap = bd.Bitmap;

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
                                c.R = (byte) (255 - c.R);
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

            //WHY ARE GALLERY IMAGES BIGGER THAN CAMERA IMAGES?!?!?!?!?!?!!!?!?!?!?!?!?!?!!?!?!?!?!?!?!?
            //If I find a fix this may actually do something
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

