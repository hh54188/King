// ***********************************************************************
// <copyright file="ColorImageWrapper.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace KinectCam
{
    using System;
    using Microsoft.Kinect;

    /// <summary>
    /// Color Image Wrapper
    /// </summary>
    internal class ColorImageWrapper : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorImageWrapper" /> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        public ColorImageWrapper(ColorImageFrame frame)
        {
            this.ImageFrame = frame;
            this.NeedDispose = true;
        }

        /// <summary>
        /// Gets or sets the image frame.
        /// </summary>
        /// <value>
        /// The image frame.
        /// </value>
        internal ColorImageFrame ImageFrame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [need dispose].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [need dispose]; otherwise, <c>false</c>.
        /// </value>
        internal bool NeedDispose { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.ImageFrame != null && this.NeedDispose)
            {
                this.ImageFrame.Dispose();
            }

            this.NeedDispose = false;
        }
    }
}