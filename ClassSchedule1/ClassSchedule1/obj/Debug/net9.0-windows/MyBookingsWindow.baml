   M S B A M L   `   `   `  ����I  EClassSchedule1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ClassSchedule1ClassSchedule1  5   	   %    ClassSchedule1.MyBookingsWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:ClassSchedule1     5   	     � Title$%  Мои бронирования��5   	   $	��400��6"   $	��600��6/    � Icon$  /icons8-school-bell-64.png��6;   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $%��Мои бронирования��6!   $��20(�69   $
��Bold"�6G   $��Center=�5      $��10q�61   �� 5   
   -   5         ��Name$ BookingsListView��5       $��1��6   $��10q�6:   ��5      �� 5      .R�5      ��  �� CornerRadius$ 8��6   ���	#FFF5F5F5$��5q�6E    �� Padding$ 10q�6P   ._�5      �  � ColumnDefinitions 5      �� 5   "   $��* �63   �� 5   "   $
��Auto �63   �� 5   "   $
��Auto �63   .+�5      �� $��0��6)   .+�5    "   �� $
��Bold"�6H   $��14(�6Z   ��6,   �� *
RoomNumber+6Z   �� 5!   "   $��0,5,0,0q�6n   ��6,   �� *BookingDate+ �� StringFormat$ Дата: {0:dd.MM.yyyy}��6n   �� 5"   "   $��0,5,0,0q�6b   ��6,   �� *	StartTime+$ Время: {0}��6b   �� 5#   "   ��6,   �� *
Duration+$8 2Продолжительность: {0} час(а)��5$      �� 5%      -   5        $��1��5%   %   $��Изменить��65   
���� ��
�������$��80��5'   %   $��30��60   $	��5,0q�6<   
 � Tag 5(   ?   �� *	BookingId+�� 5)      -   5        $��2��5)   %   $��Удалить��65   
���  ��
�������$��80��5+   %   $��30��60   $	��5,0q�6<    5,   A   �� *	BookingId+5-      5.      5/      51      52      ��53      �� 54   
   	 �� 
StartPoint$		 0,0&�6   
 �� EndPoint$	
 1,1&�6/   .��55      �  � Color$ DarkSlateBlue��6    � Offset$ 0Q�61   � 56      $
 Pink��6   $	 0.3Q�6(   � 57      $ Purple��6   $	 0.6Q�6*   � 58      $ DarkBlue��6   $ 1Q�6,   59      5;      