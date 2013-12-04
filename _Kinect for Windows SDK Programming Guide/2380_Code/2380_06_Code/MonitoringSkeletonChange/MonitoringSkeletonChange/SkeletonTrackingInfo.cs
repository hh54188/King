
namespace MonitoringSkeletonChange
{
    /// <summary>
    /// Skeletion Tracking Info
    /// </summary>
    class SkeletonTrackingInfo
    {
        /// <summary>
        /// Gets or sets the tracking ID.
        /// </summary>
        /// <value>
        /// The tracking ID.
        /// </value>
        public int TrackingID { get; set; }

        /// <summary>
        /// Gets or sets the total tracked joints.
        /// </summary>
        /// <value>
        /// The total tracked joints.
        /// </value>
        public int TotalTrackedJoints { get; set; }

        /// <summary>
        /// Gets or sets the tracked time.
        /// </summary>
        /// <value>
        /// The tracked time.
        /// </value>
        public string TrackedTime { get; set; }
    }
}
