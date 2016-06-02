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
using Android.Util;
using Encoding = System.Text.Encoding;
using File = Java.IO.File;
using Microsoft.ProjectOxford;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;


namespace OCRRecognition
{
    [Activity(Label = "OCRRecognition", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private static TextView ResultText;
        private ImageView _imageView;
        private string path;
        private string filepath;
        private static string tag = "aaaa";
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            // Get our button from the layout resource,
            // and attach an event to it
            Button loadImage = FindViewById<Button>(Resource.Id.LoadImage);
            Button sendImage = FindViewById<Button>(Resource.Id.SendImage);
            ResultText = FindViewById<TextView>(Resource.Id.TextResult);
            loadImage.Click += LoadImage;
            sendImage.Click += SendImage;

            //    path = new File(
            //    Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "OCR").AbsolutePath.ToString();
            //   filepath = System.IO.Path.Combine(path, "small.jpg");
        }
        private void LoadImage(object sender, EventArgs e)
        {
            //https://developer.xamarin.com/guides/xamarin-forms/working-with/images/
            StartActivity(typeof(Gallery));
        }
        private async void SendImage(object sender, EventArgs e)
        {
            //if the app is there load it 
            Bitmap bitmap = await GetImage();
            _imageView.SetImageBitmap(bitmap);


            MakeRequest(App.smallfilePath());


        }


        async Task<Bitmap> GetImage()
        {
            Bitmap bitmap = await BitmapFactory.DecodeFileAsync(App.smallfilePath());
            // _imageView.SetImageBitmap(bitmap);
            return bitmap;

        }

        //https://dev.projectoxford.ai/docs/services/56f91f2d778daf23d8ec6739/operations/56f91f2e778daf14a499e1fc/console 

        static async void MakeRequest(string ImageFilePath)
        {


            VisionServiceClient VisionServiceClient = new VisionServiceClient("");


            using (System.IO.Stream imageFileStream = System.IO.File.OpenRead(ImageFilePath))
            {
                //
                // Upload an image and perform OCR
                //
                OcrResults ocrResult = await VisionServiceClient.RecognizeTextAsync(imageFileStream, "unk", true);

                //  var  results = new Microsoft.ProjectOxford.Vision.Contract.OcrResults();
                //   ResultText.Text = ocrResult.Language + " " + ocrResult.Regions + " " + ocrResult.Orientation + " " + ocrResult.ToString();

                ShowRetrieveText(ocrResult);

            }


        }

        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/5b4f4479-27e9-4d73-a2f3-9a7a5229db55/mvpsample-code-with-vision-sdk-in-c-on-ocr?forum=mlapi

        private static void ShowRetrieveText(OcrResults results)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (results != null && results.Regions != null)
            {
                stringBuilder.Append("OCR = ");
                stringBuilder.AppendLine();
                foreach (var item in results.Regions)
                {
                    foreach (var line in item.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            stringBuilder.Append(word.Text);
                            stringBuilder.Append(" ");
                        }

                        stringBuilder.AppendLine();
                    }

                    stringBuilder.AppendLine();
                }
            }


            ResultText.Text = stringBuilder.ToString();

        }


        //=================================================================================================

        //    //http://javatechig.com/xamarin/upload-bitmap-image-to-server-using-http-multipart-in-xamarin-android 
        //    // Request parameters
        //queryString["language"] = "unk";
        //    queryString["detectOrientation "] = "true";
        //    var uri = "https://api.projectoxford.ai/vision/v1.0/ocr?" + queryString;
        //    Log.Info(tag, "uri " + uri);

        //    // Request body
        //    // Bitmap image = Bitmap.CreateBitmap((Bitmap)Resource.Drawable.sign);

        //    // string url =
        //   // "http://lh4.ggpht.com/_gKQKwLZ8XUs/TAevIEb8FkI/AAAAAAAAC3w/8kMg7Yze__Q/s800/Funny-Signs-Sharp-45.jpg";

        //    byte[] byteData = Encoding.UTF8.GetBytes(ImageFilePath);

        //    using (var fileContent = new ByteArrayContent(byteData))
        //    {

        //        Log.Info(tag, "fileContent " + fileContent);

        //        fileContent.Headers.ContentType = new MediaTypeHeaderValue("< application/json >");


        //        Log.Info(tag, " fileContent.Headers.ContentType " + fileContent.Headers.ContentType);



        //        //gets sent here with uri and content
        //        HttpResponseMessage response = await Httpclient.PostAsync(uri, fileContent);
        //        ResultText.Text = response.ToString();

        //    }

    }


}



