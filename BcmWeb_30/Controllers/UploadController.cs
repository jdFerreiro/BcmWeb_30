using DevExpress.Web;
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;

namespace BcmWeb_30 .Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        // GET: Upload
        [SessionExpire]
        [HandleError]
        public ActionResult Index()
        {
            ViewBag.Title = "Upload";
            return View();
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ImageUploadControlCallbackAction()
        {
            UploadControlExtension.GetUploadedFiles("uc", UploadControlHelper.LogoValidationSettings, UploadControlHelper.logoFileUploadComplete);
            return null;
        }
        [SessionExpire]
        [HandleError]
        public ActionResult DocsUploadControlCallbackAction()
        {
            UploadControlExtension.GetUploadedFiles("uc", UploadControlHelper.DocsValidationSettings, UploadControlHelper.docFileUploadComplete);
            return null;
        }
        [SessionExpire]
        [HandleError]
        public ActionResult ModalMode()
        {
            return View("ModalMode");
        }
    }

    public class UploadControlHelper
    {
        internal static HttpSessionState Session { get { return HttpContext.Current.Session; } }

        public const string LogoUploadDirectory = "~/LogosEmpresa/";
        public const string DocUploadDirectory = "~/Content/DocsFolder/";

        public static readonly UploadControlValidationSettings LogoValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif", ".bmp", },
            MaxFileSize = 20971520
        };
        public static readonly UploadControlValidationSettings DocsValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".doc", ".docx", ".txt", ".rtf" },
            MaxFileSize = 20971520
        };

        public static void docFileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {

            if (e.UploadedFile.IsValid)
            {
                string resultFilePath = HttpContext.Current.Request.MapPath(DocUploadDirectory + e.UploadedFile.FileName);
                try
                {
                    e.UploadedFile.SaveAs(resultFilePath, true); //Code Central Mode - Uncomment This Line
                    Session["uploadedFile"] = e.UploadedFile;
                    IUrlResolutionService urlResolver = sender as IUrlResolutionService;
                    if (urlResolver != null)
                    {
                        e.CallbackData = urlResolver.ResolveClientUrl(resultFilePath);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception(e.ErrorText);
            }
        }
        public static void logoFileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                string resultFilePath = HttpContext.Current.Request.MapPath(LogoUploadDirectory + e.UploadedFile.FileName);
                //e.UploadedFile.SaveAs(resultFilePath, true);//Code Central Mode - Uncomment This Line
                IUrlResolutionService urlResolver = sender as IUrlResolutionService;
                if (urlResolver != null)
                {
                    e.CallbackData = urlResolver.ResolveClientUrl(resultFilePath);
                }
            }
        }
    }
}