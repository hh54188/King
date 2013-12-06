using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace ConsoleForKinect
{
    class Program
    {
        private static KinectSensor _kinect;
        static void Main(string[] args)
        {
            Console.WriteLine("{0} - Start sample application", DateTime.Now.TimeOfDay.ToString());
            KinectSensor.KinectSensors.StatusChanged += KinectSensorsStatusChanged;
            Console.ReadLine();
            if (_kinect != null)
            {
                _kinect.Stop();
            }
        }
        static void KinectSensorsStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Console.WriteLine("{0} - New Status: {1}", DateTime.Now.TimeOfDay.ToString(), e.Status);
            _kinect = e.Sensor;
            if (e.Sensor != null)
            {
                Console.WriteLine("{0} - Sensor Status: {1}", DateTime.Now.TimeOfDay.ToString(), _kinect.Status);
                if (_kinect.Status == KinectStatus.Connected)
                {
                    _kinect.DepthStream.Enable();
                    _kinect.ColorStream.Enable();
                    _kinect.SkeletonStream.Enable();
                    _kinect.Start();
                    Console.WriteLine("{0} - Sensor Started", DateTime.Now.TimeOfDay.ToString());
                }
            }
            else
            {
                Console.WriteLine("{0} - No Sensor", DateTime.Now.TimeOfDay.ToString());
            }
        } 
    }
}
