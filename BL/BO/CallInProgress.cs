
namespace BO;
using DO;
public class CallInProgress
{
    public int AssignmentId { get; init; }

    public int CallId { get; init; }

    public CallType CallType { get; set; }

    public String? Description { get; set; }

    public String FullAddress { get; set; }

    public DateTime OpenTime { get; set; }

    public DateTime? MaxEndTime { get; set; }

    public DateTime TreatmentStartTime { get; set; }

    public double DistanceFromVolunteer { get; set; }

    public StatusCallInProgress Status { get; set; }

    //public override string ToString() => this.ToStringProperty();

}
