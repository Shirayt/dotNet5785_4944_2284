using BlApi;
using BO;

namespace BlImplementation
{
    internal class VolunteerImplementation : IVolunteer
    {
        public void AddVolunteer(Volunteer volunteer)
        {
            throw new NotImplementedException();
        }

        public void DeleteVolunteer(int volunteerId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VolunteerInList> GetVolunteersList(bool? isActive = null, Volunteer GetVolunteerDetails = null, (int volunteerId, object)  = default)
        {
            throw new NotImplementedException();
        }

        public string LoginVolunteerToSystem(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void UpdateVolunteerDetails(int volunteerId, Volunteer volunteer)
        {
            throw new NotImplementedException();
        }
    }
}
