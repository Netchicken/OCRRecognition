using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using File = Java.IO.File;
using Environment = Android.OS.Environment;

namespace OCRRecognition
{
    static class AppPath
    {
        //added a class here instead of a separate sheet
        public static File OCRfile;
        public static File smallfile;
        public static File Dir;

        //  private static File path;


        public static string GetPath()
        {
            return new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "OCR").AbsolutePath;
            //  return path;
        }

        public static string smallfilePath()
        {
            var path = System.IO.Path.Combine(GetPath(), "small.jpg");
            smallfile = new File(path); //return a file
            return path;  //return a string
        }

        public static string bigfilePath()
        {
            var path = System.IO.Path.Combine(GetPath(), "OCR.jpg");
            OCRfile = new File(path); //return a file
            return path;  //return a string
        }


    }
}