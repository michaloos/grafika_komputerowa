using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;

namespace Grafika
{
    /// <summary>
    /// Logika interakcji dla klasy HistogramImage.xaml
    /// </summary>
    public partial class HistogramImage : Window
    {
        public int colorMode = 0;
        // 0 red
        // 1 green
        // 2 blue
        // 3 brightness
        public string[] Wartosci { get; set; }
        public MainWindow mainWindow;
        byte[] red;
        byte[] green;
        byte[] blue;
        Bitmap bitmap;
        ImageSource image;
        public string color = "red";
        Func<int, string> Ilosci { get; set; }
        int[] colorsArray = new int[256];
        public SeriesCollection columnSeries { get; set; }
        public HistogramImage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            image = mainWindow.ps5Image.Source;
            bitmap = mainWindow.ImageSourceToBitmap(image);
            getColors();
            getColorsArray();

            columnSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = color,
                    Values = new ChartValues<int>(colorsArray.ToArray())
                }
            };

            var list = new List<String>();
            for (int i = 0; i < 256; i++)
            {
                list.Add(i.ToString());
            }
            Wartosci = list.ToArray();
            DataContext = this;
            
        }

        private void getColorsArray()
        {
            for(int i = 0; i < colorsArray.Length; i++)
            {
                colorsArray[i] = 0;
            }
            if(colorMode == 0)
            {
                foreach(var value in red)
                {
                    colorsArray[value] += 1;
                }
            }
            if (colorMode == 1)
            {
                foreach (var value in green)
                {
                    colorsArray[value] += 1;
                }
            }
            if (colorMode == 2)
            {
                foreach (var value in blue)
                {
                    colorsArray[value] += 1;
                }
            }
            if (colorMode == 3)
            {
                for(int i = 0; i < red.Length; i++)
                {
                    int bright = (int)(0.21 * red[i] + 0.71 * green[i] + 0.071 * blue[i]);
                    colorsArray[bright] += 1;
                }
            }
        }

        private void getColors()
        {
            int ilosc = 0;

            red = new byte[bitmap.Height * bitmap.Width];
            green = new byte[bitmap.Height * bitmap.Width];
            blue = new byte[bitmap.Height * bitmap.Width];

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    System.Drawing.Color color = bitmap.GetPixel(i, j);
                    red[ilosc] = color.R;
                    green[ilosc] = color.G;
                    blue[ilosc] = color.B;
                    ilosc++;
                }
            }
        }

        private void showRed_Click(object sender, RoutedEventArgs e)
        {
            colorMode = 0;
            color = "Red";
            getColorsArray();

            columnSeries.Clear();
            columnSeries.Add(new ColumnSeries()
            {
                Title = color,
                Values = new ChartValues<int>(colorsArray.ToArray())
            });
        }

        private void showGreen_Click(object sender, RoutedEventArgs e)
        {
            colorMode = 1;
            color = "Green";
            getColorsArray();
            columnSeries.Clear();
            columnSeries.Add(new ColumnSeries()
            {
                Title = color,
                Values = new ChartValues<int>(colorsArray.ToArray()),
            });
        }

        private void showBlue_Click(object sender, RoutedEventArgs e)
        {
            colorMode = 2;
            color = "Blue";
            getColorsArray();

            columnSeries.Clear();
            columnSeries.Add(new ColumnSeries()
            {
                Title = color,
                Values = new ChartValues<int>(colorsArray.ToArray())
            });
        }

        private void showBrightness_Click(object sender, RoutedEventArgs e)
        {
            colorMode = 3;
            color = "Brightness";
            getColorsArray();

            columnSeries.Clear();
            columnSeries.Add(new ColumnSeries()
            {
                Title = color,
                Values = new ChartValues<int>(colorsArray.ToArray())
            });
        }

        private void equalization_Click(object sender, RoutedEventArgs e)
        {
            int MN = bitmap.Width * bitmap.Height;
            int[] tempArray = new int[256];
            int firstNot0 = 0;
            double sum = 0;
            for(int i = 0; i < colorsArray.Length; i++) 
            {
                if(colorsArray[i] != 0)
                {
                    firstNot0 = i;
                    break;
                }
            }
            for(int i = 0; i < 256; i++)
            {
                sum += colorsArray[i];
                tempArray[i] = (int)((sum - firstNot0) / (MN - firstNot0) * 255);
            }
            Bitmap equalizationBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for(int i = 0; i < bitmap.Width; i++)
            {
                for(int j = 0; j < bitmap.Height; j++)
                {
                    System.Drawing.Color pixel = bitmap.GetPixel(i, j);
                    System.Drawing.Color pixelV2 = System.Drawing.Color.FromArgb(tempArray[pixel.R], tempArray[pixel.G], tempArray[pixel.B]);
                    equalizationBitmap.SetPixel(i, j, pixelV2);
                }
            }
            BitmapImage image = FromBitmapToBitmapImage(equalizationBitmap);
            mainWindow.ps5Image.Source = image;

            bitmap = mainWindow.ImageSourceToBitmap(image);

            colorMode = 3;
            color = "Brightness";
            getColors();
            getColorsArray();

            columnSeries.Clear();
            columnSeries.Add(new ColumnSeries()
            {
                Title = color,
                Values = new ChartValues<int>(colorsArray.ToArray())
            });
        }

        private BitmapImage FromBitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Bmp);
                memoryStream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private void rozszerzenie_Click(object sender, RoutedEventArgs e)
        {
            int minValue = 0;
            int maxValue = 0;
            for(int i = 0; i < 256; i++)
            {
                if (colorsArray[i] != 0)
                {
                    minValue = i;
                    break;
                }
            }
            for(int i = 255; i >= 0; i--)
            {
                if (colorsArray[i] != 0)
                {
                    maxValue = i;
                    break;
                }
            }
            int[] tempArray = new int[256];
            for(int i = 0; i < 256; i++)
            {
                tempArray[i] = 255 / (maxValue - minValue) * (i - minValue);
            }
            Bitmap rozciąganieBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    System.Drawing.Color pixel = bitmap.GetPixel(i, j);
                    System.Drawing.Color pixelV2 = System.Drawing.Color.FromArgb(tempArray[pixel.R], tempArray[pixel.G], tempArray[pixel.B]);
                    rozciąganieBitmap.SetPixel(i, j, pixelV2);
                }
            }
            BitmapImage image = FromBitmapToBitmapImage(rozciąganieBitmap);
            mainWindow.ps5Image.Source = image;

            bitmap = mainWindow.ImageSourceToBitmap(image);

            colorMode = 3;
            color = "Brightness";
            getColors();
            getColorsArray();

            columnSeries.Clear();
            columnSeries.Add(new ColumnSeries()
            {
                Title = color,
                Values = new ChartValues<int>(colorsArray.ToArray())
            });
        }
    }
}
