using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuseumTourSystem.Models.Models;

namespace MuseumTourSystem.BusinessLogic.Interfaces
{
    public interface IMuseumTourRepository
    {

            #region Tour Operations

            void AddTour(Tours tour);

            void RemoveTour(Tours tour);

            Tours GetTourById(string id);

            IEnumerable<Tours> GetAllTours();

            #endregion

            #region City Operations

            void AddCity(City city);

            void RemoveCity(City city);

            City GetCityById(string id);

            IEnumerable<City> GetAllCities();

            #endregion

            #region Museum Visit Operations

            void AddMuseumVisit(MuseumVisit museumVisit);

            void RemoveMuseumVisit(MuseumVisit museumVisit);

            MuseumVisit GetMuseumVisitById(string id);

            IEnumerable<MuseumVisit> GetAllMuseumVisits();

            #endregion

            #region Member Operations

            void AddMember(Member member);

            void RemoveMember(Member member);

            Member GetMemberById(string id);

            Member GetMemberByBookingNumber(string bookingNumber);

            IEnumerable<Member> GetAllMembers();

            #endregion

            void SaveChanges();
            void LoadData();
        }

    }

