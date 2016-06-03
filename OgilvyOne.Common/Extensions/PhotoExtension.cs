using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace OgilvyOne.Common.Extensions
{
    public class PhotoExtension
    {
        public string PhotoPath { get; set; }
        public string PhotoThumbPath { get; set; }
        public string PhotoMergePath { get; set; }
        public int PhotoMaxSize { get; set; }
        public string PhotoExts { get; set; }

        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public int FileSize { get; set; }
        private HttpPostedFileBase FileUpload { get; set; }
        private FileUpload FileControlUpload { get; set; }
        private string sImageFromBase64 { get; set; }
        private byte[] FileBytes { get; set; }


        public PhotoExtension(HttpPostedFileBase objFile, int iRequiredMaxSize = 4096, string sNewFileName = "")
        {
            //this.PhotoPath = HttpContext.Current.Server.MapPath(this.PhotoPath);
            //this.PhotoThumbPath = HttpContext.Current.Server.MapPath(this.PhotoThumbPath);

            //this.PhotoExts = PhotoExts;

            int iMaxSize = 0;
            if (int.TryParse(iRequiredMaxSize.ToString(), out iMaxSize))
            {
                this.PhotoMaxSize = iMaxSize;
            }
            else
            {
                this.PhotoMaxSize = 4096; //4MB
            }

            this.FileUpload = objFile;
            this.FileExtension = Path.GetExtension(objFile.FileName);
            this.FileSize = objFile.ContentLength;

            if (sNewFileName != "")
            {
                this.FileName = sNewFileName;
                if (!sNewFileName.Contains("."))
                {
                    this.FileName += this.FileExtension.ToLower();
                }
            }
            else
            {
                this.FileName = Guid.NewGuid().ToString() + this.FileExtension.ToLower();
            }

            if (this.PhotoPath == "" || this.PhotoThumbPath == "" || this.PhotoExts == "")
            {
                throw new Exception("Requirement file path.");
            }
        }

        public PhotoExtension(FileUpload objFile, int iRequiredMaxSize = 4096, string sNewFileName = "")
        {
            //this.PhotoPath = MagicMomentConst.MagicMomentMediaPath;
            //this.PhotoThumbPath = MagicMomentConst.MagicMomentMediaThumbPath;
            //this.PhotoExts = MagicMomentConst.Photo_Exts;

            int iMaxSize = 0;
            if (int.TryParse(iRequiredMaxSize.ToString(), out iMaxSize))
            {
                this.PhotoMaxSize = iMaxSize;
            }
            else
            {
                this.PhotoMaxSize = 4096; //4MB
            }

            this.FileControlUpload = objFile;
            this.FileExtension = Path.GetExtension(objFile.FileName);
            this.FileSize = objFile.PostedFile.ContentLength;

            if (sNewFileName != "")
            {
                this.FileName = sNewFileName;
                if (!sNewFileName.Contains("."))
                {
                    this.FileName += this.FileExtension.ToLower();
                }
            }
            else
            {
                this.FileName = Guid.NewGuid().ToString() + this.FileExtension.ToLower();
            }

            if (this.PhotoPath == "" || this.PhotoThumbPath == "" || this.PhotoExts == "")
            {
                throw new Exception("Requirement file path.");
            }
        }

        public PhotoExtension(string sBase64StreamFile, string sFileExtension, int iRequiredMaxSize = 4096, string sNewFileName = "")
        {
            //this.PhotoPath = MagicMomentConst.MagicMomentMediaPath;
            //this.PhotoThumbPath = MagicMomentConst.MagicMomentMediaThumbPath;
            //this.PhotoExts = MagicMomentConst.Photo_Exts;

            int iMaxSize = 0;
            if (int.TryParse(iRequiredMaxSize.ToString(), out iMaxSize))
            {
                this.PhotoMaxSize = iMaxSize;
            }
            else
            {
                this.PhotoMaxSize = 4096; //4MB
            }


            this.FileBytes = Convert.FromBase64String(sBase64StreamFile);
            this.FileExtension = sFileExtension;
            this.FileSize = this.FileBytes.Length;

            if (sNewFileName != "")
            {
                this.FileName = sNewFileName;
                if (!sNewFileName.Contains("."))
                {
                    this.FileName += this.FileExtension.ToLower();
                }
            }
            else
            {
                this.FileName = Guid.NewGuid().ToString() + this.FileExtension.ToLower();
            }

            if (this.PhotoPath == "" || this.PhotoThumbPath == "" || this.PhotoExts == "")
            {
                throw new Exception("Requirement file path.");
            }
        }

        public bool SavePhoto(out string sErrorMessage)
        {
            sErrorMessage = "";
            if (this.FileUpload != null || this.FileControlUpload != null)
            {
                if (!this.IsValid(out sErrorMessage))
                {
                    return false;
                }

                if (this.FileUpload != null)
                {
                    this.FileUpload.SaveAs(  this.PhotoPath + this.FileName);
                }
                else
                {
                    this.FileControlUpload.SaveAs(this.PhotoPath + this.FileName);
                }
            }
            else
            {
                File.WriteAllBytes(this.PhotoPath + this.FileName, this.FileBytes); 
            }            
            return true;
        }

        private bool IsValid(out string sErrorMessage)
        {
            sErrorMessage = "";
            if (!(";" + this.PhotoExts.ToLower() + ";").Contains(";" + this.FileExtension.ToLower() + ";"))
            {
                sErrorMessage = "InvalidFileExtension";
                return false;
            }

            if (this.FileSize > this.PhotoMaxSize)
            {
                sErrorMessage = "FileUploadTooLarge";
                return false;
            }

            return true;
        }


        public void ResizeThumbImage(System.Drawing.Image imgToResize, int iMaximumWidth, int iMaximumHeight, bool isEnforceRatio, bool isAddPadding)
        {
            var image = imgToResize;//System.Drawing.Image.FromFile(this.PhotoPath + this.FileName);
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            var iCanvasWidth = iMaximumWidth;
            var iCanvasHeight = iMaximumHeight;
            var iNewImageWidth = iMaximumWidth;
            var iNewImageHeight = iMaximumHeight;
            var xPosition = 0;
            var yPosition = 0;


            if (isEnforceRatio)
            {
                var ratioX = iMaximumWidth / (double)image.Width;
                var ratioY = iMaximumHeight / (double)image.Height;
                var ratio = ratioX < ratioY ? ratioX : ratioY;
                iNewImageHeight = (int)(image.Height * ratio);
                iNewImageWidth = (int)(image.Width * ratio);

                if (isAddPadding)
                {
                    xPosition = (int)((iMaximumWidth - (image.Width * ratio)) / 2);
                    yPosition = (int)((iMaximumHeight - (image.Height * ratio)) / 2);
                }
                else
                {
                    iCanvasWidth = iNewImageWidth;
                    iCanvasHeight = iNewImageHeight;
                }
            }

            var thumbnail = new Bitmap(iCanvasWidth, iCanvasHeight);
            var graphic = Graphics.FromImage(thumbnail);

            if (isEnforceRatio && isAddPadding)
            {
                graphic.Clear(Color.White);
            }

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.DrawImage(image, xPosition, yPosition, iNewImageWidth, iNewImageHeight);

            thumbnail.Save(this.PhotoThumbPath + this.FileName, imageEncoders[1], encoderParameters);
        }

     
        private static Dictionary<string, ImageCodecInfo> encoders = null;

        /// <summary>
        /// A quick lookup for getting image encoders
        /// </summary>
        public static Dictionary<string, ImageCodecInfo> Encoders
        {
            //get accessor that creates the dictionary on demand
            get
            {
                //if the quick lookup isn't initialised, initialise it
                if (encoders == null)
                {
                    encoders = new Dictionary<string, ImageCodecInfo>();
                }

                //if there are no codecs, try loading them
                if (encoders.Count == 0)
                {
                    //get all the codecs
                    foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
                    {
                        //add each codec to the quick lookup
                        encoders.Add(codec.MimeType.ToLower(), codec);
                    }
                }

                //return the lookup
                return encoders;
            }
        }
        public bool SavePhotoFromBase64(string sImageFromBase64, string sNewFileName, string sOutputPath, out string sErrorMessage, int iRequiredMaxSize = 4096)
        {
            sErrorMessage = "";
            //this.PhotoPath = HttpContext.Current.Server.MapPath("~" + this.PhotoPath);
            //this.PhotoThumbPath = HttpContext.Current.Server.MapPath("~" + this.PhotoThumbPath);
            int MaxSize;
            MaxSize = iRequiredMaxSize; // 4Mb
            

            System.Drawing.Image image = null;
            try
            {
                if (sNewFileName != "")
                {
                    this.FileName = sNewFileName;
                    if (!sNewFileName.Contains("."))
                    {
                        this.FileName += ".jpg";
                    }
                }
                else
                {
                    this.FileName = Guid.NewGuid().ToString() + ".jpg";
                }

                if (this.PhotoPath == "" || this.PhotoThumbPath == "" )
                {
                    throw new Exception("Requirement file path.");
                }

                byte[] bytes = Convert.FromBase64String(sImageFromBase64);

                if (bytes.Length < MaxSize)
                {

                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        image = System.Drawing.Image.FromStream(ms);
                        image.Save(sOutputPath);
                        return true;

                    }
                }
                else
                {
                    sErrorMessage = "Image size is out of limit.";
                }
            }
            catch (Exception ex)
            {
                sErrorMessage = "Exception";
                sErrorMessage = ex.Message + ex.StackTrace;
            }
            return false;
        }

      
        ///// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            //a holder for the result
            Bitmap result = new Bitmap(width, height);
            // set the resolutions the same to avoid cropping due to resolution differences
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
                
            }

            //return the resulting bitmap
            return result;
        }

        /// <summary> 
        /// Saves an image as a jpeg image, with the given quality 
        /// </summary> 
        /// <param name="path">Path to which the image would be saved.</param> 
        /// <param name="quality">An integer from 0 to 100, with 100 being the 
        /// highest quality</param> 
        /// <exception cref="ArgumentOutOfRangeException">
        /// An invalid value was entered for image quality.
        /// </exception>
        public static void SaveJpeg(string path, System.Drawing.Image image, int quality)
        {
            //ensure the quality is within the correct range
            if ((quality < 0) || (quality > 100))
            {
                //create the error message
                string error = string.Format("Jpeg image quality must be between 0 and 100, with 100 being the highest quality.  A value of {0} was specified.", quality);
                //throw a helpful exception
                throw new ArgumentOutOfRangeException(error);
            }

            //create an encoder parameter for the image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            //get the jpeg codec
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

            //create a collection of all parameters that we will pass to the encoder
            EncoderParameters encoderParams = new EncoderParameters(1);
            //set the quality parameter for the codec
            encoderParams.Param[0] = qualityParam;
            //save the image using the codec and the parameters
            image.Save(path, jpegCodec, encoderParams);
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            //do a case insensitive search for the mime type
            string lookupKey = mimeType.ToLower();

            //the codec to return, default to null
            ImageCodecInfo foundCodec = null;

            //if we have the encoder, get it to return
            if (Encoders.ContainsKey(lookupKey))
            {
                //pull the codec from the lookup
                foundCodec = Encoders[lookupKey];
            }

            return foundCodec;
        }

        public void SaveResizeImage(System.Drawing.Image OriginalImage, int maxImgSize, string StoragePath, string fileName)
        {
            int imageThumbHeight = (OriginalImage.Height * maxImgSize) / OriginalImage.Width;
            int imageThumbWidth = maxImgSize;
            if (imageThumbHeight > maxImgSize)
            {
                imageThumbWidth = (OriginalImage.Width * maxImgSize) / OriginalImage.Height;
                imageThumbHeight = maxImgSize;
            }
            Bitmap bitmapThumb = ResizeImage(OriginalImage, imageThumbWidth, imageThumbHeight);
            string pathThumb = System.IO.Path.Combine(StoragePath, fileName);
            if (!System.IO.Directory.Exists(StoragePath))
            {
                System.IO.Directory.CreateDirectory(StoragePath);
            }
            //Bitmap target = new Bitmap(122, 65);
            //Graphics g = Graphics.FromImage(bitmapThumb);
            //g.DrawImage(bitmapThumb, new Point(122, 65));

            SaveJpeg(pathThumb, bitmapThumb, 100);
        }
        public void CropImage(int Width, int Height, string sourceFilePath, string saveFilePath)
        {
            // variable for percentage resize 
            float percentageResize = 0;
            float percentageResizeW = 0;
            float percentageResizeH = 0;

            // variables for the dimension of source and cropped image 
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            // Create a bitmap object file from source file 
            Bitmap sourceImage = new Bitmap(sourceFilePath);

            // Set the source dimension to the variables 
            int sourceWidth = sourceImage.Width;
            int sourceHeight = sourceImage.Height;

            // Calculate the percentage resize 
            percentageResizeW = ((float)Width / (float)sourceWidth);
            percentageResizeH = ((float)Height / (float)sourceHeight);

            // Checking the resize percentage 
            if (percentageResizeH < percentageResizeW)
            {
                percentageResize = percentageResizeW;
                destY = System.Convert.ToInt16((Height - (sourceHeight * percentageResize)) / 2);
            }
            else
            {
                percentageResize = percentageResizeH;
                destX = System.Convert.ToInt16((Width - (sourceWidth * percentageResize)) / 2);
            }

            // Set the new cropped percentage image
            int destWidth = (int)Math.Round(sourceWidth * percentageResize);
            int destHeight = (int)Math.Round(sourceHeight * percentageResize);

            // Create the image object 
            using (Bitmap objBitmap = new Bitmap(Width, Height))
            {
                objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                {
                    // Set the graphic format for better result cropping 
                    objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    objGraphics.DrawImage(sourceImage, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);

                    // Save the file path, note we use png format to support png file 
                    objBitmap.Save(saveFilePath, ImageFormat.Jpeg);
                    sourceImage.Dispose();
                }
            }
        }
        public static Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn = null;

            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);

            memoryStream.Position = 0;

            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;

            return bmpReturn;
        }
    }
}