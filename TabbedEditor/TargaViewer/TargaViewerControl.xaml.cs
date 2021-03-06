﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TabbedEditor.Interfaces;
using TabbedEditor.IO;

namespace TabbedEditor.TargaViewer
{
    public partial class TargaViewerControl : IEditorControl
    {
        public TargaViewerControl()
        {
            InitializeComponent();
        }

        TargaFile file = new TargaFile();

        public event Action<string> TitleChangedEvent;
        public string FilePath => file.Path;
        public InspectorContent InspectorContent { get; private set; }
        public bool UnsavedChanges { get; }
        public Exception Open(string path)
        {
            file.Path = path;
            TitleChangedEvent?.Invoke(file.Name);
            
            TargaFile targaFile = TargaFile.Read(path);

            int width = targaFile.Header.Width;
            int height = targaFile.Header.Height;
            
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 90, 90, PixelFormats.Bgra32, null);

            byte[] buffer = new byte[width * height * 4];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pos = (x + y * width) * 4;
                    var lookupX = width - x - 1;
                    Color color = targaFile.Pixels[lookupX, y];
                    buffer[pos] = color.B;
                    buffer[pos + 1] = color.G;
                    buffer[pos + 2] = color.R;
                    buffer[pos + 3] = color.A;
                }
            }
            
            Int32Rect rect = new Int32Rect(0, 0, width, height);
            
            bitmap.WritePixels(rect, buffer, width * 4, 0);

            OutputImage.Source = bitmap;
            FileInfo fileInfo = new FileInfo(file.Path);
            
            InspectorContent = new InspectorContent(
                file.Name,
                new InspectorTableEntry[]
                {
                    new InspectorTableEntry("Path", file.Path, file.Path),
                    new InspectorTableEntry("File Size", FileSizeFormatter.FormatSize(fileInfo.Length)),
                    new InspectorTableEntry("Creation time", fileInfo.CreationTime.ToString(CultureInfo.InvariantCulture)),
                    new InspectorTableEntry("Last change", fileInfo.LastWriteTime.ToString(CultureInfo.InvariantCulture)),
                    new InspectorTableEntry("Dimensions", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions1", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions2", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions3", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions4", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions5", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions6", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions7", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions8", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions9", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions11", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions12", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions13", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions14", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions15", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions16", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions17", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions18", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions19", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions21", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions22", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions23", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                    new InspectorTableEntry("Dimensions24", width + " x " + height, $"Height:\t{height}\nWidth:\t{width}"), 
                },
                new Image()
                {
                    Source = new DrawingImage() { Drawing = (Drawing)FindResource("Image") }
                });
            
            return null;
        }

        public void SaveAs()
        {
            
        }

        public void Save()
        {
            
        }
    }
}