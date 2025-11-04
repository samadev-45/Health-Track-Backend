// Health.Application/Configuration/AppointmentPolicyConfig.cs
namespace Health.Application.Configuration
{
    public class AppointmentPolicyConfig
    {
       
        /// Reschedule cutoff in hours (e.g., 2 = cannot reschedule if less than 2 hours before appointment).
       
        public int RescheduleCutoffHours { get; set; } = 2;

        
        /// Maximum allowed reschedules per appointment (optional).
      
        public int MaxReschedules { get; set; } = 3;

        
        /// If true, reschedule sets appointment.Status -> Pending (doctor approval required).
        /// If false, keep existing status (e.g., Confirmed stays Confirmed).
        
        public bool RequireDoctorApprovalOnReschedule { get; set; } = true;

       
        /// Default appointment slot minutes for overlap checks (e.g., 30).
        
        public int SlotMinutes { get; set; } = 30;
    }
}
