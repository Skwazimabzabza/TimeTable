   M S B A M L   `   `   `  ����I  EClassSchedule1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ClassSchedule1ClassSchedule1  5   	       ClassSchedule1.SignWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:ClassSchedule1     5   	     � 
ResizeMode$  NoResize=�5   	    � Title$ Регистрация��5	      $	��485��6%   $	��600��62    � Icon$  /icons8-school-bell-64.png��6>   .��5
      �  � RowDefinitions 5   
   �� 5      �� 5      �� 5      .+�5   
   �� $��Регистрация ��6   $��Center=�6(   $��Center=�6E   $��30(�6`   
�������6n   �� 5   
   $��1��6   .+�5      �� $��Введите почту��6   $��20(�6-   $��Center=�6;   
�������$��0,0,0,0q�6k   �� 5      -   5         ��Name$ SignTextBox��5      $	��390��6+   $��10q�67   
������ $��0,0,0,2q�6\   �� 5      $'��!Придумайте пароль��6   $��20(�61   $��Center=�6?   
�������$��	0,20,0,14q�6o   �� 5      -   5        $ PasswordTextBox��5      $	��390��6/   
������ $��0,0,0,2q�6U   5      �� 5   
   $��2��6   .+�5      �� -   5        $��Center=�5      $*��$Зарегистрироваться��5      $	��200��5      $��40��5      $��30q�5      
�������$��16(�5       
���� ����5"      �� 5#       �� 
TargetType" ��6&   .R�5$      ��  ��Name$ border��6!    �� CornerRadius$ 20��5%      #��z��5&      #��z��5'      #��z��5(      ._�5)      �� $��Center=�6/   $��Center=�6L   5*       �� Triggers 5+      P� 5,      	 P� Property	 �@J�IsMouseOver
 P� Value$

 True��6=   . �5-   "   ��  �� 
TargetName$ border��6)    �� Property � �� �� Value
 �� ��6S   5.      P� 5/      	 � ��$

 True��6;   . �50   "   �� $ border��6)    � ��
 �� ��6S   51      53      55      �� 57      -   5        $��Center=�58      $#��Вернуться назад��59      $	��200��5:      $��40��5;      
�������$��16(�5=      
���� ����5?      �� 5@      " ��6&   .R�5A      �� $ border��6!   $ 20��5B      #��z��5C      #��z��5D      #��z��5E      ._�5F      �� $��Center=�6/   $��Center=�6L   5G       5H      P� 5I      	 �@J�IsMouseOver$

 True��6=   . �5J   "   �� $ border��6)    � ��
 �� ��6S   5K      P� 5L      	 � ��$

 True��6;   . �5M   "   �� $ border��6)    � ��
 �� ��6S   5N      5P      5R      5S       � 5T   
   -   5          �Name$ 	MainFrame��5T        � NavigationUIVisibility$ Hidden=�6#   5U      ��5V      �� 5W   
    �� 
StartPoint$	 0,0&�6    �� EndPoint$	 1,1&�6/   .��5X      �  � Color$ DarkSlateBlue��6    � Offset$ 0Q�62   � 5Y      $
 Pink��6   $	 0.3Q�6(   � 5Z      $ Purple��6   $	 0.6Q�6*   � 5[      $ DarkBlue��6   $ 1Q�6,   5\      5^      