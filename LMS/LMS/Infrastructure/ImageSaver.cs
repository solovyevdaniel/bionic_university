using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using LMS.Models;
namespace LMS.Infrastructure
{
    public class ImageSaver
    {
        private String[] avaibleExtentions = { ".jpg", ".jpeg", ".bmp", ".gif", ".png" };
        public String[] AvaibleExtentions { get { return avaibleExtentions; } }
        public static String WorkingDir;
        /**/
        public Boolean ValidateHttpImageRoute(String path)
        {
            if (path != null && avaibleExtentions.Contains(Path.GetExtension(path)))
            {
                if (path.IndexOf("http://") == 0 || path.IndexOf("https://") == 0)
                    return true;
            }
            return false;
        }

        public String Rename(String fileName, HttpPostedFileBase file)
        {
            if (fileName != null && file != null)
            {

            }
            return String.Empty;
        }
        public String SaveImageToFolder(String fileName, HttpPostedFileBase file, String dirPath = null)
        {
            if (fileName != null)
            {
                String path = Path.Combine(dirPath == null ? WorkingDir : dirPath);
                if (path != null  && file != null)
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    String imagePath = Path.Combine(path, fileName + Path.GetExtension(file.FileName));

                    file.SaveAs(imagePath);
                    String createdPath = String.Format("/images/Courses/{0}/{0}", fileName);
                    return createdPath;
                }
                return String.Empty;
            }
            return String.Empty;
        }
    }
}