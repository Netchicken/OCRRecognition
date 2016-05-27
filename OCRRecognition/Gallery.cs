//https://developer.xamarin.com/recipes/android/other_ux/pick_image/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uri = Android.Net.Uri;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace OCRRecognition {
    [Activity(Label = "Gallery")]
    public class Gallery : Activity {

        public static readonly int PickImageId = 1000; //An Id that is used to get the right OnActivityResult coming back 
        private ImageView _imageView;
        private string tag = "aaaa";



        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ImageGallery);
            _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            //Uri uri = Uri.Parse("content://media/external/images/media/5507");
            // _imageView.getl
            _imageView.SetImageResource(Resource.Drawable.sign);
            Button pickImage = FindViewById<Button>(Resource.Id.PickImage);
            pickImage.Click += pickImageClick;
            }

        private void pickImageClick(object sender, EventArgs e) {
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            //shows the gallery and passes back the chosen one
            StartActivityForResult(Intent.CreateChooser(Intent, "Select an Image"), PickImageId);
            }

        //the result of the StartActivityForResult
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data) {
            //      1000,                                 OK ,    The Data property of the Intent returned to OnActivityResult will contain the Uri of the selected image. We check the result in case the user cancelled the selection.
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null)) {
                //http://stackoverflow.com/questions/17356312/converting-of-uri-to-string/17356461#17356461

                String uriString = data.DataString;

                Log.Info(tag, data.DataString);

                Uri uri = Uri.Parse(uriString);

                _imageView.SetImageURI(uri);
                }
            }

        }



    }