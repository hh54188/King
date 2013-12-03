/* Gesture Recognition Engine */

namespace GestureRecognizer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Kinect;

    /// <summary>
    /// Gesture Recognition Engine
    /// </summary>
    public class GestureRecognitionEngine
    {
     
        /// <summary>
        /// Initializes a new instance of the <see cref="GestureRecognitionEngine" /> class.
        /// </summary>
        public GestureRecognitionEngine()
        {
        }
        
        /// <summary>
        /// Occurs when [gesture recognized].
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureRecognized;

        /// <summary>
        /// Occurs when [gesture not recognized].
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureNotRecognized;

        /// <summary>
        /// Gets or sets the type of the gesture.
        /// </summary>
        /// <value>
        /// The type of the gesture.
        /// </value>
        public GestureType GestureType { get; set; }

        /// <summary>
        /// Gets or sets the skeleton.
        /// </summary>
        /// <value>
        /// The skeleton.
        /// </value>
        public Skeleton Skeleton { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float previousDistance = 0.0f;

        /// <summary>
        /// Starts the recognize.
        /// </summary>
        public void StartRecognize()
        {
            switch (this.GestureType)
            {
                case GestureType.HandsClapping:
                    this.MatchHandClappingGesture(this.Skeleton);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the joint distance.
        /// </summary>
        /// <param name="firstJoint">The first joint.</param>
        /// <param name="secondJoint">The second joint.</param>
        /// <returns>retunr the distance</returns>
        private static float GetJointDistance(Joint firstJoint, Joint secondJoint)
        {
            float distanceX = firstJoint.Position.X - secondJoint.Position.X;
            float distanceY = firstJoint.Position.Y - secondJoint.Position.Y;
            float distanceZ = firstJoint.Position.Z - secondJoint.Position.Z;
            return (float)Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2) + Math.Pow(distanceZ, 2));
        }

        /// <summary>
        /// Matches the hand clapping gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        private void MatchHandClappingGesture(Skeleton skeleton)
        {
            if (skeleton == null)
            {
                return;
            }

            if (skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked && skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
            {
                float currentDistance = GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.HandLeft]);
                if (currentDistance < 0.1f && previousDistance > 0.1f)
                {
                    if (this.GestureRecognized != null)
                    {
                        this.GestureRecognized(this, new GestureEventArgs(RecognitionResult.Success));
                    }
                }

                previousDistance = currentDistance;
            }
        }
    }
}
