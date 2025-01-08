
namespace BO;
using DO;
public class ClosedCallInList
{
    public int Id { get; init; }
    public CallType CallType { get; set; }
    public string FullAddress { get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime? TreatmentStartTime { get; set; }
    public AssignmentStatus? Status { get; set; }

    //public override string ToString() => this.ToStringProperty();
}
