using BcmWeb_30.Data.EF;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace BcmWeb_30 .Models
{


    #region ClasesPrimarias
    public abstract class ItemsData : ModuloModel, IHierarchicalEnumerable, IEnumerable
    {
        public ItemsData()
        {
        }

        public abstract IEnumerable Data { get; }

        public IEnumerator GetEnumerator()
        {
            return Data.GetEnumerator();
        }
        public IHierarchyData GetHierarchyData(object enumeratedItem)
        {
            return (IHierarchyData)enumeratedItem;
        }
    }
    public class ItemData : ModuloModel, IHierarchyData
    {
        private readonly System.Web.Mvc.UrlHelper urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
        public string Text { get; protected set; }
        public string NavigateUrl { get; protected set; }

        public ItemData(string text, string Accion, string Controler, long IdModulo)
        {
            Text = text;
            NavigateUrl = null;
            if (!string.IsNullOrEmpty(Accion))
            {
                NavigateUrl = urlHelper.Action(Accion, Controler, new { modId = IdModulo });
            }
        }

        // IHierarchyData
        bool IHierarchyData.HasChildren
        {
            get { return HasChildren(); }
        }
        object IHierarchyData.Item
        {
            get { return this; }
        }
        string IHierarchyData.Path
        {
            get { return NavigateUrl; }
        }
        string IHierarchyData.Type
        {
            get { return GetType().ToString(); }
        }
        IHierarchicalEnumerable IHierarchyData.GetChildren()
        {
            return CreateChildren();
        }
        IHierarchyData IHierarchyData.GetParent()
        {
            return null;
        }

        protected virtual bool HasChildren()
        {
            return false;
        }
        protected virtual IHierarchicalEnumerable CreateChildren()
        {
            return null;
        }
    }
    #endregion

    public class SubModulosData : ItemsData
    {
        private HttpSessionState Session { get { return HttpContext.Current.Session; } }
        public SubModulosData(long idModulo) : base()
        {
            IdModulo = idModulo;
        }

        public override IEnumerable Data
        {

            get
            {
                long IdDocumento = 0;
                List<tblModulo> Modulos = new List<tblModulo>();

                if (Session["IdDocumento"] != null)
                    IdDocumento = long.Parse(Session["IdDocumento"].ToString());

                if (IdModulo < 11000000)
                {
                    Modulos = Metodos.GetSubModulos(IdModulo).ToList();
                    if (IdDocumento > 0)
                    {
                        Modulos.AddRange(Metodos.GetSubModulos(90000000).ToList());
                        Modulos.AddRange(Metodos.GetSubModulos(91000000).ToList());
                    }
                }
                else
                {
                    Modulos = Metodos.GetSubModulos(IdModulo).ToList();
                }
                return Modulos.Select(x => new ModuloData(x));
            }
        }
    }
    public class ModuloData : ItemData
    {
        public tblModulo Modulo { get; protected set; }

        public ModuloData(tblModulo modulo)
            : base(modulo.Nombre, string.Empty, string.Empty, modulo.IdModuloPadre)
        {
            Modulo = modulo;
        }

        protected override bool HasChildren()
        {
            return true;
        }
        protected override IHierarchicalEnumerable CreateChildren()
        {
            return new ChildsData(Modulo.IdModulo);
        }
    }
    public class ChildsData : ItemsData
    {
        public long ModuloID { get; protected set; }

        public ChildsData(long moduloID)
            : base()
        {
            ModuloID = moduloID;
        }

        public override IEnumerable Data
        {
            get
            {
                List<tblModulo> Modulos = new List<tblModulo>();
                Modulos = Metodos.GetSubModulos(ModuloID).ToList();
                List<ItemData> data = new List<ItemData>();

                foreach (tblModulo Modulo in Modulos)
                {
                    if (Modulo.IdTipoElemento == 3)
                    {
                        data.Add(new ModuloData(Modulo));
                    }
                    else
                    {
                        data.Add(new ChildData(Modulo));
                    }
                }

                return data;
                //return Modulos.OrderBy(x => x.IdModulo).ThenBy(x => x.IdTipoElemento).Select(x => );
                // return NorthwindDataProvider.DB.Products.Where(p => p.CategoryID == CategoryID).ToList().Select(p => new ProductData(p));
            }
        }
    }
    public class ChildData : ItemData
    {
        public ChildData(tblModulo modulo)
            : base(modulo.Nombre, modulo.Accion, modulo.Controller, modulo.IdModulo)
        {
        }
    }
}