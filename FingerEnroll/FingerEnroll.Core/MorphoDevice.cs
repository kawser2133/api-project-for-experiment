using System;
using System.Runtime.InteropServices;

namespace FingerEnroll.Core
{
    internal static class MorphoDevice
    {
        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool InitSdk(string A_0);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool InitDevice(string A_0);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool GetImage(string key, MorphoDevice.eval_a A_1, byte[] imageBytes, ref int width, ref int height, int A_5);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool CancelCapture(string A_0);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern int GetPreviewImageSize(string A_0);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void GetPreviewImageDimension(string A_0, ref int A_1, ref int A_2);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern int GetMorphoImageHeaderSize(string A_0);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern int GetCaptureImageSize(string A_0);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool ReleaseResource(string A_0);

        internal delegate int eval_a(IntPtr A_0, MorphoDevice.DataBit A_1, IntPtr A_2);

        internal enum DataBit
        {
            eval_a = 1,
            eval_b = 2,
            eval_c = 4,
            eval_d = 8,
            eval_e = 16, // 0x00000010
            eval_f = 32, // 0x00000020
            eval_g = 64, // 0x00000040
            eval_h = 128, // 0x00000080
            eval_i = 256, // 0x00000100
            eval_j = 512, // 0x00000200
            eval_k = 1024, // 0x00000400
            eval_l = 2048, // 0x00000800
            eval_m = 4096, // 0x00001000
            eval_n = 8192, // 0x00002000
            eval_o = 16384, // 0x00004000
            eval_p = 32768, // 0x00008000
            eval_q = 65536, // 0x00010000
            eval_r = 131072, // 0x00020000
            eval_s = 262144, // 0x00040000
            eval_t = 524288, // 0x00080000
            eval_u = 1048576, // 0x00100000
            eval_v = 2097152, // 0x00200000
            eval_w = 4194304, // 0x00400000
            eval_x = 8388608, // 0x00800000
            eval_y = 16777216, // 0x01000000
            eval_z = 33554432, // 0x02000000
            eval_aa = 67108864, // 0x04000000
            eval_ab = 134217728, // 0x08000000
            eval_ac = 268435456, // 0x10000000
            eval_ad = 536870912, // 0x20000000
            eval_ae = 1073741824, // 0x40000000
        }
    }
}