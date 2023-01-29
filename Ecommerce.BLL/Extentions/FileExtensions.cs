using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Extentions
{
    public static class FileExtensions
    {
        public static bool IsImage(this IFormFile file)
        {
            if (!file.ContentType.Contains("image"))
                return false;

            return true;
        }

        public static bool IsAllowedSize(this IFormFile file, int mb)
        {
            if (file.Length > 1024 * 1024 * mb)
                return false;

            return true;
        }

        public async static Task<string> GenerateFile(this IFormFile file, string rootPath)
        {
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);
            var unicalName = $"{Guid.NewGuid()}-{file.FileName}";
            var path = Path.Combine(rootPath, unicalName);


            var fileStream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(fileStream);
            fileStream.Close();



            return unicalName;

        }
    }
}
