using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace ClapingHands
{
    public class GestureDefinition 
    {
       
        internal static void MatchDefinitation(Microsoft.Kinect.Skeleton skeleton, GestureType gestureType)
        {
            switch (gestureType)
            {
                case GestureType.HandsClapping:
                    MatchHandClappingGesture(skeleton);
                    break;
                case GestureType.HandRaisedOverHead:
                    break;
                default:
                    break;
            }      
        }

        private static void MatchHandClappingGesture(Skeleton skeleton)
        {
            float previousDistance = 0.50f;
            if (skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked && skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
            {
                float currentDistance = GetJointDistance(skeleton.Joints[JointType.HandRight], skeleton.Joints[JointType.HandLeft]);
                if (currentDistance < 0.1f && previousDistance > 0.1f)
                {
                    
                    currentDistance = previousDistance;
                }
                else
                {
                    
                }
            }
        }


        private static float GetJointDistance(Joint firstJoint, Joint secondJoint)
        {
            float distanceX = firstJoint.Position.X - secondJoint.Position.X;
            float distanceY = firstJoint.Position.Y - secondJoint.Position.Y;
            float distanceZ = firstJoint.Position.Z - secondJoint.Position.Z;
            return (float)Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2) + Math.Pow(distanceZ, 2));
        }
    }
}
