//https://developer.xamarin.com/recipes/android/other_ux/pick_image/
//https://developer.xamarin.com/recipes/android/other_ux/camera_intent/take_a_picture_and_save_using_camera_app/

using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Widget;
using Java.IO;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

public static class App { //added a class here instead of a separate sheet
    public static File _file;
    public static File _dir;
    public static Bitmap bitmap;
    }


namespace OCRRecognition {
    [Activity(Label = "Gallery")]
    public class Gallery : Activity {

        //  public static readonly int TaskId = 1000; //An Id that is used to get the right OnActivityResult coming back 
        private ImageView _imageView;
        private string tag = "aaaa";
        //  Uri PicUri;// = Uri.Parse("content://media/external/images/media/5507");


        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ImageGallery);
            _imageView = FindViewById<ImageView>(Resource.Id.imageView1);

            CreateDirectoryForPictures();


            // _imageView.getl
            //    _imageView.SetImageResource(Resource.Drawable.sign);
            Button pickImage = FindViewById<Button>(Resource.Id.PickImage);
            pickImage.Click += pickImageClick;




            }

        private void CreateDirectoryForPictures() {
            App._dir = new File(
                Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "OCR");
            if (!App._dir.Exists()) {
                App._dir.Mkdirs();
                }
            }


        private void pickImageClick(object sender, EventArgs e) {

            //camera action
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            // App._file = new File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));

            //delete the last file so we only have one file called OCR.jpg
            App._file = new File(App._dir, "OCR.jpg");
            if (App._file.Exists()) {
                App._file.Delete();
                }

            App._file = new File(App._dir, "OCR.jpg");
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));

            StartActivityForResult(intent, 0);
            }

        //the result of the StartActivityForResult
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data) {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent); //tell everything about the new pic?

            // Display in ImageView. We will resize the bitmap to fit the display
            // Loading the full sized image will consume too much memory 
            // and cause the application to crash.


            var path = new File(
                Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "OCR").Path;

            var imagepath = System.IO.Path.Combine(path, "OCR.jpg");
            //the path to the image
            var imageFilePath = new File(imagepath);
            //load the image
            //  Bitmap image = BitmapFactory.DecodeFile(imageFilePath.AbsolutePath);

            if (System.IO.File.Exists(imageFilePath.ToString())) {
            int height = Resources.DisplayMetrics.HeightPixels;
            int width = _imageView.Height;

            BitmapHelpers.ResizeBitmap(imageFilePath.ToString(), width, height);
   }
            //if (image != null) {
            //    //  _imageView.SetImageBitmap(image);

            //    App.bitmap = null;
            //    }

            // Dispose of the Java side bitmap.
            GC.Collect();
            }

        }
    }