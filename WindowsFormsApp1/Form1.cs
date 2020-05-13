using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private List<Bitmap> _bitmaps = new List<Bitmap>();
        private Random _random = new Random();
        public Form1()
        {
            InitializeComponent();
        }
        private async void MenuItemOpen_Click(object sender, EventArgs e)
        {

            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stopwatch sw = Stopwatch.StartNew();
                menuStrip2.Enabled = ScrollBarForPercintage.Enabled = false;
                ImageToWork.Image = null;
                _bitmaps.Clear();
                var bitmap = new Bitmap(openFileDialog1.FileName);
                await Task.Run(() => { RunProcesing(bitmap); });
                Text = "100 %";
                menuStrip2.Enabled = ScrollBarForPercintage.Enabled = true;
                sw.Stop();
                Text = $"Time that process takes -- {sw.Elapsed.ToString()}";

            }
        }
        private void RunProcesing(Bitmap bitmap)
        {
            var pixels = GetPixels(bitmap);
            var pixelsInStep = (bitmap.Width * bitmap.Height) / 100;
            var currentPixelsSet = new List<Pixel>(pixels.Count - pixelsInStep);
            for (int i = 1; i < ScrollBarForPercintage.Maximum; i++)
            {
                for (int j = 0; j < pixelsInStep; j++)
                {
                    int index = _random.Next(pixels.Count);
                    currentPixelsSet.Add(pixels[index]);
                    pixels.RemoveAt(index);

                }
                var currentBitmap = new Bitmap(bitmap.Width, bitmap.Height);
                foreach (var pixel in currentPixelsSet)
                {
                    currentBitmap.SetPixel(pixel.Point.X, pixel.Point.Y, pixel.Color);
                }
                _bitmaps.Add(currentBitmap);
                this.Invoke(new Action(() =>
                {
                    Text = $"{i} %";
                }));
            }
            _bitmaps.Add(bitmap);
        }
        private List<Pixel> GetPixels(Bitmap bitmap)
        {
            var pixels = new List<Pixel>((bitmap.Height * bitmap.Width));
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixels.Add(new Pixel()
                    {
                        Color = bitmap.GetPixel(x, y),
                        Point = new Point() { X = x, Y = y }
                    });
                }
            }
            return pixels;
        }
        private void ScrollBar_Scroll(object sender, EventArgs e)
        {
            int value = ScrollBarForPercintage.Value;
            Text = $"{value.ToString()} %";
            if (_bitmaps == null || _bitmaps.Count == 0)
            {
                return;
            }

            ImageToWork.Image = _bitmaps[value - 1];
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int value = ScrollBarForPercintage.Value;
                _bitmaps[value - 1].Save(saveFileDialog1.FileName);

            }
        }
    }
}
