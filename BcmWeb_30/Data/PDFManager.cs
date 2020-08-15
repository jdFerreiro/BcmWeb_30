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
    public class PDFManager
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
        private static Phrase _Phrase;
        private static string _FechaDocumento = DateTime.UtcNow.AddHours(-4).AddMinutes(-30).ToString("dd/MM/yyyy");
        private static PdfWriter _pdfWrite;
        //private static Paragraph _parrafo;
        //private static string _pbeInformeTitle;

        private static Dictionary<string, Dictionary<string, string>> _htmlStyles = new Dictionary<string, Dictionary<string, string>>();
        #endregion
        #region "Private Methods"
        private static PdfPTable HeaderDocs(int PageNumber, string Titulo)
        {
            PdfPTable _Table = new PdfPTable(3);
            PdfPCell _Cell;
            Image _Imagen;
            Image _ImagenAplired;
            double _TotalTableWidth = _ActualPageWidth * 0.9;
            float[] _TableWidths = new float[] { (float)(_TotalTableWidth * 0.3), (float)(_TotalTableWidth * 0.4), (float)(_TotalTableWidth * 0.3) };


            _Table.SetWidths(_TableWidths);
            _Table.WidthPercentage = 90;
            _Table.SpacingBefore = 15.0F;
            _Table.SpacingAfter = 10.0F;
            _Table.HorizontalAlignment = Element.ALIGN_CENTER;

            _Imagen = Image.GetInstance(_ImagenEmpresa);
            _Imagen.Alignment = Element.ALIGN_RIGHT;
            _Imagen.ScalePercent(35);

            _Cell = new PdfPCell(_Imagen)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Rowspan = 3
            };
            _Table.AddCell(_Cell);
            _Phrase = new Phrase(Titulo, _Font9Bold);
            _Cell = new PdfPCell(_Phrase)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Rowspan = 2
            };
            _Table.AddCell(_Cell);

            _ImagenAplired = Image.GetInstance(_ImagenApliredPath);
            _ImagenAplired.Alignment = Element.ALIGN_RIGHT;
            _ImagenAplired.ScalePercent(10);
            _ImagenAplired.SpacingAfter = 0.5F;
            _ImagenAplired.SpacingBefore = 0.2F;

            _Cell = new PdfPCell(_ImagenAplired)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                FixedHeight = (float)(_ImagenAplired.Height * 0.2)
            };

            _Table.AddCell(_Cell);
            string _StringFecha = String.Format(Resources.PDFResource.FechaHeaderString, _FechaDocumento);
            _Cell = new PdfPCell(new Phrase(_StringFecha, _Font9Normal))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            _Table.AddCell(_Cell);
            _Phrase = new Phrase(Titulo, _Font9Bold);
            _Cell = new PdfPCell(_Phrase)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            _Table.AddCell(_Cell);
            _Phrase = new Phrase(String.Format(Resources.PDFResource.PaginaHeaderString, PageNumber.ToString("#0")), _Font9Normal);
            _Cell = new PdfPCell(_Phrase)
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            _Table.AddCell(_Cell);

            return _Table;
        }
        private void MoverDocumentosEscenarios(bool Negocios, string codigoInforme)
        {
            long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
            long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
            long IdUsuario = long.Parse(Session["UserId"].ToString());
            long IdTipoDocumento = long.Parse(Session["IdTipoDocumento"].ToString());
            eSystemModules Modulo = (eSystemModules)IdTipoDocumento;
            string TipoDocumento = Modulo.ToString();

            string _tempFileName = string.Format("tmp{0}_{1}_??_{2}_060502*.pdf"
                 , TipoDocumento, IdEmpresa.ToString("000")
                 , IdDocumento.ToString("000"));
            string _tempFiles = string.Format("{0}\\{1}", _tempFilePath
                , _tempFileName);
            string _fixedFiles = string.Format("{0}\\PDFDocs\\{1}", _ServerPath, _tempFileName);

            List<string> dirFiles = Directory.GetFiles(_tempFilePath, _tempFileName,
                                SearchOption.AllDirectories).OrderBy(q => q).ToList();

            if (dirFiles.Count() > 0)
            {
                foreach (string _file in dirFiles)
                {
                    string _defFile = _file.Replace(_tempFilePath, _tempFilePath.Replace("\\tempPDF","")).Replace("tmp","");
                    if (File.Exists(_defFile))
                        File.Delete(_defFile);
                    File.Move(_file, _defFile);
                }
            }
        }
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
                                _contenido = _dataDocumento.tblDocumentoContenido.Where(x => x.IdSubModulo == 10000101).FirstOrDefault();
                                if (_contenido != null)
                                    GenerarPDF_Temporal(_contenido, TipoDocumento, _orden);
                                _orden++;
                                _contenido = _dataDocumento.tblDocumentoContenido.Where(x => x.IdSubModulo == 10010200).FirstOrDefault();
                                if (_contenido != null)
                                    GenerarPDF_Temporal(_contenido, TipoDocumento, _orden);
                                _orden++;
                                _contenido = _dataDocumento.tblDocumentoContenido.Where(x => x.IdSubModulo == 10020100).FirstOrDefault();
                                if (_contenido != null)
                                    GenerarPDF_Temporal(_contenido, TipoDocumento, _orden);
                                _orden++;
                                _contenido = _dataDocumento.tblDocumentoContenido.Where(x => x.IdSubModulo == 10010300).FirstOrDefault();
                                if (_contenido != null)
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
                                                    contenido.IdEmpresa.ToString("000"),
                                                    _orden.ToString("00"),
                                                    contenido.IdDocumento.ToString("000"),
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
        public string GenerarPDF_Documento(bool Mostrar)
        {
            _ServerPath = _Server.MapPath(".").Replace("\\Documentos", string.Empty);
            _tempFilePath = string.Format("{0}\\PDFDocs\\tempPDF", _ServerPath);
            _ImagenApliredPath = string.Format("{0}\\LogosEmpresa\\LogoAplired.png", _ServerPath);

            try
            {
                if (!Directory.Exists(_tempFilePath)) Directory.CreateDirectory(_tempFilePath);

                long IdDocumento = long.Parse(Session["IdDocumento"].ToString());
                long IdEmpresa = long.Parse(Session["IdEmpresa"].ToString());
                long IdUsuario = long.Parse(Session["UserId"].ToString());
                long IdTipoDocumento = long.Parse(Session["IdTipoDocumento"].ToString());
                eSystemModules Modulo = (eSystemModules)IdTipoDocumento;
                string TipoDocumento = Modulo.ToString();
                string NombreModulo = string.Empty;

                if (GenerarArchivosTemporales())
                {
                    List<PdfReader> reader = new List<PdfReader>();
                    PdfImportedPage page;
                    int rotation;
                    int i = 0;
                    int n = 0;
                    _TableOfContent = new List<objContenido>();
                    _ActualPageWidth = PageSize.LETTER.Width;

                    using (Entities db = new Entities())
                    {
                        tblDocumento dataDocumento = (from d in db.tblDocumento
                                                      where d.IdEmpresa == IdEmpresa
                                                            && d.IdDocumento == IdDocumento
                                                            && d.IdTipoDocumento == IdTipoDocumento
                                                      select d).FirstOrDefault();

                        eEstadoDocumento EstadoDocumento = (eEstadoDocumento)dataDocumento.IdEstadoDocumento;

                        //string _docPassowrd = string.Format("BcmWeb_30.{0}.{1}", (dataDocumento.Negocios ? "N" : "T"), IdEmpresa.ToString("000"));
                        //string _ownerPassowrd = string.Format("{0}.{1}.{2}.BCMWEB", TipoDocumento, (dataDocumento.Negocios ? "N" : "T"), IdEmpresa.ToString("000"));
                        string _CodigoInforme = string.Format("{0}_{1}_{2}_{3}_{4}.{5}", TipoDocumento, IdEmpresa.ToString(), dataDocumento.NroDocumento.ToString("#000"), (EstadoDocumento == eEstadoDocumento.Certificado ? dataDocumento.FechaEstadoDocumento.ToString("MM-yyyy") : DateTime.Now.ToString("MM-yyyy")), dataDocumento.VersionOriginal, dataDocumento.NroVersion);
                        _FileName = string.Format("{0}.pdf", _CodigoInforme.Replace("-", "_"));
                        _pathFile = String.Format("{0}\\PDFDocs\\{1}", _ServerPath, _FileName);
                        _strDocURL = String.Format("{0}/PDFDocs/{1}", _AppUrl, _FileName);
                        string _LogoEmpresaPath = string.Format("{0}{1}", _ServerPath, dataDocumento.tblEmpresa.LogoURL.Replace("/", "\\").Replace("~", ""));

                        _ImagenEmpresa = Image.GetInstance(_LogoEmpresaPath);
                        _ImagenEmpresa.Alignment = Element.ALIGN_CENTER;

                        if (File.Exists(_pathFile))
                            File.Delete(_pathFile);

                        string _pattern = string.Format("tmp{0}_{1}???_{2}*.pdf",
                                                          TipoDocumento,
                                                          IdEmpresa.ToString("000"),
                                                          dataDocumento.IdDocumento.ToString("000"));

                        List<string> _pdfFiles = Directory.GetFiles(_tempFilePath, _pattern,
                                            SearchOption.AllDirectories).OrderBy(q => q).ToList();

                        _Documento = new Document();
                        _pdfWrite = PdfWriter.GetInstance(_Documento, new FileStream(_pathFile, FileMode.Create));
                        _pdfWrite.PageEvent = _PDF_Events;
                        //_pdfWrite.SetEncryption(
                        //      System.Text.Encoding.UTF8.GetBytes(_docPassowrd)
                        //    , System.Text.Encoding.UTF8.GetBytes(_ownerPassowrd)
                        //    , PdfWriter.AllowPrinting
                        //    , PdfWriter.ENCRYPTION_AES_256);

                        string[] docKeywords = new string[]
                        {
                            _FileName,
                            dataDocumento.tblEmpresa.NombreComercial,
                            TipoDocumento,
                        };

                        _Documento.Open();
                        _Documento.AddAuthor("www.BcmWeb_30.net");
                        _Documento.AddCreator("www.BcmWeb_30.net");
                        _Documento.AddKeywords(string.Join(",", docKeywords));

                        int _PaginaInicioCapitulo = 1;
                        foreach (string _fileName in _pdfFiles)
                        {
                            List<string> _fileSplit = _fileName.Split('_').ToList();
                            long _IdModulo = long.Parse(_fileSplit.Last().Split('.').First());

                            tblModulo dataModulo = db.tblModulo.Where(x => x.IdEmpresa == IdEmpresa && x.IdModulo == _IdModulo).FirstOrDefault();

                            _TableOfContent.Add(new objContenido
                            {
                                Capitulo = dataModulo.Nombre,
                                Indent = false,
                                Page = _PaginaInicioCapitulo
                            });

                            reader.Add(new PdfReader(_fileName));
                            i = 0;
                            n = reader[reader.Count - 1].NumberOfPages;
                            while (i < n)
                            {
                                i++;
                                _Documento.SetPageSize(reader[reader.Count - 1].GetPageSizeWithRotation(i));
                                _Documento.NewPage();
                                page = _pdfWrite.GetImportedPage(reader[reader.Count - 1], i);
                                PdfContentByte cb = _pdfWrite.DirectContent;
                                rotation = reader[reader.Count - 1].GetPageRotation(i);
                                if (rotation == 90 || rotation == 270)
                                    cb.AddTemplate(page, 0, -1.0F, 1.0F, 0, 0, reader[reader.Count - 1].GetPageSizeWithRotation(i).Height);
                                else
                                    cb.AddTemplate(page, 1.0F, 0, 0, 1.0F, 0, 0);

                            }
                            _ActualPageWidth = PageSize.LETTER.Width;
                            _PaginaInicioCapitulo += n;
                        }
                        long IdModuloPadre = IdTipoDocumento * 1000000;
                        NombreModulo = db.tblModulo.Where(x => x.IdEmpresa == IdEmpresa && x.IdModulo == IdModuloPadre).FirstOrDefault().Nombre;

                        GenerarIndice(NombreModulo);
                        _Documento.Close();
                        foreach (PdfReader _reader in reader)
                        {
                            _reader.Close();
                            _reader.Dispose();
                        }

                        if (Modulo == eSystemModules.PMI)
                        {
                            MoverDocumentosEscenarios(dataDocumento.Negocios, _CodigoInforme);
                        }

                        _pdfFiles = Directory.GetFiles(_tempFilePath, _pattern, 
                            SearchOption.AllDirectories).OrderBy(q => q).ToList();

                        foreach (string _fileName in _pdfFiles)
                        {
                            File.Delete(_fileName);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //_Documento.Close();
                throw ex;
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
                        default:
                            _Titulo = String.Empty;
                            _TableHeader = HeaderDocs(writer.PageNumber, _Titulo);
                            break;
                    }
                }
            }
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                if (_Documento.IsOpen())
                {
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

                    if (writer.PageNumber != 1)
                    {
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
                    }
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