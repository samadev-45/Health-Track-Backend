namespace Health.Domain.Enums
{
    /// <summary>
    /// User role types in the system
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// Patient - Primary user who tracks their own health
        /// </summary>
        Patient = 1,

        /// <summary>
        /// Family Member - Can access patient records with permission
        /// </summary>
        FamilyMember = 2,

        /// <summary>
        /// System Administrator
        /// </summary>
        Admin = 3,

        /// <summary>
        /// Healthcare Provider (Doctor/Physician)
        /// </summary>
        Doctor = 4
    }
}
