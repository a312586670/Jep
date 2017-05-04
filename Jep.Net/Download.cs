/*
 * 创建人:@谢华良
 * 创建时间:2013年4月12日
 * 目标:下载类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Web;
using System.Threading;
using System.Net;

namespace Jep.Net
{
    /// <summary>
    /// 进度条改变时触发的事件
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DownProgressEventHandler(Download sender);//进度条委托
    /// <summary>
    /// 下载类
    /// </summary>
    public class Download
    {
        private DownProgressEventHandler _Progress;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="fileName">保存的文件名</param>
        public Download(string url,string fileName)
        {
            this.Url = url;
            this.Filename = fileName;

            //HttpWebRequest Myrq = (HttpWebRequest)HttpWebRequest.Create(this.Url);
            //HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
            //this.ContentLength = myrp.ContentLength/(1024);
        }

        #region 属性
        private Thread _download=null;//下载线程

         /// <summary>
         /// 下载资源位置
         /// </summary>
         public string Url { get; set; }
 
         /// <summary>
         /// 资源保存路径
         /// </summary>
         public string Filename { get; set; }

         /// <summary>
         /// 资源长度
         /// </summary>
         public long ContentLength { get; private set; }
 
         /// <summary>
         /// 资源已下载长度
         /// </summary>
         public long FinishedLength { get; private set; }
 
         /// <summary>
         /// 下载百分比
         /// </summary>
         public int FinishedRate { get; private set; }

        /// <summary>
        /// 下载速度
        /// </summary>
         public double Spead { get; private set; }
        #endregion

        #region 事件
         public event DownProgressEventHandler Progress
         {
            add {
                this._Progress += value;
            }
            remove {
                this._Progress -= value;
            }
        }
        #endregion

        /// <summary>
        /// 进度条方法
        /// </summary>
        protected void OnProgress()
        {
            // 进度事件处理
            if (this._Progress != null)
            {
                lock (this._Progress)
                {
                    this._Progress.Invoke(this);
                }
            }
        }

        #region =文件下载=
        /// <summary>          
        /// 下载文件          
        /// </summary>               
        /// <returns></returns>
        private void DownloadFile()
        {
            try
            {
                HttpWebRequest Myrq = (HttpWebRequest)HttpWebRequest.Create(this.Url);
                HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
                this.ContentLength = myrp.ContentLength;
                long totalBytes = myrp.ContentLength;
                if (myrp.ContentLength != 0)
                {
                    System.IO.Stream st = myrp.GetResponseStream();
                    System.IO.Stream so = new System.IO.FileStream(this.Filename, FileMode.Create);
                    long totalDownloadedByte = 0;
                    byte[] by = new byte[1024];

                    TimeSpan tsBegin;//时间差
                    DateTime dtBegin = DateTime.Now;//开始时间

                    int osize = st.Read(by, 0, (int)by.Length);
                    while (osize > 0)
                    {
                      
                        totalDownloadedByte = osize + totalDownloadedByte;
                        this.FinishedLength = totalDownloadedByte;
                        this.FinishedRate =(int)((this.FinishedLength*100)/ (this.ContentLength));//百分比
                        so.Write(by, 0, osize);
                        osize = st.Read(by, 0,(int)by.Length);
                        DateTime dtEnd = DateTime.Now;//结束时间
                        tsBegin = dtEnd - dtBegin;//时间差

                        this.Spead = totalDownloadedByte/1024/tsBegin.TotalSeconds;
                        this.OnProgress();
                    }
                    so.Close();
                    st.Close();
                }
                else
                {
                    throw new Exception("文件不存在");
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        /// <summary>
        /// 下载进程开始
        /// </summary>
        public void Start()
        { 
            _download=new Thread(new ThreadStart(this.DownloadFile));
            _download.Start();
        }
    }
}
