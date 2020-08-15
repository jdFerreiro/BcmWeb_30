using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Web;
using System.Web.UI;

namespace BcmWeb_30 
{
    public class PlanTrabajo_UploadControlHelper
    {
        public static readonly UploadControlValidationSettings ValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".docx" },
            MaxFileSize = 400000
        };

        public static void uploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                string fileName = e.UploadedFile.FileName;
                string resultFilePath = "~/Content/FileManager/AnexosPlanTrabajo/" + fileName;
                e.UploadedFile.SaveAs(HttpContext.Current.Request.MapPath(resultFilePath));
                IUrlResolutionService urlResolver = sender as IUrlResolutionService;
                if (urlResolver != null)
                    e.CallbackData = urlResolver.ResolveClientUrl(resultFilePath) + "?refresh=" + Guid.NewGuid().ToString();

            }
        }
    }
}