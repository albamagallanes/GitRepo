   M S B A M L   `   `   `  ����^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers  5   �    SkeletonBasics-WPF8   2Microsoft.Samples.Kinect.SkeletonBasics.MainWindow   6	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6K   u
WpfViewers^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     6�   -   5          � Title$  Skeleton Basics��5   	   $	��735��6!   $	��770��6.    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value  SquareCheckBox �� 
TargetType �� Orientation �Name	 ��Name
 ��Name �� TextAlignment �� Triggers P� Property P� Value �� 
TargetName��%1  &	        &	 #     &	 F     '�� i     &	 �     7  7  �� 5   
   $ 	#ff6e6e6e��62   �� 5   
   $ 	#ff52318f��64   �� 5   
   $ 	#ff00BCF2��62   �� 5   
   " ��6   .�5	      ��  �@J�SnapsToDevicePixels . 64   5
      �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      � .+�5      �� $ 
Horizontal=�6)   
������ .+�5   "   � $ SquareCheckBoxChecked��6'   .+�5   &   �� $	 CheckedNormal��6,   $��Images\CheckedNormal.png��6C   $
��None=�6e   $��Center=�6t   �� 5   &   $	 CheckedHover��6,   $��Images\CheckedHover.png��6B   $
��None=�6c   $��Center=�6r   $y�	Collapsed=�6�   5   #   � 5   "   $ SquareCheckBoxUnchecked��6'   $y�	Collapsed=�6H   .+�5   &   �� $	 UncheckedNormal��6,   $ ��Images\UncheckedNormal.png��6E   $
��None=�6i   $��Center=�6x   �� 5   &   $	 UncheckedHover��6,   $��Images\UncheckedHover.png��6D   $
��None=�6g   $��Center=�6v   $y�	Collapsed=�6�   5   #   �� 5   "   $
 SquareCheckBoxText��6,   #��z��6H   $
 Left=�6i   $��Center=�6   8��  $��15(�6�   $��9,0,0,0q�6�   5      5       5      P� 5       �@d�	IsChecked$ falseY�6;   . �5   "   �� $ SquareCheckBoxChecked��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5   "   $ SquareCheckBoxUnchecked��6O    � y�$ Visible=�6?   6O   5       P� 5!       �@J�IsMouseOver . . �5"   "   �� $ CheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5#   "   $ UncheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5$   "   $ CheckedHover��6O    � y�$ Visible=�6?   6O   �� 5%   "   $ UncheckedHover��6O    � y�$ Visible=�6?   6O   �� 5&   "   8  $ SquareCheckBoxText��6h    � ��5'      5)      5+      5,      5-      .��5/      � -   5        $ 
layoutGrid��5/      $��10,35,103,0q�6    � RowDefinitions 50   
   �� 51      $
��Auto �6   �� 52      $��* �6   �� 53      $
��Auto �6   �� 54      $
��Auto �6   .+�56   
   ]� $��0��6   $��0 0 0 20q�6!   .+�57      �� $
��Left=�6   $��Images\Logo.png��6*   $
��Fill=�6C   $��32��6R   $��81��6^   $��0 10 0 5q�6i   �� 58      $��Right=�6   $��0 0 -1 0q�6/   $��Bottom=�6A   #��[  6\   $��Segoe UI)�6�   $��18(�6�   �� 6�   $�Skeleton Basics��6�   6�   �� 59      $��1��6   $��Images\Status.png��6$   $
��None=�6?   $��Center=�6N   $��0 0 0 5q�6k   5:      (� 5;   
   $��1��6    (� Stretch$ Uniform=�6   $��Center=�61   .��5<      �� -   5        $	 Image��5<      $	��486��6#   $	��400��6/   5=      �� 5>   
   -   5         ��Name$ checkBoxSeatedMode��5>   �   $��2��6   #��[ 6    $��Seated Mode��6H   $
��Auto��6^   $
��Left=�6l   $��Center=�6�   $��
0 10 10 10q�6�   6�   �� 5?   
   -   5         ��Name$ 	statusBar��5?   ?   $��3��6   $��Stretch=�6!   $��Bottom=�6R   
�������#��[  6�   .@�5@      �� $��0 0 0 10q�6   .��5A      �� -   5        $
 statusBarText��5A      $��-1 0 0 0q�63   �� 6F   $6�0Click 'Seated' to change skeletal pipeline type!��6J   6�   5B      5C      5D      5E      