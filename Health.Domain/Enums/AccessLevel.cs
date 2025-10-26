namespace Health.Domain.Enums
{
    /// <summary>
    /// Access permission levels for family members
    /// </summary>
    public enum AccessLevel
    {
        /// <summary>
        /// Can only view records, cannot edit
        /// </summary>
        ViewOnly = 1,

        /// <summary>
        /// Can view and edit records
        /// </summary>
        ViewAndEdit = 2,

        /// <summary>
        /// Full access including booking appointments
        /// </summary>
        FullAccess = 3
    }
}
