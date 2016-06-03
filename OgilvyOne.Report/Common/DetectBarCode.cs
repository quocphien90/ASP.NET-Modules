using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace OgilvyOne.Report.Common
{
    public static class DetectBarCode
    {
        
        /// <summary>
        /// Kiểm tra xem file up lên có phải là hình
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns></returns>
        public static bool IsImage(HttpPostedFile postedFile, int ImageMinimumBytes = 5*1024*1024)
        {
            //  Check the image mime types
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //  Check the image extension
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //  Check các byte đầu
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }

               

                byte[] buffer = new byte[512];
                postedFile.InputStream.Read(buffer, 0, 512);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //  Try tạo mới Bitmap, nếu có lổi thì chắc không phải là hình ảnh
            try
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        public static ArrayList ScanFullDimension(FileUpload file, out string errorMsg)
        {
            
            ArrayList lstBarCodeArr = new ArrayList();
            string[] arrBarcode = null;
            errorMsg = "";
            try
            {

                if (file != null && file.PostedFile.ContentLength > 0)
                {
                    if (file.PostedFile.ContentLength > (5 * 1024 * 1024))
                    {
                        errorMsg = "File upload vượt quá giới hạn 5MB. Vui lòng tải ảnh khác";
                        return null;
                    }
                    
                    if (IsImage(file.PostedFile))
                    {


                        System.Drawing.Bitmap imageToResized = new System.Drawing.Bitmap(file.PostedFile.InputStream);

                        //---- Scan BarCode --------//
                        BarcodeImaging.FullScanPage(ref lstBarCodeArr, imageToResized, 100);

                        //----- Print Barcode --------//
                        if (lstBarCodeArr.Count > 0)
                        {

                            return lstBarCodeArr;
                        }
                        else
                        {
                            errorMsg = "File Upload không detect được barcode. Vui lòng tải ảnh khác";
                        }



                    }
                    else
                    {
                        errorMsg = "File upload không đúng định dạng. Vui lòng tải ảnh khác";

                    }
                }
                else
                {
                    errorMsg = "File upload không đúng định dạng. Vui lòng tải ảnh khác";

                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message.ToString();
            }

            return null;
        }

    }
}