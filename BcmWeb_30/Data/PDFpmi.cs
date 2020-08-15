using BcmWeb_30.Data.EF;
using DevExpress.XtraRichEdit;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BcmWeb_30 
{
    public class PDFpmi
    {
        internal static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        internal static string Culture = HttpContext.Current.Request.UserLanguages[0];

        #region "Variables"
        private string _pathFile;

        private itsEvents _PDF_Events = new itsEvents();
        private PdfPTable _Table;
        private PdfPTable _TableResult;
        private PdfPCell _Cell;
        private static Document _Documento = new Document();
        private List<objFilaTabla> _Filas;
        private List<objCeldaTabla> _Celdas;

        private static float _ActualPageWidth;
        private static string _FileName;
        private static Chunk _Chunk;
        private static HttpServerUtility _Server = HttpContext.Current.Server;
        private static objContenido _Content = new objContenido();
        private static Uri _ContextUrl = HttpContext.Current.Request.Url;
        private static string _AppUrl = _ContextUrl.AbsoluteUri.Replace(_ContextUrl.PathAndQuery, string.Empty);
        private string _strDocURL = string.Empty;
        private static List<objContenido> _TableOfContent = null;
        private static string _ServerPath;
        private static string _tempFilePath;
        private static string _ImagenApliredPath;
        private static iTextSharp.text.Image _ImagenEmpresa;
        private static Font _FontHeader1 = FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.BLACK);
        private static Font _FontPieBold = FontFactory.GetFont("Arial", 6, Font.BOLD, BaseColor.BLACK);
        private static Font _FontPieNormal = FontFactory.GetFont("Arial", 6, Font.NORMAL, BaseColor.BLACK);
        private static Font _FontBold = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
        private static Font _FontNormal = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK);
        private static Font _Font9Bold = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK);
        private static Font _Font9Normal = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);
        private static Font _Font8Bold = FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK);
        private static Font _Font8Normal = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK);
        private static Font _FontBoldWhite = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.WHITE);
        private static Font _Font9BoldWhite = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.WHITE);
        private static Font _Font9BoldCalibri = FontFactory.GetFont("Calibri", 9, Font.BOLD, BaseColor.BLACK);
        private static Font _Font9NormalCalibri = FontFactory.GetFont("Calibri", 9, Font.NORMAL, BaseColor.BLACK);
        private static Font _FontBoldCalibri = FontFactory.GetFont("Calibri", 10, Font.BOLD, BaseColor.BLACK);
        private static Font _FontNormalCalibri = FontFactory.GetFont("Calibri", 10, Font.NORMAL, BaseColor.BLACK);

        private static Phrase _Phrase;
        private static string _FechaDocumento = DateTime.UtcNow.AddHours(-4).AddMinutes(-30).ToString("dd/MM/yyyy");
        private static PdfWriter _pdfWrite;
        //private static Paragraph _parrafo;
        private static string _INCInformeTitle;

        private static bool _importingPDF = false;

        private static Dictionary<string, Dictionary<string, string>> _htmlStyles = new Dictionary<string, Dictionary<string, string>>();
        #endregion
        #region "Private Methods"
        private bool GenerarArchivosTemporales()
        {
            bool Done = false;

            try
            {
                long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
                long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
                long IdUsuario = long.Parse(Session["UserId"].ToString());
                long IdTipoDocumento = long.Parse(Session["IdTipoDocumento"].ToString());
                eSystemModules Modulo = (eSystemModules)IdTipoDocumento;
                string TipoDocumento = Modulo.ToString();
                tblDocumentoContenido _contenido;
                int _orden = 0;

                using (Entities db = new Entities())
                {

                    tblDocumento _dataDocumento = (from d in db.tblDocumento
                                                   where d.IdEmpresa == IdEmpresa
                                                        && d.IdDocumento == IdDocumento
                                                        && d.IdTipoDocumento == IdTipoDocumento
                                                   select d).FirstOrDefault();

                    if (_dataDocumento != null && _dataDocumento.tblDocumentoContenido != null)
                    {

                        switch (Modulo)
                        {
                            case eSystemModules.PAD:
                                _orden++;
                                _contenido = _dataDocumento.tblDocumentoContenido.Where(x => x.IdSubModulo == 10010100).FirstOrDefault();
                                GenerarPDF_Temporal(_contenido, TipoDocumento, _orden);
                                _orden++;
                                _contenido = _dataDocumento.tblDocumentoContenido.Where(x => x.IdSubModulo == 10010200).FirstOrDefault();
                                GenerarPDF_Temporal(_contenido, TipoDocumento, _orden);
                                _orden++;
                                _contenido = _dataDocumento.tblDocumentoContenido.Where(x => x.IdSubModulo == 10020100).FirstOrDefault();
                                GenerarPDF_Temporal(_contenido, TipoDocumento, _orden);
                                _orden++;
                                _contenido = _dataDocumento.tblDocumentoContenido.Where(x => x.IdSubModulo == 10010300).FirstOrDefault();
                                GenerarPDF_Temporal(_contenido, TipoDocumento, _orden);
                                break;
                            default:
                                foreach (tblDocumentoContenido contenido in _dataDocumento.tblDocumentoContenido.OrderBy(x => x.IdSubModulo).ToList())
                                {
                                    _orden++;
                                    if (contenido.ContenidoBin != null && contenido.ContenidoBin.LongLength > 0)
                                    {
                                        GenerarPDF_Temporal(contenido, TipoDocumento, _orden);
                                    }
                                }
                                break;
                        }
                    }
                }
                Done = true;
            }
            catch
            {
                Done = false;
                throw;
            }
            return Done;
        }
        private void GenerarPDF_Temporal(tblDocumentoContenido contenido, string TipoDocumento, int _orden)
        {
            RichEditDocumentServer _reServer = new RichEditDocumentServer();
            MemoryStream _ms = new MemoryStream();
            string _tempFileName = string.Format("tmp{0}_{1}_{2}_{3}_{4}.pdf", TipoDocumento,
                                                    contenido.IdEmpresa.ToString("#0"),
                                                    _orden.ToString(),
                                                    contenido.IdDocumento.ToString("#0"),
                                                    contenido.IdSubModulo.ToString("00000000"));

            string _tempFile = string.Format("{0}/{1}", _tempFilePath, _tempFileName);

            if (File.Exists(_tempFile))
            {
                File.Delete(_tempFile);
            }

            FileStream _fileStream = new FileStream(_tempFile, FileMode.Create);
            MemoryStream _msData = new MemoryStream(contenido.ContenidoBin.ToArray());

            _reServer.Document.LoadDocument(_msData, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
            _reServer.ExportToPdf(_ms);
            _reServer.EndUpdate();

            _ms.WriteTo(_fileStream);
            _ms.Close();
            _msData.Close();
            _fileStream.Close();
        }
        private void GenerarPDF_Temporal(byte[] ContenidoBin, string _tempFileName)
        {
            RichEditDocumentServer _reServer = new RichEditDocumentServer();
            MemoryStream _ms = new MemoryStream();

            string _tempFile = string.Format("{0}/{1}", _tempFilePath, _tempFileName);

            if (File.Exists(_tempFile))
            {
                File.Delete(_tempFile);
            }

            FileStream _fileStream = new FileStream(_tempFile, FileMode.Create);
            MemoryStream _msData = new MemoryStream(ContenidoBin.ToArray());

            _reServer.Document.LoadDocument(_msData, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
            _reServer.ExportToPdf(_ms);
            _reServer.EndUpdate();

            _ms.WriteTo(_fileStream);
            _ms.Close();
            _msData.Close();
            _fileStream.Close();
        }
        private void GenerarIndice(string Modulo)
        {
            _Filas = new List<objFilaTabla>();
            _Celdas = new List<objCeldaTabla>();

            _Celdas.Add(new objCeldaTabla
            {
                Alineacion = Element.ALIGN_CENTER,
                Valor = Resources.PDFResource.CeldaCapituloHeader
            });
            _Celdas.Add(new objCeldaTabla
            {
                Alineacion = Element.ALIGN_CENTER,
                Valor = Resources.PDFResource.CeldaPaginaHeader
            });

            _Filas.Add(new objFilaTabla
            {
                Anchos = new int[] { 90, 10 },
                Titulo = true,
                Valores = _Celdas
            });

            foreach (objContenido _item in _TableOfContent)
            {
                if (_item.Capitulo != null && _item.Capitulo.Trim().Length > 0)
                {
                    _Celdas = new List<objCeldaTabla>();
                    if (_item.Indent)
                    {
                        int _NumSpaces = 5 * _item.NoIndent;
                        _item.Capitulo = new string(' ', _NumSpaces);
                    }
                    _Celdas.Add(new objCeldaTabla
                    {
                        Alineacion = Element.ALIGN_LEFT,
                        Valor = _item.Capitulo
                    });
                    _Celdas.Add(new objCeldaTabla
                    {
                        Alineacion = Element.ALIGN_CENTER,
                        Valor = _item.Page.ToString()
                    });

                    _Filas.Add(new objFilaTabla
                    {
                        Anchos = new int[] { 0, 0, 0, 0 },
                        Titulo = false,
                        Valores = _Celdas
                    });
                }
            }
            PaginaConTablas(Modulo, _Filas, Resources.PDFResource.ContenidoTituloString, "", 1);
        }
        private void PaginaConTablas(string Modulo, List<objFilaTabla> Filas, string Titulo, string TituloTabla, int HeaderRows)
        {
            Paragraph _Parrafo = new Paragraph
            {
                Alignment = Element.ALIGN_CENTER,
                Font = _FontHeader1,
                SpacingAfter = 10,
                SpacingBefore = 10
            };
            _Parrafo.Add(new Phrase(Modulo));
            _Documento.NewPage();
            _Documento.Add(_Parrafo);

            _Parrafo = new Paragraph
            {
                Alignment = Element.ALIGN_CENTER,
                Font = FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.RED),
                SpacingAfter = 5,
                SpacingBefore = 0
            };
            _Parrafo.Add(new Phrase(Titulo));
            _Documento.Add(_Parrafo);

            _TableResult = ArmarPaginasConTablas(_Filas, TituloTabla, HeaderRows);
            _Documento.Add(_TableResult);
        }
        private PdfPTable ArmarPaginasConTablas(List<objFilaTabla> Filas, string TituloTabla, int HeaderRows)
        {
            int _NroCeldas = Filas[0].Valores.Count;

            _Table = new PdfPTable(_NroCeldas)
            {
                HeaderRows = HeaderRows,
                WidthPercentage = 100,
                SpacingBefore = 30.0F,
                HorizontalAlignment = Element.ALIGN_CENTER,
            };
            _Table.SetWidths(Filas[0].Anchos);

            if (TituloTabla.Trim().Length > 0)
            {
                _Chunk = new Chunk
                {
                    Font = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.WHITE)
                };
                _Table.HeaderRows++;
                _Chunk.Append(TituloTabla);
                _Phrase = new Phrase(_Chunk);
                _Cell = new PdfPCell(_Phrase)
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Colspan = _NroCeldas,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BackgroundColor = new BaseColor(System.Drawing.Color.FromArgb(0x4a3c8c)),
                    BorderColor = BaseColor.WHITE
                };
                _Table.AddCell(_Cell);
            }

            try
            {
                foreach (objFilaTabla _fila in Filas)
                {
                    if (_fila.Titulo)
                    {
                        foreach (objCeldaTabla _Celda in _fila.Valores)
                        {
                            _Chunk = new Chunk(_Celda.Valor)
                            {
                                Font = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.WHITE)
                            };
                            _Cell = new PdfPCell(new Phrase(_Chunk))
                            {
                                Colspan = (_Celda.ColSpan > 0 ? _Celda.ColSpan : 1),
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                BackgroundColor = new BaseColor(System.Drawing.Color.FromArgb(0x4a3c8c)),
                                BorderColor = BaseColor.WHITE
                            };
                            _Table.AddCell(_Cell);
                        }
                    }
                    else if (_fila.SubTitulo)
                    {
                        foreach (objCeldaTabla _Celda in _fila.Valores)
                        {
                            _Chunk = new Chunk(_Celda.Valor)
                            {
                                Font = FontFactory.GetFont("Arial", 9, Font.BOLD, new BaseColor(System.Drawing.Color.FromArgb(0x4a3c8c)))
                            };
                            _Cell = new PdfPCell(new Phrase(_Chunk))
                            {
                                Colspan = (_Celda.ColSpan > 0 ? _Celda.ColSpan : 1),
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                BackgroundColor = new BaseColor(System.Drawing.Color.FromArgb(0x4a3c8c)),
                                BorderColor = BaseColor.WHITE
                            };
                            _Table.AddCell(_Cell);
                        }
                    }
                    else
                    {
                        foreach (objCeldaTabla _Celda in _fila.Valores)
                        {
                            switch (_Celda.Valor)
                            {
                                case "FondoRojo":
                                    _Cell = new PdfPCell(new Phrase(""))
                                    {
                                        Colspan = (_Celda.ColSpan > 1 ? _Celda.ColSpan : 1),
                                        BackgroundColor = BaseColor.RED,
                                        VerticalAlignment = Element.ALIGN_TOP,
                                        BorderColor = BaseColor.BLACK
                                    };
                                    _Table.AddCell(_Cell);
                                    break;
                                case "FondoAmarillo":
                                    _Cell = new PdfPCell(new Phrase(""))
                                    {
                                        Colspan = (_Celda.ColSpan > 1 ? _Celda.ColSpan : 1),
                                        BackgroundColor = BaseColor.YELLOW,
                                        VerticalAlignment = Element.ALIGN_TOP,
                                        BorderColor = BaseColor.BLACK
                                    };
                                    _Table.AddCell(_Cell);
                                    break;
                                case "FondoVerde":
                                    _Cell = new PdfPCell(new Phrase(""))
                                    {
                                        Colspan = (_Celda.ColSpan > 1 ? _Celda.ColSpan : 1),
                                        BackgroundColor = BaseColor.GREEN,
                                        VerticalAlignment = Element.ALIGN_TOP,
                                        BorderColor = BaseColor.BLACK
                                    };
                                    _Table.AddCell(_Cell);
                                    break;
                                default:
                                    _Chunk = new Chunk(_Celda.Valor)
                                    {
                                        Font = FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK)
                                    };
                                    _Cell = new PdfPCell(new Phrase(_Chunk))
                                    {
                                        HorizontalAlignment = _Celda.Alineacion,
                                        Colspan = (_Celda.ColSpan > 1 ? _Celda.ColSpan : 1),
                                        VerticalAlignment = Element.ALIGN_TOP,
                                        BorderColor = BaseColor.BLACK
                                    };
                                    _Table.AddCell(_Cell);
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return _Table;
        }
        #endregion
        #region "Public Methods"
        public static PdfPTable HeaderDocs(int PageNumber, string Titulo)
        {
            PdfPCell _Cell;
            Image _Imagen;
            Image _ImagenAplired;
            float _TotalTableWidth = _ActualPageWidth;
            float[] _TableWidths = new float[] { (_TotalTableWidth * 0.190F), (_TotalTableWidth * 0.62F), (_TotalTableWidth * 0.190F) };


            PdfPTable _Table = new PdfPTable(3);
            _Table.SetWidths(_TableWidths);
            _Table.WidthPercentage = 92.0F;
            _Table.SpacingBefore = 15.0F;
            _Table.SpacingAfter = 10.0F;
            _Table.HorizontalAlignment = Element.ALIGN_RIGHT;

            _Imagen = Image.GetInstance(_ImagenEmpresa);
            _Imagen.Alignment = Element.ALIGN_RIGHT;
            _Imagen.ScalePercent(40);

            _Cell = new PdfPCell(_Imagen)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingTop = 15f,
                PaddingBottom = 5f,
                Rowspan = 3
            };
            _Table.AddCell(_Cell);
            _Phrase = new Phrase(Titulo, _Font9Bold);
            _Cell = new PdfPCell(_Phrase)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Rowspan = 3
            };
            _Table.AddCell(_Cell);

            _ImagenAplired = Image.GetInstance(_ImagenApliredPath);
            _ImagenAplired.Alignment = Element.ALIGN_RIGHT;
            _ImagenAplired.ScalePercent(10);

            _Cell = new PdfPCell(_ImagenAplired)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingTop = 10f,
                PaddingBottom = 5f,
                //FixedHeight = _ImagenAplired.Height * 0.2f,
            };

            _Table.AddCell(_Cell);
            string _StringFecha = String.Format(Resources.PDFResource.FechaHeaderString, _FechaDocumento);
            _Cell = new PdfPCell(new Phrase(_StringFecha, _FontBoldCalibri))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingTop = -15f,
                PaddingBottom = -15f,
            };
            _Table.AddCell(_Cell);
            //_Phrase = new Phrase(String.Format(Resources.PDFResource.PaginaHeaderString, PageNumber.ToString("#0")), _Font9Normal);
            //_Cell = new PdfPCell(_Phrase)
            //{
            //    HorizontalAlignment = Element.ALIGN_CENTER,
            //    VerticalAlignment = Element.ALIGN_MIDDLE
            //};
            //_Table.AddCell(_Cell);

            return _Table;
        }

        #endregion
        public string Generar_Documento(bool Mostrar, long IdIncidente)
        {

            _ServerPath = _Server.MapPath(".").Replace("\\PMI", string.Empty);
            _tempFilePath = string.Format("{0}\\PDFDocs\\tempPDF", _ServerPath);
            _ImagenApliredPath = string.Format("{0}\\LogosEmpresa\\LogoAplired.png", _ServerPath);

            try
            {
                if (!Directory.Exists(_tempFilePath)) Directory.CreateDirectory(_tempFilePath);

                long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
                long IdModulo = long.Parse(Session["modId"].ToString());

                List<PdfReader> reader = new List<PdfReader>();

                _ActualPageWidth = PageSize.LETTER.Width;

                using (Entities db = new Entities())
                {
                    tblIncidentes _DataEjecucion = db.tblIncidentes
                        .FirstOrDefault(Doc => Doc.IdEmpresa == IdEmpresa && Doc.IdIncidente == IdIncidente);

                    if (_DataEjecucion != null)
                    {
                        tblEmpresa _Empresa = db.tblEmpresa.FirstOrDefault(em => em.IdEmpresa == IdEmpresa);

                        _FileName = string.Format("INC{0}{1}.pdf", IdEmpresa.ToString(), _DataEjecucion.IdIncidente.ToString());
                        _pathFile = string.Format("{0}\\PDFDocs\\{1}", _ServerPath, _FileName);
                        _strDocURL = string.Format("{0}/PDFDocs/{1}", _AppUrl, _FileName);
                        Uri _DocURL = new Uri(_strDocURL, System.UriKind.RelativeOrAbsolute);
                        string _LogoEmpresaPath = string.Format("{0}{1}", _ServerPath, _Empresa.LogoURL.Replace("/", "\\").Replace("~", ""));

                        _ImagenEmpresa = Image.GetInstance(_LogoEmpresaPath);
                        _ImagenEmpresa.Alignment = Element.ALIGN_CENTER;

                        if (File.Exists(_pathFile))
                            File.Delete(_pathFile);

                        _Documento = new Document(PageSize.LETTER);
                        _pdfWrite = PdfWriter.GetInstance(_Documento, new FileStream(_pathFile, FileMode.OpenOrCreate));
                        _pdfWrite.PageEvent = _PDF_Events;

                        string[] docKeywords = new string[] {
                            _FileName,
                            _Empresa.NombreComercial,
                            "INC",
                        };
                        _INCInformeTitle = "Información correspondiente al incidente";
                        _importingPDF = false;

                        _Documento.Open();
                        _Documento.AddAuthor("www.BcmWeb_30.net");
                        _Documento.AddCreator("www.BcmWeb_30.net");
                        _Documento.AddKeywords(_FileName);
                        _Documento.AddLanguage("Spanish/Español");

                        _Table = new PdfPTable(5);
                        _Table.WidthPercentage = 90;
                        _Table.SpacingBefore = 15.0F;
                        _Table.SpacingAfter = 10.0F;
                        double _TotalTableWidth = _ActualPageWidth * 0.9;
                        float[] _TableWidths = new float[] { (float)(_TotalTableWidth * 0.2), (float)(_TotalTableWidth * 0.25), (float)(_TotalTableWidth * 0.05), (float)(_TotalTableWidth * 0.2), (float)(_TotalTableWidth * 0.3) };
                        _Table.SetWidths(_TableWidths);
                        _Table.HorizontalAlignment = Element.ALIGN_CENTER;


                        _Phrase = new Phrase(_INCInformeTitle, _FontHeader1);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 5,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(" ", _FontHeader1);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 5,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Línea 1
                         * **************** */
                        _Phrase = new Phrase(string.Format("{0}:", Resources.PMIResource.captionFechaIncidente.Trim()), _Font9Bold);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(((DateTime)_DataEjecucion.FechaIncidente).ToString("dd/MM/yyyy HH:mm"), _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(" ", _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(string.Format("{0}:", Resources.PMIResource.captionTipoIncidente.Trim()), _Font9Bold);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(_DataEjecucion.tblTipoIncidente.tblCulture_TipoIncidente.FirstOrDefault(x => x.Culture == Culture || x.Culture == "es-VE").Descripcion, _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Separador 1
                         ** **************** */
                        _Phrase = new Phrase(" ", _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 5,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Línea 2
                         * **************** */
                        _Phrase = new Phrase(string.Format("{0}:", Resources.PMIResource.captionNaturalezaIncidente.Trim()), _Font9Bold);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(_DataEjecucion.tblNaturalezaIncidente.tblCulture_NaturalezaIncidente.FirstOrDefault(x => x.Culture == Culture || x.Culture == "es-VE").Descripcion, _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(" ", _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(string.Format("{0}:", Resources.PMIResource.captionFuenteIncidente.Trim()), _Font9Bold);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(_DataEjecucion.tblFuenteIncidente.tblCulture_FuenteIncidente.FirstOrDefault(x => x.Culture == Culture || x.Culture == "es-VE").Descripcion, _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Separador 2
                         ** **************** */
                        _Phrase = new Phrase(" ", _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 5,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Línea 3
                         * **************** */
                        _Phrase = new Phrase(string.Format("{0}:", Resources.PMIResource.captionDescripcion.Trim()), _Font9Bold);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(_DataEjecucion.Descripcion, _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 4,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Separador 3
                         ** **************** */
                        _Phrase = new Phrase(" ", _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 5,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Línea 4
                         * **************** */
                        _Phrase = new Phrase(string.Format("{0}:", Resources.PMIResource.captionLugarIncidente.Trim()), _Font9Bold);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(_DataEjecucion.LugarIncidente, _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 4,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Separador 4
                         ** **************** */
                        _Phrase = new Phrase(" ", _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 5,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Línea 5
                         * **************** */
                        _Phrase = new Phrase(string.Format("{0}:", Resources.PMIResource.captionDuracion.Trim()), _Font9Bold);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(_DataEjecucion.Duracion.ToString(), _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(" ", _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(string.Format("{0}:", Resources.PMIResource.captionNombreReportero.Trim()), _Font9Bold);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(_DataEjecucion.NombreReportero, _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Separador 5
                         ** **************** */
                        _Phrase = new Phrase(" ", _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 5,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Línea 6
                         * **************** */
                        _Phrase = new Phrase(string.Format("{0}:", Resources.PMIResource.captionNombreSolucionador.Trim()), _Font9Bold);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(_DataEjecucion.NombreSolucionador, _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 4,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Separador 6
                         ** **************** */
                        _Phrase = new Phrase(" ", _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 5,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        /* ****************
                         * Línea 7
                         * **************** */
                        _Phrase = new Phrase(string.Format("{0}:", Resources.PMIResource.captionObservacion.Trim()), _Font9Bold);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Phrase = new Phrase(_DataEjecucion.Observaciones, _Font9Normal);
                        _Cell = new PdfPCell(_Phrase)
                        {
                            HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Colspan = 4,
                            BorderWidthTop = 0,
                            BorderWidthLeft = 0,
                            BorderWidthBottom = 0,
                            BorderWidthRight = 0,
                        };
                        _Table.AddCell(_Cell);

                        _Documento.Add(_Table);
                    }
                }

                _Documento.Close();
                foreach (PdfReader _reader in reader)
                {
                    _reader.Close();
                    _reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _strDocURL;
        }

        #region "Objetos"
        internal class itsEvents : PdfPageEventHelper
        {
            public override void OnChapter(PdfWriter writer, Document document, float paragraphPosition, Paragraph title)
            {
                base.OnChapter(writer, document, paragraphPosition, title);
            }
            public override void OnStartPage(PdfWriter writer, Document document)
            {
                if (_Documento.IsOpen())
                {
                    PdfPTable _TableHeader = null;
                    string _Titulo = string.Empty;

                    switch (_FileName.ToUpper().Substring(0, 3))
                    {
                        case "PLC":
                            _Titulo = Resources.PDFResource.PLCTituloString;
                            break;
                        case "PAD":
                            _Titulo = Resources.PDFResource.PADTituloString;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                        case "ETC":
                            _Titulo = Resources.PDFResource.ETCTituloString;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                        case "ETG":
                            _Titulo = Resources.PDFResource.ETGTituloString;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                        case "PMI":
                            _Titulo = Resources.PDFResource.PMITituloString;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                        case "PMT":
                            _Titulo = Resources.PDFResource.PADTituloString;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                        case "PPE":
                            _Titulo = Resources.PDFResource.PPETituloString;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                        case "INC":
                            _Titulo = _INCInformeTitle;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                        case "BIA":
                            _Titulo = Resources.PDFResource.BIATituloString;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                        case "BCP":
                            _Titulo = Resources.PDFResource.BCPTituloString;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                        case "RSG":
                            _Titulo = Resources.PDFResource.RSGTituloString;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                        case "REP":
                            switch (_FileName.ToUpper().Substring(3, 3))
                            {
                                case "USR":
                                    _Titulo = Resources.PDFResource.USRTituloString;
                                    _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                                    break;
                            }
                            break;
                        case "PTI":
                            _Titulo = Resources.PDFResource.PTITituloString;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                         default:
                            _Titulo = String.Empty;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                    }

                    int _LastPage = writer.PageNumber;

                    if (!_importingPDF)
                        _Documento.Add(_TableHeader);

                }
            }
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                if (_Documento.IsOpen())
                {
                    //if (!_importingPDF)
                    //{
                    BaseColor Grey = new BaseColor(128, 128, 128);
                    PdfPTable footerTbl = new PdfPTable(3);
                    PdfPTable footerTbl2 = new PdfPTable(1);
                    PdfPCell Cell;
                    string DocName = string.Empty;
                    string Footerstring = string.Empty;

                    Image Imagen = Image.GetInstance(_ImagenApliredPath);
                    Imagen.Alignment = Element.ALIGN_CENTER;
                    Imagen.ScalePercent(10);

                    footerTbl.SetWidths(new float[] { 20, 60, 20 });
                    footerTbl.TotalWidth = document.PageSize.Width;
                    footerTbl.WidthPercentage = 70;
                    footerTbl.SpacingBefore = 10.0F;
                    footerTbl.SpacingAfter = 5.0F;
                    footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
                    footerTbl.DefaultCell.Border = 0;

                    Footerstring = Resources.PDFResource.FooterStringLinea1 + System.Environment.NewLine + Resources.PDFResource.FooterStringLinea2;
                    DocName = _FileName;


                    /* ************************************
                       ***** Armando el pie de página *****
                       ************************************ */

                    footerTbl2.DefaultCell.Border = Rectangle.NO_BORDER;
                    _Phrase = new Phrase(DocName, _Font8Normal);
                    Cell = new PdfPCell(_Phrase)
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Border = Rectangle.NO_BORDER,
                        Padding = 10
                    };
                    footerTbl.AddCell(Cell);

                    Cell = new PdfPCell(Imagen)
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = Rectangle.NO_BORDER
                    };
                    footerTbl2.AddCell(Cell);
                    _Phrase = new Phrase(Footerstring, _FontPieNormal);
                    Cell = new PdfPCell(_Phrase)
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = Rectangle.NO_BORDER
                    };
                    footerTbl2.AddCell(Cell);

                    if (_FileName.ToUpper().Substring(0, 3) != "PAD")
                        _Phrase = new Phrase(string.Format("Pág. {0}", writer.PageNumber.ToString("#0")), _FontPieNormal);
                    else
                        _Phrase = new Phrase("", _FontPieNormal);

                    Cell = new PdfPCell(_Phrase)
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Border = Rectangle.NO_BORDER,
                        Padding = 10
                    };

                    footerTbl.AddCell(footerTbl2);
                    footerTbl.AddCell(Cell);
                    footerTbl.WriteSelectedRows(0, -1, 0, (document.BottomMargin), writer.DirectContent);
                    //}
                }
            }
        }
        internal class objFilaTabla
        {
            public List<objCeldaTabla> Valores { get; set; }
            public bool Titulo { get; set; }
            private bool _SubTitulo = false;
            public bool SubTitulo
            {
                get { return _SubTitulo; }
                set { _SubTitulo = value; }
            }
            public int[] Anchos { get; set; }
        }
        internal class objCeldaTabla
        {
            public string Valor { get; set; }
            public int Alineacion { get; set; }
            int _ColSpan = 1;
            public int ColSpan
            {
                get { return _ColSpan; }
                set { _ColSpan = value; }
            }
        }
        internal class objContenido
        {
            public string Capitulo { get; set; }
            public int Page { get; set; }
            private bool _Indent = false;
            public bool Indent
            {
                get { return _Indent; }
                set { _Indent = value; }
            }
            private int _NoIndent = 1;
            public int NoIndent
            {
                get { return _NoIndent; }
                set { _NoIndent = value; }
            }
        }
        internal class Style
        {


            public string Id { get; set; }
            public string FontFamily { get; set; }
            public Single FontSize { get; set; }
            public string Foreground { get; set; }
            public string FontWeight { get; set; }
            public string Alignment { get; set; }
            public string VerticalAlignment { get; set; }
            public string BorderBrush { get; set; }
            public string BorderThickness { get; set; }
            public string CellBorderBrush { get; set; }
            public string CellPadding { get; set; }
            public string CellBorderThickness { get; set; }

        }
        #endregion
    }
}