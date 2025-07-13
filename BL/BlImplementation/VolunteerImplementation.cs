﻿using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System;
using System.Net;
namespace BlImplementation;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public BO.Role LoginVolunteerToSystem(int userId, string password)
    {
        IEnumerable<DO.Volunteer>? volunteers;
        lock (AdminManager.blMutex)
            volunteers = _dal.Volunteer.ReadAll();

        var volunteer = volunteers?.FirstOrDefault(v => v.Id == userId);

        if (volunteer is null || volunteer.Password != password)
        {
            throw new BlInvalidInputException("Invalid username or password in Login Volunteer To System");
        }

        return (BO.Role)volunteer.Role;
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortOption? sortBy = null)
    {
        IEnumerable<DO.Volunteer>? volunteersList;
        lock (AdminManager.blMutex)
            volunteersList = _dal.Volunteer.ReadAll();

        var volunteerList = from v in volunteersList ?? Enumerable.Empty<DO.Volunteer>()
                            let completedCalls = Helpers.VolunteerManager.GetCompletedCallsCount(v.Id)
                            let selfCancelledCalls = Helpers.VolunteerManager.GetSelfCancelledCallsCount(v.Id)
                            let expiredCalls = Helpers.VolunteerManager.GetExpiredCallsCount(v.Id)
                            let callInTreatmentId = Helpers.VolunteerManager.GetCallInTreatment(v.Id)
                            where !isActive.HasValue || v.IsActive == isActive.Value
                            select new BO.VolunteerInList
                            {
                                Id = v.Id,
                                FullName = v.FullName,
                                IsActive = v.IsActive,
                                AmountOfCompletedCalls = completedCalls,
                                AmountOfSelfCancelledCalls = selfCancelledCalls,
                                AmountOfExpiredCalls = expiredCalls,
                                CallInTreatmentId = callInTreatmentId
                            };

        if (sortBy != null)
        {
            volunteerList = sortBy switch
            {
                BO.VolunteerSortOption.ByName => from v in volunteerList orderby v.FullName select v,
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
            DO.Volunteer volunteer;
            lock (AdminManager.blMutex)
                volunteer = _dal.Volunteer.Read(volunteerId);

            BO.CallInProgress? callInProgress = Helpers.VolunteerManager.GetCallInProgress(volunteerId);

            return new BO.Volunteer
            {
                Id = volunteer!.Id,
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
                callInProgress = callInProgress,
            };
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Something went wrong during Get Volunteer Details in BL: ", ex);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public async Task AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            Helpers.VolunteerManager.ValidateBOVolunteerData(volunteer);

     
            var newVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                PhoneNumber = volunteer.PhoneNumber,
                Email = volunteer.Email,
                Password = volunteer.Password,
                CurrentFullAddress = volunteer.CurrentFullAddress,
                Latitude = 0,
                Longitude = 0,
                Role = (DO.Role)volunteer.Role,
                IsActive = volunteer.IsActive,
                MaxDistanceForCall = volunteer.MaxDistanceForCall??0,
                DistanceType = (DO.DistanceType)volunteer.DistanceType,
            };

            lock (AdminManager.blMutex)
            {
                _dal.Volunteer.Create(newVolunteer);
                newVolunteer.Id = volunteer.Id;
            }

            VolunteerManager.Observers.NotifyListUpdated();

            _ = Task.Run(() => Tools.UpdateCoordinatesForVolunteerAddressAsync(newVolunteer));
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Something went wrong during volunteer addition in BL: ", ex);
        }
        catch (BO.BlTemporaryNotAvailableException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }

    }

    public async Task UpdateVolunteerDetails(int volunteerId, BO.Volunteer volunteer)
    {

        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();

            DO.Volunteer existingVolunteer;
            lock (AdminManager.blMutex)
                existingVolunteer = _dal.Volunteer.Read(volunteerId);

            if (existingVolunteer.Role != DO.Role.Manager && volunteerId != existingVolunteer.Id)
            {
                throw new BlAuthorizationException("Only a manager or the volunteer themselves can update the details.");
            }

            if (existingVolunteer.Role != DO.Role.Manager && (DO.Role)volunteer.Role == DO.Role.Manager)
            {
                throw new BlAuthorizationException("Only a manager can assign the Manager role.");
            }

            if ((DO.Role)volunteer.Role != DO.Role.Manager && VolunteerManager.IsSingleManager(volunteerId, existingVolunteer.Role))
            {
                throw new BlAuthorizationException("Cannot remove the last remaining manager from the system.");
            }

            Helpers.VolunteerManager.ValidateBOVolunteerData(volunteer);

            existingVolunteer.FullName = volunteer.FullName;
            existingVolunteer.PhoneNumber = volunteer.PhoneNumber;
            existingVolunteer.Email = volunteer.Email;
            existingVolunteer.Password = volunteer.Password;
            existingVolunteer.CurrentFullAddress = volunteer.CurrentFullAddress;
            existingVolunteer.Latitude = 0;
            existingVolunteer.Longitude = 0;
            existingVolunteer.IsActive = volunteer.IsActive;
            existingVolunteer.MaxDistanceForCall = volunteer.MaxDistanceForCall;
            existingVolunteer.DistanceType = (DO.DistanceType)volunteer.DistanceType;
            existingVolunteer.Role = (DO.Role)volunteer.Role;

            lock (AdminManager.blMutex)
                _dal.Volunteer.Update(existingVolunteer);

            _ = Task.Run(() => Tools.UpdateCoordinatesForVolunteerAddressAsync(existingVolunteer));

            VolunteerManager.Observers.NotifyItemUpdated(existingVolunteer.Id);
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Somthing went wrong during Update Volunteer Details in BL: ", ex);
        }
        catch (BO.BlTemporaryNotAvailableException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public void DeleteVolunteer(int volunteerId)
    {

        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();

            DO.Volunteer volunteer;
            lock (AdminManager.blMutex)
                volunteer = _dal.Volunteer.Read(volunteerId);

            int AmountOfCompletedCalls = Helpers.VolunteerManager.GetCompletedCallsCount(volunteer.Id);
            int AmountOfSelfCancelledCalls = Helpers.VolunteerManager.GetSelfCancelledCallsCount(volunteer.Id);
            int AmountOfExpiredCalls = Helpers.VolunteerManager.GetExpiredCallsCount(volunteer.Id);

            if (Helpers.VolunteerManager.GetCallInTreatment(volunteer.Id) != null || AmountOfCompletedCalls > 0 || AmountOfExpiredCalls > 0 || AmountOfSelfCancelledCalls > 0)
            {
                throw new BlInvalidOperationException($"Volunteer with ID {volunteerId} cannot be deleted as they have handled calls.");
            }

            lock (AdminManager.blMutex)
                _dal.Volunteer.Delete(volunteerId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Somthing went wrong during volunteer deletion in BL: ", ex);
        }
        catch (BO.BlTemporaryNotAvailableException)
        {
            throw;
        }
        catch (BO.BlInvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }

        VolunteerManager.Observers.NotifyListUpdated();
    }
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
}
