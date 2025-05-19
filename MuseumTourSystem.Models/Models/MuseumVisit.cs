using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumTourSystem.Models.Models
{
    public class MuseumVisit
    {
        public string Id { get; }
    
        public string MuseumName { get; set; }

        public DateTime VisitDate { get; set; }

        public decimal Cost { get; set; }

        public City? City { get; set; }

        public List<Member> RegisteredMembers { get; }

        public MuseumVisit(string id, string museumName, DateTime visitDate, decimal cost)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Museum visit ID cannot be emp-ty", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(museumName))
            {
                throw new ArgumentException("Museum name cannot be empty", nameof(museumName));
            }

            if (cost < 0)
            {
                throw new ArgumentException("Cost cannot be negative", nameof(cost));
            }

            Id = id;
            MuseumName = museumName;
            VisitDate = visitDate;
            Cost = cost;
            RegisteredMembers = new List<Member>();
        }

        public MuseumVisit(string id, string museumName, DateTime visitDate, decimal cost, City city)
            : this(id, museumName, visitDate, cost)
        {
            City = city ?? throw new ArgumentNullException(nameof(city));
            city.AddMuseumVisit(this);
        }

        public bool RegisterMember(Member member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member), "Member cannot be null");
            }

            // Business rule: Members can only visit museums in cities that are part of their tour
            if (City == null)
            {
                throw new InvalidOperationException("Cannot register for a museum visit not assigned to any city");
            }

            if (member.Touring == null)
            {
                throw new InvalidOperationException("Member must be as-signed to a tour");
            }

            if (!member.Touring.ContainsCity(City))
            {
                return false; // Member's tour does not include this city
            }

            if (!RegisteredMembers.Contains(member))
            {
                RegisteredMembers.Add(member);
                if (!member.IsRegisteredForMuseumVisit(this))
                {
                    member.RegisteredMuseumVisits.Add(this);
                }
                return true;
            }

            return false;
        }

        public bool UnregisterMember(Member member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member), "Member cannot be null");
            }

            if (RegisteredMembers.Remove(member))
            {
                if (member.IsRegisteredForMuseumVisit(this))
                {
                    member.RegisteredMuseumVisits.Remove(this);
                }
                return true;
            }

            return false;
        }

        public bool AddMember(Member member)
        {
            return RegisterMember(member);
        }

        public bool RemoveMember(Member member)
        {
            return UnregisterMember(member);
        }

        public decimal GetTotalRevenue()
        {
            return Cost * RegisteredMembers.Count;
        }

        public bool IsRegistered(Member member)
        {
            return RegisteredMembers.Contains(member);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is MuseumVisit))
                return false;

            var other = obj as MuseumVisit;
            return Id.Equals(other!.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{MuseumName} on {VisitDate.ToShortDateString()} (€{Cost})";
        }


    }
}
