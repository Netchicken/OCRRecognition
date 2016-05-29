using System;
using System.IO;
using Android.Graphics;
using Environment = Android.OS.Environment;
using File = Java.IO.File;

namespace OCRRecognition {
    public static class BitmapHelpers {


        public static void ResizeBitmap(string fileName, int width, int height) {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width) {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
                }

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
                Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "OCR").Path;
            var filePath = System.IO.Path.Combine(path, "test.jpg");
            var stream = new FileStream(filePath, FileMode.Create);
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, stream);
            stream.Close();

            GC.Collect();
            }
        }
    }