using System.Drawing;

namespace FingerPrint.Core
{
    public class CapturedData
    {
        internal Bitmap BmpImage; //eval_a

        public CapturedData()
        {
        }

        public int FingerID { get; set; }
        public int FingerprintImageScore { get; set; }
        public string FingerprintImage { get; set; }
        public string FingerprintData { get; set; }
        public string Template { get; set; }
        public string Message { get; set; }
    }

}