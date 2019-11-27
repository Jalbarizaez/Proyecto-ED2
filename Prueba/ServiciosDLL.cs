using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba
{
    public class ServiciosDLL
    {
        public class SDES
        {
            private const int bufferLenght = 1000;
            private int[] ip = new int[8];
            private int[] ip_1 = new int[8];
            private int[] ep = new int[8];
            private int[] p10 = new int[10];
            private int[] p8 = new int[8];
            private int[] p4 = new int[4];
            private string[][] S0 = new string[4][];
            private string[][] S1 = new string[4][];

			private string SBox(string bits)
            {
                S0[0] = new string[4] { "01", "00", "11", "10" };
                S0[1] = new string[4] { "11", "10", "01", "00" };
                S0[2] = new string[4] { "00", "10", "01", "11" };
                S0[3] = new string[4] { "11", "01", "11", "10" };

                S1[0] = new string[4] { "00", "01", "10", "11" };
                S1[1] = new string[4] { "10", "00", "01", "11" };
                S1[2] = new string[4] { "11", "00", "01", "00" };
                S1[3] = new string[4] { "10", "01", "00", "11" };

                char[] bit = bits.ToCharArray();

                int F0 = IndiceSBox(bit[0].ToString() + bit[3].ToString());
                int C0 = IndiceSBox(bit[1].ToString() + bit[2].ToString());
                int F1 = IndiceSBox(bit[4].ToString() + bit[7].ToString());
                int C1 = IndiceSBox(bit[5].ToString() + bit[6].ToString());

                return S0[F0][C0] + S1[F1][C1];
            }

            private int IndiceSBox(string cadena)
            {
                if (cadena == "00")
                    return 0;
                if (cadena == "01")
                    return 1;
                if (cadena == "10")
                    return 2;
                if (cadena == "11")
                    return 3;
                else
                    return 0;
            }

			private void Read_Permutations(string path)
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] line_split = lines[i].Split(':');
                    string[] order = line_split[1].Split(',');
                    if (i == 0)
                    {
                        int count = 0;
                        foreach (var item in order)
                        {
                            p10[count] = (Convert.ToInt16(item));
                            count++;
                        }
                    }
                    if (i == 1)
                    {
                        int count = 0;
                        foreach (var item in order)
                        {
                            p8[count] = (Convert.ToInt16(item));
                            count++;
                        }
                    }
                    if (i == 2)
                    {
                        int count = 0;
                        foreach (var item in order)
                        {
                            p4[count] = (Convert.ToInt16(item));
                            count++;
                        }
                    }
                    if (i == 3)
                    {
                        int count = 0;
                        foreach (var item in order)
                        {
                            ep[count] = (Convert.ToInt16(item));
                            count++;
                        }
                    }
                    if (i == 4)
                    {
                        int count = 0;
                        foreach (var item in order)
                        {
                            ip[count] = (Convert.ToInt16(item));
                            count++;
                        }
                    }
                    if (i == 5)
                    {
                        int count = 0;
                        foreach (var item in order)
                        {

                            ip_1[count] = (Convert.ToInt16(item));
                            count++;
                        }
                    }
                }
            }

			private byte CIF(byte item, string k1, string k2)
            {
                var bits = Convert.ToString(item, 2);
                var byte_ = bits.PadLeft(8, '0');
                var first_permutation = IP(byte_);
                var second_permutation = EP(first_permutation[1]);
                var xor = XOR(k1, second_permutation);
                var third_permutation = SBox(xor);
                var xor2 = XOR(P4(third_permutation), first_permutation[0]);
                var four_permutation = EP(xor2);
                var xor3 = XOR(k2, four_permutation);
                var five_permutation = SBox(xor3);
                var xor4 = XOR(P4(five_permutation), first_permutation[1]);
                var six_permutation = xor4 + xor2;
                var final = IP_1(six_permutation);
                return Convert.ToByte((final[0] + final[1]), 2);
            }

            private void Write_bytes(string k1, string k2, string path_read, string path_write)
            {
                int count = 0;
                var write = new byte[bufferLenght];
                var buffer = new byte[bufferLenght];
                using (var File = new FileStream(path_write, FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(File))
                    {
                        using (var file = new FileStream(path_read, FileMode.Open))
                        {
                            using (var reader = new BinaryReader(file, System.Text.Encoding.ASCII))
                            {
                                while (reader.BaseStream.Position != reader.BaseStream.Length)
                                {
                                    buffer = reader.ReadBytes(bufferLenght);
                                    foreach (var item in buffer)
                                    {
                                        write[count] = CIF(item, k1, k2);
                                        count++;

                                    }
                                    writer.Write(write, 0, count);
                                    count = 0;
                                    write = new byte[bufferLenght];
                                }
                            }

                        }
                    }
                }
            }

			private string XOR(string bits1, string bits2)
            {
                var bit1 = bits1.ToCharArray();
                var bit2 = bits2.ToCharArray();
                string final = "";
                for (int i = 0; i < bit1.Length; i++)
                {
                    if (bit1[i] == bit2[i])
                    {
                        final += "0";
                    }
                    else
                    {
                        final += "1";
                    }
                }
                return final;
            }

			private string[] Keys(string key)
            {
                string[] keys = new string[2];
                var first_permutation = P10(key);
                var Left_Shift_1 = LS1(first_permutation);
                var second_permutation = P8(Left_Shift_1[0] + Left_Shift_1[1]);
                keys[0] = second_permutation;
                var Left_Shift_2 = LS2(Left_Shift_1[0] + Left_Shift_1[1]);
                var last_permutation = P8(Left_Shift_2[0] + Left_Shift_2[1]);
                keys[1] = last_permutation;
                return keys;
            }


            public void Cifrado(string bits, string path_read_S, string path_write, string path_read)
            {
                Read_Permutations(path_read_S);
                string[] keys = Keys(bits);
                Write_bytes(keys[0], keys[1], path_read, path_write);
            }

            public void Descifrado(string bits, string path_read_S, string path_write, string path_read)
            {
                Read_Permutations(path_read_S);
                string[] keys = Keys(bits);
                Write_bytes(keys[1], keys[0], path_read, path_write);
            }


            private string EP(string bits)
            {
                var bit = bits.ToCharArray();
                string final = "";
                foreach (var item in ep)
                {
                    final += bits[item];
                }
                return final;
            }

			private string[] IP_1(string bits)
            {
                var bit = bits.ToCharArray();
                string[] final = new string[2];
                int count = 0;
                foreach (var item in ip_1)
                {
                    if (count < 4)
                    {
                        final[0] += bits[item];
                        count++;
                    }
                    else
                    {
                        final[1] += bits[item];
                    }
                }
                return final;
            }

			private string[] IP(string bits)
            {
                var bit = bits.ToCharArray();
                string[] final = new string[2];
                int count = 0;
                foreach (var item in ip)
                {
                    if (count < 4)
                    {
                        final[0] += bits[item];
                        count++;
                    }
                    else
                    {
                        final[1] += bits[item];
                    }
                }
                return final;
            }

			private string P4(string bits)
            {
                var bit = bits.ToCharArray();
                string final = "";
                foreach (var item in p4)
                {
                    final += bits[item];
                }
                return final;
            }

			private string P10(string bits)
            {
                var bit = bits.ToCharArray();
                string final = "";
                foreach (var item in p10)
                {
                    final += bits[item];
                }
                return final;
            }

			private string[] LS1(string bits)
            {
                var bit = bits.ToCharArray();
                string[] final = new string[2];
                for (int i = 1; i < 5; i++)
                {
                    final[0] += bit[i];
                }
                final[0] += bit[0];
                for (int i = 6; i < 10; i++)
                {
                    final[1] += bit[i];
                }
                final[1] += bit[5];
                return final;
            }

			private string P8(string bits)
            {
                var bit = bits.ToCharArray();
                string final = "";
                foreach (var item in p8)
                {
                    final += bits[item];
                }
                return final;
            }

			private string[] LS2(string bits)
            {
                var bit = bits.ToCharArray();
                string[] final = new string[2];
                for (int i = 2; i < 5; i++)
                {
                    final[0] += bit[i];
                }
                final[0] += bit[0];
                final[0] += bit[1];
                for (int i = 7; i < 10; i++)
                {
                    final[1] += bit[i];
                }
                final[1] += bit[5];
                final[1] += bit[6];
                return final;
            }
        }

		public class NodoHuff
        {
            public byte Dato { get; set; }
            public decimal Probabilidad { get; set; }
            public NodoHuff Izquierda { get; set; }
            public NodoHuff Derecha { get; set; }
            public NodoHuff Padre { get; set; }
            public NodoHuff(decimal probabilidad)
            {
                Probabilidad = probabilidad;
                Izquierda = null;
                Derecha = null;
                Padre = null;
            }


            public NodoHuff(byte dato, decimal probabilidad)
            {
                Dato = dato;
                Probabilidad = probabilidad;
                Izquierda = null;
                Derecha = null;
                Padre = null;
            }
            public bool Hoja()
            {
                if (Derecha == null && Izquierda == null) return true;
                else return false;
            }
        }

        public class CompresionHuffman
        {
            public const int bufferLenght = 1000;
            private static NodoHuff Raiz { get; set; }
            private static Dictionary<byte, string> Tabla_Caracteres { get; set; }
            private static Dictionary<byte, int> Tabla_Frecuencias { get; set; }
            public static int Tamaño_Datos { get; set; }
            public static decimal Cantidad_Datos;
            private char separador = new char();


            public void Compresion(string path_Lectura, string path_Escritura)
            {
                Tabla_Frecuencias = new Dictionary<byte, int>();
                Tabla_Caracteres = new Dictionary<byte, string>();
                ArbolHuffman(path_Lectura);
                Obtener_Codigos_Prefijo();
                Escribir_Archivo(path_Escritura);
                Recorrido(path_Lectura, path_Escritura);

            }
            public static NodoHuff Unir_Nodos(NodoHuff Mayor, NodoHuff Menor)
            {
                NodoHuff Padre = new NodoHuff(Mayor.Probabilidad + Menor.Probabilidad);
                Padre.Izquierda = Mayor;
                Padre.Derecha = Menor;
                return Padre;
            }
            static void ArbolHuffman(string path)
            {


                using (var File = new FileStream(path, FileMode.Open))
                {
                    var buffer = new byte[bufferLenght];
                    using (var reader = new BinaryReader(File, System.Text.Encoding.ASCII))
                    {
                        Cantidad_Datos = reader.BaseStream.Length;
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            buffer = reader.ReadBytes(bufferLenght);
                            foreach (var item in buffer)
                            {
                                if (Tabla_Frecuencias.Keys.Contains(item))
                                {
                                    Tabla_Frecuencias[item]++;
                                }

                                else Tabla_Frecuencias.Add((item), 1);

                            }
                        }
                    }
                }
                List<NodoHuff> Lista_Frecuencias = new List<NodoHuff>();
                foreach (KeyValuePair<byte, int> Nodos in Tabla_Frecuencias)
                {
                    Lista_Frecuencias.Add(new NodoHuff(Nodos.Key, (Convert.ToDecimal(Nodos.Value) / Cantidad_Datos)));
                }

                Lista_Frecuencias = Lista_Frecuencias.OrderBy(x => x.Probabilidad).ToList();
                while (Lista_Frecuencias.Count > 1)
                {
                    if (Lista_Frecuencias.Count == 1)

                    {
                        break;
                    }
                    else
                    {
                        Lista_Frecuencias = Lista_Frecuencias.OrderBy(x => x.Probabilidad).ToList();
                        NodoHuff Union = Unir_Nodos(Lista_Frecuencias[1], Lista_Frecuencias[0]);
                        Lista_Frecuencias.RemoveRange(0, 2);
                        Lista_Frecuencias.Add(Union);
                    }
                }
                Raiz = Lista_Frecuencias[0];
            }
            private static void Codigos_Prefijo(NodoHuff Nodo, string recorrido)
            {
                if (Nodo.Hoja()) { Tabla_Caracteres.Add(Nodo.Dato, recorrido); return; }
                else
                {
                    if (Nodo.Izquierda != null) Codigos_Prefijo(Nodo.Izquierda, recorrido + "0");
                    if (Nodo.Derecha != null) Codigos_Prefijo(Nodo.Derecha, recorrido + "1");
                }
            }
            private static void Obtener_Codigos_Prefijo()
            {
                if (Raiz.Hoja()) Tabla_Caracteres.Add(Raiz.Dato, "1");
                else Codigos_Prefijo(Raiz, "");
            }
            private void Escribir_Archivo(string path)
            {
                var escritura = new byte[bufferLenght];
                separador = '|';
                if (Tabla_Caracteres.Keys.Contains(Convert.ToByte('|')))
                {
                    separador = 'ÿ';
                    if (Tabla_Caracteres.Keys.Contains(Convert.ToByte('ÿ')))
                    {
                        separador = 'ß';
                    }
                }


                using (var file = new FileStream(path, FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(file))
                    {
                        escritura = Encoding.UTF8.GetBytes(Cantidad_Datos.ToString().ToArray());
                        //writer.Write(Cantidad_Datos.ToString() + "|");
                        writer.Write(escritura);
                        writer.Write(Convert.ToByte(separador));
                        //Escribe el caracter junto con su Frecuencia
                        foreach (KeyValuePair<byte, int> Valores in Tabla_Frecuencias)
                        {
                            writer.Write(Valores.Key);
                            escritura = Encoding.UTF8.GetBytes(Valores.Value.ToString().ToArray());//+ Valores.Value.ToString() + "|");
                            writer.Write(escritura);
                            writer.Write(Convert.ToByte(separador));
                        }
                        writer.Write(Convert.ToByte(separador));
                    }


                }

            }
            public static void Escribir_Bytes(string path, byte[] bytes)
            {


                using (var writer = new FileStream(path, FileMode.Append))
                {

                    writer.Write(bytes, 0, bytes.Length);
                }
            }
            private static void Recorrido(string path_Lectura, string path_Escritura)
            {
                string recorrido = "";
                using (var writer = new FileStream(path_Escritura, FileMode.Append))
                {
                    using (var File = new FileStream(path_Lectura, FileMode.Open))
                    {

                        var buffer = new byte[bufferLenght];
                        var Bytes = new List<byte>();
                        using (var reader = new BinaryReader(File))
                        {

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                buffer = reader.ReadBytes(bufferLenght);
                                //Lee el archivo letra por letra
                                foreach (var item in buffer)
                                {
                                    recorrido += Tabla_Caracteres[item];
                                    if (recorrido.Length >= 8)
                                    {
                                        while (recorrido.Length > 8)
                                        {
                                            Bytes.Add(Convert.ToByte(recorrido.Substring(0, 8), 2));
                                            //Junta el codigo prefico en grupos de 8 en 8
                                            recorrido = recorrido.Remove(0, 8);
                                        }
                                    }
                                }
                                writer.Write(Bytes.ToArray(), 0, Bytes.ToArray().Length);
                                Bytes.Clear();

                                //Escribe la lista de Bytes y se imprimen como ascci

                            }
                            for (int i = recorrido.Length; i < 8; i++)
                            {
                                recorrido += "0";
                            }
                            Bytes.Add(Convert.ToByte(recorrido, 2));
                            writer.Write(Bytes.ToArray(), 0, Bytes.ToArray().Length);

                        }
                    }
                }

            }
        }

		public class DescomprimirHuff
        {
            public const int bufferLenght = 500;
            private static NodoHuff Raiz { get; set; }
            private static Dictionary<string, byte> Tabla_Caracteres { get; set; }
            private static Dictionary<byte, int> Tabla_Frecuencias { get; set; }
            public static int Tamaño_Datos { get; set; }
            public static decimal Cantidad_Datos;
            private static char separa = new char();

            public void Descompresion(string path_Lectura, string path_Escritura)
            {
                Tabla_Frecuencias = new Dictionary<byte, int>();
                Tabla_Caracteres = new Dictionary<string, byte>();
                ArbolHuffman(path_Lectura);
                Obtener_Codigos_Prefijo();

                Recorrido(path_Lectura, path_Escritura);


            }
            static void ArbolHuffman(string path)
            {


                using (var File = new FileStream(path, FileMode.Open))
                {
                    int separador = 0;
                    var buffer = new byte[bufferLenght];
                    string cantidad_datos = "";
                    string frecuencia = "";
                    string caracter = "";
                    byte bit = new byte();
                    int final = 0;
                    using (var reader = new BinaryReader(File))
                    {
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            buffer = reader.ReadBytes(bufferLenght);
                            foreach (var item in buffer)
                            {

                                if (separador == 0)
                                {
                                    if (Convert.ToChar(item) == '|' || Convert.ToChar(item) == 'ÿ' || Convert.ToChar(item) == 'ß')
                                    {
                                        separador = 1;
                                        if (Convert.ToChar(item) == '|') { separa = '|'; }
                                        else if (Convert.ToChar(item) == 'ÿ') { separa = 'ÿ'; }
                                        else { separa = 'ß'; }
                                    }
                                    else
                                    {
                                        cantidad_datos += Convert.ToChar(item).ToString();
                                    }
                                }
                                else if (separador == 2)
                                {
                                    break;
                                }
                                else
                                {
                                    if (final == 1 && Convert.ToChar(item) == separa)
                                    {
                                        final = 2;
                                        separador = 2;
                                    }
                                    else { final = 0; }

                                    if (caracter == "") { caracter = Convert.ToChar(item).ToString(); bit = item; }
                                    else if (Convert.ToChar(item) == separa && final == 0)
                                    {
                                        Tabla_Frecuencias.Add(bit, Convert.ToInt32(frecuencia));
                                        caracter = "";
                                        frecuencia = "";
                                        final = 1;
                                    }
                                    else { frecuencia += Convert.ToChar(item).ToString(); }
                                }

                            }
                        }
                    }
                    Cantidad_Datos = Convert.ToDecimal(cantidad_datos);
                }
                List<NodoHuff> Lista_Frecuencias = new List<NodoHuff>();
                foreach (KeyValuePair<byte, int> Nodos in Tabla_Frecuencias)
                {
                    Lista_Frecuencias.Add(new NodoHuff(Nodos.Key, (Convert.ToDecimal(Nodos.Value) / Cantidad_Datos)));
                }

                Lista_Frecuencias = Lista_Frecuencias.OrderBy(x => x.Probabilidad).ToList();
                while (Lista_Frecuencias.Count > 1)
                {

                    Lista_Frecuencias = Lista_Frecuencias.OrderBy(x => x.Probabilidad).ToList();
                    NodoHuff Union = Unir_Nodos(Lista_Frecuencias[1], Lista_Frecuencias[0]);
                    Lista_Frecuencias.RemoveRange(0, 2);
                    Lista_Frecuencias.Add(Union);
                }
                Raiz = Lista_Frecuencias[0];
            }
            private static void Codigos_Prefijo(NodoHuff Nodo, string recorrido)
            {
                if (Nodo.Hoja()) { Tabla_Caracteres.Add(recorrido, Nodo.Dato); return; }
                else
                {
                    if (Nodo.Izquierda != null) Codigos_Prefijo(Nodo.Izquierda, recorrido + "0");
                    if (Nodo.Derecha != null) Codigos_Prefijo(Nodo.Derecha, recorrido + "1");
                }
            }
            public static NodoHuff Unir_Nodos(NodoHuff Mayor, NodoHuff Menor)
            {
                NodoHuff Padre = new NodoHuff(Mayor.Probabilidad + Menor.Probabilidad);
                Padre.Izquierda = Mayor;
                Padre.Derecha = Menor;
                return Padre;
            }
            private static void Obtener_Codigos_Prefijo()
            {
                if (Raiz.Hoja()) Tabla_Caracteres.Add("1", Raiz.Dato);
                else Codigos_Prefijo(Raiz, "");
            }
            private static void Recorrido(string path_Lectura, string path_Escritura)
            {
                int caracteres_escritos = 0;
                int i = 0;
                string validacion = "";
                int inicio = 0;
                string recorrido = "";
                List<byte> caracteres = new List<byte>();
                using (var file = new FileStream(path_Escritura, FileMode.OpenOrCreate))
                {
                    using (var writer = new BinaryWriter(file))
                    {

                        using (var File = new FileStream(path_Lectura, FileMode.Open))
                        {

                            var buffer = new byte[bufferLenght];

                            using (var reader = new BinaryReader(File))
                            {

                                while (reader.BaseStream.Position != reader.BaseStream.Length && caracteres_escritos < Cantidad_Datos)
                                {
                                    buffer = reader.ReadBytes(bufferLenght);
                                    foreach (var item in buffer)
                                    {

                                        if (inicio == 0 && Convert.ToChar(item) == separa) { inicio = 1; }
                                        else if (inicio == 1 && Convert.ToChar(item) == separa) { inicio = 2; }
                                        else if (inicio == 2)
                                        {
                                            //Inicia descomprecion
                                            var bits = Convert.ToString(item, 2);
                                            var completo = bits.PadLeft(8, '0');
                                            recorrido += completo;
                                            var comparacion = recorrido.ToCharArray();
                                            i = 0;
                                            while (i < recorrido.Length)
                                            {
                                                validacion += comparacion[i];
                                                i++;
                                                if (Tabla_Caracteres.Keys.Contains(validacion))
                                                {
                                                    caracteres_escritos++;
                                                    i = 0;
                                                    if (caracteres_escritos <= Convert.ToInt32(Cantidad_Datos))
                                                    {
                                                        caracteres.Add(Tabla_Caracteres[validacion]);

                                                        recorrido = recorrido.Remove(0, validacion.Length);
                                                        comparacion = recorrido.ToCharArray();
                                                        validacion = "";
                                                    }
                                                }
                                            }
                                            validacion = "";

                                        }
                                        else { inicio = 0; }
                                    }

                                    writer.Write(caracteres.ToArray());
                                    caracteres.Clear();


                                }
                            }
                        }
                    }
                }
            }

        }

		public int Hash(string line)
        {
            var bytes = Encoding.UTF8.GetBytes(line);
            int number = 0;
            foreach (var item in bytes)
            {
                number += item;

            }
            number = number * 4;

            return number % 1023;

        }
    }
}
