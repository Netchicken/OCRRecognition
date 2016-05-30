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
using System.Threading.Tasks;
using System.Web;
using Android.Graphics;
using Android.Media;
using Java.IO;
using Encoding = System.Text.Encoding;

namespace OCRRecognition {
    [Activity(Label = "OCRRecognition", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity {
        private static TextView ResultText;
        private ImageView _imageView;
        protected override void OnCreate(Bundle bundle) {
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
            }
        private void LoadImage(object sender, EventArgs e) {
            //https://developer.xamarin.com/guides/xamarin-forms/working-with/images/
            StartActivity(typeof(Gallery));
            // var beachImage = new Image { Aspect = Aspect.AspectFit };
            // beachImage.Source = ImageSource.FromFile("waterfront.jpg");


            }
        private async void SendImage(object sender, EventArgs e) {
            //if the app is there load it 
            Bitmap bitmap = await GetImage();
            _imageView.SetImageBitmap(bitmap);



            MakeRequest(bitmap);
            }


        async Task<Bitmap> GetImage() {
            var path = new File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "OCR").AbsolutePath.ToString();
            var filePath = System.IO.Path.Combine(path, "test.jpg");

            //true if the caller has the required permissions and path contains the name of an existing file; otherwise, false. This method also returns false if path is null, an invalid path, or a zero-length string.
            //if (System.IO.File.Exists(filePath)) {
            Bitmap bitmap = await BitmapFactory.DecodeFileAsync(filePath);
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

        static async void MakeRequest(Bitmap bitmap) {

           // var uri = Android.Net.Uri.Parse("PathToYourResource"); 
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
           // Bitmap image = Bitmap.CreateBitmap((Bitmap)Resource.Drawable.sign);
            byte[] byteData = Encoding.UTF8.GetBytes("{bitmap}");

            using (var content = new ByteArrayContent(byteData)) {
                content.Headers.ContentType = new MediaTypeHeaderValue("< application/json >");
                response = await client.PostAsync(uri, content);
                ResultText.Text = response.ToString();

                }

            }
        }

    }

