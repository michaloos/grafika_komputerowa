using Microsoft.Win32;
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
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using System.Windows.Interop;

namespace Grafika
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region PS1

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
            colorRGBCube();
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

        #endregion

        #region PS2
        private void uploadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPEG Image|*.jpg;*.jpeg|PPM Image|*.ppm";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    zoomScale = 1;
                    ScaleTransform scale = new ScaleTransform(zoomScale, zoomScale);
                    Image.LayoutTransform = scale;
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
                catch
                {
                    MessageBoxResult result = MessageBox.Show("Podczas próby odczytu pliku coś poszło nie tak.");
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
                    MessageBoxResult result = MessageBox.Show("Podczas próby zapisu pliku coś poszło nie tak.");
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
                List<System.Drawing.Color> colors = new List<System.Drawing.Color>();
                for (int i = 0; i < lines.Count; i++)
                {
                    List<string> splitedLine = lines[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (splitedLine.Count % 3 != 0)
                    {
                        List<string> nextLine = lines[i + 1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        for(int k = 0; k < nextLine.Count; k++)
                        {
                            splitedLine.Add(nextLine[k]);
                        }
                        i++;
                        nextLine.Clear();
                        continue;
                    }
                    if(splitedLine.Count % 3 == 0)
                    {
                        for (int j = 0; j < splitedLine.Count; j += 3)
                        {
                            int Red = Int32.Parse(splitedLine[j]) * (255 / maxColor);
                            int Green = Int32.Parse(splitedLine[j + 1]) * (255 / maxColor);
                            int Blue = Int32.Parse(splitedLine[j + 2]) * (255 / maxColor);

                            System.Drawing.Color newColor = System.Drawing.Color.FromArgb(Red, Green, Blue);
                            colors.Add(newColor);
                        }
                    }
                }

                int row = 0;
                int column = 0;

                foreach (var color in colors)
                {
                    if (column < width)
                    {
                        bitmap.SetPixel(column, row, color);
                        column++;
                    }
                    else if (row < height)
                    {
                        column = 0;
                        row++;
                        bitmap.SetPixel(column, row, color);
                        column++;
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Plik jest większy niż zostało to zapisane w pliku.");
                    }
                }
                return bitmap;
                
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
                        bitmap.SetPixel(j, i, newColor);
                    }
                }
                return bitmap;
            }
            return new Bitmap(0, 0);
        }

        public double zoomScale = 1;

        private void zoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (Image.Source == null)
                return;
            
            if(zoomScale < 20)
            {
                zoomScale += 2;
            }

            ScaleTransform scale = new ScaleTransform(zoomScale, zoomScale);
            Image.LayoutTransform = scale;
        }

        private void zoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (Image.Source == null)
                return;

            if (zoomScale > 0.4)
            {
                zoomScale -= 0.4;
            }

            ScaleTransform scale = new ScaleTransform(zoomScale, zoomScale);
            Image.LayoutTransform = scale;
        }

        #endregion

        #region PS3

        private int colorMode = 0;
        //1 == RGB
        //2 == CMYK
        
        private void selectRGB_Checked(object sender, RoutedEventArgs e)
        {
            colorMode = 1;
        }

        private void selectCMYK_Checked(object sender, RoutedEventArgs e)
        {
            colorMode = 2;
        }

        private void redRGB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void greenRGB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void blueRGB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cyanCMYK_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void magentaCMYK_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void yellowCMYK_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void blackCMYK_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void color_change_RGB()
        {
            var R = redSlider.Value;
            var G = greenSlider.Value;
            var B = blueSlider.Value;
            if (colorMode == 1)
            {
                colorCanvas.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)R, (byte)G, (byte)B));
            }
        }

        private void color_change_CMYK()
        {
            double cyan = cyanSlider.Value / 100;
            double magenta = magentaSlider.Value / 100;
            double yellow = yellowSlider.Value / 100;
            double black = blackSlider.Value / 100;

            if (colorMode == 2)
            {
                var tempRed = 1 - Math.Min(1, cyan * (1 - black) + black);
                var tempGreen = 1 - Math.Min(1, magenta * (1 - black) + black);
                var tempBlue = 1 - Math.Min(1, yellow * (1 - black) + black);

                var R = 255 * tempRed;
                var G = 255 * tempGreen;
                var B = 255 * tempBlue;
                colorCanvas.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)R, (byte)G, (byte)B));
            }
        }

        private void fromRGBToCMYK()
        {
            double red = redSlider.Value / 255;
            double green = greenSlider.Value / 255;
            double blue = blueSlider.Value / 255;

            if(colorMode == 1 && (red > 0 || green > 0 || blue > 0))
            {
                double tempBlack = Math.Min(1 - red, Math.Min(1 - green, 1 - blue));
                double tempCyan = (1 - red - tempBlack) / (1 - tempBlack);
                double tempMagenta = (1 - green - tempBlack) / (1 - tempBlack);
                double tempYellow = (1 - blue - tempBlack) / (1 - tempBlack);

                blackSlider.Value = tempBlack * 100;
                cyanSlider.Value = tempCyan * 100;
                magentaSlider.Value = tempMagenta * 100;
                yellowSlider.Value = tempYellow * 100;
            }
            color_change_RGB();
        }
        
        private void fromCMYKToRGB()
        {
            double cyan = cyanSlider.Value / 100;
            double magenta = magentaSlider.Value / 100;
            double yellow = yellowSlider.Value / 100;
            double black = blackSlider.Value / 100;

            if(colorMode == 2 && (cyan > 0 || magenta > 0 || yellow > 0 || black > 0))
            {
                var tempRed = 1 - Math.Min(1,cyan*(1-black)+black);
                var tempGreen = 1 - Math.Min(1, magenta * (1 - black) + black);
                var tempBlue = 1 - Math.Min(1, yellow * (1 - black) + black);

                redSlider.Value = 255 * tempRed;
                greenSlider.Value = 255 * tempGreen;
                blueSlider.Value = 255 * tempBlue;
            }
            color_change_CMYK();
        } 

        private void redRGB_TextChanged(object sender, TextChangedEventArgs e)
        {
            fromRGBToCMYK();
        }

        private void greenRGB_TextChanged(object sender, TextChangedEventArgs e)
        {
            fromRGBToCMYK();
        }

        private void blueRGB_TextChanged(object sender, TextChangedEventArgs e)
        {
            fromRGBToCMYK();
        }

        private void cyanCMYK_TextChanged(object sender, TextChangedEventArgs e)
        {
            fromCMYKToRGB();
        }

        private void magentaCMYK_TextChanged(object sender, TextChangedEventArgs e)
        {
            fromCMYKToRGB();
        }

        private void yellowCMYK_TextChanged(object sender, TextChangedEventArgs e)
        {
            fromCMYKToRGB();
        }

        private void blackCMYK_TextChanged(object sender, TextChangedEventArgs e)
        {
            fromCMYKToRGB();
        }

        private void colorRGBCube()
        {
            Bitmap bitmapRed = new Bitmap(256, 256);
            for (int i = 255, x = 0; i >= 0 && x <= 255; i--, x++)
            {
                for (int j = 255, y = 0; j >= 0 && y <= 255; j--, y++)
                {
                    bitmapRed.SetPixel(x, y, System.Drawing.Color.FromArgb(255, i, j));
                }
            }
            BitmapImage bitmapImageRed = FromBitmapToBitmapImage(bitmapRed);
            ImageBrush brushRed = new ImageBrush(bitmapImageRed);
            sideWithRed.Material = new DiffuseMaterial(brushRed);

            Bitmap bitmapBlue = new Bitmap(256, 256);
            for (int i = 255, x = 0; i >= 0 && x <= 255; i--, x++)
            {
                for (int j = 255, y = 0; j >= 0 && y <= 255; j--, y++)
                {
                    bitmapBlue.SetPixel(x, y, System.Drawing.Color.FromArgb(i, j, 255));
                }
            }
            BitmapImage bitmapImageBlue = FromBitmapToBitmapImage(bitmapBlue);
            ImageBrush brushBlue = new ImageBrush(bitmapImageBlue);
            sideWithBlue.Material = new DiffuseMaterial(brushBlue);

            Bitmap bitmapGreen = new Bitmap(256, 256);
            for (int i = 255, x = 0; i >= 0 && x <= 255; i--, x++)
            {
                for (int j = 255, y = 0; j >= 0 && y <= 255; j--, y++)
                {
                    bitmapGreen.SetPixel(x, y, System.Drawing.Color.FromArgb(i, 255, j));
                }
            }
            BitmapImage bitmapImageGreen = FromBitmapToBitmapImage(bitmapGreen);
            ImageBrush brushGreen = new ImageBrush(bitmapImageGreen);
            sideWithGreen.Material = new DiffuseMaterial(brushGreen);

            Bitmap bitmapBlackRedGreen = new Bitmap(256, 256);
            for (int i = 255, x = 0; i >= 0 && x <= 255; i--, x++)
            {
                for (int j = 255, y = 0; j >= 0 && y <= 255; j--, y++)
                {
                    bitmapBlackRedGreen.SetPixel(x, y, System.Drawing.Color.FromArgb(i, j, 0));
                }
            }
            BitmapImage bitmapImageBlackRedGreen = FromBitmapToBitmapImage(bitmapBlackRedGreen);
            ImageBrush brushBlackRedGreen = new ImageBrush(bitmapImageBlackRedGreen);
            sideWithBlackRedGreen.Material = new DiffuseMaterial(brushBlackRedGreen);

            Bitmap bitmapBlackRedBlue = new Bitmap(256, 256);
            for (int i = 255, x = 0; i >= 0 && x <= 255; i--, x++)
            {
                for (int j = 255, y = 0; j >= 0 && y <= 255; j--, y++)
                {
                    bitmapBlackRedBlue.SetPixel(x, y, System.Drawing.Color.FromArgb(i, 0, j));
                }
            }
            BitmapImage bitmapImageBlackRedBlue = FromBitmapToBitmapImage(bitmapBlackRedBlue);
            ImageBrush brushBlackRedBlue = new ImageBrush(bitmapImageBlackRedBlue);
            sideWithBlackRedBlue.Material = new DiffuseMaterial(brushBlackRedBlue);

            Bitmap bitmapBlackGreenBlue = new Bitmap(256, 256);
            for (int i = 255, x = 0; i >= 0 && x <= 255; i--, x++)
            {
                for (int j = 255, y = 0; j >= 0 && y <= 255; j--, y++)
                {
                    bitmapBlackGreenBlue.SetPixel(x, y, System.Drawing.Color.FromArgb(0, i, j));
                }
            }
            BitmapImage bitmapImageBlackGreenBlue = FromBitmapToBitmapImage(bitmapBlackGreenBlue);
            ImageBrush brushBlackGreenBlue = new ImageBrush(bitmapImageBlackGreenBlue);
            sideWithBlackGreenBlue.Material = new DiffuseMaterial(brushBlackGreenBlue);
        }

        private void angleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = angleSlider.Value;
            myAngleRotation1.Angle = value;
            myAngleRotation2.Angle = value;
            myAngleRotation3.Angle = value;
            myAngleRotation4.Angle = value;
            myAngleRotation5.Angle = value;
            myAngleRotation6.Angle = value;
        }

        private void axisSliderx_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = axisSliderx.Value;
            var y = myAngleRotation1.Axis.Y;
            var z = myAngleRotation1.Axis.Z;
            myAngleRotation1.Axis = new Vector3D(value, y, z);
            myAngleRotation2.Axis = new Vector3D(value, y, z);
            myAngleRotation3.Axis = new Vector3D(value, y, z);
            myAngleRotation4.Axis = new Vector3D(value, y, z);
            myAngleRotation5.Axis = new Vector3D(value, y, z);
            myAngleRotation6.Axis = new Vector3D(value, y, z);
        }

        private void axisSlidery_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = axisSliderx.Value;
            var x = myAngleRotation1.Axis.X;
            var z = myAngleRotation1.Axis.Z;
            myAngleRotation1.Axis = new Vector3D(x, value, z);
            myAngleRotation2.Axis = new Vector3D(x, value, z);
            myAngleRotation3.Axis = new Vector3D(x, value, z);
            myAngleRotation4.Axis = new Vector3D(x, value, z);
            myAngleRotation5.Axis = new Vector3D(x, value, z);
            myAngleRotation6.Axis = new Vector3D(x, value, z);
        }

        private void axisSliderz_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = axisSliderx.Value;
            var x = myAngleRotation1.Axis.X;
            var y = myAngleRotation1.Axis.Y;
            myAngleRotation1.Axis = new Vector3D(x, y, value);
            myAngleRotation2.Axis = new Vector3D(x, y, value);
            myAngleRotation3.Axis = new Vector3D(x, y, value);
            myAngleRotation4.Axis = new Vector3D(x, y, value);
            myAngleRotation5.Axis = new Vector3D(x, y, value);
            myAngleRotation6.Axis = new Vector3D(x, y, value);
        }





        #endregion

        #region PS4

        private Bitmap originBitmap;
        private void uploadFIlePS4_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPEG Image|*.jpg;*.jpeg";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    zoomScale = 1;
                    ScaleTransform scale = new ScaleTransform(zoomScale, zoomScale);
                    Image.LayoutTransform = scale;
                    string extension = Path.GetExtension(openFileDialog.FileName);

                    Bitmap bitmap = new Bitmap(openFileDialog.FileName);
                    originBitmap = bitmap;
                    BitmapImage bitmapImage = FromBitmapToBitmapImage(bitmap);
                    ps4Image.Source = bitmapImage;
                    
                }
                catch
                {
                    MessageBoxResult result = MessageBox.Show("Podczas próby odczytu pliku coś poszło nie tak.");
                }
            }
        }

        private void filtrWygladz_Click(object sender, RoutedEventArgs e)
        {
            if (ps4Image.Source == null)
            {
                return;
            }
            indicator.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps4Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            var task = Task.Run(() =>
            {
                for (int i = 1; i < tempBM.Width - 1; i++)
                {
                    for (int j = 1; j < tempBM.Height - 1; j++)
                    {

                        int redValue = 0;
                        int greenValue = 0;
                        int blueValue = 0;
                        System.Drawing.Color[] colors = new System.Drawing.Color[9];
                        colors[0] = tempBM.GetPixel(i, j);
                        colors[1] = tempBM.GetPixel(i - 1, j - 1);
                        colors[2] = tempBM.GetPixel(i - 1, j);
                        colors[3] = tempBM.GetPixel(i - 1, j + 1);
                        colors[4] = tempBM.GetPixel(i, j - 1);
                        colors[5] = tempBM.GetPixel(i, j + 1);
                        colors[6] = tempBM.GetPixel(i + 1, j - 1);
                        colors[7] = tempBM.GetPixel(i + 1, j);
                        colors[8] = tempBM.GetPixel(i + 1, j + 1);
                        for (int k = 0; k < 9; k++)
                        {
                            redValue += colors[k].R;
                            greenValue += colors[k].G;
                            blueValue += colors[k].B;
                        }

                        int averageR = redValue / 9;
                        int averageG = greenValue / 9;
                        int averageB = blueValue / 9;

                        tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(averageR, averageG, averageB));
                    }
                }
            });

            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicator.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps4Image.Source = newBitmap;
                    }
                }));

            });
        }

        private void filtrMediana_Click(object sender, RoutedEventArgs e)
        {
            if (ps4Image.Source == null)
            {
                return;
            }
            indicator.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps4Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            var task = Task.Run(() => 
            {
                for (int i = 1; i < tempBM.Width - 1; i++)
                {
                    for (int j = 1; j < tempBM.Height - 1; j++)
                    {

                        int[] arrayRed = new int[9];
                        int[] arrayGreen = new int[9];
                        int[] arrayBlue = new int[9];
                        System.Drawing.Color[] colors = new System.Drawing.Color[9];
                        colors[0] = tempBM.GetPixel(i, j);
                        colors[1] = tempBM.GetPixel(i - 1, j - 1);
                        colors[2] = tempBM.GetPixel(i - 1, j);
                        colors[3] = tempBM.GetPixel(i - 1, j + 1);
                        colors[4] = tempBM.GetPixel(i, j - 1);
                        colors[5] = tempBM.GetPixel(i, j + 1);
                        colors[6] = tempBM.GetPixel(i + 1, j - 1);
                        colors[7] = tempBM.GetPixel(i + 1, j);
                        colors[8] = tempBM.GetPixel(i + 1, j + 1);
                        for (int k = 0; k < 9; k++)
                        {
                            arrayRed[k] = colors[k].R;
                            arrayGreen[k] = colors[k].G;
                            arrayBlue[k] = colors[k].B;
                        }
                        Array.Sort(arrayRed);
                        Array.Sort(arrayGreen);
                        Array.Sort(arrayBlue);

                        int medianaR = arrayRed[4];
                        int medianaG = arrayGreen[4];
                        int medianaB = arrayBlue[4];

                        tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(medianaR, medianaG, medianaB));
                    }
                } 
            });
            
            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicator.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps4Image.Source = newBitmap;
                    }              
                }));
                
            });
            
        }

        private void filtrWykrywKraw_Click(object sender, RoutedEventArgs e)
        {
            if (ps4Image.Source == null)
            {
                return;
            }
            if (grayScale == false)
            {
                return;
            }

            int[,] maskx = new int[,] 
            {
                {-1,0,1 },
                {-2,0,2 },
                {-1,0,1 }
            };
            int[,] masky = new int[,]
            {
                {1,2,1 },
                {0,0,0 },
                {-1,-2,-1 }
            };

            indicator.IsBusy = true;

            Bitmap bitmap = ImageSourceToBitmap(ps4Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;

            var task = Task.Run(() =>
            {
                for (int i = 1; i < tempBM.Width - 1; i++)
                {
                    for (int j = 1; j < tempBM.Height - 1; j++)
                    {

                        int newRedX = 0, newRedY = 0;
                        int newGreenX = 0, newGreenY = 0;
                        int newBlueX = 0, newBlueY = 0;
                        System.Drawing.Color[] colors = new System.Drawing.Color[9];
                        
                        colors[0] = tempBM.GetPixel(i - 1, j - 1); 
                        colors[1] = tempBM.GetPixel(i - 1, j);
                        colors[2] = tempBM.GetPixel(i - 1, j + 1); 
                        colors[3] = tempBM.GetPixel(i, j - 1);
                        colors[4] = tempBM.GetPixel(i, j);
                        colors[5] = tempBM.GetPixel(i, j + 1); 
                        colors[6] = tempBM.GetPixel(i + 1, j - 1); 
                        colors[7] = tempBM.GetPixel(i + 1, j);
                        colors[8] = tempBM.GetPixel(i + 1, j + 1);

                        int v = 0;
                        for(int k = 0; k < 3; k++)
                        {
                            for(int l = 0; l < 3; l++)
                            {
                                newRedX += maskx[k, l] * colors[v].R;
                                newGreenX += maskx[k, l] * colors[v].G;
                                newBlueX += maskx[k, l] * colors[v].B;

                                newRedY += masky[k, l] * colors[v].R;
                                newGreenY += masky[k, l] * colors[v].G;
                                newBlueY += masky[k, l] * colors[v].B;

                                v++;
                            }
                        }

                        int newRed = 0;
                        int newGreen = 0;
                        int newBlue = 0;

                        newRed = (int)Math.Sqrt((newRedX * newRedX) + (newRedY * newRedY));
                        newGreen = (int)Math.Sqrt((newGreenX * newGreenX) + (newGreenY * newGreenY));
                        newBlue = (int)Math.Sqrt((newBlueX * newBlueX) + (newBlueY * newBlueY));

                        newRed = newRed > 255 ? 255 : newRed;
                        newGreen = newGreen > 255 ? 255 : newGreen;
                        newBlue = newBlue > 255 ? 255 : newBlue;

                        newRed = newRed < 0 ? 0 : newRed;
                        newGreen = newGreen < 0 ? 0 : newGreen;
                        newBlue = newBlue < 0 ? 0 : newBlue;

                        tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(newRed, newGreen, newBlue));
                    }
                }
            });

            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicator.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps4Image.Source = newBitmap;
                    }
                }));

            });
        }

        private void filtrGornPrzepustWyostrz_Click(object sender, RoutedEventArgs e)
        {

        }

        private void filtrGaussa_Click(object sender, RoutedEventArgs e)
        {
            if (ps4Image.Source == null)
            {
                return;
            }

            int[,] mask = new int[,]
            {
                {1,2,1 }, // i-1,j-1 | i-1,j | i-1,j+1
                {2,4,2 }, // i,j-1   | i,j   | i,j+1
                {1,2,1 }  // i+1,j-1 | i+1,j | i+1,j+1
            };
            indicator.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps4Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            var task = Task.Run(() =>
            {
                for (int i = 1; i < tempBM.Width - 1; i++)
                {
                    for (int j = 1; j < tempBM.Height - 1; j++)
                    {

                        int newRed = 0;
                        int newGreen = 0;
                        int newBlue = 0;
                        System.Drawing.Color[] colors = new System.Drawing.Color[9];
                        colors[0] = tempBM.GetPixel(i, j); // 4
                        colors[1] = tempBM.GetPixel(i - 1, j - 1); // 1
                        colors[2] = tempBM.GetPixel(i - 1, j); // 2
                        colors[3] = tempBM.GetPixel(i - 1, j + 1); // 1
                        colors[4] = tempBM.GetPixel(i, j - 1); // 2
                        colors[5] = tempBM.GetPixel(i, j + 1); // 2
                        colors[6] = tempBM.GetPixel(i + 1, j - 1); // 1
                        colors[7] = tempBM.GetPixel(i + 1, j); // 2
                        colors[8] = tempBM.GetPixel(i + 1, j + 1); // 1

                        newRed = (colors[0].R * mask[1, 1] + colors[1].R * mask[0, 0] + colors[2].R * mask[0, 1]
                                    + colors[3].R * mask[0, 2] + colors[4].R * mask[1, 0] + colors[5].R * mask[1, 2]
                                    + colors[6].R * mask[2, 0] + colors[7].R * mask[2, 1] + colors[8].R * mask[2, 2]) / 16;
                        newGreen = (colors[0].G * mask[1, 1] + colors[1].G * mask[0, 0] + colors[2].G * mask[0, 1]
                                    + colors[3].G * mask[0, 2] + colors[4].G * mask[1, 0] + colors[5].G * mask[1, 2]
                                    + colors[6].G * mask[2, 0] + colors[7].G * mask[2, 1] + colors[8].G * mask[2, 2]) / 16;
                        newBlue = (colors[0].B * mask[1, 1] + colors[1].B * mask[0, 0] + colors[2].B * mask[0, 1]
                                    + colors[3].B * mask[0, 2] + colors[4].B * mask[1, 0] + colors[5].B * mask[1, 2]
                                    + colors[6].B * mask[2, 0] + colors[7].B * mask[2, 1] + colors[8].B * mask[2, 2]) / 16;

                        tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(newRed,newGreen,newBlue));
                    }
                }
            });

            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicator.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps4Image.Source = newBitmap;
                    }
                }));

            });
        }

        private void getBackToOriginal_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = FromBitmapToBitmapImage(originBitmap);
            ps4Image.Source = bitmapImage;
            grayScale = false;
        }

        private void okDodawanie_Click(object sender, RoutedEventArgs e)
        {
            if (ps4Image.Source == null)
            {
                return;
            }

            Bitmap tempBM = ImageSourceToBitmap(ps4Image.Source);

            var ifR = Double.TryParse(dodawanieR.Text, out double addR);
            var ifG = Double.TryParse(dodawanieG.Text, out double addG);
            var ifB = Double.TryParse(dodawanieB.Text, out double addB);

            if (ifR)
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        if (color.R != 255)
                        {
                            if (color.R + addR >= 255)
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(255, color.G, color.B));
                            }
                            else
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb((int)(color.R + addR), color.G, color.B));
                            }
                        }
                    }
                }
            }

            if (ifG)
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        if (color.G != 255)
                        {
                            if (color.G + addG >= 255)
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, 255, color.B));
                            }
                            else
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, (int)(color.G + addG), color.B));
                            }
                        }                    
                    }
                }
            }
            if (ifB)
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        if (color.B != 255)
                        {
                            if (color.B + addB >= 255)
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, color.G, 255));
                            }
                            else
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, color.G, (int)(color.B + addB)));
                            }
                        }                    
                    }
                }
            }
            BitmapImage bitmapImage = FromBitmapToBitmapImage(tempBM);
            ps4Image.Source = bitmapImage;
        }

        private void okOdejmowanie_Click(object sender, RoutedEventArgs e)
        {
            if (ps4Image.Source == null)
            {
                return;
            }

            Bitmap tempBM = ImageSourceToBitmap(ps4Image.Source);

            var ifR = Double.TryParse(odejmowanieR.Text, out double substractR);
            var ifG = Double.TryParse(odejmowanieG.Text, out double substractG);
            var ifB = Double.TryParse(odejmowanieB.Text, out double substractB);

            if (ifR)
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        if (color.R != 0)
                        {
                            if (color.R - substractR <= 0)
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(0, color.G, color.B));
                            }
                            else
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb((int)(color.R - substractR), color.G, color.B));
                            }
                        }
                    }
                }
            }

            if (ifG)
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        if (color.G != 0)
                        {
                            if (color.G - substractG <= 0)
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, 0, color.B));
                            }
                            else
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, (int)(color.G - substractG), color.B));
                            }
                        }
                    }
                }
            }
            if (ifB)
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        if (color.B != 0)
                        {
                            if (color.B - substractB <= 0)
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, color.G, 0));
                            }
                            else
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, color.G, (int)(color.B - substractB)));
                            }
                        }
                    }
                }
            }
            BitmapImage bitmapImage = FromBitmapToBitmapImage(tempBM);
            ps4Image.Source = bitmapImage;
        }

        private void okMnozenie_Click(object sender, RoutedEventArgs e)
        {
            if (ps4Image.Source == null)
            {
                return;
            }

            Bitmap tempBM = ImageSourceToBitmap(ps4Image.Source);

            var ifR = Double.TryParse(mnozenieR.Text, out double multiR);
            var ifG = Double.TryParse(mnozenieG.Text, out double multiG);
            var ifB = Double.TryParse(mnozenieB.Text, out double multiB);

            if (ifR)
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        if (color.R * multiR <= 0)
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(0, color.G, color.B));
                        }
                        else if (color.R * multiR >= 255)
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(255, color.G, color.B));
                        }
                        else
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb((int)(color.R * multiR), color.G, color.B));
                        }
                    }
                }
            }

            if (ifG)
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        if (color.G * multiG <= 0)
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, 0, color.B));
                        }
                        else if (color.G * multiG >= 255)
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, 255, color.B));
                        }
                        else
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, (int)(color.G * multiG), color.B));
                        }
                    }
                }
            }
            if (ifB)
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        if (color.B * multiB <= 0)
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, color.G, 0));
                        }
                        else if (color.B * multiB >= 255)
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, color.G, 255));
                        }
                        else
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, color.G, (int)(color.B * multiB)));
                        }
                    }
                }
            }
            BitmapImage bitmapImage = FromBitmapToBitmapImage(tempBM);
            ps4Image.Source = bitmapImage;
        }

        private void okDzielenie_Click(object sender, RoutedEventArgs e)
        {
            if (ps4Image.Source == null)
            {
                return;
            }

            Bitmap tempBM = ImageSourceToBitmap(ps4Image.Source);

            var ifR = Double.TryParse(dzielenieR.Text, out double divisionR);
            var ifG = Double.TryParse(dzielenieG.Text, out double divisionG);
            var ifB = Double.TryParse(dzielenieB.Text, out double divisionB);

            if (ifR)
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        if (color.R / divisionR <= 0)
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(0, color.G, color.B));
                        }
                        else if (color.R / divisionR >= 255)
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(255, color.G, color.B));
                        }
                        else
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb((int)(color.R / divisionR), color.G, color.B));
                        }
                    }
                }
            }

            if (ifG)
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        if (color.G / divisionG <= 0)
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, 0, color.B));
                        }
                        else if (color.G / divisionG >= 255)
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, 255, color.B));
                        }
                        else
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, (int)(color.G / divisionG), color.B));
                        }
                    }
                }
            }
            if (ifB)
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        if (color.B / divisionB <= 0)
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, color.G, 0));
                        }
                        else if (color.B / divisionB >= 255)
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, color.G, 255));
                        }
                        else
                        {
                            tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(color.R, color.G, (int)(color.B / divisionB)));
                        }
                    }
                }
            }
            BitmapImage bitmapImage = FromBitmapToBitmapImage(tempBM);
            ps4Image.Source = bitmapImage;
        }

        private bool grayScale = false;

        private void grayNO1_Click(object sender, RoutedEventArgs e)
        {
            if (ps4Image.Source == null)
            {
                return;
            }
            indicator.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps4Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            var task = Task.Run(() => 
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        byte grayColor = (byte)(0.21 * color.R + 0.71 * color.G + 0.071 * color.B);
                        tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(grayColor, grayColor, grayColor));
                    }
                }
            });

            grayScale = true;
            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicator.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps4Image.Source = newBitmap;
                    }
                }));

            });
        }

        private void grayNO2_Click(object sender, RoutedEventArgs e)
        {
            if (ps4Image.Source == null)
            {
                return;
            }

            indicator.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps4Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            var task = Task.Run(() =>
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        int grayColor = (color.R + color.G + color.B) / 3;
                        tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(grayColor, grayColor, grayColor));
                    }
                }
            });
            grayScale = true;
            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicator.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps4Image.Source = newBitmap;
                    }
                }));

            });
        }

        private void brightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(ps4Image.Source == null)
            {
                return;
            }

            int brightness = (int)brightnessSlider.Value;
            Bitmap tempBM = ImageSourceToBitmap(ps4Image.Source);

            System.Drawing.Color color;

            for(int i = 0; i < tempBM.Width; i++)
            {
                for(int j = 0; j < tempBM.Height; j++)
                {
                    color = tempBM.GetPixel(i, j);
                    int newRed = color.R + brightness;
                    int newGreen = color.G + brightness;
                    int newBlue = color.B + brightness;

                    if(newRed > 255)
                    {
                        newRed = 255;
                    }
                    
                    if (newGreen > 255)
                    {
                        newGreen = 255;
                    }
                    
                    if (newBlue > 255)
                    {
                        newBlue = 255;
                    }

                    tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(newRed,newGreen,newBlue));
                }
            }
            BitmapImage bitmapImage = FromBitmapToBitmapImage(tempBM);
            ps4Image.Source = bitmapImage;
        }

        private void lessBrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ps4Image.Source == null)
            {
                return;
            }

            int brightness = (int)lessBrightnessSlider.Value;
            Bitmap tempBM = ImageSourceToBitmap(ps4Image.Source);

            System.Drawing.Color color;

            for (int i = 0; i < tempBM.Width; i++)
            {
                for (int j = 0; j < tempBM.Height; j++)
                {
                    color = tempBM.GetPixel(i, j);
                    int newRed = color.R + brightness;
                    int newGreen = color.G + brightness;
                    int newBlue = color.B + brightness;

                    if (newRed < 0)
                    {
                        newRed = 1;
                    }

                    if (newGreen < 0)
                    {
                        newGreen = 1;
                    }

                    if (newBlue < 0)
                    {
                        newBlue = 1;
                    }
                    tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(newRed, newGreen, newBlue));
                }
            }
            BitmapImage bitmapImage = FromBitmapToBitmapImage(tempBM);
            ps4Image.Source = bitmapImage;
        }

        public Bitmap ImageSourceToBitmap(ImageSource image)
        {
            MemoryStream memory = new MemoryStream();
            BmpBitmapEncoder mem = new BmpBitmapEncoder();
            mem.Frames.Add(BitmapFrame.Create(BitmapFrame.Create((BitmapSource)image)));
            mem.Save(memory);
            memory.Position = 0;
            Bitmap bitmap = new Bitmap(memory);
            return bitmap;
        }

        private void dodawanieR_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dodawanieG_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dodawanieB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void odejmowanieR_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void odejmowanieG_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void odejmowanieB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void mnozenieR_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void mnozenieG_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void mnozenieB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dzielenieR_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dzielenieG_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dzielenieB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoom = e.NewValue;
            ScaleTransform scale = new ScaleTransform(zoom, zoom);
            ps4Image.LayoutTransform = scale;
        }
        #endregion

        private void binByUserValue_Click(object sender, RoutedEventArgs e)
        {
            if (ps5Image.Source == null || grayScaleValuePS5 == false || binUserValue.Text.Equals(""))
            {
                return;
            }
            indicatorPS5.IsBusy = true;
            int binValue = int.Parse(binUserValue.Text);
            Bitmap bitmap = ImageSourceToBitmap(ps5Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            var task = Task.Run(() =>
            {
                if (binValue < 0 || binValue > 255)
                {
                    MessageBoxResult result = MessageBox.Show("Wartość powinna mieścić się od 0 do 255!");
                }
                else
                {
                    for (int i = 0; i < tempBM.Width; i++)
                    {
                        for (int j = 0; j < tempBM.Height; j++)
                        {
                            if (tempBM.GetPixel(i, j).R >= binValue)
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.White);
                            }
                            else
                            {
                                tempBM.SetPixel(i, j, System.Drawing.Color.Black);
                            }
                        }
                    }
                }
            });
            

            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicatorPS5.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps5Image.Source = newBitmap;
                    }
                }));

            });
        }

        private Bitmap originalBitmapPS5;
        public bool grayScaleValuePS5 = false;

        private void getBackToOriginalPS5_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = FromBitmapToBitmapImage(originalBitmapPS5);
            ps5Image.Source = bitmapImage;
            grayScaleValuePS5 = false;
        }

        private void uploadFilePS5_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPEG Image|*.jpg;*.jpeg";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    zoomScale = 1;
                    ScaleTransform scale = new ScaleTransform(zoomScale, zoomScale);
                    Image.LayoutTransform = scale;
                    string extension = Path.GetExtension(openFileDialog.FileName);

                    Bitmap bitmap = new Bitmap(openFileDialog.FileName);
                    originalBitmapPS5 = bitmap;
                    BitmapImage bitmapImage = FromBitmapToBitmapImage(bitmap);
                    ps5Image.Source = bitmapImage;

                }
                catch
                {
                    MessageBoxResult result = MessageBox.Show("Podczas próby odczytu pliku coś poszło nie tak.");
                }
            }
        }

        private void zoomSliderPS5_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoom = e.NewValue;
            ScaleTransform scale = new ScaleTransform(zoom, zoom);
            ps5Image.LayoutTransform = scale;
        }

        private void grayScalePS5_Click(object sender, RoutedEventArgs e)
        {
            if (ps5Image.Source == null)
            {
                return;
            }
            indicatorPS5.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps5Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            var task = Task.Run(() =>
            {
                for (int i = 0; i < tempBM.Width; i++)
                {
                    for (int j = 0; j < tempBM.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        byte grayColor = (byte)(0.21 * color.R + 0.71 * color.G + 0.071 * color.B);
                        tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(grayColor, grayColor, grayColor));
                    }
                }
            });

            grayScaleValuePS5 = true;
            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicatorPS5.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps5Image.Source = newBitmap;
                    }
                }));

            });
        }

        private void binUserValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void histogramShow_Click(object sender, RoutedEventArgs e)
        {
            HistogramImage histogram = new HistogramImage(this);
            histogram.Show();
        }

        private void percentBlack_Click(object sender, RoutedEventArgs e)
        {
            if (ps5Image.Source == null || grayScaleValuePS5 == false || percentBlackValue.Text.Equals(""))
            {
                return;
            }

            indicatorPS5.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps5Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            double percentVal = Double.Parse(percentBlackValue.Text);
            var task = Task.Run(() =>
            {    
                int pixelAmount = tempBM.Width * tempBM.Height;
                
                double percentValBy100 = percentVal / 100;
                double howManyPixels = pixelAmount * percentValBy100;
                getColors(tempBM);
                getColorsArray();
                int suma = 0;
                int binLevel;
                for (int k = 0; k < colorsArray.Length; k++)
                {
                    suma += colorsArray[k];
                    if (suma >= howManyPixels)
                    {
                        binLevel = k;
                        for (int i = 0; i < tempBM.Width; i++)
                        {
                            for (int j = 0; j < tempBM.Height; j++)
                            {
                                if (tempBM.GetPixel(i, j).R >= binLevel)
                                {
                                    tempBM.SetPixel(i, j, System.Drawing.Color.White);
                                }
                                else
                                {
                                    tempBM.SetPixel(i, j, System.Drawing.Color.Black);
                                }
                            }
                        }
                        break;
                    }
                }
            });
            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicatorPS5.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps5Image.Source = newBitmap;
                    }
                }));

            });
            
        }

        private void percentBlackValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private byte[] grayscale;
        private void getColors(Bitmap bitmap)
        {
            int ilosc = 0;

            grayscale = new byte[bitmap.Height * bitmap.Width];

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    System.Drawing.Color color = bitmap.GetPixel(i, j);
                    grayscale[ilosc] = color.R;
                    ilosc++;
                }
            }
        }

        int[] colorsArray = new int[256];

        private void getColorsArray()
        {
            for (int i = 0; i < colorsArray.Length; i++)
            {
                colorsArray[i] = 0;
            }
            foreach (var value in grayscale)
            {
                colorsArray[value] += 1;
            }   
        }

    }
    
}
