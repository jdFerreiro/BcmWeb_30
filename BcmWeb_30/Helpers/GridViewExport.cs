using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.XtraPrinting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace BcmWeb_30 
{
    public enum GridViewExportFormat { None, Pdf, Xls, Xlsx, Rtf, Csv }
    public delegate ActionResult GridViewExportMethod(GridViewSettings settings, object dataObject);

    public class GridViewExportIniciativas
    {
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }

        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }

        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }

        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }

        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }

        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvIniciativas";
            settings.KeyFieldName = "IdIniciativa";
            settings.CallbackRouteValues = new { Controller = "PlanTrabajo", Action = "IniciativaPartialView" };
            settings.Width = Unit.Percentage(100);

            settings.TotalSummary.Add(SummaryItemType.Count, "NroIniciativa").DisplayFormat = "#,##0";
            settings.TotalSummary.Add(SummaryItemType.Sum, "PresupuestoEstimado").DisplayFormat = "#,##0.00";
            settings.TotalSummary.Add(SummaryItemType.Sum, "PresupuestoReal").DisplayFormat = "#,##0.00";
            settings.TotalSummary.Add(SummaryItemType.Sum, "MontoAbonado").DisplayFormat = "#,##0.00";
            settings.TotalSummary.Add(SummaryItemType.Sum, "MontoPendiente").DisplayFormat = "#,##0.00";
            settings.TotalSummary.Add(SummaryItemType.Average, "PorcentajeAvance").DisplayFormat = "{0:n2}%";

            settings.Columns.Add(c =>
            {
                c.FieldName = "NroIniciativa";
                c.Caption = Resources.IniciativaResource.NroIniciativa;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FooterCellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 80;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Nombre";
                c.Caption = Resources.IniciativaResource.Nombre;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 250;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Descripcion";
                c.Caption = Resources.IniciativaResource.Descripcion;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 450;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "UnidadOrganizativa";
                c.Caption = Resources.IniciativaResource.UnidadOrganizativa;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 250;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Responsable";
                c.Caption = Resources.IniciativaResource.Responsable;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 250;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaInicioEstimada";
                c.Caption = Resources.IniciativaResource.FechaInicioEstimada;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaCierreEstimada";
                c.Caption = Resources.IniciativaResource.FechaCierreEstimada;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaInicioReal";
                c.Caption = Resources.IniciativaResource.FechaInicioReal;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaCierreReal";
                c.Caption = Resources.IniciativaResource.FechaCierreReal;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "PresupuestoEstimado";
                c.Caption = Resources.IniciativaResource.PresupuestoEstimado;
                c.PropertiesEdit.DisplayFormatString = "#,##0.00";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "PresupuestoReal";
                c.Caption = Resources.IniciativaResource.PresupuestoReal;
                c.PropertiesEdit.DisplayFormatString = "#,##0.00";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "MontoAbonado";
                c.Caption = Resources.IniciativaResource.MontoAbonado;
                c.PropertiesEdit.DisplayFormatString = "#,##0.00";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "MontoPendiente";
                c.Caption = Resources.IniciativaResource.MontoPendiente;
                c.PropertiesEdit.DisplayFormatString = "#,##0.00";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "HorasEstimadas";
                c.Caption = Resources.IniciativaResource.HorasEstimadas;
                c.PropertiesEdit.DisplayFormatString = "##0";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "HorasInvertidas";
                c.Caption = Resources.IniciativaResource.HorasInvertidas;
                c.PropertiesEdit.DisplayFormatString = "##0";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "PorcentajeAvance";
                c.Caption = Resources.IniciativaResource.PorcentajeAvance;
                c.PropertiesEdit.DisplayFormatString = "{0:n2}%";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdEstatus";
                c.Caption = Resources.IniciativaResource.Estatus;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(long);
                    p.DataSource = Metodos.GetEstadosIniciativa();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Urgente";
                c.Caption = Resources.IniciativaResource.Urgente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(long);
                    p.DataSource = Metodos.GetPrioridadesIniciativa();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Observacion";
                c.Caption = Resources.IniciativaResource.Observacion;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 450;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });

            return settings;
        }

        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvIniciativas";
            settings.KeyFieldName = "IdIniciativa";
            settings.Width = Unit.Percentage(100);

            settings.TotalSummary.Add(SummaryItemType.Count, "NroIniciativa").DisplayFormat = "#,##0";
            settings.TotalSummary.Add(SummaryItemType.Sum, "PresupuestoEstimado").DisplayFormat = "#,##0.00";
            settings.TotalSummary.Add(SummaryItemType.Sum, "PresupuestoReal").DisplayFormat = "#,##0.00";
            settings.TotalSummary.Add(SummaryItemType.Sum, "MontoAbonado").DisplayFormat = "#,##0.00";
            settings.TotalSummary.Add(SummaryItemType.Sum, "MontoPendiente").DisplayFormat = "#,##0.00";
            settings.TotalSummary.Add(SummaryItemType.Average, "PorcentajeAvance").DisplayFormat = "{0:n2}%";

            settings.Columns.Add(c =>
            {
                c.FieldName = "NroIniciativa";
                c.Caption = Resources.IniciativaResource.NroIniciativa;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FooterCellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 80;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Nombre";
                c.Caption = Resources.IniciativaResource.Nombre;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 250;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Descripcion";
                c.Caption = Resources.IniciativaResource.Descripcion;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 450;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "UnidadOrganizativa";
                c.Caption = Resources.IniciativaResource.UnidadOrganizativa;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 250;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Responsable";
                c.Caption = Resources.IniciativaResource.Responsable;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 250;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaInicioEstimada";
                c.Caption = Resources.IniciativaResource.FechaInicioEstimada;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaCierreEstimada";
                c.Caption = Resources.IniciativaResource.FechaCierreEstimada;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaInicioReal";
                c.Caption = Resources.IniciativaResource.FechaInicioReal;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaCierreReal";
                c.Caption = Resources.IniciativaResource.FechaCierreReal;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "PresupuestoEstimado";
                c.Caption = Resources.IniciativaResource.PresupuestoEstimado;
                c.PropertiesEdit.DisplayFormatString = "#,##0.00";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "PresupuestoReal";
                c.Caption = Resources.IniciativaResource.PresupuestoReal;
                c.PropertiesEdit.DisplayFormatString = "#,##0.00";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "MontoAbonado";
                c.Caption = Resources.IniciativaResource.MontoAbonado;
                c.PropertiesEdit.DisplayFormatString = "#,##0.00";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "MontoPendiente";
                c.Caption = Resources.IniciativaResource.MontoPendiente;
                c.PropertiesEdit.DisplayFormatString = "#,##0.00";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "HorasEstimadas";
                c.Caption = Resources.IniciativaResource.HorasEstimadas;
                c.PropertiesEdit.DisplayFormatString = "##0";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "HorasInvertidas";
                c.Caption = Resources.IniciativaResource.HorasInvertidas;
                c.PropertiesEdit.DisplayFormatString = "##0";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "PorcentajeAvance";
                c.Caption = Resources.IniciativaResource.PorcentajeAvance;
                c.PropertiesEdit.DisplayFormatString = "{0:n2}%";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdEstatus";
                c.Caption = Resources.IniciativaResource.Estatus;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(long);
                    p.DataSource = Metodos.GetEstadosIniciativa();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Urgente";
                c.Caption = Resources.IniciativaResource.Urgente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(long);
                    p.DataSource = Metodos.GetPrioridadesIniciativa();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Observacion";
                c.Caption = Resources.IniciativaResource.Observacion;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 450;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvIniciativas";
            settings.KeyFieldName = "IdIniciativa";
            settings.CallbackRouteValues = new { Controller = "PlanTrabajo", Action = "IniciativaPartialView" };
            settings.Width = Unit.Percentage(100);

            settings.TotalSummary.Add(SummaryItemType.Count, "NroIniciativa").DisplayFormat = "#,##0";
            settings.TotalSummary.Add(SummaryItemType.Sum, "PresupuestoEstimado").DisplayFormat = "#,##0.00";
            settings.TotalSummary.Add(SummaryItemType.Sum, "PresupuestoReal").DisplayFormat = "#,##0.00";
            settings.TotalSummary.Add(SummaryItemType.Sum, "MontoAbonado").DisplayFormat = "#,##0.00";
            settings.TotalSummary.Add(SummaryItemType.Sum, "MontoPendiente").DisplayFormat = "#,##0.00";
            settings.TotalSummary.Add(SummaryItemType.Average, "PorcentajeAvance").DisplayFormat = "{0:n2}%";

            settings.Columns.Add(c =>
            {
                c.FieldName = "NroIniciativa";
                c.Caption = Resources.IniciativaResource.NroIniciativa;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FooterCellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 80;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Nombre";
                c.Caption = Resources.IniciativaResource.Nombre;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 250;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Descripcion";
                c.Caption = Resources.IniciativaResource.Descripcion;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 450;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "UnidadOrganizativa";
                c.Caption = Resources.IniciativaResource.UnidadOrganizativa;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 250;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Responsable";
                c.Caption = Resources.IniciativaResource.Responsable;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 250;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaInicioEstimada";
                c.Caption = Resources.IniciativaResource.FechaInicioEstimada;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaCierreEstimada";
                c.Caption = Resources.IniciativaResource.FechaCierreEstimada;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaInicioReal";
                c.Caption = Resources.IniciativaResource.FechaInicioReal;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaCierreReal";
                c.Caption = Resources.IniciativaResource.FechaCierreReal;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "PresupuestoEstimado";
                c.Caption = Resources.IniciativaResource.PresupuestoEstimado;
                c.PropertiesEdit.DisplayFormatString = "#,##0.00";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "PresupuestoReal";
                c.Caption = Resources.IniciativaResource.PresupuestoReal;
                c.PropertiesEdit.DisplayFormatString = "#,##0.00";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "MontoAbonado";
                c.Caption = Resources.IniciativaResource.MontoAbonado;
                c.PropertiesEdit.DisplayFormatString = "#,##0.00";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "MontoPendiente";
                c.Caption = Resources.IniciativaResource.MontoPendiente;
                c.PropertiesEdit.DisplayFormatString = "#,##0.00";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "HorasEstimadas";
                c.Caption = Resources.IniciativaResource.HorasEstimadas;
                c.PropertiesEdit.DisplayFormatString = "##0";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "HorasInvertidas";
                c.Caption = Resources.IniciativaResource.HorasInvertidas;
                c.PropertiesEdit.DisplayFormatString = "##0";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "PorcentajeAvance";
                c.Caption = Resources.IniciativaResource.PorcentajeAvance;
                c.PropertiesEdit.DisplayFormatString = "{0:n2}%";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 100;
                c.Settings.AllowAutoFilter = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdEstatus";
                c.Caption = Resources.IniciativaResource.Estatus;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(long);
                    p.DataSource = Metodos.GetEstadosIniciativa();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Urgente";
                c.Caption = Resources.IniciativaResource.Urgente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(long);
                    p.DataSource = Metodos.GetPrioridadesIniciativa();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Observacion";
                c.Caption = Resources.IniciativaResource.Observacion;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 450;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });

            return settings;
        }

    }
    public class GridViewExportCuadroIO
    {
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "gvIO";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "IdProceso";
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Caption = Resources.ReporteResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "DescEscala";
                c.Caption = Resources.ReporteResource.captionDescripcion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Escala";
                c.Caption = Resources.ReporteResource.captionValoracion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });


            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvIO";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "IdProceso";
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Caption = Resources.ReporteResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "DescEscala";
                c.Caption = Resources.ReporteResource.captionDescripcion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Escala";
                c.Caption = Resources.ReporteResource.captionValoracion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvIO";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "IdProceso";
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Caption = Resources.ReporteResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "DescEscala";
                c.Caption = Resources.ReporteResource.captionDescripcion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Escala";
                c.Caption = Resources.ReporteResource.captionValoracion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });


            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();

            settings.Name = "gvIO";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "IdProceso";
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Caption = Resources.ReporteResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "DescEscala";
                c.Caption = Resources.ReporteResource.captionDescripcion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Escala";
                c.Caption = Resources.ReporteResource.captionValoracion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });


            return settings;
        }
    }
    public class GridViewExportProgramaciones
    {
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }

        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }

        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }

        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }

        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }

        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvProgramacion";
            settings.KeyFieldName = "IdProgramacion";
            settings.CallbackRouteValues = new { Controller = "PMT", Action = "ProgramacionPartialView" };
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add(c =>
            {
                c.FieldName = "IdModuloProgramacion";
                c.Caption = Resources.DocumentoResource.captionProgramacionModulo;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                //c.Width = Unit.Percentage(35);
                c.Width = Unit.Percentage(60);
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Nombre";
                    p.ValueField = "IdModulo";
                    p.ValueType = typeof(long);
                    p.DataSource = Metodos.GetModulosPrincipalesEmpresa();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaInicio";
                c.Caption = Resources.DocumentoResource.captionProgramacionFechaInicio;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = Unit.Percentage(10);
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaFinal";
                c.Caption = Resources.DocumentoResource.captionProgramacionFechaFinal;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = Unit.Percentage(10);
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdTipoFrecuencia";
                c.Caption = Resources.DocumentoResource.captionFrecuencia;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = Unit.Percentage(10);
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(long);
                    p.DataSource = Metodos.GetPMTTipoFrecuencia();
                });
            });

            return settings;
        }

        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvProgramacion";
            settings.KeyFieldName = "IdProgramacion";
            settings.CallbackRouteValues = new { Controller = "PMT", Action = "ProgramacionPartialView" };
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add(c =>
            {
                c.FieldName = "IdModuloProgramacion";
                c.Caption = Resources.DocumentoResource.captionProgramacionModulo;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                //c.Width = Unit.Percentage(35);
                c.Width = Unit.Percentage(60);
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Nombre";
                    p.ValueField = "IdModulo";
                    p.ValueType = typeof(long);
                    p.DataSource = Metodos.GetModulosPrincipalesEmpresa();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaInicio";
                c.Caption = Resources.DocumentoResource.captionProgramacionFechaInicio;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = Unit.Percentage(10);
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaFinal";
                c.Caption = Resources.DocumentoResource.captionProgramacionFechaFinal;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = Unit.Percentage(10);
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdTipoFrecuencia";
                c.Caption = Resources.DocumentoResource.captionFrecuencia;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = Unit.Percentage(10);
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(long);
                    p.DataSource = Metodos.GetPMTTipoFrecuencia();
                });
            });


            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gvProgramacion";
            settings.KeyFieldName = "IdProgramacion";
            settings.CallbackRouteValues = new { Controller = "PMT", Action = "ProgramacionPartialView" };
            settings.Width = Unit.Percentage(100);

            settings.Columns.Add(c =>
            {
                c.FieldName = "IdModuloProgramacion";
                c.Caption = Resources.DocumentoResource.captionProgramacionModulo;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                //c.Width = Unit.Percentage(35);
                c.Width = Unit.Percentage(60);
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Nombre";
                    p.ValueField = "IdModulo";
                    p.ValueType = typeof(long);
                    p.DataSource = Metodos.GetModulosPrincipalesEmpresa();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaInicio";
                c.Caption = Resources.DocumentoResource.captionProgramacionFechaInicio;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = Unit.Percentage(10);
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaFinal";
                c.Caption = Resources.DocumentoResource.captionProgramacionFechaFinal;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = Unit.Percentage(10);
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdTipoFrecuencia";
                c.Caption = Resources.DocumentoResource.captionFrecuencia;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = Unit.Percentage(10);
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(long);
                    p.DataSource = Metodos.GetPMTTipoFrecuencia();
                });
            });


            return settings;
        }
    }
    public class GridViewExportTablaRiesgo
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }

        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }

        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }

        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }

        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }

        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvRiesgoControl";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaRiesgoPartialView", IdUnidadOrganizativa = Session["IdUnidadOrganizativaRiesgo"] };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 380;
            settings.Width = Unit.Percentage(100);

            settings.SettingsBehavior.AllowCellMerge = true;

            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.HtmlDataCellPrepared = (sender, e) =>
            {
                if (e.DataColumn.FieldName == "Estado")
                {
                    short valor = short.Parse(e.GetValue("Estado").ToString());

                    switch (valor)
                    {
                        case 3:
                            e.Cell.BackColor = System.Drawing.Color.Red;
                            e.Cell.ForeColor = System.Drawing.Color.White;
                            e.Cell.Font.Bold = true;
                            e.Cell.Text = Metodos.GetControlNameById(valor);
                            break;
                        case 2:
                            e.Cell.BackColor = System.Drawing.Color.Orange;
                            e.Cell.ForeColor = System.Drawing.Color.White;
                            e.Cell.Font.Bold = true;
                            e.Cell.Text = Metodos.GetControlNameById(valor);
                            break;
                        default:
                            e.Cell.BackColor = System.Drawing.Color.Green;
                            e.Cell.ForeColor = System.Drawing.Color.White;
                            e.Cell.Font.Bold = true;
                            e.Cell.Text = Metodos.GetControlNameById(valor);
                            break;
                    }
                }
            };

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "NroProceso";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.DocumentoResource.captionNroProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Width = Unit.Percentage(25);
                c.Caption = Resources.DocumentoResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Amenaza";
                c.Width = Unit.Percentage(25);
                c.Caption = Resources.ReporteResource.captionAmenaza;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Evento";
                c.Width = Unit.Percentage(35);
                c.Caption = Resources.ReporteResource.captionEvento;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Estado";
                c.Width = Unit.Percentage(10);
                c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                c.Caption = Resources.ReporteResource.captionControl;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.False;
            });

            return settings;
        }

        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvRiesgoControl";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaRiesgoPartialView", IdUnidadOrganizativa = Session["IdUnidadOrganizativaRiesgo"] };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 380;
            settings.Width = Unit.Percentage(100);

            settings.SettingsBehavior.AllowCellMerge = true;

            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.HtmlDataCellPrepared = (sender, e) =>
            {
                if (e.DataColumn.FieldName == "Estado")
                {
                    short valor = short.Parse(e.GetValue("Estado").ToString());

                    switch (valor)
                    {
                        case 3:
                            e.Cell.BackColor = System.Drawing.Color.Red;
                            e.Cell.ForeColor = System.Drawing.Color.White;
                            e.Cell.Font.Bold = true;
                            e.Cell.Text = Metodos.GetControlNameById(valor);
                            break;
                        case 2:
                            e.Cell.BackColor = System.Drawing.Color.Orange;
                            e.Cell.ForeColor = System.Drawing.Color.White;
                            e.Cell.Font.Bold = true;
                            e.Cell.Text = Metodos.GetControlNameById(valor);
                            break;
                        default:
                            e.Cell.BackColor = System.Drawing.Color.Green;
                            e.Cell.ForeColor = System.Drawing.Color.White;
                            e.Cell.Font.Bold = true;
                            e.Cell.Text = Metodos.GetControlNameById(valor);
                            break;
                    }
                }
            };

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "NroProceso";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.DocumentoResource.captionNroProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Width = Unit.Percentage(25);
                c.Caption = Resources.DocumentoResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Amenaza";
                c.Width = Unit.Percentage(25);
                c.Caption = Resources.ReporteResource.captionAmenaza;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Evento";
                c.Width = Unit.Percentage(35);
                c.Caption = Resources.ReporteResource.captionEvento;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Estado";
                c.Width = Unit.Percentage(10);
                c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                c.Caption = Resources.ReporteResource.captionControl;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.False;
            });

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gvRiesgoControl";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaRiesgoPartialView", IdUnidadOrganizativa = Session["IdUnidadOrganizativa"] };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 380;
            settings.Width = Unit.Percentage(100);

            settings.SettingsBehavior.AllowCellMerge = true;

            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            GridViewFormatConditionHighlight Rule1 = new GridViewFormatConditionHighlight();
            GridViewFormatConditionHighlight Rule2 = new GridViewFormatConditionHighlight();
            GridViewFormatConditionHighlight Rule3 = new GridViewFormatConditionHighlight();

            Rule1.FieldName = "Estado";
            Rule1.Expression = "[Control] == 1";
            Rule1.Format = GridConditionHighlightFormat.Custom;
            Rule1.ApplyToRow = false;
            Rule1.CellStyle.BackColor = System.Drawing.Color.Green;
            Rule1.CellStyle.ForeColor = System.Drawing.Color.White;
            Rule1.CellStyle.Font.Bold = true;
            settings.FormatConditions.Add(Rule1);

            Rule2.FieldName = "Estado";
            Rule2.Expression = "[Control] == 2";
            Rule2.Format = GridConditionHighlightFormat.Custom;
            Rule2.ApplyToRow = false;
            Rule2.CellStyle.BackColor = System.Drawing.Color.Orange;
            Rule2.CellStyle.ForeColor = System.Drawing.Color.White;
            Rule2.CellStyle.Font.Bold = true;
            settings.FormatConditions.Add(Rule2);

            Rule3.FieldName = "Estado";
            Rule3.Expression = "[Control] == 3";
            Rule3.Format = GridConditionHighlightFormat.Custom;
            Rule3.ApplyToRow = false;
            Rule3.CellStyle.BackColor = System.Drawing.Color.Red;
            Rule3.CellStyle.ForeColor = System.Drawing.Color.White;
            Rule3.CellStyle.Font.Bold = true;
            settings.FormatConditions.Add(Rule3);

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "NroProceso";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.DocumentoResource.captionNroProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Width = Unit.Percentage(25);
                c.Caption = Resources.DocumentoResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Amenaza";
                c.Width = Unit.Percentage(25);
                c.Caption = Resources.ReporteResource.captionAmenaza;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Evento";
                c.Width = Unit.Percentage(35);
                c.Caption = Resources.ReporteResource.captionEvento;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.False;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Estado";
                c.Width = Unit.Percentage(10);
                c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                c.Caption = Resources.ReporteResource.captionControl;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Settings.AllowCellMerge = DefaultBoolean.False;
            });

            return settings;
        }
    }
    public class GridViewExportReporteCuadroIO
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvIO";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "CuadroIOPartialView", IdUnidadOrganizativa = Session["IdUnidadOrganizativaPrint"] };
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "IdProceso";
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Caption = Resources.ReporteResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "DescEscala";
                c.Caption = Resources.ReporteResource.captionDescripcion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Escala";
                c.Caption = Resources.ReporteResource.captionValoracion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvIO";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "CuadroIOPartialView", IdUnidadOrganizativa = Session["IdUnidadOrganizativaPrint"] };
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "IdProceso";
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Caption = Resources.ReporteResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "DescEscala";
                c.Caption = Resources.ReporteResource.captionDescripcion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Escala";
                c.Caption = Resources.ReporteResource.captionValoracion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gvIO";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "CuadroIOPartialView", IdUnidadOrganizativa = Session["IdUnidadOrganizativaPrint"] };
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "IdProceso";
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Caption = Resources.ReporteResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "DescEscala";
                c.Caption = Resources.ReporteResource.captionDescripcion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Escala";
                c.Caption = Resources.ReporteResource.captionValoracion;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });

            return settings;
        }
    }
    public class GridViewExportReporteCuadroVI
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvVI";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "CuadroVIPartialView", CustomerID = Session["IdUnidadOrganizativaPrint"] };
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "IdProceso";
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Caption = Resources.ReporteResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionMDTHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "EscalaMTD";
                    c.Caption = Resources.ReporteResource.captionVIHeader;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "DescEscalaMTD";
                    c.Caption = Resources.ReporteResource.captionValorEscala;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionRPOHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "EscalaRPO";
                    c.Caption = Resources.ReporteResource.captionVIHeader;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "DescEscalaRPO";
                    c.Caption = Resources.ReporteResource.captionValorEscala;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionRTOHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "EscalaRTO";
                    c.Caption = Resources.ReporteResource.captionVIHeader;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "DescEscalaRTO";
                    c.Caption = Resources.ReporteResource.captionValorEscala;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionWRTHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "EscalaWRT";
                    c.Caption = Resources.ReporteResource.captionVIHeader;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "DescEscalaWRT";
                    c.Caption = Resources.ReporteResource.captionValorEscala;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            });

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvVI";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "CuadroVIPartialView", CustomerID = Session["IdUnidadOrganizativaPrint"] };
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "IdProceso";
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Caption = Resources.ReporteResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionMDTHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "EscalaMTD";
                    c.Caption = Resources.ReporteResource.captionVIHeader;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "DescEscalaMTD";
                    c.Caption = Resources.ReporteResource.captionValorEscala;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionRPOHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "EscalaRPO";
                    c.Caption = Resources.ReporteResource.captionVIHeader;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "DescEscalaRPO";
                    c.Caption = Resources.ReporteResource.captionValorEscala;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionRTOHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "EscalaRTO";
                    c.Caption = Resources.ReporteResource.captionVIHeader;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "DescEscalaRTO";
                    c.Caption = Resources.ReporteResource.captionValorEscala;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionWRTHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "EscalaWRT";
                    c.Caption = Resources.ReporteResource.captionVIHeader;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "DescEscalaWRT";
                    c.Caption = Resources.ReporteResource.captionValorEscala;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            });

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gvVI";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "CuadroVIPartialView", CustomerID = Session["IdUnidadOrganizativaPrint"] };
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "IdProceso";
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso";
                c.Caption = Resources.ReporteResource.captionProceso;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionMDTHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "EscalaMTD";
                    c.Caption = Resources.ReporteResource.captionVIHeader;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "DescEscalaMTD";
                    c.Caption = Resources.ReporteResource.captionValorEscala;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionRPOHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "EscalaRPO";
                    c.Caption = Resources.ReporteResource.captionVIHeader;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "DescEscalaRPO";
                    c.Caption = Resources.ReporteResource.captionValorEscala;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionRTOHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "EscalaRTO";
                    c.Caption = Resources.ReporteResource.captionVIHeader;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "DescEscalaRTO";
                    c.Caption = Resources.ReporteResource.captionValorEscala;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionWRTHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "EscalaWRT";
                    c.Caption = Resources.ReporteResource.captionVIHeader;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
                sb.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "DescEscalaWRT";
                    c.Caption = Resources.ReporteResource.captionValorEscala;
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.CellStyle.Wrap = DefaultBoolean.True;
                    c.PropertiesEdit.DisplayFormatString = "{0}";
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            });

            return settings;
        }
    }
    public class GridViewExportReporteCuadroPM
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvVI";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "CuadroPMPartialView", CustomerID = Session["IdUnidadOrganizativaPrint"] };

            settings.KeyFieldName = "IdProceso";
            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = 800;

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M01";
                c.Caption = Resources.ReporteResource.captionM01;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M02";
                c.Caption = Resources.ReporteResource.captionM02;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M03";
                c.Caption = Resources.ReporteResource.captionM03;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M04";
                c.Caption = Resources.ReporteResource.captionM04;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M05";
                c.Caption = Resources.ReporteResource.captionM05;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M06";
                c.Caption = Resources.ReporteResource.captionM06;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M07";
                c.Caption = Resources.ReporteResource.captionM07;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M08";
                c.Caption = Resources.ReporteResource.captionM08;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M09";
                c.Caption = Resources.ReporteResource.captionM09;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M10";
                c.Caption = Resources.ReporteResource.captionM10;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M11";
                c.Caption = Resources.ReporteResource.captionM11;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M12";
                c.Caption = Resources.ReporteResource.captionM12;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvVI";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "CuadroPMPartialView", CustomerID = Session["IdUnidadOrganizativaPrint"] };

            settings.KeyFieldName = "IdProceso";
            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = 800;

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M01";
                c.Caption = Resources.ReporteResource.captionM01;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M02";
                c.Caption = Resources.ReporteResource.captionM02;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M03";
                c.Caption = Resources.ReporteResource.captionM03;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M04";
                c.Caption = Resources.ReporteResource.captionM04;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M05";
                c.Caption = Resources.ReporteResource.captionM05;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M06";
                c.Caption = Resources.ReporteResource.captionM06;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M07";
                c.Caption = Resources.ReporteResource.captionM07;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M08";
                c.Caption = Resources.ReporteResource.captionM08;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M09";
                c.Caption = Resources.ReporteResource.captionM09;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M10";
                c.Caption = Resources.ReporteResource.captionM10;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M11";
                c.Caption = Resources.ReporteResource.captionM11;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M12";
                c.Caption = Resources.ReporteResource.captionM12;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gvVI";
            settings.SettingsDetail.MasterGridName = "gvCuadro";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "CuadroPMPartialView", CustomerID = Session["IdUnidadOrganizativaPrint"] };

            settings.KeyFieldName = "IdProceso";
            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = 800;

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M01";
                c.Caption = Resources.ReporteResource.captionM01;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M02";
                c.Caption = Resources.ReporteResource.captionM02;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M03";
                c.Caption = Resources.ReporteResource.captionM03;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M04";
                c.Caption = Resources.ReporteResource.captionM04;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M05";
                c.Caption = Resources.ReporteResource.captionM05;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M06";
                c.Caption = Resources.ReporteResource.captionM06;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M07";
                c.Caption = Resources.ReporteResource.captionM07;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M08";
                c.Caption = Resources.ReporteResource.captionM08;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M09";
                c.Caption = Resources.ReporteResource.captionM09;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M10";
                c.Caption = Resources.ReporteResource.captionM10;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M11";
                c.Caption = Resources.ReporteResource.captionM11;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Proceso_M12";
                c.Caption = Resources.ReporteResource.captionM12;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });

            return settings;
        }
    }
    public class GridViewExportReporteTablaGI
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvGI";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaGIPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            for (int Mes = 0; Mes < 12; Mes++)
            {
                settings.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "ValoresUnbound" + Mes.ToString();
                    c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                    c.Caption = Metodos.GetNombreMes(Mes);
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            }
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("ValoresUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("Valores") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("ValoresUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvGI";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaGIPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            for (int Mes = 0; Mes < 12; Mes++)
            {
                settings.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "ValoresUnbound" + Mes.ToString();
                    c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                    c.Caption = Metodos.GetNombreMes(Mes);
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            }
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("ValoresUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("Valores") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("ValoresUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gvGI";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaGIPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            for (int Mes = 0; Mes < 12; Mes++)
            {
                settings.Columns.Add(c =>
                {
                    c.Settings.AllowSort = DefaultBoolean.True;
                    c.FieldName = "ValoresUnbound" + Mes.ToString();
                    c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                    c.Caption = Metodos.GetNombreMes(Mes);
                    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                    c.HeaderStyle.Wrap = DefaultBoolean.True;
                });
            }
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("ValoresUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("Valores") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("ValoresUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
    }
    public class GridViewExportReporteTablaPUO
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvPMO";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaPUOPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Hidden;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.ImpactoOperacional))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvPMO";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaPUOPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Hidden;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.ImpactoOperacional))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gvPMO";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaPUOPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Hidden;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.ImpactoOperacional))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
    }
    public class GridViewExportReporteTablaVI
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvVI";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaVIPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionMDTHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int iMTD = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.MTD))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "MTDUnbound" + iMTD.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    iMTD++;
                }
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionRPOHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int iRPO = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.RPO))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "RPOUnbound" + iRPO.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    iRPO++;
                }
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionRTOHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int iRTO = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.RTO))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "RTOUnbound" + iRTO.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    iRTO++;
                }
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionWRTHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int iWRT = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.WRT))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "WRTUnbound" + iWRT.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    iWRT++;
                }
            });

            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("MTDUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("ValoresMTD") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("MTDUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
                if (e.Column.FieldName.Contains("RPOUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("ValoresRPO") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("RPOUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
                if (e.Column.FieldName.Contains("RTOUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("ValoresRTO") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("RTOUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
                if (e.Column.FieldName.Contains("WRTUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("ValoresWRT") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("WRTUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvVI";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaVIPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionMDTHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int iMTD = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.MTD))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "MTDUnbound" + iMTD.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    iMTD++;
                }
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionRPOHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int iRPO = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.RPO))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "RPOUnbound" + iRPO.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    iRPO++;
                }
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionRTOHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int iRTO = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.RTO))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "RTOUnbound" + iRTO.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    iRTO++;
                }
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionWRTHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int iWRT = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.WRT))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "WRTUnbound" + iWRT.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    iWRT++;
                }
            });

            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("MTDUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("ValoresMTD") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("MTDUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
                if (e.Column.FieldName.Contains("RPOUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("ValoresRPO") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("RPOUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
                if (e.Column.FieldName.Contains("RTOUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("ValoresRTO") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("RTOUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
                if (e.Column.FieldName.Contains("WRTUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("ValoresWRT") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("WRTUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gvVI";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaVIPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionMDTHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int iMTD = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.MTD))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "MTDUnbound" + iMTD.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    iMTD++;
                }
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionRPOHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int iRPO = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.RPO))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "RPOUnbound" + iRPO.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    iRPO++;
                }
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionRTOHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int iRTO = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.RTO))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "RTOUnbound" + iRTO.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    iRTO++;
                }
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionWRTHeader;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int iWRT = 0;
                foreach (string Valor in BcmWeb_30.Metodos.GetDescripcionEscala(eTipoEscala.WRT))
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "WRTUnbound" + iWRT.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                    });
                    iWRT++;
                }
            });

            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("MTDUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("ValoresMTD") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("MTDUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
                if (e.Column.FieldName.Contains("RPOUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("ValoresRPO") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("RPOUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
                if (e.Column.FieldName.Contains("RTOUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("ValoresRTO") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("RTOUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
                if (e.Column.FieldName.Contains("WRTUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("ValoresWRT") as List<int>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("WRTUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
    }
    public class GridViewExportCriticos
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvCriticos";
            settings.CallbackRouteValues = new { Controller = "Procesos", Action = "CriticosPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Hidden;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 265;
            settings.ClientSideEvents.BeginCallback = "OnBeginGridCallback";
            settings.ClientSideEvents.EndCallback = "OnEndGridCallback";
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;

            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

            settings.CommandColumn.Visible = false;
            settings.CommandColumn.Width = Unit.Percentage(10);
            settings.CommandColumn.ShowNewButtonInHeader = false;
            settings.CommandColumn.ShowDeleteButton = false;
            settings.CommandColumn.ShowEditButton = false;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Selected";
                c.Width = Unit.Percentage(5);
                c.Caption = "Marcado";
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.CheckBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "NroProceso";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionNroProceso;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Nombre";
                c.Width = Unit.Percentage(15);
                c.Caption = Resources.ProcesosResource.captionNombre;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "ImpactoFinanciero";
                c.Width = Unit.Percentage(15);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.ImpactoFinanciero);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Justify;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "ImpactoOperacional";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.ImpactoOperacional);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "MTD";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.MTD);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "RTO";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.RTO);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "RPO";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.RPO);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "WRT";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.WRT);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvCriticos";
            settings.CallbackRouteValues = new { Controller = "Procesos", Action = "CriticosPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Hidden;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 265;
            settings.ClientSideEvents.BeginCallback = "OnBeginGridCallback";
            settings.ClientSideEvents.EndCallback = "OnEndGridCallback";
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;

            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

            settings.CommandColumn.Visible = false;
            settings.CommandColumn.Width = Unit.Percentage(10);
            settings.CommandColumn.ShowNewButtonInHeader = false;
            settings.CommandColumn.ShowDeleteButton = false;
            settings.CommandColumn.ShowEditButton = false;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Selected";
                c.Width = Unit.Percentage(5);
                c.Caption = "Marcado";
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.CheckBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "NroProceso";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionNroProceso;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Nombre";
                c.Width = Unit.Percentage(15);
                c.Caption = Resources.ProcesosResource.captionNombre;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "ImpactoFinanciero";
                c.Width = Unit.Percentage(15);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.ImpactoFinanciero);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Justify;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "ImpactoOperacional";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.ImpactoOperacional);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "MTD";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.MTD);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "RTO";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.RTO);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "RPO";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.RPO);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "WRT";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.WRT);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gvCriticos";
            settings.CallbackRouteValues = new { Controller = "Procesos", Action = "CriticosPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Hidden;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 265;
            settings.ClientSideEvents.BeginCallback = "OnBeginGridCallback";
            settings.ClientSideEvents.EndCallback = "OnEndGridCallback";
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;

            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

            settings.CommandColumn.Visible = false;
            settings.CommandColumn.Width = Unit.Percentage(10);
            settings.CommandColumn.ShowNewButtonInHeader = false;
            settings.CommandColumn.ShowDeleteButton = false;
            settings.CommandColumn.ShowEditButton = false;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Selected";
                c.Width = Unit.Percentage(5);
                c.Caption = "Marcado";
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.CheckBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "NroProceso";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionNroProceso;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Nombre";
                c.Width = Unit.Percentage(15);
                c.Caption = Resources.ProcesosResource.captionNombre;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "ImpactoFinanciero";
                c.Width = Unit.Percentage(15);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.ImpactoFinanciero);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Justify;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "ImpactoOperacional";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.ImpactoOperacional);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "MTD";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.MTD);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "RTO";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.RTO);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "RPO";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.RPO);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "WRT";
                c.Width = Unit.Percentage(10);
                c.Caption = Metodos.GetNombreEscala(eTipoEscala.WRT);
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });

            return settings;
        }
    }
    public class GridViewExportRiesgo
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvRiesgo";
            settings.CallbackRouteValues = new { Controller = "Procesos", Action = "RiesgoPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Hidden;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 265;
            settings.ClientSideEvents.BeginCallback = "OnBeginGridCallback";
            settings.ClientSideEvents.EndCallback = "OnEndGridCallback";
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;

            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "NroProceso";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionNroProceso;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Nombre";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionNombre;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "WRT";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionAmenaza;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Evento";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionEvento;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            }); settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "ImpactoFinanciero";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionProbabilidad;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "ImpactoOperacional";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionImpacto;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "MTD";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionSeveridad;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "RPO";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionFuente;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "RTO";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionControl;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvRiesgo";
            settings.CallbackRouteValues = new { Controller = "Procesos", Action = "RiesgoPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Hidden;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 265;
            settings.ClientSideEvents.BeginCallback = "OnBeginGridCallback";
            settings.ClientSideEvents.EndCallback = "OnEndGridCallback";
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;

            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

            //  settings.Columns.Add(c =>
            //{
            //    c.Settings.AllowSort = DefaultBoolean.True;
            //    c.FieldName = "Selected";
            //    c.Width = Unit.Percentage(5);
            //    c.Caption = "#";
            //    c.CellStyle.Wrap = DefaultBoolean.True;
            //    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
            //    c.HeaderStyle.Wrap = DefaultBoolean.True;
            //    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            //    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            //    c.ColumnType = MVCxGridViewColumnType.CheckBox;
            //});
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "NroProceso";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionNroProceso;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Nombre";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionNombre;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "WRT";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionAmenaza;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Evento";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionEvento;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            }); settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "ImpactoFinanciero";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionProbabilidad;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "ImpactoOperacional";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionImpacto;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "MTD";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionSeveridad;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "RPO";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionFuente;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "RTO";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionControl;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gvRiesgo";
            settings.CallbackRouteValues = new { Controller = "Procesos", Action = "RiesgoPartialView" };

            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Hidden;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 265;
            settings.ClientSideEvents.BeginCallback = "OnBeginGridCallback";
            settings.ClientSideEvents.EndCallback = "OnEndGridCallback";
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;

            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

            //  settings.Columns.Add(c =>
            //{
            //    c.Settings.AllowSort = DefaultBoolean.True;
            //    c.FieldName = "Selected";
            //    c.Width = Unit.Percentage(5);
            //    c.Caption = "#";
            //    c.CellStyle.Wrap = DefaultBoolean.True;
            //    c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
            //    c.HeaderStyle.Wrap = DefaultBoolean.True;
            //    c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            //    c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
            //    c.ColumnType = MVCxGridViewColumnType.CheckBox;
            //});
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "NroProceso";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionNroProceso;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Nombre";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionNombre;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "WRT";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionAmenaza;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Evento";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionEvento;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            }); settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "ImpactoFinanciero";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionProbabilidad;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "ImpactoOperacional";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionImpacto;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "MTD";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionSeveridad;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.TextBox;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "RPO";
                c.Width = Unit.Percentage(10);
                c.Caption = Resources.ProcesosResource.captionFuente;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });
            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "RTO";
                c.Width = Unit.Percentage(5);
                c.Caption = Resources.ProcesosResource.captionControl;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.ColumnType = MVCxGridViewColumnType.Memo;
            });

            return settings;
        }
    }
    public class GridViewExportRiesgoProbabilidad
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetProbabilidadEmpresa().Select(x => x.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetProbabilidadEmpresa().Select(x => x.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetProbabilidadEmpresa().Select(x => x.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
    }
    public class GridViewExportRiesgoImpacto
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetImpactoEmpresa().Select(x => x.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetImpactoEmpresa().Select(x => x.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetImpactoEmpresa().Select(x => x.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
    }
    public class GridViewExportRiesgoSeveridad
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetSeveridadEmpresa().Select(x => x.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetSeveridadEmpresa().Select(x => x.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetSeveridadEmpresa().Select(x => x.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
    }
    public class GridViewExportRiesgoControl
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetControlEmpresa().Select(x => x.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetControlEmpresa().Select(x => x.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = System.Web.UI.WebControls.Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetControlEmpresa().Select(x => x.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
    }
    public class GridViewExportRiesgoFuente
    {
        private static HttpSessionState Session { get { return HttpContext.Current.Session; } }
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }
        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }
        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }
        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }
        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRFPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetFuenteEmpresa().Select(t => t.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRFPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetFuenteEmpresa().Select(t => t.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            var settings = new GridViewSettings();

            settings.Name = "gvTR";
            settings.CallbackRouteValues = new { Controller = "Reportes", Action = "TablaTRFPartialView" };

            settings.Width = Unit.Percentage(100);
            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.ControlStyle.Paddings.Padding = Unit.Pixel(0);
            settings.ControlStyle.Border.BorderWidth = Unit.Pixel(0);
            settings.ControlStyle.BorderBottom.BorderWidth = Unit.Pixel(1);
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Settings.VerticalScrollableHeight = 400;
            settings.Width = Unit.Percentage(100);

            settings.SettingsPager.PageSize = 20;
            settings.SettingsPager.Position = PagerPosition.Bottom;
            settings.SettingsPager.FirstPageButton.Visible = true;
            settings.SettingsPager.LastPageButton.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50", "100" };

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.EditFormLayoutProperties.SettingsAdaptivity.AdaptivityMode = FormLayoutAdaptivityMode.SingleColumnWindowLimit;
            settings.EditFormLayoutProperties.SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth = 600;

            settings.Columns.Add(c =>
            {
                c.Settings.AllowSort = DefaultBoolean.True;
                c.FieldName = "Unidad";
                c.Caption = Resources.ReporteResource.captionUnidad;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.CellStyle.Wrap = DefaultBoolean.True;
                c.PropertiesEdit.DisplayFormatString = "{0}";
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = Unit.Percentage(40);
            });
            settings.Columns.AddBand(sb =>
            {
                sb.Caption = Resources.ReporteResource.captionValorEscala;
                sb.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                int Index = 0;
                List<string> _strEscala = Metodos.GetFuenteEmpresa().Select(t => t.Descripcion).ToList();
                int pWidth = 60 / _strEscala.Count();

                foreach (string Valor in _strEscala)
                {
                    sb.Columns.Add(c =>
                    {
                        c.Settings.AllowSort = DefaultBoolean.True;
                        c.FieldName = "CantidadEscalaUnbound" + Index.ToString();
                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                        c.Caption = Valor.ToString();
                        c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        c.HeaderStyle.VerticalAlign = VerticalAlign.Middle;
                        c.HeaderStyle.Wrap = DefaultBoolean.True;
                        c.Width = Unit.Percentage(pWidth);
                    });
                    Index++;
                }
            });
            settings.CustomUnboundColumnData = (s, e) =>
            {
                if (e.Column.FieldName.Contains("CantidadEscalaUnbound"))
                {
                    var valores = e.GetListSourceFieldValue("CantidadEscala") as List<Int32>;
                    int res = 0;
                    if (valores != null)
                    {
                        var ColName = e.Column.FieldName;
                        int index = int.Parse(e.Column.FieldName.Replace("CantidadEscalaUnbound", ""));
                        res = valores[index];
                    }
                    e.Value = res;
                }
            };

            return settings;
        }
    }

    public class GridViewExportIncidentes
    {
        static string ExcelDataAwareGridViewSettingsKey = "51172A18-2073-426B-A5CB-136347B3A79F";
        static string FormatConditionsExportGridViewSettingsKey = "14634B6F-E1DC-484A-9728-F9608615B628";
        static Dictionary<GridViewExportFormat, GridViewExportMethod> exportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> ExportFormatsInfo
        {
            get
            {
                if (exportFormatsInfo == null)
                    exportFormatsInfo = CreateExportFormatsInfo();
                return exportFormatsInfo;
            }
        }

        static IDictionary Context { get { return System.Web.HttpContext.Current.Items; } }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                {
                    GridViewExportFormat.Xls,
                    (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                {
                    GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf },
                {
                    GridViewExportFormat.Csv,
                    (settings, data) => GridViewExtension.ExportToCsv(settings, data, new CsvExportOptionsEx { ExportType = DevExpress.Export.ExportType.WYSIWYG })
                }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> dataAwareExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> DataAwareExportFormatsInfo
        {
            get
            {
                if (dataAwareExportFormatsInfo == null)
                    dataAwareExportFormatsInfo = CreateDataAwareExportFormatsInfo();
                return dataAwareExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateDataAwareExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Xls, GridViewExtension.ExportToXls },
                { GridViewExportFormat.Xlsx, GridViewExtension.ExportToXlsx },
                { GridViewExportFormat.Csv, GridViewExtension.ExportToCsv }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> formatConditionsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> FormatConditionsExportFormatsInfo
        {
            get
            {
                if (formatConditionsExportFormatsInfo == null)
                    formatConditionsExportFormatsInfo = CreateFormatConditionsExportFormatsInfo();
                return formatConditionsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateFormatConditionsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf},
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data) },
                { GridViewExportFormat.Xlsx,
                    (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG})
                },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }

        static Dictionary<GridViewExportFormat, GridViewExportMethod> advancedBandsExportFormatsInfo;
        public static Dictionary<GridViewExportFormat, GridViewExportMethod> AdvancedBandsExportFormatsInfo
        {
            get
            {
                if (advancedBandsExportFormatsInfo == null)
                    advancedBandsExportFormatsInfo = CreateAdvancedBandsExportFormatsInfo();
                return advancedBandsExportFormatsInfo;
            }
        }
        static Dictionary<GridViewExportFormat, GridViewExportMethod> CreateAdvancedBandsExportFormatsInfo()
        {
            return new Dictionary<GridViewExportFormat, GridViewExportMethod> {
                { GridViewExportFormat.Pdf, GridViewExtension.ExportToPdf },
                { GridViewExportFormat.Xls, (settings, data) => GridViewExtension.ExportToXls(settings, data, new XlsExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Xlsx, (settings, data) => GridViewExtension.ExportToXlsx(settings, data, new XlsxExportOptionsEx {ExportType = DevExpress.Export.ExportType.WYSIWYG}) },
                { GridViewExportFormat.Rtf, GridViewExtension.ExportToRtf }
            };
        }

        public static string GetExportButtonTitle(GridViewExportFormat format)
        {
            if (format == GridViewExportFormat.None)
                return string.Empty;
            return string.Format("Export to {0}", format.ToString().ToUpper());
        }

        public static GridViewSettings CreateGeneralMasterGridSettings(object DataToBind)
        {
            return CreateGeneralMasterGridSettings(GridViewDetailExportMode.None, DataToBind);
        }
        public static GridViewSettings CreateGeneralMasterGridSettings(GridViewDetailExportMode exportMode, object DataToBind)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "masterGrid";
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "CategoryID";
            settings.Columns.Add("CategoryID");
            settings.Columns.Add("CategoryName");
            settings.Columns.Add("Description");
            settings.Columns.Add(c =>
            {
                c.FieldName = "Picture";
                c.ColumnType = MVCxGridViewColumnType.BinaryImage;
                BinaryImageEditProperties properties = (BinaryImageEditProperties)c.PropertiesEdit;
                properties.ImageWidth = 120;
                properties.ImageHeight = 80;
                properties.ExportImageSettings.Width = 90;
                properties.ExportImageSettings.Height = 60;
            });

            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ExportMode = exportMode;

            settings.SettingsExport.GetExportDetailGridViews = (s, e) =>
            {
                int categoryID = (int)DataBinder.Eval(e.DataItem, "CategoryID");
                GridViewExtension grid = new GridViewExtension(CreateGeneralDetailGridSettings(categoryID));
                grid.Bind(DataToBind);
                e.DetailGridViews.Add(grid);
            };

            return settings;
        }

        public static GridViewSettings CreateGeneralDetailGridSettings(int uniqueKey)
        {
            GridViewSettings settings = new GridViewSettings();
            settings.Name = "detailGrid" + uniqueKey;
            settings.Width = Unit.Percentage(100);

            settings.KeyFieldName = "ProductID";
            settings.Columns.Add("ProductID");
            settings.Columns.Add("ProductName");
            settings.Columns.Add("UnitPrice");
            settings.Columns.Add("QuantityPerUnit");

            settings.SettingsDetail.MasterGridName = "masterGrid";

            return settings;
        }

        static GridViewSettings exportGridViewSettings;
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                    exportGridViewSettings = CreateExportGridViewSettings();
                return exportGridViewSettings;
            }
        }
        static GridViewSettings CreateExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvIncidentes";
            settings.KeyFieldName = "IdIncidente";
            settings.CallbackRouteValues = new { Controller = "PMI", Action = "PMIPartialView" };
            settings.SettingsEditing.AddNewRowRouteValues = new { Controller = "PMI", Action = "EditIncidenteAddNewPartial" };
            settings.SettingsEditing.UpdateRowRouteValues = new { Controller = "PMI", Action = "EditIncidenteUpdatePartial" };
            settings.SettingsEditing.DeleteRowRouteValues = new { Controller = "PMI", Action = "EditIncidenteDeletePartial" };
            settings.SettingsEditing.Mode = GridViewEditingMode.PopupEditForm;
            settings.SettingsBehavior.ConfirmDelete = true;
            settings.Width = Unit.Percentage(100);
            settings.Settings.VerticalScrollableHeight = 325;
            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Visible;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Caption = Resources.PMIResource.GridTitle;
            settings.Styles.TitlePanel.Font.Bold = true;
            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.ShowFilterRow = true;
            settings.Settings.ShowFilterRowMenu = true;
            settings.Settings.ShowFooter = true;

            settings.CommandColumn.Visible = true;
            settings.CommandColumn.ShowNewButton = false;
            settings.CommandColumn.ShowNewButtonInHeader = true;
            settings.CommandColumn.ShowDeleteButton = true;
            settings.CommandColumn.ShowEditButton = true;
            settings.CommandColumn.FixedStyle = GridViewColumnFixedStyle.Left;
            settings.CommandColumn.Width = Unit.Pixel(150);
            settings.CommandColumn.Caption = " ";

            settings.SettingsDetail.AllowOnlyOneMasterRowExpanded = true;
            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ShowDetailButtons = true;

            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaIncidente";
                c.Caption = Resources.PMIResource.captionFechaIncidente;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdTipoIncidente";
                c.Caption = Resources.PMIResource.captionTipoIncidente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 50;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Metodos.GetTiposIncidente();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdNaturalezaIncidente";
                c.Caption = Resources.PMIResource.captionNaturalezaIncidente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 50;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Metodos.GetNaturalezaIncidente();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdFuenteIncidente";
                c.Caption = Resources.PMIResource.captionFuenteIncidente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 50;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Metodos.GetFuentesIncidente();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Descripcion";
                c.Caption = Resources.PMIResource.captionDescripcion;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 450;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "LugarIncidente";
                c.Caption = Resources.PMIResource.captionLugarIncidente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 250;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Duracion";
                c.Caption = Resources.PMIResource.captionDuracion;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 50;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "NombreReportero";
                c.Caption = Resources.PMIResource.captionNombreReportero;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "NombreSolucionador";
                c.Caption = Resources.PMIResource.captionNombreSolucionador;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Observacion";
                c.Caption = Resources.PMIResource.captionObservacion;
                c.ColumnType = MVCxGridViewColumnType.Memo;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Justify;
                c.Width = 250;
                c.CellStyle.Wrap = DefaultBoolean.True;
            });

            return settings;
        }

        public static GridViewSettings ExcelDataAwareExportGridViewSettings
        {
            get
            {
                GridViewSettings settings = Context[ExcelDataAwareGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[ExcelDataAwareGridViewSettingsKey] = settings = CreateExcelDataAwareExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateExcelDataAwareExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvIncidentes";
            settings.KeyFieldName = "IdIncidente";
            settings.CallbackRouteValues = new { Controller = "PMI", Action = "PMIPartialView" };
            settings.SettingsEditing.AddNewRowRouteValues = new { Controller = "PMI", Action = "EditIncidenteAddNewPartial" };
            settings.SettingsEditing.UpdateRowRouteValues = new { Controller = "PMI", Action = "EditIncidenteUpdatePartial" };
            settings.SettingsEditing.DeleteRowRouteValues = new { Controller = "PMI", Action = "EditIncidenteDeletePartial" };
            settings.SettingsEditing.Mode = GridViewEditingMode.PopupEditForm;
            settings.SettingsBehavior.ConfirmDelete = true;
            settings.Width = Unit.Percentage(100);
            settings.Settings.VerticalScrollableHeight = 325;
            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Visible;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Caption = Resources.PMIResource.GridTitle;
            settings.Styles.TitlePanel.Font.Bold = true;
            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.ShowFilterRow = true;
            settings.Settings.ShowFilterRowMenu = true;
            settings.Settings.ShowFooter = true;

            settings.CommandColumn.Visible = true;
            settings.CommandColumn.ShowNewButton = false;
            settings.CommandColumn.ShowNewButtonInHeader = true;
            settings.CommandColumn.ShowDeleteButton = true;
            settings.CommandColumn.ShowEditButton = true;
            settings.CommandColumn.FixedStyle = GridViewColumnFixedStyle.Left;
            settings.CommandColumn.Width = Unit.Pixel(150);
            settings.CommandColumn.Caption = " ";

            settings.SettingsDetail.AllowOnlyOneMasterRowExpanded = true;
            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ShowDetailButtons = true;

            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaIncidente";
                c.Caption = Resources.PMIResource.captionFechaIncidente;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdTipoIncidente";
                c.Caption = Resources.PMIResource.captionTipoIncidente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 50;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Metodos.GetTiposIncidente();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdNaturalezaIncidente";
                c.Caption = Resources.PMIResource.captionNaturalezaIncidente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 50;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Metodos.GetNaturalezaIncidente();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdFuenteIncidente";
                c.Caption = Resources.PMIResource.captionFuenteIncidente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 50;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Metodos.GetFuentesIncidente();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Descripcion";
                c.Caption = Resources.PMIResource.captionDescripcion;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 450;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "LugarIncidente";
                c.Caption = Resources.PMIResource.captionLugarIncidente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 250;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Duracion";
                c.Caption = Resources.PMIResource.captionDuracion;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 50;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "NombreReportero";
                c.Caption = Resources.PMIResource.captionNombreReportero;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "NombreSolucionador";
                c.Caption = Resources.PMIResource.captionNombreSolucionador;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Observacion";
                c.Caption = Resources.PMIResource.captionObservacion;
                c.ColumnType = MVCxGridViewColumnType.Memo;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Justify;
                c.Width = 250;
                c.CellStyle.Wrap = DefaultBoolean.True;
            });

            return settings;
        }
        public static GridViewSettings FormatConditionsExportGridViewSettings
        {
            get
            {
                var settings = Context[FormatConditionsExportGridViewSettingsKey] as GridViewSettings;
                if (settings == null)
                    Context[FormatConditionsExportGridViewSettingsKey] = settings = CreateFormatConditionsExportGridViewSettings();
                return settings;
            }
        }
        static GridViewSettings CreateFormatConditionsExportGridViewSettings()
        {
            GridViewSettings settings = new GridViewSettings();

            settings.Name = "gvIncidentes";
            settings.KeyFieldName = "IdIncidente";
            settings.CallbackRouteValues = new { Controller = "PMI", Action = "PMIPartialView" };
            settings.SettingsEditing.AddNewRowRouteValues = new { Controller = "PMI", Action = "EditIncidenteAddNewPartial" };
            settings.SettingsEditing.UpdateRowRouteValues = new { Controller = "PMI", Action = "EditIncidenteUpdatePartial" };
            settings.SettingsEditing.DeleteRowRouteValues = new { Controller = "PMI", Action = "EditIncidenteDeletePartial" };
            settings.SettingsEditing.Mode = GridViewEditingMode.PopupEditForm;
            settings.SettingsBehavior.ConfirmDelete = true;
            settings.Width = Unit.Percentage(100);
            settings.Settings.VerticalScrollableHeight = 325;
            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Visible;
            settings.Settings.VerticalScrollBarMode = ScrollBarMode.Auto;
            settings.Caption = Resources.PMIResource.GridTitle;
            settings.Styles.TitlePanel.Font.Bold = true;
            settings.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;
            settings.SettingsLoadingPanel.Mode = GridViewLoadingPanelMode.Disabled;
            settings.ClientSideEvents.BeginCallback = "function (s,e) { lp.Show(); }";
            settings.ClientSideEvents.EndCallback = "function (s,e) { lp.Hide(); }";

            settings.Settings.ShowFilterRow = true;
            settings.Settings.ShowFilterRowMenu = true;
            settings.Settings.ShowFooter = true;

            settings.CommandColumn.Visible = true;
            settings.CommandColumn.ShowNewButton = false;
            settings.CommandColumn.ShowNewButtonInHeader = true;
            settings.CommandColumn.ShowDeleteButton = true;
            settings.CommandColumn.ShowEditButton = true;
            settings.CommandColumn.FixedStyle = GridViewColumnFixedStyle.Left;
            settings.CommandColumn.Width = Unit.Pixel(150);
            settings.CommandColumn.Caption = " ";

            settings.SettingsDetail.AllowOnlyOneMasterRowExpanded = true;
            settings.SettingsDetail.ShowDetailRow = true;
            settings.SettingsDetail.ShowDetailButtons = true;

            settings.Columns.Add(c =>
            {
                c.FieldName = "FechaIncidente";
                c.Caption = Resources.PMIResource.captionFechaIncidente;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdTipoIncidente";
                c.Caption = Resources.PMIResource.captionTipoIncidente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 50;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Metodos.GetTiposIncidente();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdNaturalezaIncidente";
                c.Caption = Resources.PMIResource.captionNaturalezaIncidente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 50;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Metodos.GetNaturalezaIncidente();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "IdFuenteIncidente";
                c.Caption = Resources.PMIResource.captionFuenteIncidente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.FixedStyle = GridViewColumnFixedStyle.Left;
                c.Width = 50;
                c.EditorProperties().ComboBox(p =>
                {
                    p.TextField = "Descripcion";
                    p.ValueField = "Id";
                    p.ValueType = typeof(int);
                    p.DataSource = Metodos.GetFuentesIncidente();
                });
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Descripcion";
                c.Caption = Resources.PMIResource.captionDescripcion;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 450;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "LugarIncidente";
                c.Caption = Resources.PMIResource.captionLugarIncidente;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 250;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Duracion";
                c.Caption = Resources.PMIResource.captionDuracion;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.Width = 50;
                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "NombreReportero";
                c.Caption = Resources.PMIResource.captionNombreReportero;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "NombreSolucionador";
                c.Caption = Resources.PMIResource.captionNombreSolucionador;
                c.ColumnType = MVCxGridViewColumnType.DateEdit;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                c.Width = 100;
            });
            settings.Columns.Add(c =>
            {
                c.FieldName = "Observacion";
                c.Caption = Resources.PMIResource.captionObservacion;
                c.ColumnType = MVCxGridViewColumnType.Memo;
                c.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                c.HeaderStyle.Wrap = DefaultBoolean.True;
                c.CellStyle.HorizontalAlign = HorizontalAlign.Justify;
                c.Width = 250;
                c.CellStyle.Wrap = DefaultBoolean.True;
            });

            return settings;
        }

    }

}