
namespace DO;
public enum Role
{
    Manager,
    Volunteer
}

public enum DistanceType
{
    Air, 
    Walk,
    Drive
}
public record Volunteer
{
    public int Id { get; set; } // ת.ז מתנדב
    public string FullName { get; set; } // שם מלא (פרטי ומשפחה)
    public string PhoneNumber { get; set; } // טלפון סלולרי
    public string Email { get; set; } // אימייל
    public string? Password { get; set; } // סיסמה
    public string? CurrentFullAddress { get; set; } // כתובת מלאה נוכחית
    public double? Latitude { get; set; } // קו רוחב
    public double? Longitude { get; set; } // קו אורך
    public Role Role { get; set; } // תפקיד (מנהל או מתנדב)
    public bool IsActive { get; set; } // האם המתנדב פעיל
    public double? MaxDistanceForCall { get; set; } // מרחק מרבי לקבלת קריאה
    public DistanceType DistanceType { get; set; } = DistanceType.Air; // סוג המרחק, ברירת מחדל אווירי

   
}
