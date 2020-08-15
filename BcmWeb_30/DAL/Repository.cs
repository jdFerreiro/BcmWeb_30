using BcmWeb_30.Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BcmWeb_30 .Security
{
    public static class Repository
    {

        public static User GetUserDetails(string UserCode, string Password)
        {
            Encriptador _Encriptar = new Encriptador();
            string _encriptedPassword = _Encriptar.Encriptar(Password, Encriptador.HasAlgorimt.SHA1, Encriptador.Keysize.KS256);

            User Usuario = new User();

            using (Entities db= new Entities())
            {
                Usuario = (from x in db.tblUsuario
                           let _Empresas = x.tblEmpresaUsuario
                                   .Select(e => new EmpresaUsuario
                                   {
                                       IdEmpresa = e.IdEmpresa,
                                       Modulos = e.tblNivelUsuario.tblModulo_NivelUsuario.Where(m => m.tblModulo.IdModuloPadre == 0).Select(m => m.IdModulo).ToList(),
                                       Nombre = e.tblEmpresa.NombreComercial,
                                       Rol = e.tblNivelUsuario.RolUsuario

                                   }).ToList()
                           where x.Email == UserCode && x.ClaveUsuario == _encriptedPassword
                           select new User
                           {
                               Email = x.Email,
                               Estatus = (short)x.EstadoUsuario,
                               Id = x.IdUsuario,
                               Name = x.Nombre,
                               Empresas = _Empresas,
                               FechaUltimaConexion = x.FechaUltimaConexion != null ? (DateTime)x.FechaUltimaConexion : DateTime.Now,
                               PrimeraVez = (bool)x.PrimeraVez
                           }).FirstOrDefault();
            }

            return Usuario;
        }
        
    }
}