using Microsoft.AspNetCore.Http;
using rkbc.core.helper;
using rkbc.core.repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rkbc.core.helper
{
    public class ImageHelper
    {
        
        public static string GetThumbnailFileName(string fullImageFilename)
        {
            return (Path.Combine(Path.GetDirectoryName(fullImageFilename), Path.GetFileNameWithoutExtension(fullImageFilename) + "-thumb" + Path.GetExtension(fullImageFilename)));
        }

        public static void GenerateThumbnail(Bitmap src, int width, string fullImageFilename)
        {
            var height = (int)(((width * 1.0) / src.Width) * src.Height);
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(src, 0, 0, width, height);

            var name = GetThumbnailFileName(fullImageFilename);
            if (File.Exists(name)) File.Delete(name);
            result.Save(name, ImageFormat.Jpeg);
        }

        public static Bitmap ScaleImage(Bitmap image, int? maxWidth, int? maxHeight)
        {
            var ratioX = (double)(maxWidth ?? image.Width) / image.Width;
            var ratioY = (double)(maxHeight ?? image.Height) / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            //If any scaling is necessary
            if (ratio < 1.0)
            {
                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                var newImage = new Bitmap(newWidth, newHeight);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, newWidth, newHeight);
                }
                return newImage;
            }
            return image;
        }

        public static Bitmap RotateScaleAndPadToAspect(Bitmap image, int aspectX, int aspectY, int? maxWidth, ContentAlignment align, bool rotateToAspect)
        {
            //First, rotate the image so it is longest edge parallels the aspect ratio's longest edge
            if (rotateToAspect)
            {
                if (aspectY > aspectX && image.Width > image.Height) image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                else if (aspectX > aspectY && image.Height > image.Width) image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }

            //Now, pad the image if necessary------------------------------------------------------------
            var ratioDesired = (double)aspectX / (double)aspectY;
            var ratioActual = (double)image.Width / (double)image.Height;
            //Too wide, so pad top/bottom
            if (ratioActual > ratioDesired)
            {
                int ypad = (int)Math.Floor((1 / ratioDesired) * image.Width - image.Height);
                if (ypad >= 2)
                {
                    Bitmap newImageYPad = new Bitmap(image.Width, image.Height + ypad);
                    using (Graphics graphics = Graphics.FromImage(newImageYPad))
                    {
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        //No transparent for jpeg
                        graphics.Clear(Color.White);

                        float yalign = 0;   //Top
                        if (align == ContentAlignment.MiddleCenter || align == ContentAlignment.MiddleLeft || align == ContentAlignment.MiddleRight) yalign = ypad / 2;
                        if (align == ContentAlignment.BottomCenter || align == ContentAlignment.BottomLeft || align == ContentAlignment.BottomRight) yalign = ypad;
                        graphics.DrawImage(image, 0, yalign, image.Width, image.Height);
                    }
                    image = newImageYPad;
                }
            }
            //Too wide, so pad sides
            else if (ratioActual < ratioDesired)
            {
                var xpad = (int)Math.Floor(ratioDesired * image.Height - image.Width);
                Bitmap newImage = new Bitmap(image.Width + xpad, image.Height);
                if (xpad >= 2)
                {
                    Bitmap newImageXPad = new Bitmap(image.Width + xpad, image.Height);
                    using (Graphics graphics = Graphics.FromImage(newImageXPad))
                    {
                        //No transparent for jpeg
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.Clear(Color.White);

                        float xalign = 0;   //Left
                        if (align == ContentAlignment.BottomCenter || align == ContentAlignment.MiddleCenter || align == ContentAlignment.TopCenter) xalign = xpad / 2;
                        if (align == ContentAlignment.BottomRight || align == ContentAlignment.MiddleRight || align == ContentAlignment.TopRight) xalign = xpad;
                        graphics.DrawImage(image, xalign, 0, image.Width, image.Height);
                    }
                    image = newImageXPad;
                }
            }

            //Now Scale the image if necessary--------------------------------------------------------
            return (ScaleImage(image, maxWidth, null));
        }

        public static Bitmap CapMaxImagePixels(Bitmap image, int pixels)
        {
            Bitmap result = image;
            //Maximum image size
            if (image.Width * image.Height > pixels)
            {
                var size = image.Width * image.Height;
                var pct = Math.Sqrt(((double)pixels) / size);
                var nheight = (int)(image.Height * pct);
                var nwidth = (int)(image.Width * pct);
                result = new Bitmap(nwidth, nheight);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(image, 0, 0, nwidth, nheight);
                }
            }
            return (result);
        }

        public static Bitmap rotateImage(Bitmap image, int direction)
        {
            if (direction < 0) image.RotateFlip(RotateFlipType.Rotate270FlipNone);
            if (direction > 0) image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            return (image);
        }

        public static Bitmap cropImage(Bitmap image, Rectangle cropArea)
        {
            var fmat = image.RawFormat;
            Bitmap bmpImage = new Bitmap(image);
            image.Dispose();
            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            return (bmpCrop);
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public static void savePNGImage(Bitmap image, string filename)
        {
            //Get an ImageCodecInfo object that represents the PNG codec.
            //var imageCodecInfo = GetEncoderInfo("image/png");
            image.Save(filename, ImageFormat.Png);
        }

        public static void saveJpegImage(Bitmap image, string filename, long quality)
        {
            //Get an ImageCodecInfo object that represents the JPEG codec.
            var imageCodecInfo = GetEncoderInfo("image/jpeg");
            var parameters = new EncoderParameters(1);

            //Save the bitmap as a JPEG file with quality level.
            var parameter = new EncoderParameter(Encoder.Quality, quality);
            parameters.Param[0] = parameter;
            image.Save(filename, imageCodecInfo, parameters);
        }

        public static byte[] saveJpegImage(Bitmap image, long quality = 75)
        {
            //Get an ImageCodecInfo object that represents the JPEG codec.
            var imageCodecInfo = GetEncoderInfo("image/jpeg");
            var parameters = new EncoderParameters(1);

            //Save the bitmap as a JPEG file with quality level.
            var parameter = new EncoderParameter(Encoder.Quality, quality);
            parameters.Param[0] = parameter;
            MemoryStream ms = new MemoryStream();
            image.Save(ms, imageCodecInfo, parameters);
            return ms.ToArray();
        }
        
    }
}
