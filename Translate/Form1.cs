using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using GenerativeAI.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Translate
{
    public partial class Form1 : Form
    {
        private ChatBot chatBot;
        private TranExcelFile tranExcelFile;
        private List<string> areaTranslate=new List<string>();
        public Form1()
        {
            InitializeComponent();

            chatBot = new ChatBot();
            tranExcelFile = new TranExcelFile();
        }

        private async void btnTranslate_Click(object sender, EventArgs e)
        {
            rtbResult.Text=string.Empty;
            lbTranslate.Text = string.Empty;
            //areaTranslate.Add("B7:AA36");
            await tranExcelFile.Translate(
                textBox1.Text,
                areaTranslate,
                rtbResult
                );
           
            lbTranslate.Text = "完了";
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "ファイル選択";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                textBox1.Text = filePath;
            }
        }
    }
}
