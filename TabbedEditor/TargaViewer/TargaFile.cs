using System;
using System.IO;
using System.Windows.Media;

// Tagra specifications:
//    https://en.wikipedia.org/wiki/Truevision_TGA
//    http://paulbourke.net/dataformats/tga

namespace TabbedEditor.TargaViewer
{
    public class TargaFile
    {
        #region MetaData
        private string _path = "";
        public string Name { get; private set; } = "";
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                var paths = value.Split('\\');
                Name = paths[paths.Length - 1];
            }
        }
        #endregion

        public TargaHeader Header { get; private set; }
        public Color[,] Pixels;

        private void Read(BinaryReader reader)
        {
            Header = TargaHeader.Read(reader);
            
            Pixels = new Color[Header.Width, Header.Height];

            reader.ReadBytes(Header.IdLenght); // TODO Skipping ID field for now

            int index;
            
            switch (Header.ColorMapType)
            {
                case ImageType.UncompressedTrueColor:
                    for (int y = 0; y < Header.Height; y++)
                    {
                        for (int x = 0; x < Header.Width; x++)
                        {
                            Pixels[x, y] = reader.ReadColor(Header.PixelDepth);
                        }   
                    }
                    break;
                case ImageType.UncompressedColorMapped:
                    Color[] colorMap = new Color[Header.ColorMapSize];
                    for (int i = 0; i < Header.ColorMapSize; i++)
                    {
                        colorMap[i] = reader.ReadColor(Header.ColorMapPixelDepth);
                    }
                
                    for (int y = 0; y < Header.Height; y++)
                    {
                        for (int x = 0; x < Header.Width; x++)
                        {
                            Pixels[x, y] = colorMap[reader.ReadIndex(Header.PixelDepth)];
                        }   
                    }
                    break;
                case ImageType.RunLenghtTrueColor:
                    BlockHeader blockHeader = new BlockHeader();
                    Color color = Colors.White;

                    for (int y = 0; y < Header.Height; y++)
                    {
                        for (int x = 0; x < Header.Width; x++)
                        {
                            if (blockHeader.Lenght == 0)
                            {
                                blockHeader = BlockHeader.Read(reader);
                                if (blockHeader.IsRLE)
                                    color = reader.ReadColor(Header.PixelDepth);
                            }
                            
                            if (blockHeader.IsRLE)
                                Pixels[x, y] = color;
                            else
                            {
                                Pixels[x, y] = reader.ReadColor(Header.PixelDepth);
                            }

                            blockHeader.Lenght--;
                        }
                    }
                    break;
                case ImageType.RunLenghtColorMap:
                    colorMap = new Color[Header.ColorMapSize];
                    for (int i = 0; i < Header.ColorMapSize; i++)
                    {
                        colorMap[i] = reader.ReadColor(Header.ColorMapPixelDepth);
                    }
                    
                    blockHeader = new BlockHeader();
                    index = 0;

                    for (int y = 0; y < Header.Height; y++)
                    {
                        for (int x = 0; x < Header.Width; x++)
                        {
                            if (blockHeader.Lenght == 0)
                            {
                                blockHeader = BlockHeader.Read(reader);
                                if (blockHeader.IsRLE)
                                    index = reader.ReadIndex(Header.PixelDepth);
                            }
                            
                            if (blockHeader.IsRLE)
                                Pixels[x, y] = colorMap[index];
                            else
                            {
                                int pixelIndex = reader.ReadIndex(Header.PixelDepth);
                                Pixels[x, y] = colorMap[pixelIndex];
                            }

                            blockHeader.Lenght--;
                        }
                    }
                    break;
            }
        }

        public static TargaFile Read(string path)
        {
            TargaFile file = new TargaFile();
            file.Path = path;

            var stream = File.OpenRead(path);
            BinaryReader reader = new BinaryReader(stream);
            
            file.Read(reader);
            
            
            
            reader.Close();
            stream.Close();
            return file;
        }
    }
}