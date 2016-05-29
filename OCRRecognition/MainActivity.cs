//https://www.microsoft.com/cognitive-services/en-us/computer-vision-api/documentation/ocr 
//https://dev.projectoxford.ai/docs/services/56f91f2d778daf23d8ec6739/operations/56f91f2e778daf14a499e1fa
using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using Android.Graphics;
using Android.Media;
using Encoding = System.Text.Encoding;

namespace OCRRecognition {
    [Activity(Label = "OCRRecognition", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity {
        private static TextView ResultText;
        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button loadImage = FindViewById<Button>(Resource.Id.LoadImage);
            Button sendImage = FindViewById<Button>(Resource.Id.SendImage);
            ResultText = FindViewById<TextView>(Resource.Id.TextResult);
            loadImage.Click += LoadImage;
            sendImage.Click += SendImage;
            }
        private void LoadImage(object sender, EventArgs e) {
            //https://developer.xamarin.com/guides/xamarin-forms/working-with/images/
            StartActivity(typeof(Gallery));
            // var beachImage = new Image { Aspect = Aspect.AspectFit };
            // beachImage.Source = ImageSource.FromFile("waterfront.jpg");


            }
        private void SendImage(object sender, EventArgs e) {
            MakeRequest();
            }


        //https://dev.projectoxford.ai/docs/services/56f91f2d778daf23d8ec6739/operations/56f91f2e778daf14a499e1fc/console 

        static async void MakeRequest() {

            //var uri = Android.Net.Uri.Parse("PathToYourResource"); 
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{}");

            // Request parameters
            queryString["language"] = "unk";
            queryString["detectOrientation "] = "true";
            var uri = "https://api.projectoxford.ai/vision/v1.0/ocr?" + queryString;

            HttpResponseMessage response;

            // Request body
            Bitmap image = Bitmap.CreateBitmap((Bitmap)Resource.Drawable.sign);
            byte[] byteData = Encoding.UTF8.GetBytes("{image}");

            using (var content = new ByteArrayContent(byteData)) {
                content.Headers.ContentType = new MediaTypeHeaderValue("< application/json >");
                response = await client.PostAsync(uri, content);
                ResultText.Text = response.ToString();

                }

            }
        }

    }

