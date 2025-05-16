using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumTourSystem.Models.Models
{
    public class City
    {
        public string Id { get; }

        public string Name { get; set;}

        public List<MuseumVisit> MuseumVisits { get;}

        public City(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("City ID can't be empty");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("City Name ca'nt be empty");
            }
            Id = id;
            Name = name;
            MuseumVisits = new List<MuseumVisit>();
        }

        public bool AddMuseumVisit(MuseumVisit museumVisit)
        {
            if(museumVisit == null)
            {
                throw new ArgumentNullException(nameof(museumVisit), "Museum visit can't be null");
            }

            if (!MuseumVisits.Contains(museumVisit))
            {
                MuseumVisitsAdd(museumVisit);
                museumVisit.City = this;
                return true;
            }

            return false;
        }

        public bool RemoveMuseumVisit(MuseumVisit museumVisit)
        {
            if (museumVisit == null)
            {
                throw new ArgumentNullException(nameof(museumVisit), "Museum visit can't be null");
            }

            if (MuseumVisits.Remove(museumVisit))
            {
               if(museumVisit.City == this)
                {
                    museumVisit.City = null;
                }
                return true;
            }

            return false;
        }

        public bool HasMuseumVisit(string museumName)
        {
            return MuseumVisits.Any(mv => mv.MuseumName.Equals(museumName, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object? obj)
        {
            if(obj == null || !(obj is City))
            {
                return false;
            }
            var other = obj as City;
            return Id.Equals(other!.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} (ID: {Id}";
        }
    }
}
