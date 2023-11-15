using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using SecuGen.FDxSDKPro.Windows;

namespace FingerEnroll.Core
{
    //Step 3
    internal static class WSQHelper
    {
        internal static string DSAKey = "<DSAKeyValue><P>18hCCdNJQWg0kw4B9iwpsHw2SCgLnuk4sbcZCY8vCasHaUh6+Nzxs7MH4PcfVqQTw3ToORp4/CvIWYC9SEmwvPPL2HJUb0pOo5b1ldnyD+LQaKD5thd6AhkBdyvrATdD6tXEin9bgLIyo98Gf0uLqG922dyF4MfvMM8cbwWPmGE=</P><Q>iNZ0oAOA451JhSC35hlKU//IAmc=</Q><G>sDP6WUGLehQ+EJOFnTtBE/ubvlR8aJnV43hxI35nojc3dC5xJsW4Y+ur3Xkg/GF9+ldZ8KwxhuwPvx0Wp0aRFJib1fpMS4fMqG3Jd9QDrTZx+XE7iTSO6TPfyCLzZBsrFC4jwJewpdYKoi8KGhbMdKxhRgNWL8wwfegk2hVTRZo=</G><Y>HzSbzGwv0y9r9pxtN4TzX4GFZRYGNhSkCiy4mF7uVzxHAW+FboCDR6gITfgMp0nZF1D2eyfuc6L+Qg4Eo2/GjOUYW0F1eijUBYBJ4Z8ru3Ps0Nih5sNcuM9KChMHEFt8JY7g/46qQd4Ust69jc+g+EcifcUf74244DCZCmDLOY8=</Y><J>AAAAAZOxFsQun6/hxZqOI6F8MFDvs7mQDdu35zD+0v3ebXQ+3+p0ItdFbtKQwV9GI1dSqgAtK43ElLNM2kR77teete8/93rhX8/Y7lqLV5UjuXB7Nza3gjMv73D7+zTa+UYWeJJSh6A+TesWpaIooA==</J><Seed>/yE+ihF8NohTaIbibqgHyg/R1ow=</Seed><PgenCounter>AVA=</PgenCounter><X>SLNUSLRaeVdS6tCY+8QZc8Fxi30=</X></DSAKeyValue>";
        private static float _flot_b = 0.75f;
        private static SGFingerPrintManager _fpm;

        [DllImport("NCap.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool CheckQuality(string key, IntPtr A_1, int width, int height, out int A_4, out float A_5);

        [DllImport("WSQCodec.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool CreateWSQ(string A_0, string A_1, float A_2, string A_3);

        [DllImport("WSQCodec.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool CreateWSQ2(IntPtr A_0, int width, int height, int A_3, out IntPtr A_4, out int A_5, float A_6, string A_7);

        [DllImport("WSQCodec.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool CreateBmp(string A_0, string A_1, string A_2);

        [DllImport("WSQCodec.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool CreateBmp2(IntPtr A_0, int A_1, out IntPtr A_2, out int A_3, string A_4);

        [DllImport("WSQCodec.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool SegmentImage(IntPtr A_0, int A_1, int A_2, int A_3, int A_4, bool A_5, ref Eval_A A_6, string A_7);

        [DllImport("WSQCodec.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool FreeMem(IntPtr A_0);

        internal static byte[] GenWSQImage(byte[] imageBytes, int width, int height)
        {
            #region MyRegion
            switch (0)
            {
                default:
                    byte[] destination = (byte[])null;
                    IntPtr num1 = IntPtr.Zero;
                    IntPtr A_4 = IntPtr.Zero;
                    int A_5 = 0;
                    bool flag = false;
                    try
                    {
                        num1 = Marshal.AllocHGlobal(imageBytes.Length);
                        Marshal.Copy(imageBytes, 0, num1, imageBytes.Length);
                        flag = CreateWSQ2(num1, width, height, 16, out A_4, out A_5, _flot_b, DSAKey);
                    }
                    catch (Exception ex)
                    {
                        //
                    }

                    try
                    {
                        int num2 = 7;
                        while (true)
                        {
                            switch (num2)
                            {
                                case 0:
                                    num2 = 8;
                                    continue;
                                case 1:
                                    if (A_4 != IntPtr.Zero)
                                    {
                                        num2 = 4;
                                        continue;
                                    }
                                    goto case 0;
                                case 2:
                                    num2 = 5;
                                    continue;
                                case 3:
                                    Marshal.FreeHGlobal(num1);
                                    num2 = 6;
                                    continue;
                                case 4:
                                    destination = new byte[A_5];
                                    Marshal.Copy(A_4, destination, 0, A_5);
                                    FreeMem(A_4);
                                    num2 = 0;
                                    continue;
                                case 5:
                                    if (num1 != IntPtr.Zero)
                                    {
                                        num2 = 3;
                                        continue;
                                    }
                                    goto case 6;
                                case 6:
                                    num2 = 1;
                                    continue;
                                case 8:
                                    goto label_18;
                                default:
                                    if (flag)
                                    {
                                        num2 = 2;
                                        continue;
                                    }
                                    goto case 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //
                    }

                label_18:
                    return destination;
            }
            #endregion
        }

        internal static Bitmap ByteToBitmap(byte[] sourceBytes, int width, int height)
        {
            switch (0)
            {
                default:
                label_2:
                    byte[] source = new byte[sourceBytes.Length * 3];
                    int index = 0;
                    int num1 = 7;
                    BitmapData bitmapdata = new BitmapData();
                    int num2 = 0;
                    Bitmap bitmap = null;
                    while (true)
                    {
                        switch (num1)
                        {
                            case 1:
                                bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                                bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                num2 = 0;
                                num1 = 4;
                                continue;
                            case 4:
                                if (num2 <= bitmap.Height - 1)
                                {
                                    IntPtr destination = new IntPtr(bitmapdata.Scan0.ToInt64() + (long)(bitmapdata.Stride * num2));
                                    Marshal.Copy(source, num2 * bitmap.Width * 3, destination, bitmap.Width * 3);
                                    ++num2;
                                    num1 = 4;
                                    continue;
                                }
                                goto label_13;
                            case 7:
                                if (index <= sourceBytes.Length - 1)
                                {
                                    source[index * 3] = sourceBytes[index];
                                    source[index * 3 + 1] = sourceBytes[index];
                                    source[index * 3 + 2] = sourceBytes[index];
                                    ++index;
                                    num1 = 7;
                                    continue;
                                }
                                num1 = 1;
                                continue;
                            default:
                                goto label_2;
                        }
                    }
                label_13:
                    bitmap.UnlockBits(bitmapdata);
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                    return bitmap;
            }
        }

        internal static string BmpToBase64(Bitmap newImage)
        {
            string SigBase64 = string.Empty;
            using (var ms = new MemoryStream())
            {
                using (var bitmap = new Bitmap(newImage))
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    SigBase64 = "data:image/png;base64, " + Convert.ToBase64String(ms.GetBuffer()); //Get Base64
                }
            }

            return SigBase64;
        }

        internal static byte[] BitmapToBytes(Bitmap bmp) //internal byte[] eval_a(Bitmap A_0)
        {
            byte[] numArray1 = null;
            try
            {
                switch (0)
                {
                    default:
                    label_2:
                        Bitmap bitmap = eval_a((Image)bmp, 260U, true);
                        int num = 0;
                        while (true)
                        {
                            switch (num)
                            {
                                case 0:
                                    if (bitmap == null)
                                    {
                                        num = 3;
                                        continue;
                                    }
                                    Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                                    BitmapData bitmapdata = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
                                    IntPtr scan0 = bitmapdata.Scan0;
                                    int length1 = bitmapdata.Stride * bitmap.Height;
                                    byte[] numArray2 = new byte[length1];
                                    byte[] destination = numArray2;
                                    int length2 = length1;
                                    Marshal.Copy(scan0, destination, 0, length2);
                                    bitmap.UnlockBits(bitmapdata);
                                    numArray1 = numArray2;
                                    num = 1;
                                    continue;
                                case 1:
                                case 2:
                                    goto label_9;
                                case 3:
                                    numArray1 = (byte[])null;
                                    num = 2;
                                    continue;
                                default:
                                    goto label_2;
                            }
                        }
                }
            }
            catch (Exception ex)
            {

            }
            return (byte[])null;
        label_9:
            return numArray1;
        }

        public static unsafe Bitmap eval_a(Image image, uint A_1, bool A_2)
        {
            switch (0)
            {
                default:
                    Bitmap bitmap1 = (Bitmap)null;
                    try
                    {
                        int num1 = 25;
                        //int width;
                        //int height;
                        int width = 0;
                        int height = 0;
                        //Bitmap bitmap2;
                        Bitmap bitmap2 = null;
                        //ColorPalette colorPalette;
                        ColorPalette colorPalette = null;
                        //uint num2;
                        uint num2 = 0;
                        byte* numPtr1 = null;
                        //uint num3;
                        uint num3 = 0;
                        uint num4 = 0;
                        //uint num4;
                        uint num5 = 0;
                        //uint num5;
                        Bitmap bitmap3 = null;
                        //Bitmap bitmap3;
                        //BitmapData bitmapdata;
                        BitmapData bitmapdata = null;
                        //IntPtr scan0;
                        IntPtr scan0 = IntPtr.Zero;
                        uint num6 = 0;
                        //uint num6;
                        //uint num7;
                        uint num7 = 0;
                        while (true)
                        {
                            switch (num1)
                            {
                                case 0:
                                    goto label_38;
                                case 1:
                                case 10:
                                    num4 = (uint)Math.Abs(bitmapdata.Stride);
                                    num3 = 0U;
                                    num1 = 21;
                                    continue;
                                case 2:
                                case 16:
                                    num1 = 18;
                                    continue;
                                case 3:
                                    bitmap2.Palette = colorPalette;
                                    bitmap3 = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                                    Graphics graphics = Graphics.FromImage((Image)bitmap3);
                                    graphics.PageUnit = GraphicsUnit.Pixel;
                                    graphics.DrawImage(image, 0, 0, width, height);
                                    graphics.Dispose();
                                    Rectangle rect = new Rectangle(0, 0, width, height);
                                    bitmapdata = bitmap2.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                                    scan0 = bitmapdata.Scan0;
                                    num1 = 13;
                                    continue;
                                case 4:
                                case 7:
                                    num1 = 5;
                                    continue;
                                case 5:
                                    if (num2 < A_1)
                                    {
                                        num6 = (uint)byte.MaxValue;
                                        num7 = num2 * (uint)byte.MaxValue / (A_1 - 1U);
                                        num1 = 17;
                                        continue;
                                    }
                                    num1 = 3;
                                    continue;
                                case 6:
                                    num1 = 14;
                                    continue;
                                case 8:
                                case 21:
                                    num1 = 22;
                                    continue;
                                case 9:
                                    A_1 = 256U;
                                    num1 = 6;
                                    continue;
                                case 11:
                                    ++num3;
                                    num1 = 8;
                                    continue;
                                case 12:
                                    bitmap2.UnlockBits(bitmapdata);
                                    bitmap1 = bitmap2;
                                    bitmap3.Dispose();
                                    num1 = 0;
                                    continue;
                                case 13:
                                    if (bitmapdata.Stride <= 0)
                                    {
                                        numPtr1 = (byte*)((IntPtr)scan0.ToPointer() + bitmapdata.Stride * (height - 1));
                                        num1 = 1;
                                        continue;
                                    }
                                    num1 = 20;
                                    continue;
                                case 14:
                                    if (A_1 < 2U)
                                    {
                                        num1 = 19;
                                        continue;
                                    }
                                    goto case 24;
                                case 15:
                                    colorPalette.Entries[(int)num2] = Color.FromArgb((int)num6, (int)num7, (int)num7, (int)num7);
                                    ++num2;
                                    num1 = 4;
                                    continue;
                                case 17:
                                    if (num2 == 0U & A_2)
                                    {
                                        num1 = 23;
                                        continue;
                                    }
                                    goto case 15;
                                case 18:
                                    if ((long)num5 < (long)width)
                                    {
                                        byte* numPtr2 = numPtr1 + num3 * num4 + num5;
                                        Color pixel = bitmap3.GetPixel((int)num5, (int)num3);
                                        int num8 = (int)(byte)(((double)pixel.R * 0.299 + (double)pixel.G * 0.587 + (double)pixel.B * 0.114) * (double)(A_1 - 1U) / (double)byte.MaxValue + 0.5);
                                        *numPtr2 = (byte)num8;
                                        ++num5;
                                        num1 = 16;
                                        continue;
                                    }
                                    num1 = 11;
                                    continue;
                                case 19:
                                    A_1 = 2U;
                                    num1 = 24;
                                    continue;
                                case 20:
                                    numPtr1 = (byte*)scan0.ToPointer();
                                    num1 = 10;
                                    continue;
                                case 22:
                                    if ((long)num3 >= (long)height)
                                    {
                                        num1 = 12;
                                        continue;
                                    }
                                    num5 = 0U;
                                    num1 = 2;
                                    continue;
                                case 23:
                                    num6 = 0U;
                                    num1 = 15;
                                    continue;
                                case 24:
                                    width = image.Width;
                                    height = image.Height;
                                    bitmap2 = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                                    colorPalette = eval_a_x(A_1);
                                    num2 = 0U;
                                    num1 = 7;
                                    continue;
                                default:
                                    if (A_1 > 256U)
                                    {
                                        num1 = 9;
                                        continue;
                                    }
                                    goto case 6;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                label_38:
                    return bitmap1;
            }
        }

        private static ColorPalette eval_a_x(uint A_0)
        {
        label_2:
            PixelFormat format = PixelFormat.Format1bppIndexed;
            int num = 1;
            while (true)
            {
                switch (num)
                {
                    case 0:
                        num = 5;
                        continue;
                    case 1:
                        if (A_0 > 2U)
                        {
                            num = 3;
                            continue;
                        }
                        goto case 0;
                    case 2:
                        goto label_11;
                    case 3:
                        format = PixelFormat.Format4bppIndexed;
                        num = 0;
                        continue;
                    case 4:
                        format = PixelFormat.Format8bppIndexed;
                        num = 2;
                        continue;
                    case 5:
                        if (A_0 > 16U)
                        {
                            num = 4;
                            continue;
                        }
                        goto label_11;
                    default:
                        goto label_2;
                }
            }

        label_11:
            Bitmap bitmap = new Bitmap(1, 1, format);
            ColorPalette palette = bitmap.Palette;
            bitmap.Dispose();
            return palette;
        }

        internal static byte[] GenerateIsoBytes(byte[] rawImage)
        {
            byte[] minTemplate = new byte[346];
            _fpm.SetTemplateFormat(SGFPMTemplateFormat.ISO19794);

            if (_fpm.CreateTemplate(rawImage, minTemplate) != 0)
            {
                return null;
            }

            return minTemplate;
        }

        internal static byte[] GenerateAnsiBytes(byte[] rawImage)
        {
            /*ISO = 346
            ANSI = 300
            SG400 = 400*/
            byte[] minTemplate = new byte[300];

            if (_fpm.CreateTemplate(rawImage, minTemplate) != 0)
            {
                return null;
            }

            return minTemplate;
        }

        internal static bool MatchingScore(byte[] template1, byte[] template2, ref int score)
        {
            bool matched = false;
            _fpm.GetMatchingScore(template1, template2, ref score);
            _fpm.MatchTemplate(template1, template2, SGFPMSecurityLevel.HIGH, ref matched);
            return matched;
        }

        internal struct Eval_A
        {
            internal int PointX;
            internal int PointY;
            internal int c;
            internal int d;
            internal int e;
            internal int f;
            internal int g;
            internal int h;
            internal int i;
            internal int j;
            internal int k;
            internal int l;
            internal int m;
            internal int n;
            internal float o;
            internal int p;
            internal int q;
            internal int r;
            internal int s;
            internal int t;
        }
    }
}
