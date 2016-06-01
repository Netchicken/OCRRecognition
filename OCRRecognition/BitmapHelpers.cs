//The BitmapFactory.decode* methods, discussed in the Load Large Bitmaps Efficiently lesson, should not be executed on the main UI thread if the source data is read from disk or a network location (or really any source other than memory). The time this data takes to load is unpredictable and depends on a variety of factors (speed of reading from disk or network, size of image, power of CPU, etc.). If one of these tasks blocks the UI thread, the system flags your application as non-responsive and the user has the option of closing it (see Designing for Responsiveness for more information). https://developer.android.com/training/displaying-bitmaps/process-bitmap.html  A 400 KB image file can easily take up 5-10 MB of RAM.

//The AsyncTask class provides an easy way to execute some work in a background thread and publish the results back on the UI thread. To use it, create a subclass and override the provided methods. 

// (25001): Throwing OutOfMemoryError "Failed to allocate a 63489036 byte allocation with 16777216 free bytes and 52MB until OOM" 

//http://stackoverflow.com/questions/32244851/androidjava-lang-outofmemoryerror-failed-to-allocate-a-23970828-byte-allocatio 

using System;
using System.Threading.Tasks;
using System.IO;
using Android.Graphics;
using Android.Util;
using Environment = Android.OS.Environment;
using File = Java.IO.File;

namespace OCRRecognition
{
    public static class BitmapHelpers
    {
        private static string tag = "aaaaa";

        public static async void ResizeBitmap(string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk then set all the options we need to resize it.
            //InJustDecodeBounds = false; if set to true, the decoder will return null (no bitmap), but the out... fields will still be set, allowing the caller to query the bitmap without having to allocate the memory for its pixels. 

            BitmapFactory.Options options = new BitmapFactory.Options();
            //To use this method, first decode with inJustDecodeBounds set to true, pass the options through and then decode again using the new inSampleSize value and inJustDecodeBounds set to false:
            options.InJustDecodeBounds = true;
            //  BitmapFactory.DecodeFile(fileName, options); //Decode a file path into a bitmap.

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            //just generates the sample size
            //For example, consider an image that is 4000x3000 pixels with a bitmap configuration of Argb8888. It would require approximately 46.8MB of RAM to load the full image into memory. It is better to load a smaller version of the image. To tell the decoder to subsample the image and load a smaller version into memory, set InSampleSize to a value that will be used to scale down the image. For example, setting InSampleSize to 2 will cause BitmapFactory to scale the image down by a factor of 2. Any value can be used, however BitmapFactory is optimized to use a value that is factor of 2.

            if (outHeight > height || outWidth > width)
            {

                int halfHeight = height / 2;
                int halfWidth = width / 2;

                // inSampleSize = outWidth > outHeight ? outHeight / height : outWidth / width;

                // Calculate the largest inSampleSize value that is a power of 2 and keeps both height and width larger than the requested height and width.
                while ((halfHeight / inSampleSize) > height && (halfWidth / inSampleSize) > width)
                {
                    inSampleSize *= 4;
                }

            }
            inSampleSize = 2;

            Log.Info(tag, "inSampleSize " + inSampleSize);
            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = await BitmapFactory.DecodeFileAsync(fileName, options);

            SaveBitmapAsJPG(resizedBitmap);
            //    return resizedBitmap;
        }




        public static async void SaveBitmapAsJPG(Bitmap bitmap)
        {
            // var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            //rotate a bmp
            Matrix matrix = new Matrix();
            matrix.PostRotate(90);
            var rotatedBitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);

            var path = new File(
                Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "OCR").AbsolutePath;
            var filePath = System.IO.Path.Combine(path, "test.jpg");
            var stream = new FileStream(filePath, FileMode.Create);
            await rotatedBitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 70, stream);
            stream.Close();
            await stream.FlushAsync();
            // Free the native object associated with this bitmap, and clear the reference to the pixel data. This will not free the pixel data synchronously; it simply allows it to be garbage collected if there are no other references. The bitmap is marked as "dead", meaning it will throw an exception if getPixels() or setPixels() is called, and will draw nothing. 
            //  bitmap.Recycle();
       //     GC.Collect();
        }
    }
}