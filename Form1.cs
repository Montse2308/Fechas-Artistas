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
using System.Data.SqlTypes;

namespace FechasArtistas
{
    public partial class Form1 : Form
    {
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
            var eventDates = GetArtistEvents(artista);

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
            
        
        private List<string> GetArtistEvents(string artista)
        {
            List<string> eventDates = new List<string>();
            int page = 1;

            while (true)
            {
                string pageUrl = $"https://www.setlist.fm/search?page={page}&query={artista.Replace(" ", "+")}";
                List<string> pageEventDates = GetEventsOnPage(pageUrl);

                if (pageEventDates.Count == 0)
                {
                    break;  // No hay más páginas
                }

                eventDates.AddRange(pageEventDates);
                page++;
            }

            return eventDates;
        }

        private int GetTotalPages(string url, string artista)
        {
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;

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
                    string pattern = $@"<a href=""search\?page=(\d+)&amp;query={artista.Replace(" ", "+")}"">(\d+)</a>";
                    MatchCollection matches = Regex.Matches(html, pattern);

                    int totalPages = 1;

                    foreach (Match match in matches)
                    {
                        int currentPage = int.Parse(match.Groups[1].Value);
                        if (currentPage > totalPages)
                        {
                            totalPages = currentPage;
                        }
                    }

                    return totalPages;
                }
            }
        }

        private List<string> GetEventsOnPage(string url)
        {
            List<string> eventDates = new List<string>();

            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;

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

            return eventDates;
        }
    

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
