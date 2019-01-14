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

namespace WindowsFormsApp1
{
    public partial class DeepDarkFantasy : Form
    {
        public DeepDarkFantasy()
        {
            InitializeComponent();
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
        #endregion

        #region tab2功能
        HttpWebRequest httpReq;
        HttpWebResponse httpResp;

        string strBuff = "";
        char[] cbuffer = new char[256];
        int byteRead = 0;

        //string filename = @"c:\log.txt";
        ///定义写入流操作 
        public string WriteStream(string url)
        {
            Uri httpURL = new Uri(url);

            ///HttpWebRequest类继承于WebRequest，并没有自己的构造函数，需通过WebRequest的Creat方法 建立，并进行强制的类型转换 
            httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
            ///通过HttpWebRequest的GetResponse()方法建立HttpWebResponse,强制类型转换

            httpResp = (HttpWebResponse)httpReq.GetResponse();
            ///GetResponseStream()方法获取HTTP响应的数据流,并尝试取得URL中所指定的网页内容

            ///若成功取得网页的内容，则以System.IO.Stream形式返回，若失败则产生ProtoclViolationException错 误。在此正确的做法应将以下的代码放到一个try块中处理。这里简单处理 
            Stream respStream = httpResp.GetResponseStream();

            ///返回的内容是Stream形式的，所以可以利用StreamReader类获取GetResponseStream的内容，并以

            ///StreamReader类的Read方法依次读取网页源程序代码每一行的内容，直至行尾（读取的编码格式：UTF8） 
            StreamReader respStreamReader = new StreamReader(respStream, Encoding.Default);

            byteRead = respStreamReader.Read(cbuffer, 0, 256);

            while (byteRead != 0)
            {
                string strResp = new string(cbuffer, 0, byteRead);
                strBuff = strBuff + strResp;
                byteRead = respStreamReader.Read(cbuffer, 0, 256);
            }

            respStream.Close();
            return strBuff;
        }

        //解析html
        private string s1 = "//table[@class='provincetable']/tr[@class='provincetr']/td/a";
        private string s2 = "//table[@class='citytable']/tr[@class='citytr']/td/a";
        private string s3 = "//table[@class='countytable']/tr[@class='countytr']/td/a";
        private string s4 = "//table[@class='towntable']/tr[@class='towntr']/td/a";
        private string htmlResult = "";
        public string AnalyzeHtml(string url, string selectString, int tag)
        {
            var html = @"http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2017/" + url;
            HtmlWeb web = new HtmlWeb
            {
                OverrideEncoding = Encoding.GetEncoding("gb2312")
            };
            var htmlDoc = web.Load(html);
            var htmlNodes = htmlDoc.DocumentNode.SelectNodes(selectString);
            string nextUrl = "";
            string code = "";
            string name = "";
            try
            {
                foreach (var node in htmlNodes)
                {
                    nextUrl = string.IsNullOrEmpty(node.Attributes["href"].Value) ? "" : node.Attributes["href"].Value;
                    name = node.InnerHtml;
                    htmlResult += htmlNodes.IndexOf(node) + " " + code + " " + name + "\r\n";
                    if (tag < 1)
                    {
                        AnalyzeHtml(nextUrl, s2, ++tag);
                    }
                }
            }
            catch (Exception)
            {
                htmlResult = "参数错误";
            }
            return htmlResult;
        }

        private void doAnalyze_Click(object sender, EventArgs e)
        {
            var s1 = "//table[@class='provincetable']/tr[@class='provincetr']/td/a";
            var s2 = "//table[@class='citytable']/tr[@class='citytr']/td/a";
            var s3 = "//table[@class='countytable']/tr[@class='countytr']/td/a";
            var s4 = "//table[@class='towntable']/tr[@class='towntr']/td/a";
            boxHtml.Text = "";
            boxHtml.Text = AnalyzeHtml(boxUrl.Text, s1, 0);
        }
        #endregion

    }
}
