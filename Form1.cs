using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using static System.Net.WebRequestMethods;

/**
 * HERNANDEZ GALLEGOS MONTSERRAT XIMENA
 * SOLO COMO ACLARION PROFE, DE QUE LE TENGA PACIENCIA AL CODIGO, TARDA COMO 1 MINUTO EN ARRANCAR, PERO FUNCIONA, ES CULPA DEL WONDOWS FORMS JEJE
 */

namespace FechasArtistas
{
    public partial class Form1 : Form
    {
        private int pageNumber = 1; 
        private bool hasMorePages = true; 
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void busqueda_TextChanged(object sender, EventArgs e)
        {

        }

        private void buscar_Click(object sender, EventArgs e)
        {
            
            string artista = busqueda.Text;
            string baseUrl = "http://www.setlist.fm/search?query=" + artista.Replace(" ", "+");

            List<string> eventDates = new List<string>();

            while (hasMorePages)
            {
                string searchUrl = baseUrl + "&page=" + pageNumber;
                var pageEventDates = GetArtistEvents(searchUrl);
                eventDates.AddRange(pageEventDates);

                if (pageEventDates.Count == 0)
                {
                    hasMorePages = false;
                }

                pageNumber++;
            }

            if (eventDates.Count == 0)
            {
                richTextBox1.Text = "No se encontraron eventos para ese artista.";
            }
            else
            {
                richTextBox1.Text = "Fechas de eventos para el artista " + artista + ":\r\n";
                foreach (var eventDate in eventDates)
                {
                    richTextBox1.Text += eventDate + "\r\n";
                }
            }


        }


        private List<string> GetArtistEvents(string url)
        {
            List<string> eventDates = new List<string>();

            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;

            try
            {
                

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("No se pudo obtener una respuesta. Estado = " + response.StatusCode);
                }
                using (Stream dataStream = response.GetResponseStream())
                {
                    if (dataStream == null)
                    {
                        throw new Exception("No se recibió respuesta.");
                    }

                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        string html = reader.ReadToEnd();

                        string pattern = @"<div class=""condensed dateBlock"">\s*<span class=""month"">([^<]+)</span>\s*<span class=""day"">([^<]+)</span>\s*<span class=""year"">([^<]+)</span>";
                        MatchCollection matches = Regex.Matches(html, pattern);

                        foreach (Match match in matches)
                        {
                            string month = match.Groups[1].Value;
                            string day = match.Groups[2].Value;
                            string year = match.Groups[3].Value;
                            string eventDate = $"{month} {day}, {year}";
                            eventDates.Add(eventDate);
                        }
                    }
                }
            }
            catch (WebException)
            {
                hasMorePages = false;
            }

            return eventDates;
        
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
