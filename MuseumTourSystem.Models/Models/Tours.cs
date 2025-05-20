using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumTourSystem.Models.Models
{
    public class Tours
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List <City> Cities{ get;}

        public List <Member> Members{ get;}

        public Tours(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("Tour ID cannot be empty ", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Tour Name cannot be empty ", nameof(name));
            }

            Id = id;
            Name = name;
            Cities = new List<City>();
            Members = new List<Member>();
        }


        public bool AddCity(City city)
        {
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city), "City can't be null");
            }

            if (!Cities.Contains(city))
            {
                Cities.Add(city);
                return true;
            }
            return false;
        }

        public bool RemoveCity(City city)
        {
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city), "City can't be null");
            }

            return Cities.Remove(city);
        }

        public bool AddMember(Member member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member), "Member can't be null");
            }

            if (!Members.Contains(member))
            {
                Members.Add(member);
               member.Touring = this;
                return true;
            }
            return false;
        }

        public bool RemoveMember(Member member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member), "Member can't be null");
            }

            if (!Members.Remove(member))
            {
                member.Touring = null;
                return true;
            }
            return false;
        }

        public bool ContainsCity(City city)
        {
            return Cities.Contains(city);
        }

        public bool ContainsCityByName(string cityName)
        {
            return Cities.Exists(c => c.Name.Equals(cityName, StringComparison.OrdinalIgnoreCase));
        }

        public bool ContainsMember(Member member)
        {
            return Members.Contains(member);
        }

        public bool ContainsMemberByBookingNUmber(string bookingNumber)
        {
            return Members.Exists(m => m.BookingNumber.Equals(bookingNumber, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object? obj)
        {
            if(obj == null || !(obj is Tours))
            {
                return false;
            }
            var other = obj as Tours;
            return Id.Equals(other!.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} (ID:{Id}";
        }
    }
}
