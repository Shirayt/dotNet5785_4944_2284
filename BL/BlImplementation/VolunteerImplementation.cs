//namespace BlImplementation;
//using BlApi;

//internal class VolunteerImplementation : IVolunteer
//{
//    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

//    public string LoginVolunteerToSystem(string username, string password)
//    {
//        var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Email == username);

//        if (volunteer == null || volunteer.Password != password)
//        {
//            throw new Exception("Invalid email or password.");
//        }

//        return volunteer.Role.ToString();
//    }

//    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, Enum? sortBy = null)
//    {
//        var volunteers = _dal.Volunteer.ReadAll();

//        if (isActive.HasValue)
//        {
//            volunteers = volunteers.Where(v => v.IsActive == isActive.Value);
//        }

//        if (sortBy != null)
//        {
//            switch (sortBy)
//            {
//                case VolunteerSortOption.ByName:
//                    volunteers = volunteers.OrderBy(v => v.FullName);
//                    break;
//                case VolunteerSortOption.ByCompletedCalls:
//                    volunteers = volunteers.OrderByDescending(v => v.AmountOfCompletedCalls);
//                    break;
//                default:
//                    break;
//            }
//        }

//        return volunteers.Select(v => new BO.VolunteerInList
//        {
//            Id = v.Id,
//            FullName = v.FullName,
//            IsActive = v.IsActive,
//            //AmountOfCompletedCalls = v.AmountOfCompletedCalls,
//            //AmountOfSelfCancelledCalls = v.AmountOfSelfCancelledCalls,
//            //AmountOfExpiredCalls = v.AmountOfExpiredCalls,
//            //CallInTreatmentId = v.callInProgress ? (int?)v.Id : null
//        });
//    }


//    public BO.Volunteer GetVolunteerDetails(int volunteerId)
//    {
//        var volunteer = _dal.Volunteer.Read(volunteerId);
//        if (volunteer == null)
//        {
//            throw new Exception($"Volunteer with ID {volunteerId} does not exist.");
//        }

//        return new BO.Volunteer
//        {
//            Id = volunteer.Id,
//            FullName = volunteer.FullName,
//            PhoneNumber = volunteer.PhoneNumber,
//            Email = volunteer.Email,
//            Password = volunteer.Password,
//            CurrentFullAddress = volunteer.CurrentFullAddress,
//            Latitude = volunteer.Latitude,
//            Longitude = volunteer.Longitude,
//            Role = volunteer.Role,
//            IsActive = volunteer.IsActive,
//            MaxDistanceForCall = volunteer.MaxDistanceForCall,
//            DistanceType = volunteer.DistanceType,
//            AmountOfCompletedCalls = volunteer.AmountOfCompletedCalls,
//            AmountOfSelfCancelledCalls = volunteer.AmountOfSelfCancelledCalls,
//            AmountOfExpiredCalls = volunteer.AmountOfExpiredCalls,
//            callInProgress = volunteer.callInProgress
//        };
//    }

//    public void UpdateVolunteerDetails(int volunteerId, BO.Volunteer volunteer)
//    {
//        var existingVolunteer = _dal.Volunteer.Read(volunteerId);
//        if (existingVolunteer == null)
//        {
//            throw new Exception($"Volunteer with ID {volunteerId} does not exist.");
//        }

//        existingVolunteer.FullName = volunteer.FullName;
//        existingVolunteer.PhoneNumber = volunteer.PhoneNumber;
//        existingVolunteer.Email = volunteer.Email;
//        existingVolunteer.Password = volunteer.Password;
//        existingVolunteer.CurrentFullAddress = volunteer.CurrentFullAddress;
//        existingVolunteer.Latitude = volunteer.Latitude;
//        existingVolunteer.Longitude = volunteer.Longitude;
//        existingVolunteer.Role = volunteer.Role;
//        existingVolunteer.IsActive = volunteer.IsActive;
//        existingVolunteer.MaxDistanceForCall = volunteer.MaxDistanceForCall;
//        existingVolunteer.DistanceType = volunteer.DistanceType;
//        existingVolunteer.AmountOfCompletedCalls = volunteer.AmountOfCompletedCalls;
//        existingVolunteer.AmountOfSelfCancelledCalls = volunteer.AmountOfSelfCancelledCalls;
//        existingVolunteer.AmountOfExpiredCalls = volunteer.AmountOfExpiredCalls;
//        existingVolunteer.callInProgress = volunteer.callInProgress;

//        _dal.Volunteer.Update(existingVolunteer);
//    }

//    public void DeleteVolunteer(int volunteerId)
//    {
//        var volunteer = _dal.Volunteer.Read(volunteerId);
//        if (volunteer == null)
//        {
//            throw new Exception($"Volunteer with ID {volunteerId} does not exist.");
//        }

//        _dal.Volunteer.Delete(volunteerId);
//    }

//    public void AddVolunteer(BO.Volunteer volunteer)
//    {
//        if (_dal.Volunteer.Read(volunteer.Id) != null)
//        {
//            throw new Exception($"A volunteer with ID {volunteer.Id} already exists.");
//        }

//        var newVolunteer = new DO.Volunteer
//        {
//            Id = volunteer.Id,
//            FullName = volunteer.FullName,
//            PhoneNumber = volunteer.PhoneNumber,
//            Email = volunteer.Email,
//            Password = volunteer.Password,
//            CurrentFullAddress = volunteer.CurrentFullAddress,
//            Latitude = volunteer.Latitude,
//            Longitude = volunteer.Longitude,
//            Role = volunteer.Role,
//            IsActive = volunteer.IsActive,
//            MaxDistanceForCall = volunteer.MaxDistanceForCall,
//            DistanceType = volunteer.DistanceType,
//            AmountOfCompletedCalls = volunteer.AmountOfCompletedCalls,
//            AmountOfSelfCancelledCalls = volunteer.AmountOfSelfCancelledCalls,
//            AmountOfExpiredCalls = volunteer.AmountOfExpiredCalls,
//            callInProgress = volunteer.callInProgress
//        };

//        _dal.Volunteer.Add(newVolunteer);
//    }
//}
