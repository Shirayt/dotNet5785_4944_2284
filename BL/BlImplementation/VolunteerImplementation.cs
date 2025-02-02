using BlApi;
namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public BO.Role LoginVolunteerToSystem(string username, string password)
    {
        var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Email == username);

        if (volunteer == null || volunteer.Password != password)
        {
            throw new Exception("Invalid email or password.");
        }

        return (BO.Role)volunteer.Role;
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortOption? sortBy = null)
    {
        var volunteers = _dal.Volunteer.ReadAll().AsEnumerable();
        var assignments = _dal.Assignment.ReadAll();

        if (isActive.HasValue)
        {
            volunteers = volunteers.Where(v => v.IsActive == isActive.Value);
        }

        var volunteerList = volunteers.Select(v => new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            IsActive = v.IsActive,
            AmountOfCompletedCalls = Helpers.VolunteerManager.GetCompletedCallsCount(v.Id),
            AmountOfSelfCancelledCalls = Helpers.VolunteerManager.GetSelfCancelledCallsCount(v.Id),
            CallInTreatmentId = Helpers.VolunteerManager.GetCallInTreatment(v.Id)
        });

        if (sortBy != null)
        {
            switch (sortBy)
            {
                case BO.VolunteerSortOption.ByName:
                    volunteerList = volunteerList.OrderBy(v => v.FullName);
                    break;
                case BO.VolunteerSortOption.ByCompletedCalls:
                    volunteerList = volunteerList.OrderByDescending(v => v.AmountOfCompletedCalls);
                    break;
                default:
                    volunteerList = volunteerList.OrderBy(v => v.Id);
                    break;
            }
        }

        return volunteerList;
    }

    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId);

            if (volunteer == null)
            {
                throw new Exception($"Volunteer with ID {volunteerId} does not exist.");
            }

            BO.CallInProgress? callInProgress = Helpers.VolunteerManager.GetCallInProgress(volunteerId);

            return new BO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                PhoneNumber = volunteer.PhoneNumber,
                Email = volunteer.Email,
                Password = volunteer.Password,
                CurrentFullAddress = volunteer.CurrentFullAddress,
                Latitude = volunteer.Latitude,
                Longitude = volunteer.Longitude,
                Role = (BO.Role)volunteer.Role,
                IsActive = volunteer.IsActive,
                MaxDistanceForCall = volunteer.MaxDistanceForCall,
                DistanceType = (BO.DistanceType)volunteer.DistanceType,
                AmountOfCompletedCalls = Helpers.VolunteerManager.GetCompletedCallsCount(volunteer.Id),
                AmountOfSelfCancelledCalls = Helpers.VolunteerManager.GetSelfCancelledCallsCount(volunteer.Id),
                AmountOfExpiredCalls = Helpers.VolunteerManager.GetExpiredCallsCount(volunteer.Id),
                callInProgress = callInProgress
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving volunteer details: {ex.Message}");
        }
    }
    public void UpdateVolunteerDetails(int volunteerId, BO.Volunteer volunteer)
    {
        try
        {
            var existingVolunteer = _dal.Volunteer.Read(volunteerId);

            if (existingVolunteer == null)
            {
                throw new Exception($"Volunteer with ID {volunteerId} does not exist.");
            }

            if (existingVolunteer.Role != DO.Role.Manager && volunteerId != existingVolunteer.Id)
            {
                throw new Exception("Only a manager or the volunteer themselves can update the details.");
            }

            Helpers.VolunteerManager.ValidateBOVolunteerData(volunteer);

            // Update the volunteer details
            existingVolunteer.FullName = volunteer.FullName;
            existingVolunteer.PhoneNumber = volunteer.PhoneNumber;
            existingVolunteer.Email = volunteer.Email;
            existingVolunteer.Password = volunteer.Password;
            existingVolunteer.CurrentFullAddress = volunteer.CurrentFullAddress;
            existingVolunteer.Latitude = volunteer.Latitude;
            existingVolunteer.Longitude = volunteer.Longitude;
            existingVolunteer.IsActive = volunteer.IsActive;
            existingVolunteer.MaxDistanceForCall = volunteer.MaxDistanceForCall;
            existingVolunteer.DistanceType = (DO.DistanceType)volunteer.DistanceType;
            // Only managers can update volunteer's role
            existingVolunteer.Role = existingVolunteer.Role == DO.Role.Manager ? (DO.Role)volunteer.Role : existingVolunteer.Role;

            // Update volunteer data in the database
            _dal.Volunteer.Update(existingVolunteer);
        }
        catch (Exception ex)//DO.DataException
        {
            throw new Exception("Error updating volunteer details.", ex);//BO.Logic
        }
    }

    public void DeleteVolunteer(int volunteerId)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId);

            if (volunteer == null)
            {
                throw new Exception($"Volunteer with ID {volunteerId} does not exist.");
            }

            /// Checks if the volunteer is currently handling or has ever handled a call.
            if (Helpers.VolunteerManager.GetCallInTreatment(volunteer.Id) == null)
            {
                throw new Exception($"Volunteer with ID {volunteerId} cannot be deleted as they have handled calls.");
            }

            _dal.Volunteer.Delete(volunteerId);
        }
        catch (Exception ex)//DO.DataException
        {
            throw new Exception("Error updating volunteer details.", ex);//BO.Logic
        }
    }

    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            Helpers.VolunteerManager.ValidateBOVolunteerData(volunteer);

            if (_dal.Volunteer.Read(volunteer.Id) != null)
            {
                throw new Exception($"A volunteer with ID {volunteer.Id} already exists.");
            }

            var newVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                PhoneNumber = volunteer.PhoneNumber,
                Email = volunteer.Email,
                Password = volunteer.Password,
                CurrentFullAddress = volunteer.CurrentFullAddress,
                Latitude = volunteer.Latitude,
                Longitude = volunteer.Longitude,
                Role = (DO.Role)volunteer.Role,
                IsActive = volunteer.IsActive,
                MaxDistanceForCall = volunteer.MaxDistanceForCall,
                DistanceType = (DO.DistanceType)volunteer.DistanceType,
            };

            _dal.Volunteer.Create(newVolunteer);
        }
        catch (Exception ex)//DO.DataException
        {
            throw new Exception("Error updating volunteer details.", ex);//BO.Logic
        }
    }
}

