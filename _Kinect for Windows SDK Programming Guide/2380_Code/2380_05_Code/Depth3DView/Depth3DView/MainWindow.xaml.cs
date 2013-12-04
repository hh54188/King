// Depth3D View
// Refer : http://msdn.microsoft.com/en-us/library/ms747437.aspx &&  http://msdn.microsoft.com/en-us/library/ms746607.aspx for details
// understanding of 3D graphics and Rendering.

namespace Depth3DView
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int pixelHeight = 240;
        int pixelWidth = 320;
        private GeometryModel3D[] modelPoints = new GeometryModel3D[320 * 240];
        private KinectSensor sensor;
        private GeometryModel3D geometryModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetData();
            this.StartSensor();
        }
        /// <summary>
        /// Sets the data.
        /// </summary>
        private void SetData()
        {
            int i = 0;
            int posZ = 0;
            for (int posY = 0; posY < pixelHeight; posY += 2)
            {
                for (int posX = 0; posX < pixelWidth; posX += 2)
                {
                    modelPoints[i] = CreateTriangleModel(new Point3D(posX, posY, posZ), new Point3D(posX, posY + 2, posZ), new Point3D(posX + 2, posY + 2, posZ));
                    modelPoints[i].Transform = new TranslateTransform3D(0, 0, 0);
                    modelGroup.Children.Add(modelPoints[i]);
                    i++;
                }
            }
        }

        /// <summary>
        /// Creates the triangle model.
        /// </summary>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns></returns>
        private GeometryModel3D CreateTriangleModel(Point3D p0, Point3D p1, Point3D p2)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Black));
            geometryModel = new GeometryModel3D(mesh, material);
            return geometryModel;
        }

        /// <summary>
        /// Starts the sensor.
        /// </summary>
        private void StartSensor()
        {
            this.sensor = KinectSensor.KinectSensors[0];
            this.sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            this.sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
            this.sensor.Start();
        }

        /// <summary>
        /// Handles the DepthFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DepthImageFrameReadyEventArgs" /> instance containing the event data.</param>
        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame == null)
                {
                    return;
                }
                short[] pixelData = new short[depthImageFrame.PixelDataLength];
                depthImageFrame.CopyPixelDataTo(pixelData);
                int translatePoint = 0;
                for (int posY = 0; posY < depthImageFrame.Height; posY += 2)
                {
                    for (int posX = 0; posX < depthImageFrame.Width; posX += 2)
                    {
                        int depth = ((ushort)pixelData[posX + posY * depthImageFrame.Width]) >> 3;
                        if (depth == sensor.DepthStream.UnknownDepth)
                        {
                            continue;
                        }
                        ((TranslateTransform3D)modelPoints[translatePoint].Transform).OffsetZ = depth;
                        translatePoint++;
                    }
                }
            }
        }

        /// <summary>
        /// Xs the slider_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void XSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
              new Action(
                  delegate()
                  {
                      camera.Position = new Point3D(
      e.NewValue,
          camera.Position.Y, camera.Position.Z);
                  }
              ));
        }

        /// <summary>
        /// Ys the slider_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void YSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                          new Action(
                              delegate()
                              {
                                  camera.Position = new Point3D(
                 camera.Position.X,
                       e.NewValue, camera.Position.Z);
                              }
                          ));
        }

        /// <summary>
        /// Zs the slider_ value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ZSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
              new Action(
                  delegate()
                  {
                      camera.Position = new Point3D(
      camera.Position.X,
          camera.Position.Y, e.NewValue);
                  }
              ));
        }
    }
}