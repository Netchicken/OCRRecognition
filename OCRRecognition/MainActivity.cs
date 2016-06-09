//https://www.microsoft.com/cognitive-services/en-us/computer-vision-api/documentation/ocr 
//https://dev.projectoxford.ai/docs/services/56f91f2d778daf23d8ec6739/operations/56f91f2e778daf14a499e1fa
using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Android.Graphics;
using Android.Media;
using Android.Provider;
using Android.Util;
using Android.Webkit;
using Encoding = System.Text.Encoding;
using File = Java.IO.File;
using Microsoft.ProjectOxford;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Color = Android.Graphics.Color;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;


namespace OCRRecognition
{
    [Activity(Label = "OCR Recognition", MainLauncher = true, Icon = "@drawable/OCR")]
    public class MainActivity : Activity
    {

        private static TextView ResultText;
        private ImageView OCRmageView;
        private WebView WebViewForGif;
        //  private string path;
        //  private string filepath;
        //  private static string tag = "aaaa";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //stops the keyboard from showing when the app starts
            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            OCRmageView = FindViewById<ImageView>(Resource.Id.imageView1);
            // Get our button from the layout resource,
            // and attach an event to it
            Button TakePic = FindViewById<Button>(Resource.Id.LoadImage);
            Button UploadPic = FindViewById<Button>(Resource.Id.SendImage);
            Button ReviewPic = FindViewById<Button>(Resource.Id.ReviewPic);
            ResultText = FindViewById<TextView>(Resource.Id.TextResult);
            WebViewForGif = FindViewById<WebView>(Resource.Id.webView1);
            TakePic.Click += TakePic_Click;
            UploadPic.Click += UploadPic_Click;
            ReviewPic.Click += ReviewPic_Click;
            WebViewForGif.Click += WebViewForGif_Click;
            CreateDirectoryForPictures();
            LoadAnimatedGif();
        }

        private void WebViewForGif_Click(object sender, EventArgs e)
        {

            //if its not there make it appear
            if (WebViewForGif.Visibility == ViewStates.Gone)
            {
                LoadAnimatedGif();
            }
            else
            { //if its there make it gone
                WebViewForGif.Visibility = ViewStates.Gone;

            }
        }

        private void CreateDirectoryForPictures()
        {
            AppPath.Dir = new File(AppPath.GetPath()); //get the directory path

            //if the folder doesn't exist then make it
            if (!AppPath.Dir.Exists())
            {
                AppPath.Dir.Mkdir();
            }
        }

        private async void ReviewPic_Click(object sender, EventArgs e)
        {
            //just a test here
            LoadAnimatedGif();

            //if the app is there load it 
            if (AppPath.smallfilePath().Contains("small.jpg"))
            {
                Bitmap bitmap = await LoadImage();
                OCRmageView.SetImageBitmap(bitmap);
            }
            else
            {
                Toast.MakeText(this, "No Image to show", ToastLength.Long);
            }
        }

        async Task<Bitmap> LoadImage()
        {
            Bitmap bitmap = await BitmapFactory.DecodeFileAsync(AppPath.smallfilePath());
            return bitmap;
        }

        private void TakePic_Click(object sender, EventArgs e)
        {

            //weirdity going on, have to rebuild the path here instead of using the one already in the AppPath class
            var path = System.IO.Path.Combine(AppPath.GetPath(), "OCR.jpg");
            AppPath.OCRfile = new File(path); //return a file
            //if the path exists then run the camera 
            if (AppPath.OCRfile.Exists())
            {
                //camera action
                Intent intent = new Intent(MediaStore.ActionImageCapture);

                //MediaStore – contents of the user’s device: audio (albums, artists, genres, playlists), images (including thumbnails) & video.
                intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(AppPath.OCRfile));

                StartActivityForResult(intent, 0);
            }

            else
            {
                Toast.MakeText(this, "No path working" + AppPath.OCRfile.ToString(), ToastLength.Long).Show();

            }

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


            BitmapOperations.SaveSmallBitmapAsJPG();

        }


        private void LoadAnimatedGif()
        {
            WebViewForGif.Visibility = ViewStates.Visible;
            // expects to find the 'loading_icon_small.gif' file in the 'root' of the assets folder, compiled as AndroidAsset.
            WebViewForGif.LoadUrl("file:///android_asset/loadingImage.gif");
           // this makes it transparent so you can load it over a background
            WebViewForGif.SetBackgroundColor(new Color(0, 0, 0, 0));
            WebViewForGif.SetLayerType(LayerType.Software, null);
           
        }

        private async void UploadPic_Click(object sender, EventArgs e)
        {

            //if the app is there load it 
            Bitmap bitmap = await GetImage();
            OCRmageView.SetImageBitmap(bitmap);

            BitmapOperations.SendImageForOCR(AppPath.smallfilePath());



            ResultText.Text = BitmapOperations.ResultTextFromOCR;

        }


        async Task<Bitmap> GetImage()
        {
            Bitmap bitmap = await BitmapFactory.DecodeFileAsync(AppPath.smallfilePath());
            // _imageView.SetImageBitmap(bitmap);
            WebViewForGif.Visibility = ViewStates.Gone;
            return bitmap;

        }
    }

    //    //https://dev.projectoxford.ai/docs/services/56f91f2d778daf23d8ec6739/operations/56f91f2e778daf14a499e1fc/console 

    //    static async void MakeRequest(string ImageFilePath)
    //    {


    //        VisionServiceClient VisionServiceClient = new VisionServiceClient("9de3f9782faa47789b4168aea74a6d7a");


    //        using (System.IO.Stream imageFileStream = System.IO.File.OpenRead(ImageFilePath))
    //        {
    //            //
    //            // Upload an image and perform OCR
    //            //
    //            OcrResults ocrResult = await VisionServiceClient.RecognizeTextAsync(imageFileStream, "unk", true);

    //            //  var  results = new Microsoft.ProjectOxford.Vision.Contract.OcrResults();
    //            //   ResultText.Text = ocrResult.Language + " " + ocrResult.Regions + " " + ocrResult.Orientation + " " + ocrResult.ToString();

    //            ShowRetrieveText(ocrResult);

    //        }


    //    }

    //    //https://social.msdn.microsoft.com/Forums/vstudio/en-US/5b4f4479-27e9-4d73-a2f3-9a7a5229db55/mvpsample-code-with-vision-sdk-in-c-on-ocr?forum=mlapi

    //    private static void ShowRetrieveText(OcrResults results)
    //    {
    //        StringBuilder stringBuilder = new StringBuilder();

    //        if (results != null && results.Regions != null)
    //        {
    //            stringBuilder.Append("OCR = ");
    //            stringBuilder.AppendLine();
    //            foreach (var item in results.Regions)
    //            {
    //                foreach (var line in item.Lines)
    //                {
    //                    foreach (var word in line.Words)
    //                    {
    //                        stringBuilder.Append(word.Text);
    //                        stringBuilder.Append(" ");
    //                    }

    //                    stringBuilder.AppendLine();
    //                }

    //                stringBuilder.AppendLine();
    //            }
    //        }


    //        ResultText.Text = stringBuilder.ToString();

    //    }



    //}


}



