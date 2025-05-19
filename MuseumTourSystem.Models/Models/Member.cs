using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumTourSystem.Models.Models
{
    public class Member
    {
        public int Id { get; }

        public string Name { get; }

        public string BookingNumber { get; }

        public Tours? Touring { get; }

        public List<MuseumVisit> RegisteredMuseumVisits { get; }

        public int IncludedVisits { get; set; } = 2;

        public Member(string id, string name, string bookingNumber)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("Member ID can't be empty");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Member name can't be empty");
            }

            if (string.IsNullOrWhiteSpace(bookingNumber))
            {
                throw new ArgumentNullException("Booking Number can't be empty");
            }

            Id = id;
            Name = name;
            BookingNumber = bookingNumber;
            RegisteredMuseumVisits = new List<MuseumVisit>();
        }

        public bool RegisteredForMuseumVisit(MuseumVisit museumVisit)
        {
            if (museumVisit == null)
            {
                throw new ArgumentNullException("Member visit can't be null");
            }

            if (Touring == null)
            {
                throw new InvalidOperationException("Cannot register for museum visits without being assigned to a tour");
            }

            if (museumVisit.City == null)
            {
                throw new InvalidOperationException("Museum visit must be associated with a city");
            }

            if (!Tours.ContainCity(museumVisit.City))
            {

            }

            if (!RegisteredMuseumVisits.Contains(museumVisit))
            {
                throw new InvalidOperationException("Museum visit must be associated with a city");
            }

            return false;
        }

        public bool CancelMuseumVisit(MuseumVisit museumVisit)
        {
            if (museumVisit == null)
            {
                throw new ArgumentNullException(nameof(museumVisit), "Museum visit cannot be null");
            }

            if (RegisteredMuseumVisits.Remove(museumVisit))
            {
                museumVisit.UnregisterMember(this);
                return true;
            }

            return false;
        }
        public bool IsRegisteredForMuseumVisit(MuseumVisit museumVisit)
        {
            if (museumVisit == null)
            {
                throw new ArgumentNullException(nameof(museumVisit), "Museum visit cannot be null");
            }

            return RegisteredMuseumVisits.Contains(museumVisit);
        }

        public decimal GetTotalMuseumVisitCost()
        {
            return RegisteredMuseumVisits.Sum(mv => mv.Cost);
        }

        public decimal CalculateAdditionalCost()
        {
            if (RegisteredMuseumVisits.Count <= IncludedVisits)
            {
                return 0;
            }

            int additionalVisits = RegisteredMuseumVisits.Count - IncludedVisits;
            return RegisteredMuseumVisits.OrderByDescending(mv => mv.Cost)
                .Skip(IncludedVisits)
                .Sum(mv => mv.Cost);
        }
        public decimal CalculateAdditionalCost()
        {
            if (RegisteredMuseumVisits.Count <= IncludedVisits)
            {
                return 0;
            }

            int additionalVisits = RegisteredMuseumVisits.Count - IncludedVisits;
            return RegisteredMuseumVisits.OrderByDescending(mv => mv.Cost)
                .Skip(IncludedVisits)
                .Sum(mv => mv.Cost);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Member))
                return false;

            var other = obj as Member;
            return Id.Equals(other!.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} (Booking: {BookingNumber})";
        }


    }
}
