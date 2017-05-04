/*Enowit科技有限公司
 * 创建人:谢华良
 * 创建时间:2013年4月15日
 * 目标：USB摄像头操作类
 */
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace Jep.Hardware.USB
{

    /// <summary>
    /// USB摄像头操作类
    /// </summary>
    public class USBCamera
    {
        #region 属性
        private const int WM_USER=0x400;
        private const int WS_CHILD=0x40000000;
        private const int WS_VISIBLE=0x10000000;
        private const int WM_CAP_START=WM_USER;
        private const int WM_CAP_STOP=WM_CAP_START + 68;
        private const int WM_CAP_DRIVER_CONNECT=WM_CAP_START + 10;
        private const int WM_CAP_DRIVER_DISCONNECT=WM_CAP_START + 11;
        private const int WM_CAP_SAVEDIB=WM_CAP_START + 25;
        private const int WM_CAP_GRAB_FRAME=WM_CAP_START + 60;
        private const int WM_CAP_SEQUENCE=WM_CAP_START + 62;
        private const int WM_CAP_FILE_SET_CAPTURE_FILEA=WM_CAP_START + 20;
        private const int WM_CAP_SEQUENCE_NOFILE=WM_CAP_START+ 63;
        private const int WM_CAP_SET_OVERLAY=WM_CAP_START+ 51;
        private const int WM_CAP_SET_PREVIEW=WM_CAP_START+ 50;
        private const int WM_CAP_SET_CALLBACK_VIDEOSTREAM=WM_CAP_START +6;
        private const int WM_CAP_SET_CALLBACK_ERROR=WM_CAP_START +2;
        private const int WM_CAP_SET_CALLBACK_STATUSA=WM_CAP_START +3;
        private const int WM_CAP_SET_CALLBACK_FRAME=WM_CAP_START +5;
        private const int WM_CAP_SET_SCALE=WM_CAP_START+ 53;
        private const int WM_CAP_SET_PREVIEWRATE=WM_CAP_START+ 52;
        private IntPtr hWndC;
        private bool bStat = false;

        private IntPtr mControlPtr;
        private int mWidth;//图像宽度
        private int mHeight;//图像高度
        private int mLeft;//左边距离
        private int mTop;//顶部距离
        private string _GrabImagePath="";
        private string _VideoPath="";
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化摄像头
        /// </summary>
        /// <param name="handle">控件的句柄</param>
        /// <param name="left">开始显示的左边距</param>
        /// <param name="top">开始显示的上边距</param>
        /// <param name="width">要显示的宽度</param>
        /// <param name="height">要显示的长度</param>
        public USBCamera(IntPtr handle,int left,int top,int width,int height)
        {
            mControlPtr=handle;
            mWidth=width;
            mHeight=height;
            mLeft=left;
            mTop=top;
        }
        #endregion

        #region 设置图像大小
        /// <summary>
        /// 设置图像大小
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSize(int width, int height)
        {
            this.mWidth = width;
            this.mHeight = height;
            this.Stop();
            this.Start();
        }
        #endregion

        #region "属性设置"
        /// <summary>
        /// 视频左边距
        /// </summary>
        public int Left
        {
            get {return mLeft;}
            set {mLeft=value;}
        }

        /// <summary>
        /// 视频上边距
        /// </summary>
        public int Top
        {
            get {return mTop;}
            set {mTop=value;}
        }

        /// <summary>
        /// 视频宽度
        /// </summary>
        public int Width
        {
            get {return mWidth;}
            set {mWidth=value;}
        }

        /// <summary>
        /// 视频高度
        /// </summary>
        public int Height
        {
            get {return mHeight;}
            set {mHeight=value;}
        }

        /// <summary>
        /// 抓图文件存放路径
        /// 例：d:\a.bmp
        /// </summary>
        public string grabImagePath
        {
            get {return _GrabImagePath;}
            set {_GrabImagePath=value;}
        }

        /// <summary>
        /// 录像文件存放路径
        /// 例：d:\a.avi
        /// </summary>
        public string VideoPath
        {
            get {return _VideoPath;}
            set {_VideoPath=value;}
        }
        #endregion

        #region =调用系统API=
        [DllImport("avicap32.dll")]
        private static extern IntPtr capCreateCaptureWindowA(byte[] lpszWindowName,int dwStyle,int x,int y,int nWidth,int nHeight,IntPtr hWndParent,int nID);

        [DllImport("avicap32.dll")]
        private static extern int capGetVideoFormat(IntPtr hWnd,IntPtr psVideoFormat,int wSize );
        [DllImport("User32.dll")]
        private static extern bool SendMessage(IntPtr hWnd,int wMsg,int wParam,int lParam);
        #endregion

        #region =开始加载摄像头图像=
        /// <summary>
        /// 开始显示图像
        /// </summary>
        public void Start()
        {
            if (bStat)
                return;

            bStat = true;
            byte[] lpszName=new byte[100];

            hWndC=capCreateCaptureWindowA(lpszName,WS_CHILD|WS_VISIBLE ,mLeft,mTop,mWidth,mHeight,mControlPtr,0);

            if (hWndC.ToInt32()!=0)
            {
                SendMessage(hWndC, WM_CAP_SET_CALLBACK_VIDEOSTREAM, 0, 0);
                SendMessage(hWndC, WM_CAP_SET_CALLBACK_ERROR, 0, 0);
                SendMessage(hWndC, WM_CAP_SET_CALLBACK_STATUSA, 0, 0);
                SendMessage(hWndC, WM_CAP_DRIVER_CONNECT, 0, 0);
                SendMessage(hWndC, WM_CAP_SET_SCALE, 1, 0);
                SendMessage(hWndC, WM_CAP_SET_PREVIEWRATE, 66, 0);
                SendMessage(hWndC, WM_CAP_SET_OVERLAY, 1, 0);
                SendMessage(hWndC, WM_CAP_SET_PREVIEW, 1, 0);
            }
            return;
        }
        #endregion

        #region =停止显示=
        /// <summary>
        /// 停止显示
        /// </summary>
        public void Stop()
        {
            SendMessage(hWndC,WM_CAP_DRIVER_DISCONNECT,0,0);
            bStat=false;
        }
        #endregion

        #region =抓图=
        /// <summary>
        /// 抓图
        /// </summary>
        public void GrabImage()
        {
            IntPtr hBmp = Marshal.StringToHGlobalAnsi(_GrabImagePath);
            SendMessage(hWndC,WM_CAP_SAVEDIB,0,hBmp.ToInt32());
        }

        /// <summary>
        /// 抓图
        /// </summary>
        /// <param name="fileName">要保存bmp文件的路径</param>
        public void GrabImage(string fileName)
        {
            IntPtr hBmp = Marshal.StringToHGlobalAnsi(fileName);
            SendMessage(hWndC, WM_CAP_SAVEDIB, 0, hBmp.ToInt32());    
        }
        #endregion

        #region =开始录像=
        /// <summary>
        /// 录像
        /// </summary>
        public void KinesCope()
        {
            IntPtr hBmp=Marshal.StringToHGlobalAnsi(_VideoPath);
            SendMessage(hWndC,WM_CAP_FILE_SET_CAPTURE_FILEA,0,hBmp.ToInt32());
            SendMessage(hWndC,WM_CAP_SEQUENCE,0,0);
        }

        /// <summary>
        /// 录像
        /// </summary>
        /// <param name="filePath">要保存avi文件的路径</param>
        public void KinesCope(string filePath)
        {
            IntPtr hBmp = Marshal.StringToHGlobalAnsi(filePath);
            SendMessage(hWndC, WM_CAP_FILE_SET_CAPTURE_FILEA, 0, hBmp.ToInt32());
            SendMessage(hWndC, WM_CAP_SEQUENCE, 0, 0);
        }
        #endregion

        #region =停止录像=
        /// <summary>
        /// 停止录像
        /// </summary>
        public void StopKinesCope()
        {
            SendMessage(hWndC,WM_CAP_STOP,0,0);
        }
        #endregion
    }
}


