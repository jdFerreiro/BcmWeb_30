using DevExpress.Utils;
using DevExpress.Web.Mvc;
using DevExpress.XtraCharts;
using System;
using System.Collections.Generic;
using System.Data;

namespace BcmWeb_30 .Models
{
    public class ReporteModel : ModulosUserModel
    {
        public long IdUnidadOrganizativa { get; set; }
        public string UnidadOrganizativa
        {
            get
            {
                return Metodos.GetNombreUnidadCompleto(this.IdUnidadOrganizativa);
            }
        }
        public List<TipoEscalaGrafico> EscalaGrafico { get; set; }
        public List<String> Escalas { get; set; }
        public long IdEscalaGrafico { get; set; }
        public object DataCuadro { get; set; }
        public object DataMTD { get; set; }
        public object DataRPO { get; set; }
        public object DataRTO { get; set; }
        public object DataWRT { get; set; }
    }
    public class TipoEscalaGrafico
    {
        public long IdTipoEscala { get; set; }
        public string TipoEscala { get; set; }
    }
    public class TipoEscalaTexto
    {
        public string IdTipoEscala { get; set; }
        public string TipoEscala { get; set; }
    }
    public class ChartHelpers
    {
        public static ChartControlSettings GetChartSettings()
        {
            var settings = new ChartControlSettings();

            settings.Name = "chart";
            settings.Width = 920;
            settings.Height = 500;
            settings.BorderOptions.Visibility = DefaultBoolean.False;
            settings.CrosshairEnabled = DefaultBoolean.False;

            //settings.Legends.Default(l =>
            //{
            //    l.AlignmentHorizontal = LegendAlignmentHorizontal.Right;
            //});
            settings.Legends.Default(l =>
            {
                l.AlignmentHorizontal = LegendAlignmentHorizontal.Center;
                l.AlignmentVertical = LegendAlignmentVertical.BottomOutside;
                l.MaxHorizontalPercentage = 50;
                l.HorizontalIndent = 12;
                l.Direction = LegendDirection.LeftToRight;
                l.Border.Visibility = DefaultBoolean.True;
            });

            settings.Series.Template(t =>
            {
                t.Views().SideBySideBarSeriesView(v =>
                {
                    v.SeriesLabel(l =>
                    {
                        l.Position = (BarSeriesLabelPosition)Enum.Parse(typeof(BarSeriesLabelPosition), "Center");
                        l.TextOrientation = (TextOrientation)Enum.Parse(typeof(TextOrientation), "Horizontal");
                        l.Indent = 0;
                        l.ResolveOverlappingMode = ResolveOverlappingMode.Default;
                    });
                });
                t.SetDataMembers("Escala", "Cantidad");
                t.LabelsVisibility = DefaultBoolean.True;
            });

            settings.XYDiagram(d =>
            {
                d.AxisY.Interlaced = true;
                d.AxisY.Title.Text = "";
                d.AxisY.Title.Visibility = DefaultBoolean.True;
            });

            return settings;
        }
        public static DataTable GenerateDataIO(object dataObject)
        {
            var data = new DataTable();
            var col1 = new DataColumn("IdUnidad", typeof(long));
            var col2 = new DataColumn("Unidad", typeof(string));
            var col3 = new DataColumn("Escala", typeof(string));
            var col4 = new DataColumn("Cantidad", typeof(int));

            data.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4 });
            List<objGraphIO> dataMaster = dataObject as List<objGraphIO>;

            foreach (objGraphIO dataGraph in dataMaster)
            {
                var row = data.NewRow();
                row[0] = dataGraph.IdUnidad;
                row[1] = dataGraph.Unidad;
                row[2] = dataGraph.Escala;
                row[3] = dataGraph.Cantidad;

                data.Rows.Add(row);
            }

            return data;
        }
    }
    public class TablasGeneralesModel
    {
        public string Descripcion { get; set; }
        public string Valoracion { get; set; }
    }
    public class RiesgoControlModel : ModulosUserModel
    {
        public long IdUnidadOrganizativa { get; set; }
        public string UnidadOrganizativa
        {
            get
            {
                return Metodos.GetNombreUnidadCompleto(this.IdUnidadOrganizativa);
            }
        }
    }
    public class DataRiesgoControl
    {
        public int NroProceso { get; set; }
        public string Proceso { get; set; }
        public string Amenaza { get; set; }
        public string Evento { get; set; }
        public string Implantado { get; set; }
        public string Implantar { get; set; }
        public int IdEstado { get; set; }
        public string Estado { get; set; }
        public short Probabilidad { get; set; }
        public short Control { get; set; }
        public short Impacto { get; set; }
    }
}