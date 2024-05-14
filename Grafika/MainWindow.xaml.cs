using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

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
            ColorRgbCube();
        }

        private void DrawingChecked(object sender, RoutedEventArgs e) => ACTION_MODE = 1;

        private void MovingChecked(object sender, RoutedEventArgs e) => ACTION_MODE = 2;

        private void ChangeShapeChecked(object sender, RoutedEventArgs e) => ACTION_MODE = 3;

        private void LineChecked(object sender, RoutedEventArgs e) => SHAPE_MODE = 1;

        private void RectangleChecked(object sender, RoutedEventArgs e) => SHAPE_MODE = 2;

        private void EllipseChecked(object sender, RoutedEventArgs e) => SHAPE_MODE = 3;

        private void CanvasFieldMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ACTION_MODE != 1)
            {
                return;
            }

            startPoints = e.GetPosition(CanvasField);
            shape = null;
        }

        private void CanvasFieldMouseMove(object sender, MouseEventArgs e)
        {
            if (SHAPE_MODE == 0 || ACTION_MODE == 0)
            {
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (ACTION_MODE == 1)
                {
                    if (shape != null)
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

                    shape.MouseDown += SelectShape; //event do zaznaczenia wybranej figury
                    shape.MouseMove += MoveShape; //event do przesuwania myszą figury
                    shape.MouseUp += MovedShape; //event do "skończenia" przesuwania figury

                    CanvasField.Children.Add(shape);
                }
            }
        }

        private void MovedShape(object sender, MouseEventArgs e)
        {
            shape = sender as Shape;
            shape.ReleaseMouseCapture();
        }

        private void MoveShape(object sender, MouseEventArgs e)
        {
            if (ACTION_MODE == 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Shape dragShape)
                {
                    System.Windows.Point currentPosition = e.GetPosition(CanvasField);
                    var transform = dragShape.RenderTransform as TranslateTransform ?? new TranslateTransform();
                    transform.X = TranslateTransform.X + (currentPosition.X - clickPosition.X);
                    transform.Y = TranslateTransform.Y + (currentPosition.Y - clickPosition.Y);
                    dragShape.RenderTransform = new TranslateTransform(transform.X, transform.Y);
                }
            }
            if (ACTION_MODE == 3 && e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Shape changeShape)
                {
                    if (changeShape is Line)
                    {
                        _ = e.GetPosition(CanvasField);
                    }
                }
            }
        }

        private void SelectShape(object sender, MouseEventArgs e)
        {
            if (ACTION_MODE != 1)
            {
                shape = sender as Shape;
                if (e.LeftButton == MouseButtonState.Pressed && ACTION_MODE == 2)
                {
                    TranslateTransform = shape.RenderTransform as TranslateTransform ?? new TranslateTransform();
                    clickPosition = e.GetPosition(CanvasField);
                    shape.CaptureMouse();
                }
            }
        }

        private void DrawLineClick(object sender, RoutedEventArgs e)
        {
            var x1Point = Double.TryParse(x1line.Text, out double x1);
            var y1Point = Double.TryParse(y1line.Text, out double y1);
            var x2Point = Double.TryParse(x2line.Text, out double x2);
            var y2Point = Double.TryParse(y2line.Text, out double y2);

            if (x1Point && y1Point && x2Point && y2Point)
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
                shape.MouseDown += SelectShape; //event do zaznaczenia wybranej figury
                shape.MouseMove += MoveShape; //event do przesuwania myszą figury
                shape.MouseUp += MovedShape; //event do "skończenia" przesuwania figury
                CanvasField.Children.Add(shape);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
            }
        }

        private void DrawRectangleClick(object sender, RoutedEventArgs e)
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

                shape.MouseDown += SelectShape; //event do zaznaczenia wybranej figury
                shape.MouseMove += MoveShape; //event do przesuwania myszą figury
                shape.MouseUp += MovedShape; //event do "skończenia" przesuwania figury

                CanvasField.Children.Add(shape);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
            }

        }

        private void DrawEllipseClick(object sender, RoutedEventArgs e)
        {
            var x1Point = Double.TryParse(x1ellipse.Text, out double x1);
            var y1Point = Double.TryParse(y1ellipse.Text, out double y1);
            var radius = Double.TryParse(radiusellipse.Text, out double r);

            if (x1Point && y1Point && radius)
            {
                shape = new Ellipse
                {
                    Width = Math.Abs(r * 2),
                    Height = Math.Abs(r * 2),
                    StrokeThickness = 5,
                    Stroke = System.Windows.Media.Brushes.Yellow
                };

                shape.SetValue(Canvas.LeftProperty, x1);
                shape.SetValue(Canvas.TopProperty, y1);

                shape.MouseDown += SelectShape; //event do zaznaczenia wybranej figury
                shape.MouseMove += MoveShape; //event do przesuwania myszą figury
                shape.MouseUp += MovedShape; //event do "skończenia" przesuwania figury

                CanvasField.Children.Add(shape);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Wprowadzone dane są niepoprawne!");
            }
        }

        private void LineBiggerClick(object sender, RoutedEventArgs e)
        {
            if (ACTION_MODE != 3 || shape == null || !(shape is Line))
            {
                return;
            }

            var changeLine = Double.TryParse(changeLineValue.Text, out double value);
            if (changeLine)
            {
                Line line = shape as Line;
                if (line.X2 < line.X1)
                {
                    value *= -1;
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

                line.X2 += a / 2;
                line.Y2 = m * line.X2 + c;
            }
            else
            {
                _ = MessageBox.Show("Wprowadzone dane są niepoprawne!");
            }
        }

        private void ChangeRectangleClick(object sender, RoutedEventArgs e)
        {
            if (ACTION_MODE != 3 || shape == null || shape is Line || shape is Ellipse)
            {
                return;
            }

            var width = Double.TryParse(changeWidth.Text, out double newWidth);
            var height = Double.TryParse(changeHeight.Text, out double newHeight);

            if (width && height)
            {
                System.Windows.Shapes.Rectangle rectangle = shape as System.Windows.Shapes.Rectangle;
                //nowe wartości nie mogą spowodować że szerokość/wysokość będzie mniejsza od zera
                if (rectangle.Width + newWidth <= 0 || rectangle.Height + newHeight <= 0)
                {
                    _ = MessageBox.Show("Wprowadzone dane są niepoprawne!");
                }
                else
                {
                    double x = Canvas.GetLeft((UIElement)rectangle) + rectangle.Width + newWidth;
                    double y = Canvas.GetTop((UIElement)rectangle) + rectangle.Height + newHeight;
                    //kształt figury nie może wyjść poza obszar do rysowania
                    if (x >= CanvasField.ActualWidth || y >= CanvasField.ActualHeight)
                    {
                        _ = MessageBox.Show("Wprowadzone dane są niepoprawne!");
                    }
                    else
                    {
                        rectangle.Width += newWidth;
                        rectangle.Height += newHeight;
                    }
                }
            }
            else
            {
                _ = MessageBox.Show("Wprowadzone dane są niepoprawne!");
            }
        }

        private void ChangeEllipseClick(object sender, RoutedEventArgs e)
        {
            if (ACTION_MODE != 3 || shape == null || shape is Line || shape is System.Windows.Shapes.Rectangle)
            {
                return;
            }

            var radius = Double.TryParse(changeRadius.Text, out double newRadius);

            if (radius)
            {
                Ellipse ellipse = shape as Ellipse;
                double oldRadius = ellipse.Width / 2;
                //nowy promień nie może być mniejszy od 0
                if (oldRadius + newRadius <= 0)
                {
                    _ = MessageBox.Show("Wprowadzone dane są niepoprawne!");
                }
                else
                {
                    double x = Canvas.GetLeft((UIElement)ellipse) + (2 * (oldRadius + newRadius));
                    double y = Canvas.GetTop((UIElement)ellipse) + (2 * (oldRadius + newRadius));
                    //kształt figury nie może wyjść poza obszar do rysowania
                    if (x >= CanvasField.ActualWidth || y >= CanvasField.ActualHeight)
                    {
                        _ = MessageBox.Show("Wprowadzone dane są niepoprawne!");
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
                _ = MessageBox.Show("Wprowadzone dane są niepoprawne!");
            }
        }

        #endregion

        #region PS2
        private void UploadFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JPEG Image|*.jpg;*.jpeg|PPM Image|*.ppm"
            };

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
                    _ = MessageBox.Show("Podczas próby odczytu pliku coś poszło nie tak.");
                }
            }
        }

        private void SaveFileClick(object sender, RoutedEventArgs e)
        {
            if (Image.Source == null)
            {
                return;
            }

            SaveFileDialog saveFile = new SaveFileDialog
            {
                Filter = "JPEG Image|*.jpg"
            };

            var save = new JpegBitmapEncoder();
            if (saveFile.ShowDialog() != false)
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
                    using (var stream = saveFile.OpenFile())
                    {
                        save.Save(stream);
                    }

                }
                catch
                {
                    _ = MessageBox.Show("Podczas próby zapisu pliku coś poszło nie tak.");
                }
            }

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
                        if (!String.IsNullOrEmpty(line) && line.StartsWith("#"))
                        {
                            int index = line.IndexOf("#");
                            if (index >= 0)
                            {
                                line = line.Substring(0, index);
                                linesToAvoidP6++;
                            }
                        }

                        if (!String.IsNullOrEmpty(line) && !line.StartsWith("#") && !fileType.Equals(""))
                        {
                            if (width != 0 && height != 0 && maxColor != 0 && !fileType.Equals(""))
                            {
                                lines.Add(line);
                            }
                            if (width == 0 && width == 0 && !fileType.Equals(""))
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
                                    width = correctValues[0];
                                }
                                if (correctValues.Count == 2)
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
                            else if (height == 0 && width != 0 && !fileType.Equals(""))
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
                            else if (height != 0 && width != 0 && maxColor == 0 && !fileType.Equals(""))
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
                        for (int k = 0; k < nextLine.Count; k++)
                        {
                            splitedLine.Add(nextLine[k]);
                        }
                        i++;
                        nextLine.Clear();
                        continue;
                    }
                    if (splitedLine.Count % 3 == 0)
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
                        _ = MessageBox.Show("Plik jest większy niż zostało to zapisane w pliku.");
                    }
                }
                return bitmap;

            }
            else if (fileType.Equals("P6"))
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
                    for (int j = 0; j < width; j++)
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

        private void ZoomInClick(object sender, RoutedEventArgs e)
        {
            if (Image.Source == null)
                return;

            if (zoomScale < 20)
            {
                zoomScale += 2;
            }

            ScaleTransform scale = new ScaleTransform(zoomScale, zoomScale);
            Image.LayoutTransform = scale;
        }

        private void ZoomOutClick(object sender, RoutedEventArgs e)
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

        private void SelectRgbChecked(object sender, RoutedEventArgs e) => colorMode = 1;

        private void SelectCmykChecked(object sender, RoutedEventArgs e) => colorMode = 2;

        private void RedRgbPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void GreenRgbPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BlueRGgbPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CyanCmykPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void MagentaCmykPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void YellowCmykPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BlackCmykPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ColorChangeRgb()
        {
            var R = redSlider.Value;
            var G = greenSlider.Value;
            var B = blueSlider.Value;
            if (colorMode == 1)
            {
                colorCanvas.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)R, (byte)G, (byte)B));
            }
        }

        private void ColorChangeCmyk()
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

        private void FromRgbToCmyk()
        {
            double red = redSlider.Value / 255;
            double green = greenSlider.Value / 255;
            double blue = blueSlider.Value / 255;

            if (colorMode == 1 && (red > 0 || green > 0 || blue > 0))
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
            ColorChangeRgb();
        }

        private void FromCmykToRgb()
        {
            double cyan = cyanSlider.Value / 100;
            double magenta = magentaSlider.Value / 100;
            double yellow = yellowSlider.Value / 100;
            double black = blackSlider.Value / 100;

            if (colorMode == 2 && (cyan > 0 || magenta > 0 || yellow > 0 || black > 0))
            {
                var tempRed = 1 - Math.Min(1, cyan * (1 - black) + black);
                var tempGreen = 1 - Math.Min(1, magenta * (1 - black) + black);
                var tempBlue = 1 - Math.Min(1, yellow * (1 - black) + black);

                redSlider.Value = 255 * tempRed;
                greenSlider.Value = 255 * tempGreen;
                blueSlider.Value = 255 * tempBlue;
            }
            ColorChangeCmyk();
        }

        private void RedRgbTextChanged(object sender, TextChangedEventArgs e) => FromRgbToCmyk();

        private void GreenRgbTextChanged(object sender, TextChangedEventArgs e) => FromRgbToCmyk();

        private void BlueRgbTextChanged(object sender, TextChangedEventArgs e) => FromRgbToCmyk();

        private void CyanCmykTextChanged(object sender, TextChangedEventArgs e) => FromCmykToRgb();

        private void MagentaCmykTextChanged(object sender, TextChangedEventArgs e) => FromCmykToRgb();

        private void YellowCmykTextChanged(object sender, TextChangedEventArgs e) => FromCmykToRgb();

        private void BlackCmykTextChanged(object sender, TextChangedEventArgs e) => FromCmykToRgb();

        private void ColorRgbCube()
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

        private void AngleSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = angleSlider.Value;
            myAngleRotation1.Angle = value;
            myAngleRotation2.Angle = value;
            myAngleRotation3.Angle = value;
            myAngleRotation4.Angle = value;
            myAngleRotation5.Angle = value;
            myAngleRotation6.Angle = value;
        }

        private void AxisSliderxValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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

        private void AxisSlideryValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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

        private void AxisSliderzValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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
        private void UploadFilePS4Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JPEG Image|*.jpg;*.jpeg"
            };

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
                    _ = MessageBox.Show("Podczas próby odczytu pliku coś poszło nie tak.");
                }
            }
        }

        private void FiltrWygladzClick(object sender, RoutedEventArgs e)
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

        private void FiltrMedianaClick(object sender, RoutedEventArgs e)
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

        private void FiltrWykrywKrawClick(object sender, RoutedEventArgs e)
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
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
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

        private void FiltrGornPrzepustWyostrzClick(object sender, RoutedEventArgs e)
        {

        }

        private void FiltrGaussaClick(object sender, RoutedEventArgs e)
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

        private void GetBackToOriginalClick(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = FromBitmapToBitmapImage(originBitmap);
            ps4Image.Source = bitmapImage;
            grayScale = false;
        }

        private void OkDodawanieClick(object sender, RoutedEventArgs e)
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

        private void OkOdejmowanieClick(object sender, RoutedEventArgs e)
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

        private void OkMnozenieClick(object sender, RoutedEventArgs e)
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

        private void OkDzielenieClick(object sender, RoutedEventArgs e)
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

        private void GrayNO1Click(object sender, RoutedEventArgs e)
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

        private void GrayNO2Click(object sender, RoutedEventArgs e)
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

        private void BrightnessSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ps4Image.Source == null)
            {
                return;
            }

            int brightness = (int)brightnessSlider.Value;
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

                    if (newRed > 255)
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

                    tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(newRed, newGreen, newBlue));
                }
            }
            BitmapImage bitmapImage = FromBitmapToBitmapImage(tempBM);
            ps4Image.Source = bitmapImage;
        }

        private void LessBrightnessSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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

        private void DodawanieRPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DodawanieGPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DodawanieBPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OdejmowanieRPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OdejmowanieGPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OdejmowanieBPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void MnozenieRPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void MnozenieGPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void MnozenieBPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DzielenieRPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DzielenieGPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DzielenieBPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ZoomSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoom = e.NewValue;
            ScaleTransform scale = new ScaleTransform(zoom, zoom);
            ps4Image.LayoutTransform = scale;
        }
        #endregion

        #region PS5

        private void BinByUserValueClick(object sender, RoutedEventArgs e)
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

        private void GetBackToOriginalPS5Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = FromBitmapToBitmapImage(originalBitmapPS5);
            ps5Image.Source = bitmapImage;
            grayScaleValuePS5 = false;
        }

        private void UploadFilePS5Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JPEG Image|*.jpg;*.jpeg"
            };

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
                    _ = MessageBox.Show("Podczas próby odczytu pliku coś poszło nie tak.");
                }
            }
        }

        private void ZoomSliderPS5ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoom = e.NewValue;
            ScaleTransform scale = new ScaleTransform(zoom, zoom);
            ps5Image.LayoutTransform = scale;
        }

        private void GrayScalePS5Click(object sender, RoutedEventArgs e)
        {
            if (ps5Image.Source == null)
            {
                return;
            }
            indicatorPS5.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps5Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            Bitmap setBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var task = Task.Run(() =>
            {
                for (int i = 0; i < setBitmap.Width; i++)
                {
                    for (int j = 0; j < setBitmap.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        byte grayColor = (byte)(0.21 * color.R + 0.71 * color.G + 0.071 * color.B);
                        setBitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(grayColor, grayColor, grayColor));
                    }
                }
            });

            grayScaleValuePS5 = true;
            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicatorPS5.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(setBitmap);
                    if (newBitmap != null)
                    {
                        ps5Image.Source = newBitmap;
                    }
                }));

            });
        }

        private void BinUserValuePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void HistogramShowClick(object sender, RoutedEventArgs e)
        {
            HistogramImage histogram = new HistogramImage(this);
            histogram.Show();
        }

        private void PercentBlackClick(object sender, RoutedEventArgs e)
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
                if (percentVal < 0 || percentVal > 100)
                {
                    MessageBoxResult result = MessageBox.Show("Podaj liczbę procentową mieszczącą się w zakresie 0-100.");
                }
                else
                {
                    int pixelAmount = tempBM.Width * tempBM.Height;

                    double percentValBy100 = percentVal / 100;
                    double howManyPixels = pixelAmount * percentValBy100;
                    GetColors(tempBM);
                    GetColorsArray();
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

        private void PercentBlackValuePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private byte[] grayscale;
        private void GetColors(Bitmap bitmap)
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

        readonly int[] colorsArray = new int[256];

        private void GetColorsArray()
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

        private void BinIterationClick(object sender, RoutedEventArgs e)
        {
            if (ps5Image.Source == null || grayScaleValuePS5 == false)
            {
                return;
            }

            indicatorPS5.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps5Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            Bitmap setBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var task = Task.Run(() =>
            {

                int minValue = 0;
                int maxValue = 0;

                GetColors(tempBM);
                GetColorsArray();
                for (int i = 0; i < colorsArray.Length; i++)
                {
                    if (colorsArray[i] != 0)
                    {
                        minValue = i;
                        break;
                    }
                }
                for (int i = colorsArray.Length - 1; i >= 0; i--)
                {
                    if (colorsArray[i] != 0)
                    {
                        maxValue = i;
                        break;
                    }
                }
                int t = (maxValue + minValue) / 2;
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

        #endregion

        #region PS6

        private int SELECTED_MODE_BEZIER = 0;
        // 1 dodaj punkty
        // 2 przesuwaj punkty
        private Shape shapePS5 = null;
        private List<Shape> shapes = new List<Shape>();
        private List<System.Windows.Point> bezierPoints = new List<System.Windows.Point>();

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            canvasPS5.Children.Clear();
            shapes = new List<Shape>();
            bezierPoints = new List<System.Windows.Point>();
            shape = null;
            xValue.Text = "";
            yValue.Text = "";
        }

        private void CreatePointChecked(object sender, RoutedEventArgs e) => SELECTED_MODE_BEZIER = 1;

        private void MovePointChecked(object sender, RoutedEventArgs e) => SELECTED_MODE_BEZIER = 2;

        private void CanvasPS5MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SELECTED_MODE_BEZIER != 1)
            {
                return;
            }
            if (SELECTED_MODE_BEZIER == 1)
            {
                System.Windows.Point point = e.GetPosition(canvasPS5);
                shapePS5 = new Ellipse
                {
                    Width = 14,
                    Height = 14,
                    StrokeThickness = 8,
                    Stroke = System.Windows.Media.Brushes.Black
                };
                shapePS5.SetValue(Canvas.LeftProperty, point.X - 7);
                shapePS5.SetValue(Canvas.TopProperty, point.Y - 7);

                shapePS5.MouseDown += SelectShapePS5; //event do zaznaczenia wybranej figury
                shapePS5.MouseMove += MoveShapePS5; //event do przesuwania myszą figury
                shapePS5.MouseUp += MovedShapePS5; //event do "skończenia" przesuwania figury

                canvasPS5.Children.Add(shapePS5);
                shapes.Add(shapePS5);

            }
            Lines();
            DrawBezier();
        }

        private void MovedShapePS5(object sender, MouseEventArgs e)
        {
            shapePS5 = sender as Shape;
            shapePS5.ReleaseMouseCapture();
        }

        private void MoveShapePS5(object sender, MouseEventArgs e)
        {
            if (SELECTED_MODE_BEZIER == 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Shape dragShape)
                {
                    System.Windows.Point currentPosition = e.GetPosition(canvasPS5);
                    dragShape.SetValue(Canvas.LeftProperty, currentPosition.X - 7);
                    dragShape.SetValue(Canvas.TopProperty, currentPosition.Y - 7);
                }
                Lines();
                DrawBezier();
            }

        }

        private void SelectShapePS5(object sender, MouseEventArgs e)
        {
            if (SELECTED_MODE_BEZIER == 2)
            {
                shapePS5 = sender as Shape;
                if (e.LeftButton == MouseButtonState.Pressed && SELECTED_MODE_BEZIER == 2)
                {
                    TranslateTransform = shapePS5.RenderTransform as TranslateTransform ?? new TranslateTransform();
                    clickPosition = e.GetPosition(canvasPS5);
                    shapePS5.CaptureMouse();
                }
            }
        }

        private void CanvasPS5MouseMove(object sender, MouseEventArgs e)
        {
            if (SELECTED_MODE_BEZIER != 2)
            {
                return;
            }
            else
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    shapePS5.MouseDown += SelectShapePS5; //event do zaznaczenia wybranej figury
                    shapePS5.MouseMove += MoveShapePS5; //event do przesuwania myszą figury
                    shapePS5.MouseUp += MovedShapePS5; //event do "skończenia" przesuwania figury
                }
            }
        }

        private void AddPointByXYClick(object sender, RoutedEventArgs e)
        {
            double x = Double.Parse(xValue.Text);
            double y = Double.Parse(yValue.Text);

            if (x == 0 || y == 0)
            {
                return;
            }

            System.Windows.Point point = new System.Windows.Point(x, y);
            shapePS5 = new Ellipse
            {
                Width = 14,
                Height = 14,
                StrokeThickness = 8,
                Stroke = System.Windows.Media.Brushes.Black
            };
            shapePS5.SetValue(Canvas.LeftProperty, point.X - 7);
            shapePS5.SetValue(Canvas.TopProperty, point.Y - 7);

            shapePS5.MouseDown += SelectShapePS5; //event do zaznaczenia wybranej figury
            shapePS5.MouseMove += MoveShapePS5; //event do przesuwania myszą figury
            shapePS5.MouseUp += MovedShapePS5; //event do "skończenia" przesuwania figury

            canvasPS5.Children.Add(shapePS5);
            shapes.Add(shapePS5);

            Lines();
            DrawBezier();
        }

        private System.Windows.Shapes.Path pathLine = null;

        private void Lines()
        {
            canvasPS5.Children.Clear();
            foreach (var shape in shapes)
            {
                canvasPS5.Children.Add(shape);
            }
            PathFigure figure = new PathFigure
            {
                StartPoint = new System.Windows.Point()
                {
                    X = Canvas.GetLeft(shapes.ElementAt(0)) + 7,
                    Y = Canvas.GetTop(shapes.ElementAt(0)) + 7,
                }
            };

            foreach (var point in shapes)
            {
                LineSegment segment = new LineSegment
                {
                    Point = new System.Windows.Point()
                    {
                        X = Canvas.GetLeft(point) + 7,
                        Y = Canvas.GetTop(point) + 7,
                    }
                };
                figure.Segments.Add(segment);
            }

            pathLine = new System.Windows.Shapes.Path()
            {
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 3,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection()
                    {
                        figure
                    }
                }
            };
            canvasPS5.Children.Add(pathLine);
        }

        private void BeziereCurve()
        {
            bezierPoints = new List<System.Windows.Point>();
            if (shapes.Count <= 2)
            {
                return;
            }
            int n = shapes.Count;
            for (double t = 0; t <= 1; t += 0.01)
            {
                double X = 0;
                double Y = 0;
                for (int i = 0; i < n; i++)
                {
                    double currentX = Canvas.GetLeft(shapes[i]) + 7;
                    double currentY = Canvas.GetTop(shapes[i]) + 7;

                    X += currentX * (Silnia(n - 1) / (Silnia(i) * Silnia(n - 1 - i))) * Math.Pow((1 - t), (n - 1 - i)) * Math.Pow(t, i);
                    Y += currentY * (Silnia(n - 1) / (Silnia(i) * Silnia(n - 1 - i))) * Math.Pow((1 - t), (n - 1 - i)) * Math.Pow(t, i);
                }
                X = Math.Round(X, 2);
                Y = Math.Round(Y, 2);
                bezierPoints.Add(new System.Windows.Point(X, Y));
            }
        }

        private void DrawBezier()
        {
            if (shapes.Count <= 2)
            {
                return;
            }

            BeziereCurve();


            PathFigure figure = new PathFigure
            {
                StartPoint = new System.Windows.Point()
                {
                    X = Canvas.GetLeft(shapes.ElementAt(0)) + 7,
                    Y = Canvas.GetTop(shapes.ElementAt(0)) + 7,
                }
            };

            foreach (var point in bezierPoints)
            {
                LineSegment segment = new LineSegment
                {
                    Point = point
                };
                figure.Segments.Add(segment);
            }

            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path()
            {
                Stroke = System.Windows.Media.Brushes.LimeGreen,
                StrokeThickness = 2,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection()
                    {
                        figure
                    }
                }
            };

            canvasPS5.Children.Add(path);
        }

        private int Silnia(int x)
        {
            if (x <= 1)
                return 1;
            else
                return x * Silnia(x - 1);
        }
        #endregion

        #region PS7

        private int PS7_MODE;
        // 1 = tworzenie
        // 2 = przesuwanie
        // 3 = obracanie
        // 4 = skalowanie
        // 5 = zatwierdzenie naryswania
        private Polygon _polygon = null;
        private System.Windows.Point startPoint = new System.Windows.Point();
        private PointCollection points = new PointCollection();

        private void PS7CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PS7_MODE != 1)
            {
                return;
            }

            startPoint = e.GetPosition(ps7Canvas);

            if (PS7_MODE == 1)
            {
                points.Add(startPoint);
                MakePolygon(_polygon);
                ps7Canvas.Children.Remove(_polygon);
                ps7Canvas.Children.Add(_polygon);
            }
        }

        private void MakePolygon(Polygon polygon)
        {
            if (polygon == null)
            {
                polygon = new Polygon
                {
                    Stroke = System.Windows.Media.Brushes.Red,
                    StrokeThickness = 5,
                    Points = points
                };
                polygon.MouseDown += SelectPolygon;
                polygon.MouseMove += MovePolygon;
                polygon.MouseWheel += PolygonWheel;
                polygon.MouseUp += MovedPolygon;
            }
            else
            {
                polygon.Points = points;
            }
            _polygon = polygon;
        }

        private void PolygonWheel(object sender, MouseWheelEventArgs e)
        {
            _polygon = sender as Polygon;
            _polygon.Stroke = System.Windows.Media.Brushes.Red;
            if (PS7_MODE == 4)
            {
                PointCollection scaledPoints = new PointCollection();
                if (e.Delta > 0)
                {
                    foreach (var point in _polygon.Points)
                    {
                        System.Windows.Point tempPoint = point;
                        tempPoint.X *= 1.025;
                        tempPoint.Y *= 1.025;
                        scaledPoints.Add(tempPoint);
                    }
                    points = scaledPoints;
                    MakePolygon(_polygon);
                    ps7Canvas.Children.Remove(_polygon);
                    ps7Canvas.Children.Add(_polygon);
                }
                else
                {
                    foreach (var point in _polygon.Points)
                    {
                        System.Windows.Point tempPoint = point;
                        tempPoint.X *= 0.975;
                        tempPoint.Y *= 0.975;
                        scaledPoints.Add(tempPoint);
                    }
                    points = scaledPoints;
                    MakePolygon(_polygon);
                    ps7Canvas.Children.Remove(_polygon);
                    ps7Canvas.Children.Add(_polygon);
                }
            }
            _polygon.Stroke = System.Windows.Media.Brushes.Black;
        }

        private void MovedPolygon(object sender, MouseEventArgs e)
        {
            _polygon = sender as Polygon;
            _polygon.ReleaseMouseCapture();
        }

        private void SelectPolygon(object sender, MouseEventArgs e)
        {
            if (PS7_MODE != 1)
            {
                if (_polygon != null)
                    _polygon.Stroke = System.Windows.Media.Brushes.Black;

                _polygon = sender as Polygon;
                _polygon.Stroke = System.Windows.Media.Brushes.Red;
                if (e.LeftButton == MouseButtonState.Pressed && PS7_MODE == 2)
                {
                    TranslateTransform = _polygon.RenderTransform as TranslateTransform ?? new TranslateTransform();
                    clickPosition = e.GetPosition(ps7Canvas);
                    _polygon.CaptureMouse();
                }
                if (PS7_MODE == 5)
                {
                    _polygon.Stroke = System.Windows.Media.Brushes.Black;
                }
            }
        }

        private void MovePolygon(object sender, MouseEventArgs e)
        {
            if (PS7_MODE == 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Polygon dragShape)
                {
                    System.Windows.Point currentPosition = e.GetPosition(ps7Canvas);
                    var transform = dragShape.RenderTransform as TranslateTransform ?? new TranslateTransform();
                    transform.X = TranslateTransform.X + (currentPosition.X - clickPosition.X);
                    transform.Y = TranslateTransform.Y + (currentPosition.Y - clickPosition.Y);
                    dragShape.RenderTransform = new TranslateTransform(transform.X, transform.Y);
                }
            }
            if (ACTION_MODE == 3 && e.LeftButton == MouseButtonState.Pressed)
            {

            }
            if (ACTION_MODE == 4 && e.LeftButton == MouseButtonState.Pressed)
            {

            }
        }

        private void PS7CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (PS7_MODE != 2)
            {
                return;
            }
        }

        private void CreatePolygonChecked(object sender, RoutedEventArgs e) => PS7_MODE = 1;

        private void TranslatePolygonChecked(object sender, RoutedEventArgs e) => PS7_MODE = 2;

        private void RotatePolygonChecked(object sender, RoutedEventArgs e) => PS7_MODE = 3;

        private void ScalePolygonChecked(object sender, RoutedEventArgs e) => PS7_MODE = 4;

        private void DonePolygonChecked(object sender, RoutedEventArgs e)
        {
            PS7_MODE = 5;
            if (_polygon != null)
            {
                allPointCollections.Add(_polygon.Points);
                _polygon.Stroke = System.Windows.Media.Brushes.Black;
            }

            _polygon = null;
            points = new PointCollection();
        }

        private void AddNewPointPolygonClick(object sender, RoutedEventArgs e)
        {
            if (PS7_MODE != 1)
            {
                return;
            }
            else
            {
                var xCorrect = Double.TryParse(xNewPointPolygon.Text, out double xValue);
                var yCorrect = Double.TryParse(yNewPointPolygon.Text, out double yValue);

                if (xCorrect == true && yCorrect == true && _polygon != null)
                {
                    System.Windows.Point point = new System.Windows.Point(xValue, yValue);
                    points.Add(point);
                    MakePolygon(_polygon);
                    ps7Canvas.Children.Remove(_polygon);
                    ps7Canvas.Children.Add(_polygon);
                }
            }
        }

        private void RotatePolygonByValueClick(object sender, RoutedEventArgs e)
        {
            if (PS7_MODE != 3)
            {
                return;
            }
            else
            {
                var xCorrect = Double.TryParse(rotateXPoint.Text, out double xValue);
                var yCorrect = Double.TryParse(rotateYPoint.Text, out double yValue);
                var angleCorrect = Double.TryParse(rotationAnglePolygon.Text, out double angle);

                if (xCorrect == true && yCorrect == true && angleCorrect == true && _polygon != null)
                {
                    PointCollection scaledPoints = new PointCollection();
                    foreach (var point in _polygon.Points)
                    {
                        System.Windows.Point tempPoint = point;
                        tempPoint.X += xValue + (point.X - xValue) * Math.Cos(angle) - (point.Y - yValue) * Math.Sin(angle);
                        tempPoint.Y += yValue + (point.X - xValue) * Math.Sin(angle) + (point.Y - yValue) * Math.Cos(angle);
                        scaledPoints.Add(tempPoint);
                    }

                    points = scaledPoints;

                    MakePolygon(_polygon);
                    ps7Canvas.Children.Remove(_polygon);
                    ps7Canvas.Children.Add(_polygon);
                }
            }
        }

        private void TranslatePolygonByXYClick(object sender, RoutedEventArgs e)
        {
            if (PS7_MODE != 2)
            {
                return;
            }
            else
            {
                //_polygon = sender as Polygon;
                var xCorrect = Double.TryParse(xTranslatePolygon.Text, out double xValue);
                var yCorrect = Double.TryParse(yTranslatePolygon.Text, out double yValue);

                if (xCorrect == true && yCorrect == true && _polygon != null)
                {
                    PointCollection scaledPoints = new PointCollection();
                    foreach (var point in _polygon.Points)
                    {
                        System.Windows.Point tempPoint = point;
                        tempPoint.X += xValue;
                        tempPoint.Y += yValue;
                        scaledPoints.Add(tempPoint);
                    }

                    points = scaledPoints;

                    MakePolygon(_polygon);
                    ps7Canvas.Children.Remove(_polygon);
                    ps7Canvas.Children.Add(_polygon);
                }
            }
        }

        private void ScalePolygonByValueClick(object sender, RoutedEventArgs e)
        {
            if (PS7_MODE != 4)
            {
                return;
            }
            else
            {
                var xCorrect = Double.TryParse(scaleXPoint.Text, out double xValue);
                var yCorrect = Double.TryParse(scaleYPoint.Text, out double yValue);
                var scaleCorrect = Double.TryParse(scalePolygonValue.Text, out double scale);

                if (xCorrect == true && yCorrect == true && scaleCorrect == true && _polygon != null)
                {
                    PointCollection scaledPoints = new PointCollection();
                    foreach (var point in _polygon.Points)
                    {
                        System.Windows.Point tempPoint = point;
                        tempPoint.X += xValue + (point.X - xValue) + scale;
                        tempPoint.Y += yValue + (point.Y - yValue) + scale;
                        scaledPoints.Add(tempPoint);
                    }

                    points = scaledPoints;

                    MakePolygon(_polygon);
                    ps7Canvas.Children.Remove(_polygon);
                    ps7Canvas.Children.Add(_polygon);
                }
            }
        }

        private List<PointCollection> allPointCollections = new List<PointCollection>();

        private void DeserializeCanvasClick(object sender, RoutedEventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<PointCollection>));
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    List<PointCollection> openedList;
                    using (FileStream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        openedList = (List<PointCollection>)serializer.Deserialize(stream);
                        stream.Close();
                    }

                    foreach (var pointCollection in openedList)
                    {
                        Polygon polygon = new Polygon
                        {
                            Points = pointCollection,
                            Stroke = System.Windows.Media.Brushes.Black,
                            StrokeThickness = 5
                        };
                        polygon.MouseDown += SelectPolygon;
                        polygon.MouseMove += MovePolygon;
                        polygon.MouseWheel += PolygonWheel;
                        polygon.MouseUp += MovedPolygon;
                        ps7Canvas.Children.Add(polygon);
                    }
                    allPointCollections = openedList;
                }
                catch
                {
                    MessageBoxResult result = MessageBox.Show("Podczas próby zapisu pliku coś poszło nie tak.");
                }
            }
        }

        private void SerializeCanvasClick(object sender, RoutedEventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<PointCollection>));
            SaveFileDialog saveFile = new SaveFileDialog();
            if (saveFile.ShowDialog() == true)
            {
                using (FileStream stream = new FileStream(saveFile.FileName, FileMode.Create))
                {
                    serializer.Serialize(stream, allPointCollections);
                    stream.Close();
                }
            }
        }

        private void ResetCanvasPS7Click(object sender, RoutedEventArgs e)
        {
            ps7Canvas.Children.Clear();
            allPointCollections.Clear();
            points.Clear();
        }

        #endregion

        #region PS8
        private Bitmap originalBitmapPS8;
        public bool grayScaleValuePS8 = false;

        private void UploadFilePS8Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JPEG Image|*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    zoomScale = 1;
                    ScaleTransform scale = new ScaleTransform(zoomScale, zoomScale);
                    Image.LayoutTransform = scale;
                    string extension = Path.GetExtension(openFileDialog.FileName);

                    Bitmap bitmap = new Bitmap(openFileDialog.FileName);
                    originalBitmapPS8 = bitmap;
                    BitmapImage bitmapImage = FromBitmapToBitmapImage(bitmap);
                    ps8Image.Source = bitmapImage;

                }
                catch
                {
                    _ = MessageBox.Show("Podczas próby odczytu pliku coś poszło nie tak.");
                }
            }
        }

        private readonly byte[,] kernelDylatacja = new byte[3, 3]
        {
            { 0, 1, 0},
            { 1, 1, 1},
            { 0, 1, 0}
        };

        private Bitmap DilationOperation(Bitmap bitmap, byte[,] kernel)
        {
            Bitmap tempBM = bitmap;
            for (int i = 1; i < tempBM.Width - 1; i++)
            {
                for (int j = 1; j < tempBM.Height - 1; j++)
                {
                    System.Drawing.Color color = tempBM.GetPixel(i, j);
                    int newR = color.R;
                    int newG = color.G;
                    int newB = color.B;
                    for (int k = -1; k <= 1; k++)
                    {
                        for (int l = -1; l <= 1; l++)
                        {
                            if (kernel[k + 1, l + 1] == 1)
                            {
                                System.Drawing.Color newColor = tempBM.GetPixel(i + k, j + l);
                                newR = Math.Max(color.R, newColor.R);
                                newG = Math.Max(color.G, newColor.G);
                                newB = Math.Max(color.B, newColor.B);
                            }
                            else
                            {
                                continue;
                            }

                        }
                    }
                    tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(newR, newG, newB));
                }
            }
            return tempBM;
        }

        private Bitmap ErosionOperation(Bitmap bitmap, byte[,] kernel)
        {
            Bitmap tempBM = bitmap;
            for (int i = 1; i < tempBM.Width - 1; i++)
            {
                for (int j = 1; j < tempBM.Height - 1; j++)
                {
                    System.Drawing.Color color = tempBM.GetPixel(i, j);
                    int newR = color.R;
                    int newG = color.G;
                    int newB = color.B;
                    for (int k = -1; k <= 1; k++)
                    {
                        for (int l = -1; l <= 1; l++)
                        {
                            if (kernel[k + 1, l + 1] == 1)
                            {
                                System.Drawing.Color newColor = tempBM.GetPixel(i + k, j + l);
                                newR = Math.Min(color.R, newColor.R);
                                newG = Math.Min(color.G, newColor.G);
                                newB = Math.Min(color.B, newColor.B);
                            }
                            else
                            {
                                continue;
                            }

                        }
                    }
                    tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(newR, newG, newB));
                }
            }
            return tempBM;
        }

        private void DylatacjaClick(object sender, RoutedEventArgs e)
        {
            if (ps8Image.Source == null)
            {
                return;
            }
            indicatorPS8.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps8Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;

            var task = Task.Run(() =>
            {
                tempBM = DilationOperation(bitmap, kernelDylatacja);
            });

            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicatorPS8.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps8ImageFiltred.Source = newBitmap;
                    }
                }));

            });
        }

        private void ErozjaClick(object sender, RoutedEventArgs e)
        {
            if (ps8Image.Source == null)
            {
                return;
            }
            indicatorPS8.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps8Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;

            var task = Task.Run(() =>
            {
                tempBM = ErosionOperation(bitmap, kernelDylatacja);
            });

            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicatorPS8.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps8ImageFiltred.Source = newBitmap;
                    }
                }));

            });
        }

        private void OtwarcieClick(object sender, RoutedEventArgs e)
        {
            if (ps8Image.Source == null)
            {
                return;
            }
            indicatorPS8.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps8Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;

            var task = Task.Run(() =>
            {
                tempBM = ErosionOperation(bitmap, kernelDylatacja);
                tempBM = DilationOperation(tempBM, kernelDylatacja);
            });

            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicatorPS8.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps8ImageFiltred.Source = newBitmap;
                    }
                }));

            });
        }

        private void DomkniecieClick(object sender, RoutedEventArgs e)
        {
            if (ps8Image.Source == null)
            {
                return;
            }
            indicatorPS8.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps8Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;

            var task = Task.Run(() =>
            {
                tempBM = DilationOperation(bitmap, kernelDylatacja);
                tempBM = ErosionOperation(tempBM, kernelDylatacja);
            });

            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicatorPS8.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps8ImageFiltred.Source = newBitmap;
                    }
                }));

            });
        }

        readonly private byte[,] b1 = new byte[3, 3]
        {
            { 0, 1, 0},
            { 1, 1, 1},
            { 0, 1, 0}
        };

        readonly private byte[,] b2 = new byte[3, 3]
        {
            { 1, 0, 1},
            { 0, 0, 0},
            { 1, 0, 1}
        };

        private Bitmap ComplementOperation(Bitmap bitmap)
        {
            _ = System.Drawing.Color.Black;
            _ = System.Drawing.Color.White;
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {

                    System.Drawing.Color color = bitmap.GetPixel(i, j);
                    // if(color == colorBlack)
                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(ImplementedValue(color.R), ImplementedValue(color.G), ImplementedValue(color.B)));

                    // if(color == colorWhite)
                    //     bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(implementedValue(colorBlack.R), implementedValue(colorBlack.G), implementedValue(colorBlack.B)));
                }
            }
            return bitmap;
        }

        private int ImplementedValue(int value)
        {
            return 255 - value;
        }

        private Bitmap AndOperation(Bitmap bitmap1, Bitmap bitmap2)
        {
            Bitmap result = bitmap1;

            for (int i = 0; i < bitmap1.Width; i++)
            {
                for (int j = 0; j < bitmap1.Height; j++)
                {
                    System.Drawing.Color color1 = bitmap1.GetPixel(i, j);
                    System.Drawing.Color color2 = bitmap2.GetPixel(i, j);
                    if (color1 != color2)
                    {
                        continue;
                    }
                    else
                    {
                        result.SetPixel(i, j, System.Drawing.Color.FromArgb(ImplementedValue(color1.R), ImplementedValue(color1.G), ImplementedValue(color1.B)));
                    }
                }
            }

            return result;
        }

        private void HitOrMissClick(object sender, RoutedEventArgs e)
        {
            if (ps8Image.Source == null)
            {
                return;
            }
            indicatorPS8.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps8Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            Bitmap result1 = bitmap;
            Bitmap result2 = bitmap;
            Bitmap Ac = bitmap;

            var task = Task.Run(() =>
            {
                result1 = ErosionOperation(tempBM, b1);
                Ac = ComplementOperation(tempBM);
                result2 = ErosionOperation(Ac, b2);
                tempBM = AndOperation(result1, result2);
            });

            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicatorPS8.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps8ImageFiltred.Source = newBitmap;
                    }
                }));

            });
        }

        private void GetBackToOriginalPS8Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = FromBitmapToBitmapImage(originalBitmapPS8);
            ps8Image.Source = bitmapImage;
            grayScaleValuePS8 = false;
        }

        private void GrayScalePS8Click(object sender, RoutedEventArgs e)
        {
            if (ps8Image.Source == null)
            {
                return;
            }
            indicatorPS8.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps8Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            Bitmap setBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var task = Task.Run(() =>
            {
                for (int i = 0; i < setBitmap.Width; i++)
                {
                    for (int j = 0; j < setBitmap.Height; j++)
                    {
                        System.Drawing.Color color = tempBM.GetPixel(i, j);
                        byte grayColor = (byte)(0.21 * color.R + 0.71 * color.G + 0.071 * color.B);
                        setBitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(grayColor, grayColor, grayColor));
                    }
                }
            });

            grayScaleValuePS8 = true;
            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicatorPS8.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(setBitmap);
                    if (newBitmap != null)
                    {
                        ps8Image.Source = newBitmap;
                    }
                }));

            });
        }

        private void OriginalZoomPS8ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoom = e.NewValue;
            ScaleTransform scale = new ScaleTransform(zoom, zoom);
            ps8Image.LayoutTransform = scale;
        }

        private void FilteredZoomPS8ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoom = e.NewValue;
            ScaleTransform scale = new ScaleTransform(zoom, zoom);
            ps8ImageFiltred.LayoutTransform = scale;
        }

        private void BinaryzacjaClick(object sender, RoutedEventArgs e)
        {
            if (ps8Image.Source == null || binaryzacjaPS8.Text == "")
            {
                return;
            }
            indicatorPS8.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(ps8Image.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            int binValue = int.Parse(binaryzacjaPS8.Text);
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
                    indicatorPS8.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        ps8Image.Source = newBitmap;
                    }
                }));

            });
        }

        private void BinaryzacjaPS8PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion

        #region PS9

        private Bitmap originalBitmapPS9;
        private int COLOR_MODE = 0;
        //1 = red
        //2 = green
        //3 = blue

        private void UploadFilePS9Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JPEG Image|*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    zoomScale = 1;
                    ScaleTransform scale = new ScaleTransform(zoomScale, zoomScale);
                    Image.LayoutTransform = scale;
                    string extension = Path.GetExtension(openFileDialog.FileName);

                    Bitmap bitmap = new Bitmap(openFileDialog.FileName);
                    originalBitmapPS9 = bitmap;
                    BitmapImage bitmapImage = FromBitmapToBitmapImage(bitmap);
                    originalImagePS9.Source = bitmapImage;

                }
                catch
                {
                    _ = MessageBox.Show("Podczas próby odczytu pliku coś poszło nie tak.");
                }
            }
        }


        private bool IsGreenPixel(System.Drawing.Color color1, int tolerancja) =>
            color1.G > color1.R + tolerancja && color1.G > color1.B + tolerancja;

        private bool IsRedPixel(System.Drawing.Color color1, int tolerancja) =>
            color1.R > color1.G + tolerancja && color1.R > color1.B + tolerancja;

        private bool IsBluePixel(System.Drawing.Color color1, int tolerancja) =>
            color1.B > color1.R + tolerancja && color1.B > color1.G + tolerancja;

        private void GreenPercentClick(object sender, RoutedEventArgs e)
        {
            if (originalImagePS9.Source == null)
            {
                return;
            }

            indicatorPS9.IsBusy = true;
            Bitmap bitmap = ImageSourceToBitmap(originalImagePS9.Source);
            BitmapImage newBitmap = null;
            Bitmap tempBM = bitmap;
            Bitmap setBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int allPixels = bitmap.Width * bitmap.Height;
            int countPixels = 0;

            var task = Task.Run(() =>
            {
                for (int i = 0; i < setBitmap.Width; i++)
                {
                    for (int j = 0; j < setBitmap.Height; j++)
                    {
                        switch (COLOR_MODE)
                        {
                            case 1:
                                if (IsRedPixel(tempBM.GetPixel(i, j), 2))
                                {
                                    tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(tempBM.GetPixel(i, j).R, tempBM.GetPixel(i, j).G, 255));
                                    countPixels++;
                                }
                                break;

                            case 2:
                                if (IsGreenPixel(tempBM.GetPixel(i, j), 2))
                                {
                                    tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(255, tempBM.GetPixel(i, j).G, tempBM.GetPixel(i, j).B));
                                    countPixels++;
                                }
                                break;

                            case 3:
                                if (IsBluePixel(tempBM.GetPixel(i, j), 2))
                                {
                                    tempBM.SetPixel(i, j, System.Drawing.Color.FromArgb(tempBM.GetPixel(i, j).R, 255, tempBM.GetPixel(i, j).B));
                                    countPixels++;
                                }
                                break;
                        }
                    }
                }
            });
            task.ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    indicatorPS9.IsBusy = false;
                    newBitmap = FromBitmapToBitmapImage(tempBM);
                    if (newBitmap != null)
                    {
                        double percent = (double)(countPixels * 100 / allPixels);
                        changedImagePS9.Source = newBitmap;
                        resultPercentPS9.Content = percent + "%";
                    }
                }));

            });
        }

        private void OriginalZoomValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoom = e.NewValue;
            ScaleTransform scale = new ScaleTransform(zoom, zoom);
            originalImagePS9.LayoutTransform = scale;
        }

        private void ChangedZoomValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoom = e.NewValue;
            ScaleTransform scale = new ScaleTransform(zoom, zoom);
            changedImagePS9.LayoutTransform = scale;
        }

        private void PS9RedChecked(object sender, RoutedEventArgs e)
        {
            COLOR_MODE = 1;
            greenPercent.IsEnabled = true;
        }

        private void PS9GreenChecked(object sender, RoutedEventArgs e)
        {
            COLOR_MODE = 2;
            greenPercent.IsEnabled = true;
        }

        private void PS9BlueChecked(object sender, RoutedEventArgs e)
        {
            COLOR_MODE = 3;
            greenPercent.IsEnabled = true;
        }

        #endregion


    }
}
