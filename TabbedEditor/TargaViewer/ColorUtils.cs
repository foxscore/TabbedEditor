using System.IO;
using System.Windows.Media;

namespace TabbedEditor.TargaViewer
{
    public static class ColorUtils
    {
        public static Color ReadColor(this BinaryReader reader, byte pixelDepth)
        {
            byte r = reader.ReadByte();
            byte g = r;
            byte b = r;
            byte a = 255;

            if (pixelDepth > 8)
            {
                g = reader.ReadByte();
                b = reader.ReadByte();
            }

            if (pixelDepth > 24)
                a = reader.ReadByte();

            return Color.FromArgb(a, r, g, b);
        }

        public static int ReadIndex(this BinaryReader reader, byte pixelDepth)
        {
            int index = 0;
            
            if (pixelDepth == 8)
            {
                index = reader.ReadByte();
            }
            else if (pixelDepth == 16)
            {
                index = reader.ReadInt16();
            }

            return index;
        }
    }
}