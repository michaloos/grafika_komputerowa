using System;
using System.Collections.Generic;
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

        private Point startPoints = new Point();
        private Point endPoints = new Point();
        private TranslateTransform TranslateTransform;
        private Point clickPosition = new Point();
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
                                Stroke = Brushes.Red
                            };
                            break;
                        //tworzenie prostokąta
                        case 2:
                            shape = new Rectangle
                            {
                                Width = Math.Abs(startPoints.X - endPoints.X),
                                Height = Math.Abs(startPoints.Y - endPoints.Y),
                                StrokeThickness = 5,
                                Stroke = Brushes.Green
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
                                Height = Math.Abs(startPoints.Y - endPoints.Y),
                                StrokeThickness = 5,
                                Stroke = Brushes.Yellow
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
                    Point currentPosition = e.GetPosition(CanvasField);
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
                        Point currentPosition = e.GetPosition(CanvasField);
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
                    Stroke = Brushes.Red
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
                shape = new Rectangle
                {
                    Width = width,
                    Height = height,
                    StrokeThickness = 5,
                    Stroke = Brushes.Green
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
                    Stroke = Brushes.Yellow
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
                Rectangle rectangle = shape as Rectangle;
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
            if (ACTION_MODE != 3 || shape == null || shape is Line || shape is Rectangle)
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
    }
}
