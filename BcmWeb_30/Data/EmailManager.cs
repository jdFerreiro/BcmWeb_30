using BcmWeb_30.Data.EF;
using BcmWeb_30.Security;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BcmWeb_30 
{

    public static class EmailManager
    {

        private static string _pathImages = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Images");
        private static string _pathLogos = System.Web.Hosting.HostingEnvironment.MapPath("~/LogosEmpresa");

        public static void AppSettings(out string UserID, out string Password, out string SMTPPort, out string Host, out string From)
        {
            From = ConfigurationManager.AppSettings.Get("FromEmail");
            UserID = ConfigurationManager.AppSettings.Get("UserID");
            Password = ConfigurationManager.AppSettings.Get("Password");
            SMTPPort = ConfigurationManager.AppSettings.Get("SMTPPort");
            Host = ConfigurationManager.AppSettings.Get("Host");
        }
        public static void SendEmail(string From, string Subject, string Body, string text, string To, string UserID, string Password, string SMTPPort, string Host, bool showAplired)
        {


            LinkedResource _logo = new LinkedResource(String.Format("{0}\\Logo-BiaPlus.png", _pathImages), MediaTypeNames.Image.Jpeg)
            {
                ContentId = "Logo-BiaPlus",
                TransferEncoding = TransferEncoding.Base64
            };

            ContentType mimeType = new System.Net.Mime.ContentType("text/html");
            HtmlString _htmlString = new HtmlString(Body);

            AlternateView alternate = AlternateView.CreateAlternateViewFromString(_htmlString.ToHtmlString(), mimeType);
            alternate.LinkedResources.Add(_logo);
            if (showAplired)
            {
                LinkedResource _logoAplired = new LinkedResource(String.Format("{0}\\LogoAplired.png", _pathLogos), MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "LogoAplired",
                    TransferEncoding = TransferEncoding.Base64
                };
                alternate.LinkedResources.Add(_logoAplired);
            }


            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(To);
            mail.From = new MailAddress(From);
            mail.Subject = Subject;
            mail.Body = text;
            mail.AlternateViews.Add(alternate);
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.Normal;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = Host;
            smtp.Port = Convert.ToInt16(SMTPPort);
            smtp.Credentials = new NetworkCredential(UserID, Password);
            smtp.EnableSsl = false;
            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void EnviarEmailConfirmPassword(string Email, ControllerContext _context, RouteCollection routeCollection,
                                                      string esquema, string HostName)
        {
            string Text = string.Empty;
            string Html = string.Empty;
            Encriptador _Encriptar = new Encriptador();

            using (Entities db = new Entities())
            {
                tblUsuario Usuario = db.tblUsuario.Where(x => x.Email == Email).FirstOrDefault();

                string _encriptedId = _Encriptar.Encriptar(Usuario.IdUsuario.ToString(), Encriptador.HasAlgorimt.SHA1
                                                         , Encriptador.Keysize.KS256);

                var routeValues = new RouteValueDictionary();
                routeValues.Add("eui", _encriptedId);

                string _RecoverPasswordPage = UrlHelper.GenerateUrl(null, "RecoveryPage", "Account", esquema, HostName,
                        String.Empty, routeValues, routeCollection, _context.RequestContext, false);

                Text = string.Format(Resources.ForgotPasswordResource.ContenidoEmailTexto, Environment.NewLine, _RecoverPasswordPage
                                   , Usuario.Nombre);

                Html = string.Format(Resources.ForgotPasswordResource.ContenidoEmailHTML
                                   , Resources.ForgotPasswordResource.HeaderString
                                   , _RecoverPasswordPage
                                   , Usuario.Nombre
                                   , Resources.BCMWebPublic.labelAppSlogan
                                   , Resources.ForgotPasswordResource.RecuperarHeaderString);
            }

            string UserID = string.Empty;
            string Password = string.Empty;
            string SMTPPort = string.Empty;
            string Host = string.Empty;
            string From = string.Empty;

            AppSettings(out UserID, out Password, out SMTPPort, out Host, out From);
            SendEmail(From, Resources.ForgotPasswordResource.HeaderString, Html, Text, Email, UserID, Password, SMTPPort, Host, true);
        }
        public static void EnviarEmailContacto(string Nombre, string Email, string Asunto, string Mensaje, ControllerContext _context,
                                               RouteCollection routeCollection, string esquema, string HostName)
        {
            string Text = string.Empty;
            string Html = string.Empty;
            Encriptador _Encriptar = new Encriptador();

            Text = string.Format(Resources.ContactResource.MensajeEmailClienteTexto, Environment.NewLine, Nombre);

            Html = string.Format(Resources.ContactResource.MensajeEmailClienteHTML
                               , Resources.ContactResource.HeaderEmailClienteHtml
                               , Nombre
                               , Resources.BCMWebPublic.labelAppSlogan);

            string UserID = string.Empty;
            string Password = string.Empty;
            string SMTPPort = string.Empty;
            string Host = string.Empty;
            string From = string.Empty;

            AppSettings(out UserID, out Password, out SMTPPort, out Host, out From);
            SendEmail(From, Resources.ContactResource.SubjectEmailCliente, Html, Text, Email, UserID, Password, SMTPPort, Host, true);

            Text = string.Format(Resources.ContactResource.MensajeEmailApliredTexto, Environment.NewLine, Nombre, Email, Asunto, Mensaje);

            Html = string.Format(Resources.ContactResource.MensajeEmailApliredHTML
                               , Resources.ContactResource.HeaderEmailApliredHtml
                               , Nombre
                               , Email
                               , Asunto
                               , Mensaje
                               , Resources.BCMWebPublic.labelAppSlogan);

            SendEmail(From, Resources.ContactResource.SubjectEmailAplired, Html, Text, Resources.ContactResource.EmailContacto, UserID, Password, SMTPPort, Host, true);

        }
        public static void EnviarEmailUpdatePerfil(string Email, ControllerContext _context, RouteCollection routeCollection,
                                                      string esquema, string HostName, BcmWeb_30.Models.PerfilModelView Perfil)
        {
            string Text = string.Empty;
            string Html = string.Empty;
            Encriptador _Encriptar = new Encriptador();

            using (Entities db = new Entities())
            {
                tblUsuario Usuario = db.tblUsuario.Where(x => x.Email == Email).FirstOrDefault();

                string _encriptedId = _Encriptar.Encriptar(Usuario.IdUsuario.ToString(), Encriptador.HasAlgorimt.SHA1
                                                         , Encriptador.Keysize.KS256);

                var routeValues = new RouteValueDictionary();
                routeValues.Add("eui", _encriptedId);

                Text = string.Format(Resources.PerfilResource.ContenidoEmailTexto, Environment.NewLine
                                   , Perfil.Nombre, Perfil.Email, Perfil.Password);

                Html = string.Format(Resources.PerfilResource.ContenidoEmailHTML
                                   , Resources.ForgotPasswordResource.HeaderString
                                   , Resources.BCMWebPublic.labelAppSlogan
                                   , Perfil.Nombre
                                   , Perfil.Email
                                   , Perfil.Password);
            }

            string UserID = string.Empty;
            string Password = string.Empty;
            string SMTPPort = string.Empty;
            string Host = string.Empty;
            string From = string.Empty;

            AppSettings(out UserID, out Password, out SMTPPort, out Host, out From);
            SendEmail(From, Resources.PerfilResource.AsuntoEmailString, Html, Text, Email, UserID, Password, SMTPPort, Host, true);
        }
    }

}
