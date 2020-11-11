using System;
using System.IO;

namespace TabbedEditor.TargaViewer
{
    public class TargaHeader
    {
        public byte IdLenght { get; private set; }
        // public byte ColorMapType { get; private set; }
        public ImageType ColorMapType { get; private set; }
        public int XOrigin { get; private set; }
        public int YOrigin { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public byte PixelDepth { get; private set; }
        // TODO Implement usage of the Image descriptor

        public int ColorMapSize { get; private set; }
        public byte ColorMapPixelDepth { get; private set; }
        public byte ImageDescriptor { get; private set; }

        public static TargaHeader Read(BinaryReader reader)
        {
            TargaHeader header = new TargaHeader();

            // TODO Add titles to section
            #region Section 1
            header.IdLenght = reader.ReadByte();
            #endregion
            #region Section 2
            reader.ReadByte(); // TODO Skip Color map type, is redundant information
            # endregion
            #region Section 3
            byte imageTypeByte = reader.ReadByte();
            switch (imageTypeByte)
            {
                case 0:
                    // TODO
                    throw new TagraFileContainsNoImageException("The ");
                    break;
                case 1:
                    header.ColorMapType = ImageType.UncompressedColorMapped;
                    break;
                case 2:
                case 3:
                    header.ColorMapType = ImageType.UncompressedTrueColor;
                    break;
                case 9:
                    header.ColorMapType = ImageType.RunLenghtColorMap;
                    break;
                case 10:
                case 11:
                    header.ColorMapType = ImageType.RunLenghtTrueColor;
                    break;
            }
            #endregion
            #region Section 4
            reader.ReadBytes(2); // Skip color map index offset
            header.ColorMapSize = reader.ReadInt16();
            header.ColorMapPixelDepth = reader.ReadByte();
            #endregion
            #region Section 5
            header.XOrigin = reader.ReadInt16();
            header.YOrigin = reader.ReadInt16();
            header.Width = reader.ReadInt16();
            header.Height = reader.ReadInt16();
            header.PixelDepth = reader.ReadByte();
            header.ImageDescriptor = reader.ReadByte();
            #endregion

            return header;
        }
    }

    public class TagraFileContainsNoImageException : Exception
    {
        public TagraFileContainsNoImageException(string title) : base(title) { }
    }
}