using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.SharedClasses
{
    public static class FilesProcessor
    {
        public static string UploadedFile(IFormFile model, int id, IWebHostEnvironment hostingEnvironment)
        {
            string uniqueFileName = null;
            if (model != null)
            {
                string path = Path.Combine(hostingEnvironment.WebRootPath, "attachments", id.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "attachments");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.FileName;
                string filePath = Path.Combine(path, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
