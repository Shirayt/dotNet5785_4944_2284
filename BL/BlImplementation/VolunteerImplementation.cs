﻿using BlApi;
using BO;
namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public BO.Role LoginVolunteerToSystem(string username, string password)
    {
        var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Email == username);

        if (volunteer == null || volunteer.Password != password)
        {
            throw new BlInvalidInputException("Invalid email or password in Login Volunteer To System.");
        }

        return (BO.Role)volunteer.Role;
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortOption? sortBy = null)
    {
        //using LINQ to object
        var volunteerList = from v in _dal.Volunteer.ReadAll()
                            let completedCalls = Helpers.VolunteerManager.GetCompletedCallsCount(v.Id)
                            let selfCancelledCalls = Helpers.VolunteerManager.GetSelfCancelledCallsCount(v.Id)
                            let callInTreatmentId = Helpers.VolunteerManager.GetCallInTreatment(v.Id)
                            where !isActive.HasValue || v.IsActive == isActive.Value
                            select new BO.VolunteerInList
                            {
                                Id = v.Id,
                                FullName = v.FullName,
                                IsActive = v.IsActive,
                                AmountOfCompletedCalls = completedCalls,
                                AmountOfSelfCancelledCalls = selfCancelledCalls,
                                CallInTreatmentId = callInTreatmentId
                            };

        if (sortBy != null)
        {
            volunteerList = sortBy switch
            {
                BO.VolunteerSortOption.ByName => from v in volunteerList orderby v.FullName select v,//using LINQ to object
                BO.VolunteerSortOption.ByCompletedCalls => from v in volunteerList orderby v.AmountOfCompletedCalls descending select v,
                _ => from v in volunteerList orderby v.Id select v
            };
        }

        return volunteerList;
    }


    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId);

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
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Something went wrong during Get Volunteer Details in BL: ", ex);
        }
    }
    public void UpdateVolunteerDetails(int volunteerId, BO.Volunteer volunteer)
    {
        try
        {
            var existingVolunteer = _dal.Volunteer.Read(volunteerId);

            if (existingVolunteer.Role != DO.Role.Manager && volunteerId != existingVolunteer.Id)
            {
                throw new BlAuthorizationException("Only a manager or the volunteer themselves can update the details.");
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
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Somthing went wrong during Update Volunteer Details in BL: ", ex);
        }
    }

    public void DeleteVolunteer(int volunteerId)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId);

            /// Checks if the volunteer is currently handling or has ever handled a call.
            if (Helpers.VolunteerManager.GetCallInTreatment(volunteer.Id) == null)
            {
                throw new BlInvalidOperationException($"Volunteer with ID {volunteerId} cannot be deleted as they have handled calls.");
            }

            _dal.Volunteer.Delete(volunteerId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Somthing went wrong during volunteer deletion in BL: ", ex);
        }
    }

    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            Helpers.VolunteerManager.ValidateBOVolunteerData(volunteer);

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
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Somthing went wrong during volunteer addition in BL: ", ex);
        }
    }
}

