using DO;

namespace BO;
public class VolunteerInList
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    public int AmountOfCompletedCalls { get; set; }
    public int AmountOfSelfCancelledCalls { get; set; }
    public int AmountOfExpiredCalls { get; set; }
    public int? CallInTreatmentId { get; set; }
    //public override string ToString() => this.ToStringProperty();

}
