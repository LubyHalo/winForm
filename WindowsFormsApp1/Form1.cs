using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.modules;

namespace WindowsFormsApp1
{
    public partial class DeepDarkFantasy : Form
    {
        public DeepDarkFantasy()
        {
            InitializeComponent();
            bwAnalyze = new BackgroundWorker();                      //新建BackgroundWorker
            bwAnalyze.WorkerReportsProgress = true;                  //允许报告进度
            bwAnalyze.WorkerSupportsCancellation = true;             //允许取消线程
            bwAnalyze.DoWork += bwAnalyze_DoWork;                       //设置主要工作逻辑
            bwAnalyze.ProgressChanged += bwAnalyze_ProgressChanged;     //进度变化的相关处理
            bwAnalyze.RunWorkerCompleted += bwAnalyze_RunWorkerCompleted;  //线程完成时的处理
        }
        #region tab1功能
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            var arr = richTextBox1.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in arr)
            {
                var index = item.IndexOf("（") > 0 ? item.IndexOf("（") : item.IndexOf("(");
                if (index > 0)
                {
                    richTextBox2.Text += item.Substring(index) + "\r\n";
                }
                else
                {
                    richTextBox2.Text += item + "\r\n";
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            var arr = richTextBox1.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //label1.Text = "共" + arr.Length.ToString() + "行";
            label1.Text = string.Format("共{0}行", arr.Length.ToString());
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            var arr = richTextBox2.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //label2.Text = "共" + arr.Length.ToString() + "行";
            label2.Text = string.Format("共{0}行", arr.Length.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(richTextBox2.Text);
        }

        private void indexOfBtn_Click(object sender, EventArgs e)
        {
            var xxx = richTextBox1.Text.IndexOf("<br>");
            richTextBox2.Text = richTextBox1.Text.Substring(0, xxx);
        }
        #endregion

        #region tab2功能
        Division division = new Division();

        private void doAnalyze_Click(object sender, EventArgs e)
        {
            //boxHtml.Text = "";
            //boxHtml.Text = AnalyzeHtml(boxUrl.Text, param[0], 1);
            doAnalyze.Enabled = false;
            messageBox.Text = "数据获取中。。";
            bwAnalyze.RunWorkerAsync();   //调用该方法才会启动线程
        }

        private string res = "";
        private void bwAnalyze_DoWork(object sender, DoWorkEventArgs e)
        {
            res = division.AnalyzeHtml(boxUrl.Text);
        }

        private void bwAnalyze_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressBar1.Value = e.ProgressPercentage;
        }

        private void bwAnalyze_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //MessageBox.Show("线程工作完成");
            doAnalyze.Enabled = true;
            messageBox.Text = "数据获取已完成";
            boxHtml.Text = res;
        }
        #endregion
    }
}
