using System;
using System.IO;
using Android.Graphics;
using Android.Util;
using Environment = Android.OS.Environment;
using File = Java.IO.File;

namespace OCRRecognition {
    public static class BitmapHelpers {
        private static string tag = "aaaaa";

        public static void ResizeBitmap(string fileName, int width, int height) {
            // First we get the the dimensions of the file on disk then set all the options we need to resize it.
            //InJustDecodeBounds = false; if set to true, the decoder will return null (no bitmap), but the out... fields will still be set, allowing the caller to query the bitmap without having to allocate the memory for its pixels. 

            BitmapFactory.Options options = new BitmapFactory.Options();
            //To use this method, first decode with inJustDecodeBounds set to true, pass the options through and then decode again using the new inSampleSize value and inJustDecodeBounds set to false:
            options.InJustDecodeBounds = false;
            //  BitmapFactory.DecodeFile(fileName, options); //Decode a file path into a bitmap.

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            //just generates the sample size

            if (outHeight > height || outWidth > width) {

                int halfHeight = height / 2;
                int halfWidth = width / 2;

                // inSampleSize = outWidth > outHeight ? outHeight / height : outWidth / width;

                // Calculate the largest inSampleSize value that is a power of 2 and keeps both height and width larger than the requested height and width.
                while ((halfHeight / inSampleSize) > height && (halfWidth / inSampleSize) > width) {
                    inSampleSize *= 2;
                    }

                }
            Log.Info(tag, "inSampleSize " + inSampleSize);
            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            SaveBitmapAsJPG(resizedBitmap);
            //    return resizedBitmap;
            }




        public static void SaveBitmapAsJPG(Bitmap bitmap) {
            // var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;


            var path = new File(
                Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "OCR").AbsolutePath;
            var filePath = System.IO.Path.Combine(path, "test.jpg");
            var stream = new FileStream(filePath, FileMode.Create);
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, stream);
            stream.Close();
            // Free the native object associated with this bitmap, and clear the reference to the pixel data. This will not free the pixel data synchronously; it simply allows it to be garbage collected if there are no other references. The bitmap is marked as "dead", meaning it will throw an exception if getPixels() or setPixels() is called, and will draw nothing. 
            //  bitmap.Recycle();
            GC.Collect();
            }
        }
    }