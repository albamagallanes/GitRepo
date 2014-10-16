//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System.Windows.Media.Imaging;
    using System;
    using System.Windows.Media.Media3D;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();
                this.sensor.DepthStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.AllFramesReady += this.SensorAllFramesReady;

                


                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

                /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            ImageSource tmp;
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame()) {
                if (colorFrame == null) { return;  }
                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);
                int stride = colorFrame.Width * 4;
                //Image.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
                tmp  = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
            }
             
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }
            SensorSkeletonFrameReady(sender, e, tmp);


        }
        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        //private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        private void SensorSkeletonFrameReady(object sender, AllFramesReadyEventArgs e, ImageSource tmp)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                //dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                dc.DrawImage(tmp, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        //RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);
 
            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;                    
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;                    
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];
            Joint joint3 = skeleton.Joints[JointType.ShoulderCenter];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked ||
                joint3.TrackingState == JointTrackingState.NotTracked
                )
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred &&
                joint3.TrackingState == JointTrackingState.Inferred
                
                )
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked && joint3.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            if (joint0.JointType == JointType.ShoulderLeft && joint1.JointType == JointType.ElbowLeft)
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri("C:\\Users\\monita\\Documents\\VSProjects\\SkeletonBasics-WPF\\Images\\hombro.png", UriKind.Absolute);
                image.DecodePixelWidth = 100;
                image.DecodePixelHeight = 100;
                //image.Rotation = skeleton.BoneOrientations[joint0.JointType].HierarchicalRotation.Matrix.;
                image.EndInit();
                //image.Rotation= skeleton.BoneOrientations[joint0.JointType].AbsoluteRotation;
                double angle = GetBodySegmentAngle(joint3, joint0, joint1)*-1;
                Console.WriteLine("Angle " + angle);
                double midpointX = ((this.SkeletonPointToScreen(joint0.Position).X + this.SkeletonPointToScreen(joint1.Position).X) / 2);
                double midpointY = ((this.SkeletonPointToScreen(joint0.Position).Y + this.SkeletonPointToScreen(joint1.Position).Y) / 2);

                RotateTransform transform = new RotateTransform(angle, this.SkeletonPointToScreen(joint0.Position).X, this.SkeletonPointToScreen(joint0.Position).Y);
                drawingContext.PushTransform(transform);
                /*TransformedBitmap tb = new TransformedBitmap();
                tb.BeginInit();
                tb.Source = image;
                // Set image rotation.
                RotateTransform transform = new RotateTransform(angle);
                tb.Transform = transform;
                tb.EndInit();
                */
                drawingContext.DrawImage(image, new Rect(this.SkeletonPointToScreen(joint0.Position).X-20, this.SkeletonPointToScreen(joint0.Position).Y, 100, 100));
                drawingContext.Pop();
            }
            else
            {
                drawPen.Thickness = 10;
                drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
            }
        }
        //Hombro -  codo - cuello
        public double GetBodySegmentAngle(Joint joint1, Joint joint2, Joint joint3)
        {
            float x1 = joint1.Position.X - joint2.Position.X;
            float y1 = joint1.Position.Y - joint2.Position.Y;
            double arctan = Math.Atan2(y1, x1);
            Console.WriteLine("ARCT "+arctan*(180/Math.PI));
            float x2 = joint2.Position.X - joint3.Position.X;
            float y2 = joint2.Position.Y - joint3.Position.Y;
            float z = 0.0F;
            //Microsoft.Xna.Framework.Vector3 vectorJoint1ToJoint2 = new Microsoft.Xna.Framework.Vector3(joint1.Position.X - joint2.Position.X, joint1.Position.Y - joint2.Position.Y, 0);
            //Microsoft.Xna.Framework.Vector3 vectorJoint2ToJoint3 = new Microsoft.Xna.Framework.Vector3(joint2.Position.X - joint3.Position.X, joint2.Position.Y - joint3.Position.Y, 0);
            
            Microsoft.Xna.Framework.Vector3 vectorJoint1ToJoint2 = new Microsoft.Xna.Framework.Vector3(x1, y1, z);
            vectorJoint1ToJoint2.X = x1;
            vectorJoint1ToJoint2.Y = y1;
            vectorJoint1ToJoint2.Z = z;
            Microsoft.Xna.Framework.Vector3 vectorJoint2ToJoint3 = new Microsoft.Xna.Framework.Vector3(x2, y2, z);
            vectorJoint2ToJoint3.X = x2;
            vectorJoint2ToJoint3.Y = y2;
            vectorJoint2ToJoint3.Z = z;

            //Microsoft.Xna.Framework.Vector3 v = new Microsoft.Xna.Framework.Vector3(1.4F, 2.5F, 10.0F);
 
            vectorJoint1ToJoint2.Normalize();
            vectorJoint2ToJoint3.Normalize();
            Microsoft.Xna.Framework.Vector3 crossProduct = new Microsoft.Xna.Framework.Vector3();
            //Cross product
            float v3x = (vectorJoint1ToJoint2.Y * vectorJoint2ToJoint3.Z) - (vectorJoint1ToJoint2.Z * vectorJoint2ToJoint3.Y);
            float v3y = (vectorJoint1ToJoint2.Z * vectorJoint2ToJoint3.X) - (vectorJoint1ToJoint2.X * vectorJoint2ToJoint3.Z);
            float v3z = (vectorJoint1ToJoint2.X * vectorJoint2ToJoint3.Y) - (vectorJoint1ToJoint2.Y * vectorJoint2ToJoint3.X);
            crossProduct.X=v3x;
            crossProduct.Y=v3y;
            crossProduct.Z = v3z;
            //Microsoft.Xna.Framework.Vector3.Cross(vectorJoint1ToJoint2, vectorJoint2ToJoint3);

            double crossProductLength = crossProduct.Z;
            double dotProduct; // = Microsoft.Xna.Framework.Vector3.Dot(vectorJoint1ToJoint2, vectorJoint2ToJoint3);
            dotProduct = (vectorJoint1ToJoint2.X * vectorJoint2ToJoint3.X) + (vectorJoint1ToJoint2.Y * vectorJoint2ToJoint3.Y) + (vectorJoint1ToJoint2.Z * vectorJoint2ToJoint3.Z);
            double segmentAngle = Math.Atan2(crossProductLength, dotProduct);

            // Convert the result to degrees.
            double degrees = ((segmentAngle * (180 / Math.PI))-90)%360;

            // Add the angular offset.  Use modulo 360 to convert the value calculated above to a range
            // from 0 to 360.
            //degrees = (degrees + _RotationOffset) % 360;
            /*
            // Calculate whether the coordinates should be reversed to account for different sides
            if (_ReverseCoordinates)
            {
                degrees = CalculateReverseCoordinates(degrees);
            }
            */
            return degrees;
        }

        /// <summary>
        /// Handles the checking or unchecking of the seated mode combo box
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void CheckBoxSeatedModeChanged(object sender, RoutedEventArgs e)
        {
            if (null != this.sensor)
            {
                if (this.checkBoxSeatedMode.IsChecked.GetValueOrDefault())
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                }
                else
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                }
            }
        }
    }
}