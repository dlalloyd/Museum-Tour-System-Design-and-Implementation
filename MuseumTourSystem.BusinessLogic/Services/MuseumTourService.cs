using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuseumTourSystem.Models.Models;
using MuseumTourSystem.BusinessLogic.Interfaces;
using System.Xml.Linq;

namespace MuseumTourSystem.BusinessLogic.Services
{
    public class MuseumTourService
    {
        private readonly IMuseumTourRepository _repository;

        public MuseumTourService(IMuseumTourRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #region Tour Management
        public Tours AddTour(string id, string name)
        {
            if (_repository.GetTourById(id) != null)
            {
                throw new ApplicationException($"A tour with the ID of '{id}' already exists");
            }

            var tour = new Tours(id, name);
            _repository.AddTour(tour);
            _repository.SaveChanges();

            return tour;

        }

        public bool RemoveTour(string id)
        {
            var tour = _repository.GetTourById(id);
            if (tour == null)
            {
                return false;
            }

            _repository.RemoveTour(tour);
            _repository.SaveChanges();

            return true;
        }

        public Tours GetTourById(string id)
        {
            return _repository.GetTourById(id);
        }

        public IEnumerable<Tours> GetAllTours()
        {
            return _repository.GetAllTours();
        }

        #endregion

        #region City Management
        public City AddCity(string id, string name)
        {
            // Checks if city with the same ID already exists
            if (_repository.GetCityById(id) != null)
            {
                throw new ApplicationException($"A city with ID '{id}' al-ready exists");
            }

            var city = new City(id, name);
            _repository.AddCity(city);
            _repository.SaveChanges();

            return city;
        }

        public bool RemoveCity(string id)
        {
            var city = _repository.GetCityById(id);
            if (city == null)
            {
                return false;
            }

            // Checks if the city is associated with any tours
            var tours = _repository.GetAllTours().Where(t => t.Cities.Contains(city));
            if (tours.Any())
            {
                // Remove the city from all associated tours
                foreach (var tour in tours)
                {
                    tour.RemoveCity(city);
                }
            }

            _repository.RemoveCity(city);
            _repository.SaveChanges();

            return true;
        }

        public City GetCityById(string id)
        {
            return _repository.GetCityById(id);
        }

        public IEnumerable<City> GetAllCities()
        {
            return _repository.GetAllCities();
        }

        public bool AddCityToTour(string tourId, string cityId)
        {
            var tour = _repository.GetTourById(tourId);
            if (tour == null)
            {
                throw new ApplicationException($"Tour with ID '{tourId}' not found");
            }

            var city = _repository.GetCityById(cityId);
            if (city == null)
            {
                throw new ApplicationException($"City with ID '{cityId}' not found");
            }

            var result = tour.AddCity(city);
            _repository.SaveChanges();

            return result;
        }

        public bool RemoveCityFromTour(string tourId, string cityId)
        {
            var tour = _repository.GetTourById(tourId);
            if (tour == null)
            {
                throw new ApplicationException($"Tour with ID '{tourId}' not found");
            }

            var city = _repository.GetCityById(cityId);
            if (city == null)
            {
                throw new ApplicationException($"City with ID '{cityId}' not found");
            }            // Check if there are members registered for muse-um visits in this city
            foreach (var member in tour.Members)
            {
                foreach (var visit in member.RegisteredMuseumVisits)
                {
                    if (visit.City != null && visit.City.Equals(city))
                    {
                        throw new ApplicationException($"Cannot remove city '{city.Name}' because member '{member.Name}' has a museum visit scheduled in this city");
                    }
                }
            }

            var result = tour.RemoveCity(city);
            _repository.SaveChanges();

            return result;
        }

        #endregion


        #region Museum Visit Management

        public MuseumVisit AddMuseumVisit(string id, string cityId, string museumName, DateTime visitDate, decimal cost)
        {
            // Check if museum visit with same ID already exists
            if (_repository.GetMuseumVisitById(id) != null)
            {
                throw new ApplicationException($"A museum visit with ID '{id}' already exists");
            }

            var city = _repository.GetCityById(cityId);
            if (city == null)
            {
                throw new ApplicationException($"City with ID '{cityId}' not found");
            }

            var museumVisit = new MuseumVisit(id, museumName, visitDate, cost, city);
            city.AddMuseumVisit(museumVisit);
            _repository.AddMuseumVisit(museumVisit);
            _repository.SaveChanges();

            return museumVisit;
        }

        public bool RemoveMuseumVisit(string id)
        {
            var museumVisit = _repository.GetMuseumVisitById(id);
            if (museumVisit == null)
            {
                return false;
            }

            // Remove the museum visit from its city
            if (museumVisit.City != null)
            {
                museumVisit.City.RemoveMuseumVisit(museumVisit);
            }

            // Remove all members from this museum visit
            var members = new List<Member>(museumVisit.RegisteredMembers);
            foreach (var member in members)
            {
                museumVisit.RemoveMember(member);
            }

            _repository.RemoveMuseumVisit(museumVisit);
            _repository.SaveChanges();

            return true;
        }

        public MuseumVisit GetMuseumVisitById(string id)
        {
            return _repository.GetMuseumVisitById(id);
        }

        public IEnumerable<MuseumVisit> GetAllMuseumVisits()
        {
            return _repository.GetAllMuseumVisits();
        }

        #endregion


        #region Member Management

        public Member AddMember(string id, string tourId, string name, string bookingNumber)
        {
            // Check if member with same booking number already exists in system
            var existingMember = _repository.GetMemberByBookingNumber(bookingNumber);
            if (existingMember != null)
            {
                throw new ApplicationException($"A member with booking num-ber '{bookingNumber}' already exists");
            }

            var tour = _repository.GetTourById(tourId);
            if (tour == null)
            {
                throw new ApplicationException($"Tour with ID '{tourId}' not found");
            }

            var member = new Member(id, name, bookingNumber);
            tour.AddMember(member);
            _repository.AddMember(member);
            _repository.SaveChanges();

            return member;
        }

        public bool RemoveMember(string id)
        {
            var member = _repository.GetMemberById(id);
            if (member == null)
            {
                return false;
            }

            // Remove member from their tour
            if (member.Touring != null)
            {
                member.Touring.RemoveMember(member);
            }            // Remove member from all museum visits
            var visits = new List<MuseumVisit>(member.RegisteredMuseumVisits);
            foreach (var visit in visits)
            {
                visit.RemoveMember(member);
            }

            _repository.RemoveMember(member);
            _repository.SaveChanges();

            return true;
        }

        public Member GetMemberById(string id)
        {
            return _repository.GetMemberById(id);
        }

        public Member GetMemberByBookingNumber(string bookingNumber)
        {
            return _repository.GetMemberByBookingNumber(bookingNumber);
        }

        public IEnumerable<Member> GetAllMembers()
        {
            return _repository.GetAllMembers();
        }

        public bool AddMemberToMuseumVisit(string memberId, string visitId)
        {
            var member = _repository.GetMemberById(memberId);
            if (member == null)
            {
                throw new ApplicationException($"Member with ID '{memberId}' not found");
            }

            var visit = _repository.GetMuseumVisitById(visitId);
            if (visit == null)
            {
                throw new ApplicationException($"Museum visit with ID '{visitId}' not found");
            }

            var result = visit.AddMember(member);
            _repository.SaveChanges();

            return result;
        }

        public bool RemoveMemberFromMuseumVisit(string memberId, string visitId)
        {
            var member = _repository.GetMemberById(memberId);
            if (member == null)
            {
                throw new ApplicationException($"Member with ID '{memberId}' not found");
            }

            var visit = _repository.GetMuseumVisitById(visitId);
            if (visit == null)
            {
                throw new ApplicationException($"Museum visit with ID '{visitId}' not found");
            }

            var result = visit.RemoveMember(member);
            _repository.SaveChanges();

            return result;
        }

        #endregion
    
 
    }
}
