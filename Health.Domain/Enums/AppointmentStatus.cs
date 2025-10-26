namespace Health.Domain.Enums
{
    /// <summary>
    /// Status of appointment lifecycle
    /// </summary>
    public enum AppointmentStatus
    {
        /// <summary>
        /// Appointment requested, awaiting doctor confirmation
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Doctor confirmed the appointment
        /// </summary>
        Confirmed = 2,

        /// <summary>
        /// Appointment completed successfully
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Cancelled by patient or admin
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// Rejected by doctor
        /// </summary>
        Rejected = 5,

        /// <summary>
        /// Patient didn't show up
        /// </summary>
        NoShow = 6
    }
}
