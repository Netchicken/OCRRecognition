//https://developer.xamarin.com/recipes/android/other_ux/pick_image/
//https://developer.xamarin.com/recipes/android/other_ux/camera_intent/take_a_picture_and_save_using_camera_app/

using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Widget;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Uri = Android.Net.Uri;

//public static class App
//{ //added a class here instead of a separate sheet
//    public static File OCRfile;
//    public static File smallfile;
//    public static File Dir;

//    //  private static File path;


//    public static string GetPath()
//    {
//        return new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "OCR").AbsolutePath;
//        //  return path;
//    }

//    public static string smallfilePath()
//    {
//        var path = System.IO.Path.Combine(GetPath(), "small.jpg");
//        smallfile = new File(path); //return a file
//        return path;  //return a string
//    }

//    public static string bigfilePath()
//    {
//        var path = System.IO.Path.Combine(GetPath(), "OCR.jpg");
//        OCRfile = new File(path); //return a file
//        return path;  //return a string
//    }



//}


namespace OCRRecognition
{
    [Activity(Label = "Take a Picture and view it.")]
    public class GalleryOLDDeprecated : Activity
    {

        //  public static readonly int TaskId = 1000; //An Id that is used to get the right OnActivityResult coming back 
        private ImageView OCRimageView;
        private string tag = "aaaa";
        //  Uri PicUri;// = Uri.Parse("content://media/external/images/media/5507");


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ImageGallery);
            OCRimageView = FindViewById<ImageView>(Resource.Id.imageView1);
          //  OCRimageView.i = (Drawable)Resource.Drawable.OCR;
            //  CreateDirectoryForPictures();


            // _imageView.getl
            //    _imageView.SetImageResource(Resource.Drawable.sign);
            Button pickImage = FindViewById<Button>(Resource.Id.PickImage);
            Button LoadImage = FindViewById<Button>(Resource.Id.LoadImage);
            pickImage.Click += pickImageClick;
            LoadImage.Click += loadImageClick;



        }

        private async void loadImageClick(object sender, EventArgs e)
        {
            //if the app is there load it 
            if (AppPath.smallfilePath().Contains("small.jpg"))
            {
                //async it
                Bitmap bitmap = await LoadImage();
                OCRimageView.SetImageBitmap(bitmap);
            }

            else
            {

                Toast.MakeText(this, "No image to show", ToastLength.Long);

            }
        }

        async Task<Bitmap> LoadImage()
        {
            //var path = new File(
            //    Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "OCR").AbsolutePath.ToString();
            //var filePath = System.IO.Path.Combine(path, "small.jpg");

            Bitmap bitmap = await BitmapFactory.DecodeFileAsync(AppPath.smallfilePath());
            // _imageView.SetImageBitmap(bitmap);
            return bitmap;

        }

        //private void CreateDirectoryForPictures()
        //{
        //    AppPath.Dir = new File(AppPath.GetPath()); //get the directory path

        //    //if the folder doesn't exist then make it
        //    if (!AppPath.Dir.Exists())
        //    {
        //        AppPath.Dir.Mkdir();
        //    }
        //}


        private void pickImageClick(object sender, EventArgs e)
        {

            //camera action
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            //MediaStore – contents of the user’s device: audio (albums, artists, genres, playlists), images (including thumbnails) & video.
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(AppPath.OCRfile));

            StartActivityForResult(intent, 0);
        }

        //the result of the StartActivityForResult
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery by adding it to the media database

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);

            AppPath.OCRfile = new File(AppPath.bigfilePath());
            Uri contentUri = Uri.FromFile(AppPath.OCRfile);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent); //tell everything about the new pic?


            SaveSmallBitmapAsJPG();

        }
        //http://stackoverflow.com/questions/477572/strange-out-of-memory-issue-while-loading-an-image-to-a-bitmap-object/823966#823966 


        public static async void SaveSmallBitmapAsJPG()
        {

            //  var path = new File(
            //                Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "OCR").AbsolutePath;
            //   var smallfilePath = System.IO.Path.Combine(path, "small.jpg");
            //   var bigfilePath = System.IO.Path.Combine(path, "OCR.jpg");

            BitmapFactory.Options opts = new BitmapFactory.Options();

            // load the image and have BitmapFactory resize it for us.
            opts.InSampleSize = 4; //  1/4 size
            opts.InJustDecodeBounds = false; //set to false to get the whole image not just the bounds
            Bitmap bitmap = await BitmapFactory.DecodeFileAsync(AppPath.bigfilePath(), opts);


            //rotate a bmp
            Matrix matrix = new Matrix();
            matrix.PostRotate(90);
            var rotatedBitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);


            //write it back
            using (var stream = new FileStream(AppPath.smallfilePath(), FileMode.Create))
            {
                await rotatedBitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 70, stream);//70% compressed
                stream.Close();
            }

            //    await stream.FlushAsync();
            // Free the native object associated with this bitmap, and clear the reference to the pixel data. This will not free the pixel data synchronously; it simply allows it to be garbage collected if there are no other references. The bitmap is marked as "dead", meaning it will throw an exception if getPixels() or setPixels() is called, and will draw nothing. 
            //  bitmap.Recycle();
            //     GC.Collect();
        }
    }
}