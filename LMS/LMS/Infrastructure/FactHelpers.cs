using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using LMS.Models;
using System.Data.Entity;
using System.Text;

namespace LMS.Infrastructure
{
    public static class Saver
    {
        public static String SaveFileToFolder(String fileName, HttpPostedFileBase file, String dirPath)
        {
            if (fileName != null && file != null && dirPath != null)
            {
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                String filePath = Path.Combine(dirPath, fileName/* + Path.GetExtension(file.FileName)*/);

                file.SaveAs(filePath);

                String createdPath = String.Format("/Files/FactFiles/{0}", fileName/* + Path.GetExtension(file.FileName)*/);

                
                string[] splitedStr = createdPath.Split(new Char[] { '/', '\\', '.' });
                var len = splitedStr.Count();
                var str = "/Files/FactFiles/" + splitedStr[len - 2] + "." + splitedStr[len - 1];
                return str;
            }
            return String.Empty;
        }
    }
}