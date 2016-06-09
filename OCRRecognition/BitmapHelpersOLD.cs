//The BitmapFactory.decode* methods, discussed in the Load Large Bitmaps Efficiently lesson, should not be executed on the main UI thread if the source data is read from disk or a network location (or really any source other than memory). The time this data takes to load is unpredictable and depends on a variety of factors (speed of reading from disk or network, size of image, power of CPU, etc.). If one of these tasks blocks the UI thread, the system flags your application as non-responsive and the user has the option of closing it (see Designing for Responsiveness for more information). https://developer.android.com/training/displaying-bitmaps/process-bitmap.html  A 400 KB image file can easily take up 5-10 MB of RAM.

//The AsyncTask class provides an easy way to execute some work in a background thread and publish the results back on the UI thread. To use it, create a subclass and override the provided methods. 

// (25001): Throwing OutOfMemoryError "Failed to allocate a 63489036 byte allocation with 16777216 free bytes and 52MB until OOM" 

//http://stackoverflow.com/questions/32244851/androidjava-lang-outofmemoryerror-failed-to-allocate-a-23970828-byte-allocatio 

using System;
using System.Threading.Tasks;
using System.IO;
using Android.Graphics;
using Android.Util;
using Java.IO;
using Environment = Android.OS.Environment;
using File = Java.IO.File;

namespace OCRRecognition
{
    public static class BitmapHelpersOLD
    {
        private static string tag = "aaaaa";

        public static async void ResizeBitmap(string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk then set all the options we need to resize it.
            //InJustDecodeBounds = false; if set to true, the decoder will return null (no bitmap), but the out... fields will still be set, allowing the caller to query the bitmap without having to allocate the memory for its pixels. 

            //   BitmapFactory.Options options = new BitmapFactory.Options();
            //To use this method, first decode with inJustDecodeBounds set to true, pass the options through and then decode again using the new inSampleSize value and inJustDecodeBounds set to false:
            //   options.InJustDecodeBounds = true; //don't load the image, just the bounds of it

            //   var stream = new FileStream(fileName, FileMode.Create);
            //  BitmapFactory.DecodeFile(fileName, options); //Decode a file path into a bitmap.
            //   BitmapFactory.DecodeStreamAsync(stream, null, options);
            //   stream.Close();

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            //int outHeight = options.OutHeight;
            //int outWidth = options.OutWidth;
            //int inSampleSize = 1;

            //just generates the sample size
            //For example, consider an image that is 4000x3000 pixels with a bitmap configuration of Argb8888. It would require approximately 46.8MB of RAM to load the full image into memory. It is better to load a smaller version of the image. To tell the decoder to subsample the image and load a smaller version into memory, set InSampleSize to a value that will be used to scale down the image. For example, setting InSampleSize to 2 will cause BitmapFactory to scale the image down by a factor of 2. Any value can be used, however BitmapFactory is optimized to use a value that is factor of 2.

            //if (outHeight > height || outWidth > width)
            //{

            //    int halfHeight = height / 2;
            //    int halfWidth = width / 2;

            //    // inSampleSize = outWidth > outHeight ? outHeight / height : outWidth / width;

            //    // Calculate the largest inSampleSize value that is a power of 2 and keeps both height and width larger than the requested height and width.
            //    while ((halfHeight / inSampleSize) > height && (halfWidth / inSampleSize) > width)
            //    {
            //        inSampleSize *= 4;
            //    }

            //}
            //For example, an image with resolution 2048x1536 that is decoded with an inSampleSize of 4 produces a bitmap of approximately 512x384. Loading this into memory uses 0.75MB rather than 12MB for the full image (assuming a bitmap configuration of ARGB_8888).

            BitmapFactory.Options opts = new BitmapFactory.Options();

            //  inSampleSize = 4; //this overrides the while above setting the image to a 1/4 of the size

            //   Log.Info(tag, "inSampleSize " + inSampleSize);
            // Now we will load the image and have BitmapFactory resize it for us.
            opts.InSampleSize = 4;
            opts.InJustDecodeBounds = false; //set to false tog et the whole image not just the bounds
                                             //     Bitmap resizedBitmap = await BitmapFactory.DecodeFileAsync(fileName, opts);

            SaveBitmapAsJPG();

        }

        //http://stackoverflow.com/questions/477572/strange-out-of-memory-issue-while-loading-an-image-to-a-bitmap-object/823966#823966 


        public static async void SaveBitmapAsJPG()
        {

            var path = new File(
                           Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "OCR").AbsolutePath;
            var smallfilePath = System.IO.Path.Combine(path, "small.jpg");
            var bigfilePath = System.IO.Path.Combine(path, "OCR.jpg");

            BitmapFactory.Options opts = new BitmapFactory.Options();

            // load the image and have BitmapFactory resize it for us.
            opts.InSampleSize = 4; //  1/4 size
            opts.InJustDecodeBounds = false; //set to false to get the whole image not just the bounds
            Bitmap bitmap = await BitmapFactory.DecodeFileAsync(bigfilePath, opts);


            //rotate a bmp
            Matrix matrix = new Matrix();
            matrix.PostRotate(90);
            var rotatedBitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);


            //write it back
            using (var stream = new FileStream(smallfilePath, FileMode.Create))
            {
                //70% compressed
                await rotatedBitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 70, stream);
                stream.Close();
            }

            //    await stream.FlushAsync();
            // Free the native object associated with this bitmap, and clear the reference to the pixel data. This will not free the pixel data synchronously; it simply allows it to be garbage collected if there are no other references. The bitmap is marked as "dead", meaning it will throw an exception if getPixels() or setPixels() is called, and will draw nothing. 
            //  bitmap.Recycle();
            //     GC.Collect();
        }
    }
}