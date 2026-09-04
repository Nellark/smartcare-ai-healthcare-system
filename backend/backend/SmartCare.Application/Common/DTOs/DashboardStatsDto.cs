namespace SmartCare.Application.Common.DTOs;

public class DashboardStatsDto
{
    public int TotalPatients { get; set; }
    public int TotalDoctors { get; set; }
    public int TotalAppointments { get; set; }
    public IReadOnlyList<PatientDto> RecentPatients { get; set; } = new List<PatientDto>();
    public IReadOnlyList<AppointmentDto> RecentAppointments { get; set; } = new List<AppointmentDto>();
}
