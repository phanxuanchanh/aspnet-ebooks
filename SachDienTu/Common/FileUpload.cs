using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace SachDienTu.Common
{
    public class FileUpload
    {
        public static string PDFFilePath = $"~/PDFs";

        public enum UploadState { Success, Failed, Failed_AlreadyExist, Failed_InvalidFile };
        public FileUpload()
        {

        }

        public bool CheckExists(string filePath)
        {
            if (File.Exists(filePath))
                return true;
            return false;
        }

        public UploadState UploadPDF(HttpPostedFileBase fileBase, ref string filePath)
        {
            if (fileBase == null)
                throw new Exception("@'fileBase' must be not null");
            if (!IsValidDocument(fileBase))
                return UploadState.Failed_InvalidFile;
            string path = $"{GeneratePDFFolder()}/{fileBase.FileName}";
            filePath = path.Replace(PDFFilePath, "");
            string fileMapPath = HttpContext.Current.Server.MapPath(path);
            if (CheckExists(fileMapPath))
                return UploadState.Failed_AlreadyExist;

            fileBase.SaveAs(fileMapPath);
            return UploadState.Success;
        }

        public string GeneratePDFFolder()
        {
            string path1 = $"{PDFFilePath}/{DateTime.Now.Year}";
            string mapPath = HttpContext.Current.Server.MapPath(path1);
            if (!Directory.Exists(mapPath))
            {
                Directory.CreateDirectory(mapPath);
            }
            string path2 = $"{path1}/{DateTime.Now.Month}";
            mapPath = HttpContext.Current.Server.MapPath(path2);
            if (!Directory.Exists(mapPath))
            {
                Directory.CreateDirectory(mapPath);
            }
            return path2;
        }

        public bool IsValidImage(HttpPostedFileBase fileBase)
        {
            List<string> contentType = new List<string>();
            contentType.Add("image/jpeg");
            contentType.Add("image/png");
            contentType.Add("image/gif");
            string mime = fileBase.ContentType;

            if (contentType.Contains(mime))
                return true;
            return false;
        }

        public bool IsValidDocument(HttpPostedFileBase fileBase)
        {
            List<string> contentType = new List<string>();
            //contentType.Add("application/epub+zip");
            contentType.Add("application/pdf");
            //contentType.Add("image/gif");
            string mime = fileBase.ContentType;

            if (contentType.Contains(mime))
                return true;
            return false;
        }

        public bool RemovePDF(string filePath)
        {
            string fullFilePath = $"{PDFFilePath}/{filePath}";
            string fullFileMapPath = HttpContext.Current.Server.MapPath(fullFilePath);
            if (File.Exists(fullFileMapPath))
            {
                File.Delete(fullFileMapPath);
                return true;
            }
            return false;
        }
    }
}