using BcmWeb_30.Models;
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace BcmWeb_30 .Controllers
{
    public class FileManagerController : Controller
    {
        // GET: FileManager
        public ActionResult Index(long IdClaseDocumento, long modId)
        {
            Session["IdClaseDocumento"] = IdClaseDocumento;
            Session["IdModulo"] = modId;
            Session["modId"] = modId;

            Auditoria.RegistarAccion(eTipoAccion.AccederAnexoModuloWeb);

            return View();
        }
        [ValidateInput(false)]
        public ActionResult FileManagerPartial()
        {
            return PartialView("FileManagerPartial", FileManagerControllerFileManagerSettings.Model);
        }
        [SessionExpire]
        [HandleError]

        public FileStreamResult FileManagerPartialDownload()
        {
            return FileManagerExtension.DownloadFiles(FileManagerControllerFileManagerSettings.CreateFileManagerDownloadSettings(), FileManagerControllerFileManagerSettings.Model);
        }
        [HttpPost]
        [SessionExpire]
        [HandleError]
        public JsonResult RegistrarOperacion(string Tipo, string nombre, string viejo = "")
        {
            bool success = true;
            Auditoria.RegistarOperacionAnexoModulo(Tipo, nombre, viejo, true);

            return Json(new { success });
        }
        public JsonResult RegistrarOperacionIniciativa(string Tipo, string nombre, string viejo = "")
        {
            bool success = true;
            Auditoria.RegistarOperacionAnexoIniciativa(Tipo, nombre, viejo, true);
            Session["GridViewData"] = Metodos.GetIniciativas();

            return Json(new { success });
        }

    }
    public class FileManagerControllerFileManagerSettings
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        private static HttpServerUtility Server { get { return HttpContext.Current.Server; } }

        private static long IdEmpresa;
        private static int IdClaseDocumento;
        private static long IdModulo;

        public static string RootFolder = @"~\Content\FileManager";

        public static string Model
        {
            get
            {
                FileManagerModel model = new FileManagerModel();

                IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
                IdClaseDocumento = int.Parse(Session["IdClaseDocumento"].ToString());
                IdModulo = long.Parse(Session["IdModulo"].ToString());
                string _Modulo = Metodos.GetModuloName(IdModulo);
                string _rootFolder = string.Format("~\\Content\\FileManager\\E{0}\\{1}", IdEmpresa.ToString("000"), _Modulo);
                string path = Server.MapPath(_rootFolder);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return _rootFolder;
            }
        }
        public static FileManagerSettings CreateFileManagerDownloadSettings()
        {
            var settings = new FileManagerSettings();
            settings.SettingsEditing.AllowDownload = true;
            settings.Name = "FileManager";
            return settings;
        }
    }
    public class FileManagerPlanTrabajoControllerFileManagerSettings
    {
        public static string _IdIniciativa { get; set; }
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        private static HttpServerUtility Server { get { return HttpContext.Current.Server; } }

        private static long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());

        public static string RootFolder = @"~\Content\FileManager";

        public static string Model
        {
            get
            {
                string IdIniciativa = Session["IdIniciativa"].ToString();
                string _rootFolder = string.Format("{0}Content\\FileManager\\AnexosPlanTrabajo\\E{1}\\I{2}", Server.MapPath("/"), IdEmpresa.ToString("000"), IdIniciativa);

                if (!Directory.Exists(_rootFolder))
                    Directory.CreateDirectory(_rootFolder);

                return _rootFolder;
            }
        }
        public static FileManagerSettings CreateFileManagerDownloadSettings()
        {
            var settings = new DevExpress.Web.Mvc.FileManagerSettings();
            settings.SettingsEditing.AllowDownload = true;
            settings.Name = "FileManager";
            return settings;
        }
    }

}