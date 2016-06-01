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

            path = new File(
             Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "OCR").AbsolutePath.ToString();
            filepath = System.IO.Path.Combine(path, "test.jpg");
        }
        private void LoadImage(object sender, EventArgs e)
        {
            //https://developer.xamarin.com/guides/xamarin-forms/working-with/images/
            StartActivity(typeof(Gallery));
            // var beachImage = new Image { Aspect = Aspect.AspectFit };
            // beachImage.Source = ImageSource.FromFile("waterfront.jpg");


        }
        private async void SendImage(object sender, EventArgs e)
        {
            //if the app is there load it 
            Bitmap bitmap = await GetImage();
            _imageView.SetImageBitmap(bitmap);


            MakeRequest(filepath);


        }


        async Task<Bitmap> GetImage()
        {


            //true if the caller has the required permissions and path contains the name of an existing file; otherwise, false. This method also returns false if path is null, an invalid path, or a zero-length string.
            //if (System.IO.File.Exists(filePath)) {
            Bitmap bitmap = await BitmapFactory.DecodeFileAsync(filepath);
            // _imageView.SetImageBitmap(bitmap);
            return bitmap;
            //   bitmap.Recycle();
            //  GC.Collect();

            //  App.bitmap = null;
            //} else {
            //Toast.MakeText(this, "Nope, no image here", ToastLength.Long).Show();
            //return null;
            //}
        }

        //https://dev.projectoxford.ai/docs/services/56f91f2d778daf23d8ec6739/operations/56f91f2e778daf14a499e1fc/console 

        static async void MakeRequest(string ImageFilePath)
        {

            // var uri = Android.Net.Uri.Parse("PathToYourResource"); 
            //  var Httpclient = new HttpClient();
            // var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            //  Httpclient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "");

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



        private static void ShowRetrieveText(OcrResults results)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (results != null && results.Regions != null)
            {
                stringBuilder.Append("Text: ");
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



