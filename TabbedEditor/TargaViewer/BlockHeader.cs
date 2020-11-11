using System.Collections;
using System.IO;

namespace TabbedEditor.TargaViewer
{
    public class BlockHeader
    {
        public bool IsRLE { get; private set; }
        public byte Lenght;

        public static BlockHeader Read(BinaryReader reader)
        {
            BlockHeader header = new BlockHeader();
            byte[] headerByte = reader.ReadBytes(1);
            BitArray bitArray = new BitArray(headerByte);

            header.IsRLE = bitArray[7];
            bitArray[7] = false;

            byte[] outputArray = new byte[1];
            bitArray.CopyTo(outputArray, 0);
            header.Lenght = outputArray[0];
            header.Lenght++;

            return header;
        }
    }
}