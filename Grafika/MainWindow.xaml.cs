﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Encoder = System.Drawing.Imaging.Encoder;
using Path = System.IO.Path;
using System.Drawing;
using System.Collections;

namespace Grafika
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Shape shape = null;
        private int SHAPE_MODE;
        //SHAPE_MODE = 1 => line
        //SHAPE_MODE = 2 => rectangle
        //SHAPE_MODE = 3 => ellipse

        private int ACTION_MODE;
        //ACTION_MODE = 1 => draw
        //ACTION_MODE = 2 => move
        //ACTION_MODE = 3 => change shape

        private System.Windows.Point startPoints = new System.Windows.Point();
        private System.Windows.Point endPoints = new System.Windows.Point();
        private TranslateTransform TranslateTransform;
        private System.Windows.Point clickPosition = new System.Windows.Point();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void drawing_Checked(object sender, RoutedEventArgs e)
        {
            ACTION_MODE = 1;
        }

        private void moving_Checked(object sender, RoutedEventArgs e)
        {
            ACTION_MODE = 2;
        }

        private void changeShape_Checked(object sender, RoutedEventArgs e)
        {
            ACTION_MODE = 3;
        }

        private void line_Checked(object sender, RoutedEventArgs e)
        {
            SHAPE_MODE = 1;
        }

        private void rectangle_Checked(object sender, RoutedEventArgs e)
        {
            SHAPE_MODE = 2;
        }

        private void ellipse_Checked(object sender, RoutedEventArgs e)
        {
            SHAPE_MODE = 3;
        }

        private void CanvasField_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(ACTION_MODE != 1)
            {
                return;
            }
            
            startPoints = e.GetPosition(CanvasField);
            shape = null;
        }

        private void CanvasField_MouseMove(object sender, MouseEventArgs e)
        {
            if(SHAPE_MODE == 0 || ACTION_MODE == 0)
            {
                return;
            }
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                if (ACTION_MODE == 1)
                {
                    if(shape != null)
                    {
                        CanvasField.Children.Remove(shape);
                    }
                    endPoints = e.GetPosition(CanvasField);

                    switch (SHAPE_MODE)
                    {
                        //tworzenie linii
                        case 1:
                            shape = new Line
                            {
                                X1 = startPoints.X,
                                Y1 = startPoints.Y,
                                X2 = endPoints.X,
                                Y2 = endPoints.Y,
                                StrokeThickness = 5,
                                Stroke = System.Windows.Media.Brushes.Red
                            };
                            break;
                        //tworzenie prostokąta
                        case 2:
                            shape = new System.Windows.Shapes.Rectangle
                            {
                                Width = Math.Abs(startPoints.X - endPoints.X),
                                Height = Math.Abs(startPoints.Y - endPoints.Y),
                                StrokeThickness = 5,
                                Stroke = System.Windows.Media.Brushes.Green
                            };
                            var currentPosXRec = startPoints.X < endPoints.X ? startPoints.X : endPoints.X;
                            var currentPosYRec = startPoints.Y < endPoints.Y ? startPoints.Y : endPoints.Y;
                            shape.SetValue(Canvas.LeftProperty, currentPosXRec);
                            shape.SetValue(Canvas.TopProperty, currentPosYRec);
                            break;
                        //tworzenie ellipsy
                        case 3:
                            shape = new Ellipse
                            {
                                Width = Math.Abs(startPoints.X - endPoints.X),
                                Height = Math.Abs(startPoints.X - endPoints.X),
                                StrokeThickness = 5,
                                Stroke = System.Windows.Media.Brushes.Yellow
                            };
                            var currentPosXEli = startPoints.X < endPoints.X ? startPoints.X : endPoints.X;
                            var currentPosYEli = startPoints.Y < endPoints.Y ? startPoints.Y : endPoints.Y;
                            shape.SetValue(Canvas.LeftProperty, currentPosXEli);
                            shape.SetValue(Canvas.TopProperty, currentPosYEli);
                            break;
                    }

                    shape.MouseDown += Select_Shape; //event do zaznaczenia wybranej figury
                    shape.MouseMove += Move_Shape; //event do przesuwania myszą figury
                    shape.MouseUp += Moved_Shape; //event do "skończenia" przesuwania figury

                    CanvasField.Children.Add(shape);
                }
            }
        }

        private void Moved_Shape(object sender, MouseEventArgs e)
        {
            shape = sender as Shape;
            shape.ReleaseMouseCapture();
        }

        private void Move_Shape(object sender, MouseEventArgs e)
        {
            if(ACTION_MODE == 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                var dragShape = sender as Shape;
                if (dragShape != null)
                {
                    System.Windows.Point currentPosition = e.GetPosition(CanvasField);
                    var transform = dragShape.RenderTransform as TranslateTransform ?? new TranslateTransform();
                    transform.X = TranslateTransform.X + (currentPosition.X - clickPosition.X);
                    transform.Y = TranslateTransform.Y + (currentPosition.Y - clickPosition.Y);
                    dragShape.RenderTransform = new TranslateTransform(transform.X, transform.Y);
                }
            }
            if(ACTION_MODE == 3 && e.LeftButton == MouseButtonState.Pressed)
            {
                var changeShape = sender as Shape;
                if(changeShape != null)
                {
                    if(changeShape is Line)
                    {
                        System.Windows.Point currentPosition = e.GetPosition(CanvasField);
                    }
                }
            }
        }

        private void Select_Shape(object sender, MouseEventArgs e)
        {
            if(ACTION_MODE != 1)
            {
                shape = sender as Shape;
                if(e.LeftButton == MouseButtonState.Pressed && ACTION_MODE == 2)
                {
                    TranslateTransform = shape.RenderTransform as TranslateTransform ?? new TranslateTransform();
                    clickPosition = e.GetPosition(CanvasField);
                    shape.CaptureMouse();
                }
            }
        }

        private void drawLine_Click(object sender, RoutedEventArgs e)
        {
            var x1Point = Double.TryParse(x1line.Text,out double x1);
            var y1Point = Double.TryParse(y1line.Text,out double y1);
            var x2Point = Double.TryParse(x2line.Text,out double x2);
            var y2Point = Double.TryParse(y2line.Text,out double y2);

            if(x1Point && y1Point && x2Point && y2Point)
            {
                shape = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    StrokeThickness = 5,
                    Stroke = System.Windows.Media.Brushes.Red
                };
                shape.MouseDown += Select_Shape; //event do zaznaczenia wybranej figury
                shape.MouseMove += Move_Shape; //event do przesuwania myszą figury
                shape.MouseUp += Moved_Shape; //event do "skończenia" przesuwania figury
                CanvasField.Children.Add(shape);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
            }
        }

        private void drawRectangle_Click(object sender, RoutedEventArgs e)
        {
            var x1Point = Double.TryParse(x1rectangle.Text, out double x1);
            var y1Point = Double.TryParse(y1rectangle.Text, out double y1);
            var x2Point = Double.TryParse(widthRectangle.Text, out double width);
            var y2Point = Double.TryParse(heightRectangle.Text, out double height);

            if (x1Point && y1Point && x2Point && y2Point)
            {
                shape = new System.Windows.Shapes.Rectangle
                {
                    Width = width,
                    Height = height,
                    StrokeThickness = 5,
                    Stroke = System.Windows.Media.Brushes.Green
                };
                shape.SetValue(Canvas.LeftProperty, x1);
                shape.SetValue(Canvas.TopProperty, y1);

                shape.MouseDown += Select_Shape; //event do zaznaczenia wybranej figury
                shape.MouseMove += Move_Shape; //event do przesuwania myszą figury
                shape.MouseUp += Moved_Shape; //event do "skończenia" przesuwania figury
                
                CanvasField.Children.Add(shape);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
            }

        }

        private void drawEllipse_Click(object sender, RoutedEventArgs e)
        {
            var x1Point = Double.TryParse(x1ellipse.Text, out double x1);
            var y1Point = Double.TryParse(y1ellipse.Text, out double y1);
            var radius = Double.TryParse(radiusellipse.Text, out double r);

            if(x1Point && y1Point && radius)
            {
                shape = new Ellipse
                {
                    Width = Math.Abs(r*2),
                    Height = Math.Abs(r*2),
                    StrokeThickness = 5,
                    Stroke = System.Windows.Media.Brushes.Yellow
                };

                shape.SetValue(Canvas.LeftProperty, x1);
                shape.SetValue(Canvas.TopProperty, y1);

                shape.MouseDown += Select_Shape; //event do zaznaczenia wybranej figury
                shape.MouseMove += Move_Shape; //event do przesuwania myszą figury
                shape.MouseUp += Moved_Shape; //event do "skończenia" przesuwania figury
                
                CanvasField.Children.Add(shape);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
            }
        }

        private void lineBigger_Click(object sender, RoutedEventArgs e)
        {
            if(ACTION_MODE != 3 || shape == null || !(shape is Line))
            {
                return;
            }

            var changeLine = Double.TryParse(changeLineValue.Text, out double value);
            if (changeLine)
            {
                Line line = shape as Line;
                if (line.X2 < line.X1)
                {
                    value = value * -1;
                }
                
                double currentLenght = Math.Sqrt(Math.Pow(Math.Abs(line.X1 - line.X2), 2) + Math.Pow(Math.Abs(line.Y1 - line.Y2), 2));
                double cos = line.ActualWidth / currentLenght;

                double newLenght = currentLenght + value;
                
                // width/currentLenght = (width + a)/newLenght
                //          (width + a)
                //  cos = -------------  => cos * newLenght = width + a => a = (cos * newLenght) - width
                //          newLenght
                double a = (cos * newLenght) - line.ActualWidth;

                //wzór ogólny linii to ax + by = d
                //wzór kierunkowy na linię to y = mx + c => c = y - mx
                //m > 0 funkcja rośnie
                //m < 0 funkcja maleje
                //m == 0 funkcja stała
                //m = (y2-y1)/(x2-x1)
                double m = (line.Y2 - line.Y1) / (line.X2 - line.X1);
                double c = line.Y1 - m * line.X1;

                line.X2 = line.X2 + a/2;
                line.Y2 = m * line.X2 + c;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
            }
        }

        private void changeRectangle_Click(object sender, RoutedEventArgs e)
        {
            if (ACTION_MODE != 3 || shape == null || shape is Line || shape is Ellipse)
            {
                return;
            }

            var width = Double.TryParse(changeWidth.Text, out double newWidth);
            var height = Double.TryParse(changeHeight.Text, out double newHeight);

            if(width && height)
            {
                System.Windows.Shapes.Rectangle rectangle = shape as System.Windows.Shapes.Rectangle;
                //nowe wartości nie mogą spowodować że szerokość/wysokość będzie mniejsza od zera
                if(rectangle.Width + newWidth <= 0 || rectangle.Height + newHeight <= 0)
                {
                    MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
                }
                else
                {
                    double x = Canvas.GetLeft((UIElement)rectangle) + rectangle.Width + newWidth;
                    double y = Canvas.GetTop((UIElement)rectangle) + rectangle.Height + newHeight;
                    //kształt figury nie może wyjść poza obszar do rysowania
                    if (x >= CanvasField.ActualWidth || y >= CanvasField.ActualHeight)
                    {
                        MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
                    }
                    else
                    {
                        rectangle.Width = rectangle.Width + newWidth;
                        rectangle.Height = rectangle.Height + newHeight;
                    }
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
            }
        }

        private void changeEllipse_Click(object sender, RoutedEventArgs e)
        {
            if (ACTION_MODE != 3 || shape == null || shape is Line || shape is System.Windows.Shapes.Rectangle)
            {
                return;
            }

            var radius = Double.TryParse(changeRadius.Text,out double newRadius);

            if (radius)
            {
                Ellipse ellipse = shape as Ellipse;
                double oldRadius = ellipse.Width / 2;
                //nowy promień nie może być mniejszy od 0
                if (oldRadius + newRadius <= 0)
                {
                    MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
                }
                else
                {
                    double x = Canvas.GetLeft((UIElement)ellipse) + (2 * (oldRadius + newRadius));
                    double y = Canvas.GetTop((UIElement)ellipse) + (2 * (oldRadius + newRadius));
                    //kształt figury nie może wyjść poza obszar do rysowania
                    if (x >= CanvasField.ActualWidth || y >= CanvasField.ActualHeight)
                    {
                        MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
                    }
                    else
                    {
                        ellipse.Width = 2 * (oldRadius + newRadius);
                        ellipse.Height = 2 * (oldRadius + newRadius);
                    }
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
            }
        }

        private void uploadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPEG Image|*.jpg;*.jpeg|PPM Image|*.ppm";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string extension = Path.GetExtension(openFileDialog.FileName);
                    if (!extension.ToLower().Equals(".ppm"))
                    {
                        Bitmap bitmap = new Bitmap(openFileDialog.FileName);
                        BitmapImage bitmapImage = FromBitmapToBitmapImage(bitmap);
                        Image.Source = bitmapImage;
                    }
                    else
                    {
                        Bitmap bitmap = PPMToBitmap(openFileDialog.FileName);
                        BitmapImage bitmapImage = FromBitmapToBitmapImage(bitmap);
                        Image.Source = bitmapImage;
                    }              
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            if(Image.Source == null)
            {
                return;
            }

            SaveFileDialog saveFile = new SaveFileDialog
            {
                Filter = "JPEG Image|*.jpg"
            };

            var save = new JpegBitmapEncoder();
            if(saveFile.ShowDialog() != false)
            {
                try
                {
                    if (compression1L.IsSelected)
                    {
                        save.QualityLevel = (int)1L;
                    }
                    else if (compression25L.IsSelected)
                    {
                        save.QualityLevel = (int)25L;
                    }
                    else if (compression50L.IsSelected)
                    {
                        save.QualityLevel = (int)50L;
                    }
                    else if (compression75L.IsSelected)
                    {
                        save.QualityLevel = (int)75L;
                    }
                    else if (compression100L.IsSelected)
                    {
                        save.QualityLevel = (int)100L;
                    }
                    else
                    {
                        save.QualityLevel = (int)50L;
                    }

                    save.Frames.Add(BitmapFrame.Create((BitmapSource)Image.Source));
                    using(var stream = saveFile.OpenFile())
                    {
                        save.Save(stream);
                    }

                }
                catch
                {

                }
            }
            
        }

        private BitmapImage FromBitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using(MemoryStream memoryStream = new MemoryStream())
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

        private Bitmap PPMToBitmap(string file)
        {
            string fileType = "";
            int width = 0;
            int height = 0;
            int maxColor = 0;
            int linesToAvoidP6 = 0;

            List<string> lines = new List<string>();

            long previousLength = 0;
            using (var fs = File.OpenRead(file))
            {
                fs.Position = previousLength;
                previousLength = fs.Length;
                using (var streamRead = new StreamReader(fs))
                {
                    
                    while (!streamRead.EndOfStream)
                    {
                        if (fileType.Equals("P6") && width != 0 && height != 0 && maxColor != 0)
                        {
                            break;
                        }
                        var line = streamRead.ReadLine();
                        if(!String.IsNullOrEmpty(line) && line.StartsWith("#"))
                        {
                            int index = line.IndexOf("#");
                            if (index >= 0)
                            {
                                line = line.Substring(0, index);
                                linesToAvoidP6++;
                            }
                        }
                        
                        if (!String.IsNullOrEmpty(line) && !line.StartsWith("#") && !fileType.Equals("") )
                        {
                            if (width != 0 && height != 0 && maxColor != 0 && !fileType.Equals(""))
                            {
                                lines.Add(line);
                            }
                            if (width == 0 && width == 0 && !fileType.Equals(""))
                            {
                                linesToAvoidP6++;
                                List<int> correctValues = new List<int>(); //ostateczne wartości
                                List<string> splitedList = line.Split(new char[]{' ','\t' },StringSplitOptions.RemoveEmptyEntries).ToList();
                                foreach (var element in splitedList)
                                {
                                    if (Int32.TryParse(element, out int value))
                                        correctValues.Add(value);
                                }
                                if(correctValues.Count == 1)
                                {
                                    width = correctValues[0];
                                }
                                if(correctValues.Count == 2)
                                {
                                    width = correctValues[0];
                                    height = correctValues[1];
                                }
                                if (correctValues.Count == 3)
                                {
                                    width = correctValues[0];
                                    height = correctValues[1];
                                    maxColor = correctValues[2];
                                }
                            }
                            else if(height == 0 && width != 0 && !fileType.Equals(""))
                            {
                                linesToAvoidP6++;
                                List<int> correctValues = new List<int>(); //ostateczne wartości
                                List<string> splitedList = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                foreach (var element in splitedList)
                                {
                                    if (Int32.TryParse(element, out int value))
                                        correctValues.Add(value);
                                }
                                if (correctValues.Count == 1)
                                {
                                    height = correctValues[0];
                                }
                                if (correctValues.Count == 2)
                                {
                                    height = correctValues[0];
                                    maxColor = correctValues[1];
                                }
                            }
                            else if(height != 0 && width != 0 && maxColor == 0 && !fileType.Equals(""))
                            {
                                linesToAvoidP6++;
                                List<int> correctValues = new List<int>(); //ostateczne wartości
                                List<string> splitedList = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                foreach (var element in splitedList)
                                {
                                    if (Int32.TryParse(element, out int value))
                                        correctValues.Add(value);
                                }
                                if (correctValues.Count == 1)
                                {
                                    maxColor = correctValues[0];
                                }
                            }
                        }
                        if (line.StartsWith("P") && fileType.Equals(""))
                        {
                            if (line.StartsWith("P3"))
                            {
                                fileType = "P3";
                            }
                            else if (line.StartsWith("P6"))
                            {
                                fileType = "P6";
                                linesToAvoidP6++;
                            }
                            else
                            {
                                MessageBoxResult result = MessageBox.Show("Zły format pliku PPM!");
                                return new Bitmap(width, height);
                            }
                        }
                    }
                }
            }

            if (fileType.Equals("P3"))
            {
                Bitmap bitmap = new Bitmap(width, height);

            }
            else if(fileType.Equals("P6"))
            {
                Bitmap bitmap = new Bitmap(width, height);
                var binRead = new BinaryReader(new FileStream(file, FileMode.Open));

                //ile linii z początku pliku muszę ominąć
                int counter = 0;
                for (int i = 0; i < width * height; i++)
                {
                    if (binRead.ReadByte() == '\n')
                    {
                        counter++;
                    }  
                    if (counter == linesToAvoidP6)
                    {
                        break;
                    }
                        
                }
                for (int i = 0; i < height; i++)
                {
                    for(int j = 0; j < width; j++)
                    {
                        int Red = (int)(binRead.ReadByte() * (double)(255 / maxColor));
                        int Green = (int)(binRead.ReadByte() * (double)(255 / maxColor));
                        int Blue = (int)(binRead.ReadByte() * (double)(255 / maxColor));

                        System.Drawing.Color newColor = System.Drawing.Color.FromArgb(Red, Green, Blue);
                        bitmap.SetPixel(j, i,newColor);
                    }
                }
                return bitmap;
            }
            return new Bitmap(0, 0);
        }
    }
}
