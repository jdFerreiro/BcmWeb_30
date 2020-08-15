using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BcmWeb_30 .Security
{
    public class Encriptador
    {

        public enum HasAlgorimt
        {
            MD5,
            SHA,
            SHA1,
        }

        public enum Keysize
        {
            KS32 = 32,
            KS64 = 64,
            KS128 = 128,
            KS256 = 256,
        }

        private string passBase = "";

        private string saltValue = "";

        private int passwordIterations = 0;

        private string initVector = "";

        public string InicioClaves(string Clave1_5Caracteres, string Clave2_5Caracteres, string Clave3_16Caracasteres, int NroIteraciones)
        {
            try
            {
                string ok = "";
                if ((Clave1_5Caracteres.Length < 5))
                {
                    ok = "LA CLAVE 1 DEBE SER DE 5 CARACTERES MINIMO DE LARGO";
                }
                else
                {
                    passBase = Clave1_5Caracteres;
                    // Valido la clave 2
                    if ((Clave2_5Caracteres.Length < 8))
                    {
                        ok = "LA CLAVE 2 DEBE SER DE 5 CARACTERES MINIMO DE LARGO";
                    }
                    else
                    {
                        saltValue = Clave2_5Caracteres;
                        // Valido la clave 3
                        if ((Clave3_16Caracasteres.Length < 16))
                        {
                            ok = "LA CLAVE 3 DEBE SER DE 16 CARACTERES MINIMO DE LARGO";
                        }
                        else
                        {
                            initVector = Clave3_16Caracasteres;
                            // Valido el nro de iteraciones
                            if ((NroIteraciones < 1))
                            {
                                ok = "EL NRO DE ITERACIONES DEBE SER MAYOR A CERO (0)";
                            }
                            else
                            {
                                passwordIterations = NroIteraciones;
                                ok = "OK";
                            }
                        }
                    }
                }
                return ok;
            }
            catch
            {
                throw;
            }
        }
        public string Encriptar(string textoQueEncriptaremos, HasAlgorimt hashAlgorithm, Keysize keySize)
        {
            this.InicioClaves("8CMW38!N37", "GRUP0@PL1R3D!N3T81@W38700LS", "B1@W38GRUP0@PL1R3D!N3T81@W38700LS!N3T", 5);
            try
            {
                // Valido los campos
                if (((passBase != "") && ((saltValue != "") && ((passwordIterations > 0) && (initVector != "")))))
                {
                    if (initVector.Length >= 16)
                        initVector = initVector.Substring(1, 16);
                    else
                        initVector = ((initVector + new string(' ', 16))).Substring(1, 16).Replace(" ", "%");

                    if (saltValue.Length >= 16)
                        saltValue = saltValue.Substring(1, 16);
                    else
                        saltValue = ((saltValue + new string(' ', 16))).Substring(1, 16).Replace(" ", "%");

                    byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                    byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(textoQueEncriptaremos);
                    Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passBase, saltValueBytes, passwordIterations);
                    byte[] keyBytes = password.GetBytes(((int)keySize / 8));
                    RijndaelManaged symmetricKey = new RijndaelManaged();
                    symmetricKey.Mode = CipherMode.CBC;
                    ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    byte[] cipherTextBytes = memoryStream.ToArray();
                    memoryStream.Close();
                    //cryptoStream.Close();
                    string cipherText = Convert.ToBase64String(cipherTextBytes);
                    return cipherText;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                throw;
            }
        }
        public string Desencriptar(string textoEncriptado, HasAlgorimt hashAlgorithm, Keysize keySize)
        {
            this.InicioClaves("8CMW38!N37", "GRUP0@PL1R3D!N3T81@W38700LS", "B1@W38GRUP0@PL1R3D!N3T81@W38700LS!N3T", 5);
            try
            {
                if (((passBase != "")
                            && ((saltValue != "")
                            && ((passwordIterations > 0)
                            && (initVector != "")))))
                {
                    if (initVector.Length >= 16)
                        initVector = initVector.Substring(1, 16);
                    else
                        initVector = ((initVector + new string(' ', 16))).Substring(1, 16).Replace(" ", "%");

                    if (saltValue.Length >= 16)
                        saltValue = saltValue.Substring(1, 16);
                    else
                        saltValue = ((saltValue + new string(' ', 16))).Substring(1, 16).Replace(" ", "%");

                    byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                    byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                    byte[] plainTextBytes = Convert.FromBase64String(textoEncriptado);
                    Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passBase, saltValueBytes, passwordIterations);
                    byte[] keyBytes = password.GetBytes(((int)keySize / 8));
                    RijndaelManaged symmetricKey = new RijndaelManaged();
                    symmetricKey.Mode = CipherMode.CBC;
                    ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                    MemoryStream MemoryStream = new MemoryStream();
                    CryptoStream CryptoStream = new CryptoStream(MemoryStream, decryptor, CryptoStreamMode.Write);
                    CryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    CryptoStream.FlushFinalBlock();
                    byte[] cipherTextBytes = MemoryStream.ToArray();
                    MemoryStream.Close();
                    string plainText = Encoding.UTF8.GetString(cipherTextBytes);
                    return plainText;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                throw;
            }
        }
    }
}