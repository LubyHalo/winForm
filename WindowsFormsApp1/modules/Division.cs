using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.modules
{
    class Division
    {
        #region 读入文档（未启用）
        //HttpWebRequest httpReq;
        //HttpWebResponse httpResp;

        //string strBuff = "";
        //char[] cbuffer = new char[256];
        //int byteRead = 0;

        ////string filename = @"c:\log.txt";
        /////定义写入流操作 
        //public string WriteStream(string url)
        //{
        //    Uri httpURL = new Uri(url);

        //    ///HttpWebRequest类继承于WebRequest，并没有自己的构造函数，需通过WebRequest的Creat方法 建立，并进行强制的类型转换 
        //    httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
        //    ///通过HttpWebRequest的GetResponse()方法建立HttpWebResponse,强制类型转换

        //    httpResp = (HttpWebResponse)httpReq.GetResponse();
        //    ///GetResponseStream()方法获取HTTP响应的数据流,并尝试取得URL中所指定的网页内容

        //    ///若成功取得网页的内容，则以System.IO.Stream形式返回，若失败则产生ProtoclViolationException错 误。在此正确的做法应将以下的代码放到一个try块中处理。这里简单处理 
        //    Stream respStream = httpResp.GetResponseStream();

        //    ///返回的内容是Stream形式的，所以可以利用StreamReader类获取GetResponseStream的内容，并以

        //    ///StreamReader类的Read方法依次读取网页源程序代码每一行的内容，直至行尾（读取的编码格式：UTF8） 
        //    StreamReader respStreamReader = new StreamReader(respStream, Encoding.Default);

        //    byteRead = respStreamReader.Read(cbuffer, 0, 256);

        //    while (byteRead != 0)
        //    {
        //        string strResp = new string(cbuffer, 0, byteRead);
        //        strBuff = strBuff + strResp;
        //        byteRead = respStreamReader.Read(cbuffer, 0, 256);
        //    }

        //    respStream.Close();
        //    return strBuff;
        //}
        #endregion

        #region 解析网页文档并输出
        //解析html
        private readonly List<string> param = new List<string> {
            "//table[@class='provincetable']/tr[@class='provincetr']/td/a",
            "//table[@class='citytable']/tr[@class='citytr']",
            "//table[@class='countytable']/tr[@class='countytr']",
            "//table[@class='towntable']/tr[@class='towntr']",
            "//table[@class='villagetable']/tr[@class='villagetr']"
        };
        private string htmlResult = "";

        //创建一个空表
        private DataTable dt = new DataTable();

        public DataTable InitDataTable ()
        {
            dt.Columns.Add("province", Type.GetType("System.String"));
            dt.Columns.Add("city", Type.GetType("System.String"));
            dt.Columns.Add("county", Type.GetType("System.String"));
            dt.Columns.Add("towntable", Type.GetType("System.String"));
            return dt;
        }

        public string AnalyzeHtml(string url = "index.html", int selectIndex = 0, int tag = 1)
        {
            var html = @"http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2017/" + url;
            var subUrl = "";
            if (tag > 2)
            {
                subUrl = url.Substring(0, (tag - 2) * 3);
            }
            HtmlWeb web = new HtmlWeb
            {
                OverrideEncoding = Encoding.GetEncoding("gb2312")
            };
            var htmlDoc = web.Load(html);
            var htmlNodes = htmlDoc.DocumentNode.SelectNodes(param[selectIndex]);
            if (htmlNodes == null)
            {
                for (int i = selectIndex; i < param.Count; i++)
                {
                    if (htmlDoc.DocumentNode.SelectNodes(param[i]) != null)
                    {
                        htmlNodes = htmlDoc.DocumentNode.SelectNodes(param[i]);
                        break;
                    }
                }
            }
            string nextUrl = "";
            string code = "";
            string name = "";
            //string htmlResult = "";
            //try
            //{
                if (htmlNodes != null)
                {
                    //dt.Columns.Add(tag.ToString(), typeof(string));
                    foreach (var node in htmlNodes)
                    {
                        if (tag > 1)
                        {
                            for (int i = 0; i < tag; i++)
                            {
                                htmlResult += "  ";
                            }
                            nextUrl = node.FirstChild.FirstChild.HasChildNodes ? (subUrl + node.FirstChild.FirstChild.Attributes["href"].Value) : "--------------";
                            code = node.FirstChild.FirstChild.HasChildNodes ? node.FirstChild.FirstChild.InnerHtml : node.FirstChild.InnerHtml;
                            name = node.LastChild.FirstChild.HasChildNodes ? node.LastChild.FirstChild.InnerHtml : node.LastChild.InnerHtml;
                        }
                        else
                        {
                            nextUrl = string.IsNullOrEmpty(node.Attributes["href"].Value) ? "" : node.Attributes["href"].Value;
                            name = node.InnerHtml.Substring(0, node.InnerHtml.IndexOf("<br>"));
                            htmlResult += "\r\n";
                        }
                        htmlResult += code + " " + name + "\r\n";
                        if (tag < 4 && nextUrl != "--------------")
                        {
                            AnalyzeHtml(nextUrl, tag, tag + 1);
                        }
                    }
                }
            //}
            //catch (Exception)
            //{
            //    //MessageBox.Show("参数错误");
            //    htmlResult = "参数错误";
            //}
            return htmlResult;
        }
        #endregion
    }
}
