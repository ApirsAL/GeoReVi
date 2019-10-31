using System.Drawing;
using System.IO;

namespace GeoReVi
{
    /// <summary>
    /// A class to convert images
    /// </summary>
    public static class ImageHelper
    {
        //Converts a bitmap to an image
        public static byte[] BitmapToByte(Bitmap btmp)
        {
            using (var stream = new MemoryStream())
            {
                btmp.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }
    }
}
