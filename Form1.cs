using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Escritorio
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        private string ruta, ultima;
        private bool borra = false;

        public Form1()
        {
            bool todos = true;

            InitializeComponent();

            iniciaRuta();

            ultima = eligeImagen(File.Exists("recientes.txt"));
            cambiaFondo(ultima);
            anade();

            Environment.Exit(0);
            
        }

        private void cambiaFondo(string nombre)
        {
            string wallpaper = ruta + "\\" + nombre;

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaper, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        private String eligeImagen(bool recientes)
        {
            List<string> yapuestos;
            List<string> elegibles = new List<String>();
            Random aleatorio = new Random();
            string[] files;
            int elegido = 1;
            String nombre = "";

            if (recientes)
            {
                string[] lines = File.ReadAllLines("recientes.txt");
                yapuestos = new List<string>(lines);

                try
                {
                    // Obtener todos los archivos del directorio
                    files = Directory.GetFiles(ruta);
                    List<string> archivos = new List<string>(files);

                    foreach (string file in archivos)
                    {
                        String nombreArchivo = file.Substring(ruta.Length + 1, file.Length - ruta.Length - 1);

                        if (!comprueba(nombreArchivo, yapuestos))
                            elegibles.Add(file);

                    }

                    if (elegibles.Count > 1)
                    {
                        elegido = aleatorio.Next(0, elegibles.Count - 1);
                        nombre = Path.GetFileName(archivos[elegido]);
                    }
                    else
                    {
                        nombre = Path.GetFileName(elegibles[0]);
                        borra = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            } else
            {
                files = Directory.GetFiles(ruta);
                elegido = aleatorio.Next(0, files.Length - 1);
                nombre = Path.GetFileName(files[elegido]);
            }

            return nombre;
        }

        private bool comprueba(string nombreArchivo, List<string> yapuestos)
        {
            foreach (string puesto in yapuestos)
            {
                if (nombreArchivo.Equals(puesto))
                    return true;
            }
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogo = folderBrowserDialog1.ShowDialog(this);

            ruta = folderBrowserDialog1.SelectedPath;

            graba();
        }

        private void graba() {
            FileStream fichero = new FileStream("ruta.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter fs = new StreamWriter(fichero);

            fs.WriteLine(ruta);

            fs.Close();
            fichero.Close();
        }

        private void iniciaRuta()
        {
            FileStream fichero = new FileStream("ruta.txt", FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(fichero);

            ruta = sr.ReadLine();

            sr.Close();
            fichero.Close();
        }

        private void anade()
        {
            if (!borra)
                File.AppendAllText("recientes.txt", ultima + Environment.NewLine);
            else
            {
                FileStream fichero = new FileStream("recientes.txt", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter fs = new StreamWriter(fichero);

                fs.Close();
                fichero.Close();
            }
        }
    }
}
