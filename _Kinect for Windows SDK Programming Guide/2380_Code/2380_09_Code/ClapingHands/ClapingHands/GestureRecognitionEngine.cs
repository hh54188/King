using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace ClapingHands
{
    class GestureRecognitionEngine : IDisposable
    {
        public event EventHandler<GestureEventArgs> GestureRecognitionRejected;
        public event EventHandler<GestureEventArgs> GestureRecognized;

        public GestureType GestureType { get; set; }
        public Skeleton Skeleton { get; set; }

        public GestureRecognitionEngine()
        {

        }

        public GestureRecognitionEngine(Skeleton skeleton)
        {
            this.Skeleton = skeleton;
        }


        public GestureRecognitionEngine(Skeleton skeleton, GestureType gestureType)
        {
            this.Skeleton = skeleton;
            this.GestureType = gestureType;
            this.GestureRecognized += new EventHandler<GestureEventArgs>(GestureRecognitionEngine_GestureRecognized);
          
        }

        public void StartRecognize()
        {
            switch (this.GestureType)
            {
                case GestureType.HandsClapping:
                    GestureDefinition.MatchDefinitation(this.Skeleton, this.GestureType);
                    break;
                case GestureType.HandRaisedOverHead:
                    break;
                default:
                    break;
            }
        }

        private void ProcessGesture(Skeleton skeleton, GestureType gestureType)
        {
            GestureDefinition.GetDefinition(gestureType);
        }

        void GestureRecognitionEngine_GestureRecognized(object sender, GestureEventArgs e)
        {
            if (this.GestureRecognized != null)
            {
                this.GestureRecognized(this, e);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
