   M S B A M L   `   `   `  ���� B  >Grafika, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:GrafikaGrafika      Grafika.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:Grafika       � Title$  
MainWindow��$	��800��$
��1200��$��12(�.��� .+��� .@��� $	��PS1��.���  � ColumnDefinitions �� $	��900 ��� $	��150 ��� $��* � � RowDefinitions �� $��60 ��� $	��130 ��� $	��130 ��� $	��115 ��� $	��100 ��� $	��110 ��� $��* �.+��� 
������� �� CornerRadius$ 5��
���   �$��1q�$��1��$��2��$��1q��� 
�������$ 5��
���   �$��1q�$��1��$��2��$��1��$��1q��� 
�������$ 5��
���   �$��1q�$��1��$��2��$��2��$��1q��� 
�������$ 5��
���   �$��1q�$��1��$��2��$��3��$��1q��� 
�������$ 5��
���   �$��1q�$��1��$��2��$��4��$��1q��� 
�������$ 5��
���   �$��1q�$��1��$��2��$��5��$��1q��� 
�������$ 5��
���   �$��1q�$��1��$��2��$��6��$��1q��� 
�������$ 5��
���   �$��1q�$��0��$��0��$��7��$��1q��� $��1��$��0��$��5q�$��Center=�.+�	� -   $��drawing��.��	Rysowanie	� -   $��moving��.��Przesuwanie	� -   $��changeShape��.��Zmiana kształtu�� $��2��$��0��$��5q�$��Center=�.+�	� -   $
��line��.��Linia	� -   $��	rectangle��.��
Prostokąt	� -   $��ellipse��.��Okrąg�� $��1��$��2��$��1��$��5q�$��Center=�.+�]� $��Stretch=�$��1q�.+��� $��Współrzędna x1:��$��1q��� -   $��x1line��$��1q�]� $��Stretch=�$��1q�.+��� $��Współrzędna y1:��$��1q��� -   $��y1line��$��1q�]� $��Stretch=�$��1q�.+��� $��Współrzędna x2:��$��1q��� -	   $��x2line��$��1q�]� $��Stretch=�$��1q�.+��� $��Współrzędna y2:��$��1q��� -
   $��y2line��$��1q��� -   $��drawLine��.��Stwórz linię�� $��1��$��2��$��2��$��5q�$��Center=�.+�]� $��Stretch=�$��1q�.+��� $��Współrzędna x1:��$��1q��� -   $��x1rectangle��$��1q�]� $��Stretch=�$��1q�.+��� $��Współrzędna y1:��$��1q��� -   $��y1rectangle��$��1q�]� $��Stretch=�$��1q�.+��� $��Szerokość:��$��1q��� -   $��widthRectangle��$��1q�]� $��Stretch=�$��1q�.+��� $��Wysokość:��$��1q��� -   $��heightRectangle��$��1q��� -   $��drawRectangle��$��1q�.��Stwórz prostokąt�� $��1��$��2��$��3��$��5q�$��Center=�.+�]� $��Stretch=�$��1q�.+��� $��Współrzędna x1:��$��1q��� -   $��	x1ellipse��$��1q�]� $��Stretch=�$��1q�.+��� $��Współrzędna y1:��$��1q��� -   $��	y1ellipse��$��1q�]� $��Stretch=�$��1q�.+��� $��	Promień:��$��1q��� -   $��radiusellipse��$��1q��� -   $��drawEllipse��$��1q�.��Stwórz okrąg�� $��1��$��2��$��4��$��5q�$��Center=�.+�]� $��Stretch=�$��1q�.+��� $ ��Zmiana długości linii o:��$��1q��� -   $��changeLineValue��$��1q��� -   $��
lineBigger��$��1q�.��Zmiana długości linii�� $��1��$��2��$��5��$��5q�$��Center=�.+�]� $��Stretch=�$��1q�.+��� $��Szerokość zmieniona o:��$��1q��� -   $��changeWidth��$��1q�]� $��Stretch=�$��1q�.+��� $��Wysokość zmieniona o:��$��1q��� -   $��changeHeight��$��1q��� -   $��changeRectangle��$��1q�.��Zmiana rozmiaru prostokąta�� $��1��$��2��$��6��$��5q�$��Center=�.+�]� $��Stretch=�$��1q�.+��� $��Promień zmieniony o:��$��1q��� -   $��changeRadius��$��1q��� -   $��changeEllipse��$��1q�.��Zmiana rozmiaru okręgu�� -   $��CanvasField��$��0��$��7��$��5q�
�������}�. �� $	��PS2��.���  �� $	��100 ��� $��* �.+��� $ 5��
���   �$��1q�$��1���� $��5q�$��0��$
��Left=� �� Orientation$ Vertical=�.+��� -   $��
uploadFile��$��5q�.��Wczytaj plik�� -   $��saveFile��$��5q�.��Zapisz plik]� $��Stretch=�.+��� $(��"Wybierz poziom kompresji do zapisu��$��5q��� $��5q�$	��100��.@��� -   $��compression1L��.��1L�� -    $��compression25L��.��25L�� -!   $��compression50L��.��50L�� -"   $��compression75L��.��75L�� -#   $��compression100L��.��100L�� -$   $��Image��$��1��$��5q�$��Uniform=�