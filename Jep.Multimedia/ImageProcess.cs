using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Jep.Multimedia
{
    /// <summary>
    /// 提供常用图片处理方法
    /// </summary>
    public class ImageProcess
    {
        private Bitmap _LDPic(Bitmap mybm, int val)
        {
            Bitmap bm = new Bitmap(mybm.Width, mybm.Height);//初始化一个记录经过处理后的图片对象
            int x, y, resultR, resultG, resultB;//x、y是循环次数，后面三个是记录红绿蓝三个值的
            Color pixel;
            for (x = 0; x < mybm.Width; x++)
            {
                for (y = 0; y < mybm.Height; y++)
                {
                    pixel = mybm.GetPixel(x, y);//获取当前像素的值
                    resultR = Math.Min(pixel.R + val, 255);//检查红色值会不会超出[0, 255]
                    resultG = Math.Min(pixel.G + val, 255);//检查绿色值会不会超出[0, 255]
                    resultB = Math.Min(pixel.B + val, 255);//检查蓝色值会不会超出[0, 255]
                    bm.SetPixel(x, y, Color.FromArgb(resultR, resultG, resultB));//绘图
                }
            }
            return bm;
        }

        /// <summary>
        /// 调整光暗
        /// </summary>
        /// <param name="mybm">原始图片</param>
        /// <param name="val">增加或减少的光暗值</param>
        public Bitmap LDPic(Bitmap mybm, int val)
        {
            return _LDPic(mybm, val);
        }

        private Bitmap _RePic(Bitmap mybm)
        {
            Bitmap bm = new Bitmap(mybm.Width, mybm.Height);//初始化一个记录处理后的图片的对象
            int x, y, resultR, resultG, resultB;
            Color pixel;
            for (x = 0; x < mybm.Width; x++)
            {
                for (y = 0; y < mybm.Height; y++)
                {
                    pixel = mybm.GetPixel(x, y);//获取当前坐标的像素值
                    resultR = 255 - pixel.R;//反红
                    resultG = 255 - pixel.G;//反绿
                    resultB = 255 - pixel.B;//反蓝
                    bm.SetPixel(x, y, Color.FromArgb(resultR, resultG, resultB));//绘图
                }
            }
            return bm;
        }

        /// <summary>
        /// 反色处理
        /// </summary>
        /// <param name="mybm">原始图片</param>
        public Bitmap RePic(Bitmap mybm)
        {
            return _RePic(mybm);
        }

        private Bitmap _Emboss(Bitmap oldBitmap)
        {
            Bitmap newBitmap = new Bitmap(oldBitmap.Width, oldBitmap.Height);
            Color color1, color2;
            for (int x = 0; x < oldBitmap.Width - 1; x++)
            {
                for (int y = 0; y < oldBitmap.Height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    color1 = oldBitmap.GetPixel(x, y);
                    color2 = oldBitmap.GetPixel(x + 1, y + 1);
                    r = Math.Abs(color1.R - color2.R + 128);
                    g = Math.Abs(color1.G - color2.G + 128);
                    b = Math.Abs(color1.B - color2.B + 128);
                    if (r > 255) r = 255;
                    if (r < 0) r = 0;
                    if (g > 255) g = 255;
                    if (g < 0) g = 0;
                    if (b > 255) b = 255;
                    if (b < 0) b = 0;
                    newBitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return newBitmap;
        }

        /// <summary>
        /// 浮雕处理
        /// </summary>
        /// <param name="oldBitmap">原始图片</param>
        public Bitmap Emboss(Bitmap oldBitmap)
        {
            return _Emboss(oldBitmap);
        }

        private Bitmap _ResizeImage(Bitmap bmp, int newW, int newH)
        {
            try
            {
                Bitmap bap = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(bap);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(bap, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bap.Width, bap.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return bap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 拉伸图片
        /// </summary>
        /// <param name="bmp">原始图片</param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        public Bitmap ResizeImage(Bitmap bmp, int newW, int newH)
        {
            return _ResizeImage(bmp, newW, newH);
        }

        private Bitmap _FilPic(Bitmap mybm)
        {
            Bitmap bm = new Bitmap(mybm.Width, mybm.Height);//初始化一个记录滤色效果的图片对象
            int x, y;
            Color pixel;

            for (x = 0; x < mybm.Width; x++)
            {
                for (y = 0; y < mybm.Height; y++)
                {
                    pixel = mybm.GetPixel(x, y);//获取当前坐标的像素值
                    bm.SetPixel(x, y, Color.FromArgb(0, pixel.G, pixel.B));//绘图
                }
            }
            return bm;
        }

        /// <summary>
        /// 滤色处理
        /// </summary>
        /// <param name="mybm">原始图片</param>
        public Bitmap FilPic(Bitmap mybm)
        {
            return _FilPic(mybm);
        }

        private Bitmap _RevPicLR(Bitmap mybm)
        {
            Bitmap bm = new Bitmap(mybm.Width, mybm.Height);
            int x, y, z; //x,y是循环次数,z是用来记录像素点的x坐标的变化的
            Color pixel;
            for (y = mybm.Height - 1; y >= 0; y--)
            {
                for (x = mybm.Width - 1, z = 0; x >= 0; x--)
                {
                    pixel = mybm.GetPixel(x, y);//获取当前像素的值
                    bm.SetPixel(z++, y, Color.FromArgb(pixel.R, pixel.G, pixel.B));//绘图
                }
            }
            return bm;
        }

        /// <summary>
        /// 左右翻转
        /// </summary>
        /// <param name="mybm">原始图片</param>
        public Bitmap RevPicLR(Bitmap mybm)
        {
            return _RevPicLR(mybm);
        }

        private Bitmap _RevPicUD(Bitmap mybm)
        {
            Bitmap bm = new Bitmap(mybm.Width, mybm.Height);
            int x, y, z;
            Color pixel;
            for (x = 0; x < mybm.Width; x++)
            {
                for (y = mybm.Height - 1, z = 0; y >= 0; y--)
                {
                    pixel = mybm.GetPixel(x, y);//获取当前像素的值
                    bm.SetPixel(x, z++, Color.FromArgb(pixel.R, pixel.G, pixel.B));//绘图
                }
            }
            return bm;
        }

        /// <summary>
        /// 上下翻转
        /// </summary>
        /// <param name="mybm">原始图片</param>
        public Bitmap RevPicUD(Bitmap mybm)
        {
            return _RevPicUD(mybm);
        }

        private bool _Compress(string oldfile, string newfile, Size newSize)
        {
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(oldfile);
                System.Drawing.Imaging.ImageFormat thisFormat = img.RawFormat;
                Bitmap outBmp = new Bitmap(newSize.Width, newSize.Height);
                Graphics g = Graphics.FromImage(outBmp);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, new Rectangle(0, 0, newSize.Width, newSize.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
                g.Dispose();
                EncoderParameters encoderParams = new EncoderParameters();
                long[] quality = new long[1];
                quality[0] = 100;
                EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                encoderParams.Param[0] = encoderParam;
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICI = null;
                for (int x = 0; x < arrayICI.Length; x++)
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICI = arrayICI[x]; //设置JPEG编码
                        break;
                    }
                img.Dispose();
                if (jpegICI != null) outBmp.Save(newfile, System.Drawing.Imaging.ImageFormat.Jpeg);
                outBmp.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 压缩到指定尺寸
        /// </summary>
        /// <param name="oldfile">原文件</param>
        /// <param name="newfile">新文件</param>
        /// <param name="newSize">新的尺寸</param>
        public bool Compress(string oldfile, string newfile, Size newSize)
        {
            return _Compress(oldfile, newfile, newSize);
        }

        private Color _Gray(Color c)
        {
            int rgb = Convert.ToInt32((double)(((0.3 * c.R) + (0.59 * c.G)) + (0.11 * c.B)));
            return Color.FromArgb(rgb, rgb, rgb);
        }

        /// <summary>
        /// 将指定像素点转换成灰度像素点
        /// </summary>
        /// <param name="c">需要转换的颜色</param>
        /// <returns></returns>
        public Color Gray(Color c)
        {
            return _Gray(c);
        }

        private Bitmap _BWPic(Bitmap mybm)
        {
            Bitmap bm = new Bitmap(mybm.Width, mybm.Height);
            int x, y, result; //x,y是循环次数，result是记录处理后的像素值
            Color pixel;
            for (x = 0; x < mybm.Width; x++)
            {
                for (y = 0; y < mybm.Height; y++)
                {
                    pixel = mybm.GetPixel(x, y);//获取当前坐标的像素值
                    result = (pixel.R + pixel.G + pixel.B) / 3;//取红绿蓝三色的平均值
                    bm.SetPixel(x, y, Color.FromArgb(result, result, result));
                }
            }
            return bm;
        }

        /// <summary>
        /// 转换为黑白图片
        /// </summary>
        /// <param name="mybt">要进行处理的图片</param>
        public Bitmap BWPic(Bitmap mybm)
        {
            return _BWPic(mybm);
        }

        private void _GetFrames(string pPath, string pSavedPath)
        {
            Image gif = Image.FromFile(pPath);
            string filename = Path.GetFileNameWithoutExtension(pPath);
            FrameDimension fd = new FrameDimension(gif.FrameDimensionsList[0]);
            int count = gif.GetFrameCount(fd); //获取帧数(gif图片可能包含多帧，其它格式图片一般仅一帧)
            for (int i = 0; i < count; i++)    //以Jpeg格式保存各帧
            {
                gif.SelectActiveFrame(fd, i);
                gif.Save(pSavedPath + "\\" + filename + "_frame_" + i + ".jpg", ImageFormat.Jpeg);
            }
        }

        /// <summary>
        /// 获取图片中的各帧
        /// </summary>
        /// <param name="pPath">图片路径</param>
        /// <param name="pSavePath">保存路径</param>
        public void GetFrames(string pPath, string pSavedPath)
        {
            _GetFrames(pPath, pSavedPath);
        }
    }
}
