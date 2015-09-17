//This is the WinForms project that implements the depth histogram
//other items of code developed in the article are included and not used.


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Research.Kinect.Nui;
using Microsoft.Research.Kinect.Audio;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;

namespace kinectDepth
{
    public partial class Form1 : Form
    {

        Runtime nui;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            nui = Runtime.Kinects[0];
            nui.Initialize(RuntimeOptions.UseDepth);
            nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.Depth);
            nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(DepthFrameReady);
        
        }
        void DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage PImage = e.ImageFrame.Image;

         //  int x = PImage.Width / 2;
         //  int y = PImage.Height / 2;
         //  int d = getValue(PImage, x, y);
         //  MessageBox.Show(d.ToString());

            int temp = 0; 
            int[] count = new int[0x1FFF / 4 +1];
            for (int i = 0; i < PImage.Bits.Length; i += 2)
            {
                temp= (PImage.Bits[i+1]<<8 | PImage.Bits[i])& 0x1FFF ;
                count[temp >> 2]++;
                temp <<= 2;
                PImage.Bits[i] = (byte) (temp & 0xFF);
                PImage.Bits[i + 1] = (byte) (temp >> 8);
            }
            chart1.Series[0].Points.Clear();
            for (int i = 1; i <( 0x1FFF / 4); i++)
            {
                chart1.Series[0].Points.Add(count[i]);
            }
            Application.DoEvents();
            pictureBox1.Image = DepthToBitmap(PImage);
        }
        
        int getValue(PlanarImage PImage, int x, int y)
        {
            int d = PImage.Bits[x * PImage.BytesPerPixel + y * PImage.Width * PImage.BytesPerPixel + 1];
            d <<= 8;
            d += PImage.Bits[x * PImage.BytesPerPixel + y * PImage.Width * PImage.BytesPerPixel];
            return d;
        }
        
        Bitmap DepthToBitmap(PlanarImage PImage)
        {
            Bitmap bmap = new Bitmap(
             PImage.Width,
             PImage.Height,
             PixelFormat.Format16bppRgb555);

            BitmapData bmapdata = bmap.LockBits(
                new Rectangle(0, 0, PImage.Width,
                       PImage.Height),
                ImageLockMode.WriteOnly,
                bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            Marshal.Copy(PImage.Bits,
                         0,
                         ptr,
                         PImage.Width *
                         PImage.BytesPerPixel *
                         PImage.Height);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            nui.Uninitialize();
        }
    }
}

