using System;
using System.Collections.Generic;
using System.Drawing;
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

            //Ilosci = value => ;

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
            color = "red";
        }

        private void showGreen_Click(object sender, RoutedEventArgs e)
        {
            colorMode = 1;
            color = "green";
        }

        private void showBlue_Click(object sender, RoutedEventArgs e)
        {
            colorMode = 2;
            color = "blue";
        }

        private void showBrightness_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
