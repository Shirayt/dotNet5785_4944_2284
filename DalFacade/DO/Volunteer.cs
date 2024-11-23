
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
    int Id { get; set; } // ת.ז מתנדב
    string FullName { get; set; } // שם מלא (פרטי ומשפחה)
    string PhoneNumber { get; set; } // טלפון סלולרי
    string Email { get; set; } // אימייל
    string? Password { get; set; } // סיסמה
    string? CurrentFullAddress { get; set; } // כתובת מלאה נוכחית
    double? Latitude { get; set; } // קו רוחב
    double? Longitude { get; set; } // קו אורך
    Role Role { get; set; } // תפקיד (מנהל או מתנדב)
    bool IsActive { get; set; } // האם המתנדב פעיל
    double? MaxDistanceForCall { get; set; } // מרחק מרבי לקבלת קריאה
    DistanceType DistanceType { get; set; } = DistanceType.Air; // סוג המרחק, ברירת מחדל אווירי


    public Volunteer() { }

}
