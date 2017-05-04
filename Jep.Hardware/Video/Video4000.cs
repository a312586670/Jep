/*Enowit科技有限公司
 * 创建人:@谢华良
 * 创建时间:2013年4月16日
 * 目标:VC4000视频采集卡帮助类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

namespace Jep.Hardware.Video
{
    /// <summary>
    /// VC4000视频采集帮助类，要使用该帮助类的全部功能首先要加入Mix.dll,CapturePicture.dll,MediaTransmit.dll,Sa7134Capture.dll
    /// wmv9vcm.dll,xvidcore.dll类库
    /// </summary>
    public class Video4000
    {
        /// <summary>
        /// 显示类型
        /// </summary>
        public enum DisPlayTranStype
        {
            NOT_DISPLAY = 0,
            /// <summary>
            /// VGA的OVERLAY显示模式  
            /// </summary>
            PCI_VIEDOMEMORY = 1,
            /// <summary>
            /// PCI_E的OVERLAY显示模式
            /// </summary>
            PCI_MEMORY_VIDEOMEMORY = 2,
            /// <summary>
            /// OFFSCREEN表面显示模式
            /// </summary>
            PCI_OFFSCREEN_VIDEOMEMORY = 3
        }

        public enum eFieldFrequency
        {
            FIELD_FREQ_50HZ = 0,
            FIELD_FREQ_60HZ = 1,
            FIELD_FREQ_0HZ = 2
        }

        public enum CAPMODEL
        {
            /// <summary>
            /// 捕获无效
            /// </summary>
            CAP_NULL_STREAM = 0,	
            /// <summary>
            /// 原始流回调
            /// </summary>
            CAP_ORIGIN_STREAM = 1,
            /// <summary>
            /// Mpeg4流
            /// </summary>
            CAP_MPEG4_STREAM = 2,
            /// <summary>
            /// 
            /// </summary>
            CAP_MPEG4_XVID_STREAM = 3,
            CAP_ORIGIN_MPEG4_STREAM = 4,
            CAP_ORIGIN_XVID_STREAM = 5,
            CAP_WMV9_STREAM = 6,
            CAP_ORIGIN_WMV9_STREAM = 7
        }

        /// <summary>
        /// MP4MODEL
        /// </summary>
        public enum MP4MODEL
        {
            /// <summary>
            /// 存为MPEG文件
            /// </summary>
            MPEG4_AVIFILE_ONLY = 0,			//存为MPEG文件
            /// <summary>
            /// MPEG数据回调
            /// </summary>
            MPEG4_CALLBACK_ONLY = 1,		//MPEG数据回调
            /// <summary>
            /// 存为MPEG文件并回调
            /// </summary>
            MPEG4_AVIFILE_CALLBACK = 2		//存为MPEG文件并回调
        }

        /// <summary>
        /// 压缩模型
        /// </summary>
        public enum COMPRESSMODE
        {
            XVID_CBR_MODE = 0,
            XVID_VBR_MODE = 1
        }

        /// <summary>
        /// 视频色调
        /// </summary>
        public enum COLORCONTROL
        {
            /// <summary>
            /// 亮度
            /// </summary>
            BRIGHTNESS = 0,                //@emem control for brightness
            /// <summary>
            /// 对比度
            /// </summary>
            CONTRAST = 1,                   //@emem control for contrast
            /// <summary>
            /// 饱和度
            /// </summary>
            SATURATION = 2,                 //@emem control for saturation
            /// <summary>
            /// 色调
            /// </summary>
            HUE = 3,                        //@emem control for hue
            /// <summary>
            /// 清晰度
            /// </summary>
            SHARPNESS = 4                  //@emem control for sharpness
        }

        // init sdk
        /// <summary>
        /// 流回调
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="pBuffer"></param>
        /// <param name="dwSize"></param>
        public delegate void PrcVidCapCallBack(int dwCard, IntPtr pBuffer, int dwSize);
        //motion detection callback
        public delegate void PrcCbMotionDetect(int dwCard, bool bMove, byte[] pbuff, int dwSize, IntPtr lpContext);
        //mpg stream callback
        public delegate void PrcVidMpegCallBack(int dwCard, bool bMove, byte[] pbuff, int dwSize, bool isKeyFrm);

        //copy memory
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern void MoveMemory(IntPtr dest, IntPtr src, int size);

        #region 写入.ini为后缀的文件
        //write ini file
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        /// <summary>
        /// 写入配置文件.ini格式
        /// </summary>
        /// <param name="section">section节点</param>
        /// <param name="key">key键</param>
        /// <param name="val">key键的值</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static long WriteInitializationFile(string section, string key, string val, string filePath)
        {
            return WritePrivateProfileString(section, key, val, filePath);
        }
        #endregion

        #region 读取.ini为后缀的文件
        //read ini file
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                                                             string key, string def,
                                                            StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 读取.ini文件指定部分的值
        /// </summary>
        /// <param name="section">section节点</param>
        /// <param name="key">key</param>
        /// <param name="filePath"></param>
        /// <returns>string</returns>
        public static string GetInitializationFile(string section, string key,string filePath)
        {
            StringBuilder temp=new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, filePath);
            return temp.ToString();
        }
        #endregion

        #region 初始化SDK
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAInitSdk", CharSet = CharSet.Ansi)]
        private static extern int VCAInitSdk(IntPtr hWndMain, DisPlayTranStype eDispTransType, bool bInitAudDev);

        /// <summary>
        /// 初始化SDK
        /// </summary>
        /// <param name="hWndMain">overlay窗口句柄，overlay窗口就是包含多路显示小窗口的大窗口,overlay窗口必须有一个，多路显示小窗口必须包含再其内部。</param>
        /// <param name="eDispTransType">显卡模式</param>
        /// <param name="bInitAudDev">是否初始化音频设备</param>
        /// <returns></returns>
        public static int InitSDK(IntPtr hWndMain, DisPlayTranStype eDispTransType, bool bInitAudDev)
        {
            return VCAInitSdk(hWndMain, eDispTransType, bInitAudDev);
        }
        #endregion

        #region 初始化视频设备
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAInitVidDev", CharSet = CharSet.Ansi)]
        private static extern bool VCAInitVidDev(DisPlayTranStype eDispTransType);

        /// <summary>
        /// 初始化视频设备，当视频不显示，只需视频录像或音频处理时、或通过CAInitSdk()函数已经初始化完成可以不初始化。
        /// </summary>
        /// <param name="eDispTransType">显卡模式</param>
        /// <returns></returns>
        public static bool InitVidDev(DisPlayTranStype eDispTransType)
        {
            return VCAInitVidDev(eDispTransType);
        }
        #endregion

        #region 释放SDK资源
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAUnInitSdk", CharSet = CharSet.Ansi)]
        private static extern void VCAUnInitSdk();

        /// <summary>
        /// 释放SDK资源
        /// </summary>
        public static void UninitSDK()
        {
            VCAUnInitSdk();
        }
        #endregion

        #region 得到设备总数
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAGetDevNum", CharSet = CharSet.Ansi)]
       
        private static extern Int32 VCAGetDevNum();

        /// <summary>
        /// 得到设备总数
        /// </summary>
        /// <returns>设备总数</returns>
        public static int GetDevNum()
        {
            return VCAGetDevNum();
        }
        #endregion

        #region 设置视频源大小
        [DllImport("Sa7134Capture.dll")]
        private static extern int VCASetVidCapSize(int dwCard, int dwWidth, int dwHeight);

        /// <summary>
        /// 设置视频源大小
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="dwWidth">视频源宽度</param>
        /// <param name="dwHeight">视频源高度</param>
        /// <returns></returns>
        public static int SetVidCapSize(int dwCard, int dwWidth, int dwHeight)
        {
            return VCASetVidCapSize(dwCard, dwWidth, dwHeight);
        }
        #endregion

        #region 得到视频源输入频率，即可得到视频源输入制式
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAGetVidFieldFrq", CharSet = CharSet.Ansi)]
        private static extern Int32 VCAGetVidFieldFrq(Int32 dwCard, ref eFieldFrequency VidSourceFieldRate);

        /// <summary>
        /// 得到视频源输入频率，即可得到视频源输入制式
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="VidSourceFieldRate">视频源输入频率</param>
        /// <returns></returns>
        public static int GetVidFieldFrq(int dwCard, ref eFieldFrequency VidSourceFieldRate)
        {
            return VCAGetVidFieldFrq(dwCard, ref VidSourceFieldRate);
        }
        #endregion

        #region 打开设备
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAOpenDevice", CharSet = CharSet.Ansi)]
      
        private static extern bool VCAOpenDevice(Int32 dwCard, IntPtr hPreviewWnd);

        /// <summary>
        /// 打开设备,hPreviewWnd为视频预览窗口的句柄，该窗口背景色用户必需设置为RGB(255,0,255)
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="hPreviewWnd">指定卡号将要视频预览的窗口句柄</param>
        /// <returns></returns>
        public static bool OpenDevice(Int32 dwCard, IntPtr hPreviewWnd)
        { 
            return VCAOpenDevice(dwCard, hPreviewWnd);
        }
        #endregion

        #region 关闭设备
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCACloseDevice", CharSet = CharSet.Ansi)]
        private static extern bool VCACloseDevice(Int32 dwCard);

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <returns></returns>
        public static bool CloseDevice(int dwCard)
        {
            return VCACloseDevice(dwCard);
        }
        #endregion

        #region 开始视频预览
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAStartVideoPreview", CharSet = CharSet.Ansi)]
      
        private static extern bool VCAStartVideoPreview(Int32 dwCard);

        /// <summary>
        /// 开始视频预览
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <returns></returns>
        public static bool StartVideoPreview(int dwCard)
        { 
            return VCAStartVideoPreview(dwCard);
        }
        #endregion

        #region 停止视频预览
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAStopVideoPreview", CharSet = CharSet.Ansi)]
       
        private static extern bool VCAStopVideoPreview(Int32 dwCard);

        /// <summary>
        /// 停止视频预览
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <returns></returns>
        public static bool StopVideoPreview(int dwCard)
        {
            return VCAStopVideoPreview(dwCard);
        }
        #endregion

        #region 更新视频预览
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAUpdateVideoPreview", CharSet = CharSet.Ansi)]
        
        private static extern bool VCAUpdateVideoPreview(Int32 dwCard, IntPtr hPreviewWnd);

        /// <summary>
        /// 更新视频预览，比如改变了显示窗口时，调用
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="hPreviewWnd">指定卡号将要视频预览的窗口句柄</param>
        /// <returns></returns>
        public static bool UpdateVideoPreview(int dwCard, IntPtr hPreviewWnd)
        {
            return VCAUpdateVideoPreview(dwCard, hPreviewWnd);
        }
        #endregion

        #region 更新窗口句柄
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAUpdateOverlayWnd", CharSet = CharSet.Ansi)]
     
        public static extern bool VCAUpdateOverlayWnd(IntPtr hOverlayWnd);

        /// <summary>
        /// 更新overlay窗口，当overlay窗口句柄改变或尺寸，位置改变时调用，overlay窗口就是包含多路显示小窗口的大窗口。overlay窗口必须有一个，多路显示小窗口必须包含再其内部
        /// </summary>
        /// <param name="hOverlayWnd">overlay窗口句柄</param>
        /// <returns></returns>
        public static bool UpdateOverlayWnd(IntPtr hOverLayWnd)
        {
            return VCAUpdateOverlayWnd(hOverLayWnd);
        }
        #endregion

        #region 保存快照数据到相应的缓冲区
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCASaveBitsToBuf", CharSet = CharSet.Ansi)]
       
        private static extern bool VCASaveBitsToBuf(Int32 dwCard, IntPtr pDestBuf, ref Int32 dwWidth, ref Int32 dwHeight);

        /// <summary>
        /// 保存快照数据到相应的缓冲区
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="pDestBuf">缓冲区指针</param>
        /// <param name="dwWidth">图像宽度</param>
        /// <param name="dwHeight">图像高度</param>
        /// <returns></returns>
        public static bool SaveBitsToBuf(int dwCard, IntPtr pDestBuf, ref int dwWidth, ref int dwHeight)
        {
            return VCASaveBitsToBuf(dwCard, pDestBuf, ref dwWidth,ref dwHeight);
        }
        #endregion

        #region 保存快照为JPEG文件
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCASaveAsJpegFile", CharSet = CharSet.Ansi)]
        
        private static extern bool VCASaveAsJpegFile(Int32 dwCard, string lpFileName, Int32 dwQuality);

        /// <summary>
        /// 保存快照为JPEG文件
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="lpFileName">文件名</param>
        /// <param name="dwQuality">压缩质量0~100</param>
        /// <returns></returns>
        public static bool SaveAsJpegFile(int dwCard, string fileName, int dwQuality)
        {
            return VCASaveAsJpegFile(dwCard, fileName, dwQuality);
        }
        #endregion

        #region 保存快照为BMP文件
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCASaveAsBmpFile", CharSet = CharSet.Ansi)]
       
        private static extern bool VCASaveAsBmpFile(Int32 dwCard, string lpFileName);

        /// <summary>
        /// 保存快照为BMP文件
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="lpFileName">文件名</param>
        /// <returns></returns>
        public static bool SaveAsBmpFile(int dwCard, string fileName)
        {
            return VCASaveAsBmpFile(dwCard, fileName);
        }
        #endregion

        #region 视频捕获
        [DllImport("Sa7134Capture.dll")]
      
        private static extern int VCAStartVideoCapture(int dwCard,CAPMODEL enCapMode,MP4MODEL enMp4Mode,string FileName);

        /// <summary>
        /// 视频捕获
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="enCapMode">视频模型</param>
        /// <param name="enMp4Mode">MP4模型</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static int StartVideoCapture(int dwCard, CAPMODEL enCapMode, MP4MODEL enMp4Mode, string fileName)
        {
            return VCAStartVideoCapture(dwCard, enCapMode, enMp4Mode, fileName);
        }
        #endregion

        #region 停止视频捕获
        [DllImport("Sa7134Capture.dll")]
       
        private static extern int VCAStopVideoCapture(int dwCard);

        /// <summary>
        /// 停止视频捕获
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <returns></returns>
        public static int StopVideoCapture(int dwCard)
        {
            return VCAStopVideoCapture(dwCard);
        }
        #endregion

        #region 暂停视频捕获
        [DllImport("Sa7134Capture.dll")]
       
        private static extern int VCAPauseCapture(int dwCard);

        /// <summary>
        /// 暂停视频捕获
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <returns></returns>
        public static int PauseCapture(int dwCard)
        {
            return VCAPauseCapture(dwCard);
        }
        #endregion

        #region 设置视频帧频率
        [DllImport("Sa7134Capture.dll")]
       
        private static extern int VCASetVidCapFrameRate(int dwCard, int dwFrameRate, bool bFrameRateReduction);

        /// <summary>
        ///设置视频帧频率
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="dwFrameRate">帧频率</param>
        /// <param name="bFrameRateReduction">帧频率是否减少</param>
        /// <returns></returns>
        public static int SetVidCapFrameRate(int dwCard, int dwFrameRate, bool bFrameRateReduction)
        {
            return VCASetVidCapFrameRate(dwCard, dwFrameRate, bFrameRateReduction);
        }
        #endregion

        #region 设置视频比特率
        [DllImport("Sa7134Capture.dll")]
        
        private static extern int VCASetBitRate(int dwCard, int dwBitRate);

        /// <summary>
        /// 设置视频比特率
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="dwBitRate">比特率</param>
        /// <returns></returns>
        public static int SetBitRate(int dwCard, int dwBitRate)
        {
            return VCASetBitRate(dwCard, dwBitRate);
        }
        #endregion

        #region 设置时间间隔
        [DllImport("Sa7134Capture.dll")]
      
        private static extern int VCASetKeyFrmInterval(int dwCard, int dwKeyFrmInterval);

        /// <summary>
        /// 设置视频比特率
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="dwKeyFrmInterval">时间间隔</param>
        /// <returns></returns>
        public static int SetKeyFrmInterval(int dwCard, int dwKeyFrmInterval)
        {
            return VCASetKeyFrmInterval(dwCard, dwKeyFrmInterval);
        }
        #endregion

        #region 设置视频量化度
        [DllImport("Sa7134Capture.dll")]
       
        private static extern int VCASetXVIDQuality(int dwCard, int dwQuantizer, int dwMotionPrecision);

        /// <summary>
        /// 设置视频量化度
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="dwQuantizer">量化值</param>
        /// <param name="dwMotionPrecision">运动精度</param>
        /// <returns></returns>
        public static int SetXVIDQuality(int dwCard, int dwQuantizer, int dwMotionPrecision)
        {
            return VCASetXVIDQuality(dwCard, dwQuantizer, dwMotionPrecision);
        }
        #endregion

        #region 设置视频压缩模式
        [DllImport("Sa7134Capture.dll")]
        
        private static extern int VCASetXVIDCompressMode(int dwCard, COMPRESSMODE enCompessMode);

        /// <summary>
        /// 设置视频压缩模式
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="enCompessMode">压缩模式</param>
        /// <returns></returns>
        public static int SetXVIDCompressMode(int dwCard, COMPRESSMODE enCompessMode)
        {
            return VCASetXVIDCompressMode(dwCard, enCompessMode);
        }
        #endregion

        #region 设置视频设备颜色
        [DllImport("Sa7134Capture.dll")]
       
        private static extern int VCASetVidDeviceColor(int dwCard, COLORCONTROL enCtlType, int dwValue);

        /// <summary>
        /// 设置视频亮度
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="enCtlType">压缩模式</param>
        /// <param name="dwValue">压缩模式</param>
        /// <returns></returns>
        public static int SetVidDeviceColor(int dwCard, COLORCONTROL enCtlType, int dwValue)
        {
            return VCASetVidDeviceColor(dwCard, enCtlType, dwValue);
        }
        #endregion

        #region 注册视频流回调函数
        [DllImport("Sa7134Capture.dll")]
        private static extern int VCARegVidCapCallBack(int dwCard, PrcVidCapCallBack ppCall);

        /// <summary>
        /// 注册视频流回调函数
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="ppCall">流回调函数</param>
        /// <returns></returns>
        public static int RegVideoCapCallBack(int dwCard, PrcVidCapCallBack ppCall)
        {
            return VCARegVidCapCallBack(dwCard, ppCall);
        }
        #endregion

        #region 获取视频大小
        [DllImport("Sa7134Capture.dll")]
        private static extern int VCAGetVidCapSize(int dwCard, ref int dwWidth, ref int dwHeight);

        
        /// <summary>
        /// 获取视频大小
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="dwWidth">视频宽度</param>
        /// <param name="dwHeight">视频高度</param>
        /// <returns></returns>
        public static int GetVidCapSize(int dwCard, ref int dwWidth, ref int dwHeight)
        {
            return VCAGetVidCapSize(dwCard,ref dwWidth,ref dwHeight);
        }
        #endregion

        #region 启用移动侦测
        /// <summary>
        /// 启用移动侦测
        /// </summary>
        /// <param name="dwCard">卡号</param>
        /// <param name="bEnaDetect">是否检测</param>
        /// <param name="pAreaMap">区域地图</param>
        /// <param name="nSizeOfMap">尺寸图大小</param>
        /// <param name="nPersistTime">时间</param>
        /// <param name="nFrameRateReduce">帧速率降低值</param>
        /// <param name="lpContext">压缩模式</param>
        /// <param name="OnObjectMove">压缩模式</param>
        /// <returns></returns>
        [DllImport("Sa7134Capture.dll")]
        public static extern int VCAEnableMotionDetect(int dwCard,bool bEnaDetect,byte[] pAreaMap,int nSizeOfMap,int nPersistTime,
                                                  int nFrameRateReduce,
                                                  IntPtr lpContext,
                                                  PrcCbMotionDetect OnObjectMove);
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AFileName"></param>
        /// <param name="pYUVBuffer"></param>
        /// <param name="pRGBBuffer"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <returns></returns>
        [DllImport("CapturePicture.dll")]
        public static extern int CaptureFileAsBmp(string AFileName,
                                                    IntPtr pYUVBuffer,
                                                    IntPtr pRGBBuffer,
                                                    int nWidth, int nHeight);
    }
}
