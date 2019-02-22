using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace ChangeDesktopWalper
{
    public partial class Form1 : Form
    {
        private string Url { get; set; }
        private RadioButton radioButton { get; set; }

        private readonly string MainUrl;

        public Form1()
        {
            InitializeComponent();

            Url = textBox1.Text;
            MainUrl = Url;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckFolder();

            HtmlWeb web = new HtmlWeb()
            {
                PreRequest = request =>
                    {
                        request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                        return true;
                    }
            };

            List<string> urlList = new List<string>();

            if (radioButton != null)
            {
                var doc = web.Load(Url);

                foreach (HtmlNode divNode1 in doc.DocumentNode.SelectNodes("//div[@class=\"wallpapers wallpapers_zoom wallpapers_main\"]"))
                {
                    foreach (HtmlNode divNode2 in divNode1.SelectNodes("//a[@class=\"wallpapers__link\"]"))
                    {
                        var str = divNode2.GetAttributeValue("href", "");

                        urlList.Add(str);
                    }
                }
            }
            else
            {
                MessageBox.Show("Chose category");
            }

            var imgList = new List<string>(CreateImgUrl(urlList));

            DownloadImage(imgList);
        }

        private void CheckFolder()
        {
            string pathName = System.IO.Path.Combine("D:\\", "Photo");

            try
            {
                if(!Directory.Exists(pathName))
                    System.IO.Directory.CreateDirectory(pathName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private async void DownloadImage(List<string> imageList)
        {
            foreach (var item in imageList)
            {
                using (WebClient wc = new WebClient())
                {
                    Uri uri = new Uri(item);

                    string fileName = System.IO.Path.Combine("D:\\", "Photo", uri.Segments.Last());

                    await wc.DownloadFileTaskAsync(uri, fileName);
                }
            }
        }

        private List<string> CreateImgUrl(List<string> urlList)
        {
            List<string> newImgList = new List<string>();

            foreach (var item in urlList)
            {
                var i = @"https://images.wallpaperscraft.ru" + item;//.Replace("wallpaperscraft.ru", "images.wallpaperscraft.ru");

                int index = i.LastIndexOf('/');

                i = i.Remove(index, 1);
                i = i.Insert(index, "_");
                newImgList.Add(i.Replace("download", "image") + ".jpg");
            }

            return newImgList;
        }

        private void radioButton_Click(object sender, EventArgs e)
        {
            Url = MainUrl;

            radioButton = sender as RadioButton;

            if (radioButton == null) return;

            if (radioButton.Text == "favorites")
            {
                Url += "60_" + radioButton.Text + "/1920x1080";

                textBox1.Text = Url;
                return;
            }

            Url += radioButton.Text + "/1920x1080";

            textBox1.Text = Url;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var path = Path.Combine("D:\\", "Photo");
            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }
    }
}
