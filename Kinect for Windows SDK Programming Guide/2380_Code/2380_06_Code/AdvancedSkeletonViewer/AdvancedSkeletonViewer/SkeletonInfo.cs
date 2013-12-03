using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
namespace AdvancedSkeletonViewer
{
    /// <summary>
    /// Skeleton Info
    /// </summary>
    class SkeletonInfo
    {
        /// <summary>
        /// Gets or sets the frame ID.
        /// </summary>
        /// <value>
        /// The frame ID.
        /// </value>
        public int FrameID { get; set; }

        /// <summary>
        /// Gets or sets the skeleton.
        /// </summary>
        /// <value>
        /// The skeleton.
        /// </value>
        public Skeleton Skeleton { get; set; }
    }
}
