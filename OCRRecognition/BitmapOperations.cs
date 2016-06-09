using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace OCRRecognition
{
    class BitmapOperations
    {
        public static string ResultTextFromOCR;

        public static async void SaveSmallBitmapAsJPG()
        {
            //http://stackoverflow.com/questions/477572/strange-out-of-memory-issue-while-loading-an-image-to-a-bitmap-object/823966#823966 

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



        //https://dev.projectoxford.ai/docs/services/56f91f2d778daf23d8ec6739/operations/56f91f2e778daf14a499e1fc/console 

        public static async void SendImageForOCR(string ImageFilePath)
        {


            VisionServiceClient VisionServiceClient = new VisionServiceClient("9de3f9782faa47789b4168aea74a6d7a");


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


            ResultTextFromOCR = stringBuilder.ToString();

        }

    }
}