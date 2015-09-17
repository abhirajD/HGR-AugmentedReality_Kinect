//WPF Greyscale viewer
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using Microsoft.Kinect.Nui;
using Microsoft.Kinect.Audio;
namespace WPFDepth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Runtime nui = Runtime.Kinects[0];
            nui.Initialize(RuntimeOptions.UseDepth);
            nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution640x480, ImageType.Depth);
            nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(DepthFrameReady);
        }

        void DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage PImage = e.ImageFrame.Image;

      //     int x = PImage.Width / 2;
      //     int y = PImage.Height / 2;
      //     int d = getValue(PImage, x, y);
      //     MessageBox.Show(d.ToString());
            int temp = 0;
            for (int i = 0; i < PImage.Bits.Length; i += 2)
            {
                temp = (PImage.Bits[i + 1] << 8 | PImage.Bits[i]);
                temp <<=3;
                PImage.Bits[i] = (byte)(temp & 0xFF);
                PImage.Bits[i + 1] = (byte)(temp >> 8);
            }
            image1.Source = DepthToBitmapSource(PImage);
          
        }
        int getValue(PlanarImage PImage, int x, int y)
        {
            int d = PImage.Bits[x * PImage.BytesPerPixel + y * PImage.Width * PImage.BytesPerPixel + 1];
            d <<= 8;
            d += PImage.Bits[x * PImage.BytesPerPixel + y * PImage.Width * PImage.BytesPerPixel];
            return d;
        }
        BitmapSource DepthToBitmapSource(PlanarImage PImage)
        {
            BitmapSource bmap = BitmapSource.Create(
            PImage.Width,
            PImage.Height,
            96, 96,
            PixelFormats.Gray16,
            null,
            PImage.Bits,
            PImage.Width * PImage.BytesPerPixel);
            return bmap;
        }
    }
}
