   M S B A M L   `   `   `  ����   C3    C3.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Доставка��$	��700��$
��1000��.��� $��10q� � ColumnDefinitions �� $	��300 ��� $��* �.+��� $��0��$��5q�$	��Top=�.+��� -   $��btnGetOrders��$#��Получить заказы��$��30��$��0 0 0 10q��� -   $��btnAddOrder��$!��Добавить заказ��$��30��$��0 0 0 10q�&   System.Windows.Controls.DataGrid  -   $��
ordersGrid��$	��400��   AutoGenerateColumns$ False��   
IsReadOnly$
 True��$��0 0 0 10q�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ ID��   Width5  /System.Windows.Controls.DataGridLengthConverter$ 60 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *ID+  $ Priority��$ 80  �� *
Priority+  $ X��$ 80  �� *Destination.X+  $ Y��$ 80  �� *Destination.Y+�� $��Маршрут:��$
��Bold"��� -   $��routeTextBox��$��60�� �� TextWrapping$
 Wrap=�	 �� 
IsReadOnly$
	 True��"
 �� VerticalScrollBarVisibility$

 Auto=��� $*��$Стоимость маршрута:��$
��Bold"�$��0 10 0 0q��� -   $��routeCostTextBox��$��30��$
	 True���� -   $��graphCanvas��$
��Auto��$
��Auto��$��1��
�������