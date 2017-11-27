using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace HttpClientWinForm
{
    public partial class frmDefault : Form
    {
        public string RequestMethod;

        public frmDefault()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!CheckInput())
            {
                return;
            }
            if (RequestMethod.Equals("GET"))
            {
                HttpGet();
            }
            else if (RequestMethod.Equals("POST"))
            {
                HttpPost();
            }
            else
            {
                MessageBox.Show("抱歉，系统发生不可控的异常！可能是由于本程序主线程被外部软件恶意注入，现为数据安全考虑，将自动退出。", "侦测到非法操作", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 检查用户输入
        /// </summary>
        /// <returns>检查结果（布尔值）</returns>
        bool CheckInput()
        {
            if (textBoxAddress.Text.Trim().Equals(""))
            {
                MessageBox.Show("抱歉，应用目标URL不能为空！请补全相应区域表单后再次尝试。","无效操作",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                textBoxAddress.Focus();
                return false;
            }
            else if (!radioButtonGet.Checked && !radioButtonPost.Checked)
            {
                MessageBox.Show("抱歉，您必须选择一个有效的请求方式！请补全相应区域表单后再次尝试。", "无效操作", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                if (radioButtonGet.Checked)
                {
                    RequestMethod = "GET";
                }
                else if (radioButtonPost.Checked)
                {
                    RequestMethod = "POST";
                }
                else
                {
                    RequestMethod = "GET";
                }
                return true;
            }
        }

        /// <summary>
        /// HttpClient-Post方法
        /// </summary>
        void HttpPost()
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(textBoxAddress.Text.Trim());
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = Encoding.UTF8.GetByteCount(textBoxQueryString.Text.Trim());
                req.KeepAlive = false;
                req.ProtocolVersion = HttpVersion.Version11;
                using(StreamWriter sw = new StreamWriter(req.GetRequestStream()))
                {
                    sw.Write(textBoxQueryString.Text.Trim());
                    sw.Close();
                }
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                string encoding = res.ContentEncoding;
                if(encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8";
                }
                StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = sr.ReadToEnd();
                sr.Close();
                textBoxOutputArea.Text = retString;
            }
            catch (Exception)
            {
                MessageBox.Show("抱歉，网络请求失败！请检查您的URL键入是否正确、本机网络状态是否正常。", "网络响应超时", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        /// <summary>
        /// HttpClient-Get方法
        /// </summary>
        void HttpGet()
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(textBoxAddress.Text.Trim());
                req.Method = "GET";
                req.ContentType = "text/plain;charset=utf-8";
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Stream mrs = res.GetResponseStream();
                StreamReader msr = new StreamReader(mrs, Encoding.UTF8);
                string retString = msr.ReadToEnd();
                msr.Close();
                mrs.Close();
                textBoxOutputArea.Text = retString;
            }
            catch (Exception)
            {
                MessageBox.Show("抱歉，网络请求失败！请检查您的URL键入是否正确、本机网络状态是否正常。", "网络响应超时", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        /// <summary>
        /// 窗口关闭监控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmDefault_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (textBoxOutputArea.Text != "")
            {
                DialogResult dr = MessageBox.Show("Hi，又见面了！我们监测到您的工作区存在某些执行结果，继续退出将会丢失这些内容。确认继续关闭？", "防误退提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = false;
                }
            }
            
        }

        /// <summary>
        /// 窗口尺寸修改事件监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmDefault_ResizeEnd(object sender, EventArgs e)
        {
            if(this.Width <= 530)
            {
                this.Width = 530;
            }
            if(this.Height <= 427)
            {
                this.Height = 427;
            }
        }

        private void btnClearScreen_Click(object sender, EventArgs e)
        {
            textBoxOutputArea.Text = "";
        }
    }
}
