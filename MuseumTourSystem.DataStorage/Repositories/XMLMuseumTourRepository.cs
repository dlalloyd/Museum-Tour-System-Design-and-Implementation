using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MuseumTourSystem.Models.Models;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using MuseumTourSystem.BusinessLogic.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.ConstrainedExecution;

namespace MuseumTourSystem.DataStorage.Repositories
{
    public class XMLMuseumTourRepository : IMuseumTourRepository
    {
        private readonly string _dataFilePath;
        
        private readonly string _schemaFilePath;

        private readonly List<Tours> _tours;
        private readonly List<City> _cities;
        private readonly List<MuseumVisit> _museumVisits;
        private readonly List<Member> _members;

        public XMLMuseumTourRepository(string dataFilePath, string schemaFilePath)
        {
            _dataFilePath = dataFilePath ?? throw new ArgumentNullException(nameof(dataFilePath));
            _schemaFilePath = schemaFilePath ?? throw new ArgumentNullException(nameof(schemaFilePath));

            _tours = new List<Tours >();
            _cities = new List<City>();
            _museumVisits = new List<MuseumVisit>();
            _members = new List<Member>();

            // Create directories if they don't exist
            Directory.CreateDirectory(Path.GetDirectoryName(dataFilePath));
            Directory.CreateDirectory(Path.GetDirectoryName(schemaFilePath));

            // Create schema file if it doesn't exist
            if (!File.Exists(schemaFilePath))
            {
                CreateSchemaFile();
            }

            // Load data if file exists
            if (File.Exists(dataFilePath))
            {
                LoadData();
            }
        }

        #region Tour Operations

        public void AddTour(Tours tour)
        {
            if (tour == null)
            {
                throw new ArgumentNullException(nameof(tour));
            }

            if (_tours.Any(t => t.Id == tour.Id))
            {
                throw new ArgumentException($"Tour with ID '{tour.Id}' al-ready exists");
            }

            _tours.Add(tour);
        }

        public void RemoveTour(Tours tour)
        {
            if (tour == null)
            {
                throw new ArgumentNullException(nameof(tour));
            }

            _tours.Remove(tour);
        }

        public Tours GetTourById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID cannot be null or empty", nameof(id));
            }

            return _tours.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Tours> GetAllTours()
        {
            return _tours;
        }

        #endregion

        #region City Operations

        public void AddCity(City city)
        {
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city));
            }

            if (_cities.Any(c => c.Id == city.Id))
            {
                throw new ArgumentException($"City with ID '{city.Id}' al-ready exists");
            }

            _cities.Add(city);
        }

        public void RemoveCity(City city)
        {
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city));
            }

            _cities.Remove(city);
        }

        public City GetCityById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID cannot be null or empty", nameof(id));
            }

            return _cities.FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<City> GetAllCities()
        {
            return _cities;
        }

        #endregion


        #region Museum Visit Operations
        public void AddMuseumVisit(MuseumVisit museumVisit)
        {
            if (museumVisit == null)
            {
                throw new ArgumentNullException(nameof(museumVisit));
            }

            if (_museumVisits.Any(m => m.Id == museumVisit.Id))
            {
                throw new ArgumentException($"Museum visit with ID '{museumVisit.Id}' already exists");
            }

            _museumVisits.Add(museumVisit);
        }

        public void RemoveMuseumVisit(MuseumVisit museumVisit)
        {
            if (museumVisit == null)
            {
                throw new ArgumentNullException(nameof(museumVisit));
            }

            _museumVisits.Remove(museumVisit);
        }

        public MuseumVisit GetMuseumVisitById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID cannot be null or empty", nameof(id));
            }

            return _museumVisits.FirstOrDefault(m => m.Id == id);
        }

        public IEnumerable<MuseumVisit> GetAllMuseumVisits()
        {
            return _museumVisits;
        }

        #endregion


        #region Member Operations

        public void AddMember(Member member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (_members.Any(m => m.Id == member.Id))
            {
                throw new ArgumentException($"Member with ID '{member.Id}' already exists");
            }

            if (_members.Any(m => m.BookingNumber == member.BookingNumber))
            {
                throw new ArgumentException($"Member with booking number '{member.BookingNumber}' already exists");
            }

            _members.Add(member);
        }

        public void RemoveMember(Member member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            _members.Remove(member);
        }

        public Member GetMemberById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID cannot be null or empty", nameof(id));
            }

            return _members.FirstOrDefault(m => m.Id == id);
        }

        public Member GetMemberByBookingNumber(string bookingNumber)
        {
            if (string.IsNullOrEmpty(bookingNumber))
            {
                throw new ArgumentException("Booking number cannot be null or empty", nameof(bookingNumber));
            }

            return _members.FirstOrDefault(m => m.BookingNumber == bookingNumber);
        }

        public IEnumerable<Member> GetAllMembers()
        {
            return _members;
        }

        #endregion


        public void SaveChanges()
        {
            // Create an XML document
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("MuseumTourSystem",
                    new XElement("Tours", _tours.Select(t => SerializeTour(t))),
                    new XElement("Cities", _cities.Select(c => SerializeCity(c))),
                    new XElement("MuseumVisits", _museumVisits.Select(m => SerializeMuseumVisit(m))),
                    new XElement("Members", _members.Select(m => SerializeMember(m)))
                )
            );

            // Save the document
            doc.Save(_dataFilePath);

            // Validate the XML against the schema
            ValidateXmlAgainstSchema(_dataFilePath, _schemaFilePath);
        }


        public void LoadData()
        {
            if (!File.Exists(_dataFilePath))
            {
                return;
            }

            try
            {
                // Validate the XML against the schema
                ValidateXmlAgainstSchema(_dataFilePath, _schemaFilePath);

                // Load the XML document
                var doc = XDocument.Load(_dataFilePath);

                // Clear existing data
                _tours.Clear();
                _cities.Clear();
                _museumVisits.Clear();
                _members.Clear();

                // Load cities first
                if (doc.Root.Element("Cities") != null)
                {
                    foreach (var cityElement in doc.Root.Element("Cities").Elements("City"))
                    {
                        var city = DeserializeCity(cityElement);
                        _cities.Add(city);
                    }
                }

                // Load tours
                if (doc.Root.Element("Tours") != null)
                {
                    foreach (var tourElement in doc.Root.Element("Tours").Elements("Tour"))
                    {
                        var tour = DeserializeTour(tourElement);
                        _tours.Add(tour);
                    }
                }

                // Load museum visits
                if (doc.Root.Element("MuseumVisits") != null)
                {
                    foreach (var visitElement in doc.Root.Element("MuseumVisits").Elements("MuseumVisit"))
                    {
                        var visit = DeserializeMuseumVisit(visitElement);
                        _museumVisits.Add(visit);
                    }
                }

                // Load members
                if (doc.Root.Element("Members") != null)
                {
                    foreach (var memberElement in doc.Root.Element("Members").Elements("Member"))
                    {
                        var member = DeserializeMember(memberElement);
                        _members.Add(member);
                    }
                }

                // Establish relationships between objects
                EstablishRelationships();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to load data from XML file: {ex.Message}", ex);
            }
        }

        #region Serialization Methods

        /// <summary>
        /// Serializes a tour object to XML
        /// </summary>
        private XElement SerializeTour(Tours tour)
        {
            var element = new XElement("Tour",
                new XAttribute("Id", tour.Id),
                new XElement("Name", tour.Name),
                new XElement("Cities", tour.Cities.Select(c => new XElement("CityRef", c.Id))),
                new XElement("Members", tour.Members.Select(m => new XElement("MemberRef", m.Id)))
            );

            return element;
        }

        /// <summary>
        /// Serializes a city object to XML
        /// </summary>
        private XElement SerializeCity(City city)
        {
            var element = new XElement("City",
                new XAttribute("Id", city.Id),
                new XElement("Name", city.Name),
                new XElement("MuseumVisits", city.MuseumVisits.Select(m => new XElement("MuseumVisitRef", m.Id)))
            );

            return element;
        }

        /// <summary>
        /// Serializes a museum visit object to XML
        /// </summary>
        private XElement SerializeMuseumVisit(MuseumVisit visit)
        {
            var element = new XElement("MuseumVisit",
                new XAttribute("Id", visit.Id),
                new XElement("MuseumName", visit.MuseumName),
                new XElement("VisitDate", visit.VisitDate.ToString("yyyy-MM-dd")),
                new XElement("Cost", visit.Cost),
                new XElement("CityRef", visit.City?.Id ?? string.Empty),
                new XElement("RegisteredMembers", visit.RegisteredMembers.Select(m => new XElement("MemberRef", m.Id)))
            );

            return element;
        }

        /// <summary>
        /// Serializes a member object to XML
        /// </summary>
        private XElement SerializeMember(Member member)
        {
            var element = new XElement("Member",
                new XAttribute("Id", member.Id),
                new XElement("Name", member.Name),
                new XElement("BookingNumber", member.BookingNumber),
                new XElement("TourRef", member.Touring?.Id ?? string.Empty),
                new XElement("MuseumVisits", member.RegisteredMuseumVisits.Select(v => new XElement("MuseumVisitRef", v.Id)))
            );

            return element;
        }

        /// <summary>
        /// Deserialises a tour element from XML
        /// </summary>
        private Tours DeserializeTour(XElement element)
        {
            var id = element.Attribute("Id").Value;
            var name = element.Element("Name").Value;

            return new Tours(id, name);
        }

        /// <summary>
        /// Deserialises a city element from XML
        /// </summary>
        private City DeserializeCity(XElement element)
        {
            var id = element.Attribute("Id").Value;
            var name = element.Element("Name").Value;

            return new City(id, name);
        }

        /// <summary>
        /// Deserialises a museum visit element from XML
        /// </summary>
        private MuseumVisit DeserializeMuseumVisit(XElement element)
        {
            var id = element.Attribute("Id").Value;
            var museumName = element.Element("MuseumName").Value;
            var visitDate = DateTime.Parse(element.Element("VisitDate").Value);
            var cost = decimal.Parse(element.Element("Cost").Value);
            var cityId = element.Element("CityRef").Value;

            var city = GetCityById(cityId);
            if (city == null)
            {
                throw new ApplicationException($"City with ID '{cityId}' not found for museum visit '{id}'");
            }

            return new MuseumVisit(id, museumName, visitDate, cost, city);
        }

        /// <summary>
        /// Deserialises a member element from XML
        /// </summary>
        private Member DeserializeMember(XElement element)
        {
            var id = element.Attribute("Id").Value;
            var name = element.Element("Name").Value;
            var bookingNumber = element.Element("BookingNumber").Value;

            return new Member(id, name, bookingNumber);
        }

        /// <summary>
        /// Establishes relationships between objects after loading
        /// </summary>
        private void EstablishRelationships()
        {
            // Load XML document
            var doc = XDocument.Load(_dataFilePath);

            // Establish tour-city relationships
            foreach (var tourElement in doc.Root.Element("Tours").Elements("Tour"))
            {
                var tourId = tourElement.Attribute("Id").Value;
                var tour = GetTourById(tourId);

                foreach (var cityRefElement in tourElement.Element("Cities").Elements("CityRef"))
                {
                    var cityId = cityRefElement.Value;
                    var city = GetCityById(cityId);

                    if (city != null)
                    {
                        tour.AddCity(city);
                    }
                }
            }

            // Establish city-museum visit relationships
            foreach (var cityElement in doc.Root.Element("Cities").Elements("City"))
            {
                var cityId = cityElement.Attribute("Id").Value;
                var city = GetCityById(cityId);

                foreach (var visitRefElement in cityElement.Element("MuseumVisits").Elements("MuseumVisitRef"))
                {
                    var visitId = visitRefElement.Value;
                    var visit = GetMuseumVisitById(visitId);

                    if (visit != null)
                    {
                        city.AddMuseumVisit(visit);
                    }
                }
            }

            // Establish tour-member relationships
            foreach (var memberElement in doc.Root.Element("Members").Elements("Member"))
            {
                var memberId = memberElement.Attribute("Id").Value;
                var member = GetMemberById(memberId);

                var tourRefElement = memberElement.Element("TourRef");
                if (tourRefElement != null && !string.IsNullOrEmpty(tourRefElement.Value))
                {
                    var tourId = tourRefElement.Value;
                    var tour = GetTourById(tourId);

                    if (tour != null)
                    {
                        tour.AddMember(member);
                    }
                }
            }

            // Establish museum visit-member relationships
            foreach (var visitElement in doc.Root.Element("MuseumVisits").Elements("MuseumVisit"))
            {
                var visitId = visitElement.Attribute("Id").Value;
                var visit = GetMuseumVisitById(visitId);

                foreach (var memberRefElement in visitElement.Element("RegisteredMembers").Elements("MemberRef"))
                {
                    var memberId = memberRefElement.Value;
                    var member = GetMemberById(memberId);

                    if (member != null && member.Touring != null && member.Touring.ContainsCity(visit.City))
                    {
                        visit.AddMember(member);
                    }
                }
            }
        }

        #endregion

        #region XML Schema Validation

        /// <summary>
        /// Creates the XML schema file
        /// </summary>
        private void CreateSchemaFile()
        {
            var schema = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""MuseumTourSystem"">
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""Tours"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""Tour"" minOccurs=""0"" maxOccurs=""unbounded"">
                <xs:complexType>
                  <xs:sequence>
                    <xs:element name=""Name"" type=""xs:string"" />
                    <xs:element name=""Cities"">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element name=""CityRef"" type=""xs:string"" minOccurs=""0"" maxOccurs=""unbounded"" />
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                    <xs:element name=""Members"">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element name=""MemberRef"" type=""xs:string"" minOccurs=""0"" maxOccurs=""unbounded"" />
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                  </xs:sequence>
                  <xs:attribute name=""Id"" type=""xs:string"" use=""required"" />
                </xs:complexType>
              </xs:element>
            </xs:sequence>
          </xs:complexType>
        </xs:element>
        <xs:element name=""Cities"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""City"" minOccurs=""0"" maxOccurs=""unbounded"">
                <xs:complexType>
                  <xs:sequence>
                    <xs:element name=""Name"" type=""xs:string"" />
                    <xs:element name=""MuseumVisits"">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element name=""MuseumVisitRef"" type=""xs:string"" minOccurs=""0"" maxOccurs=""unbounded"" />
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                  </xs:sequence>
                  <xs:attribute name=""Id"" type=""xs:string"" use=""required"" />
                </xs:complexType>
              </xs:element>
            </xs:sequence>
          </xs:complexType>
        </xs:element>
        <xs:element name=""MuseumVisits"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""MuseumVisit"" minOccurs=""0"" maxOccurs=""unbounded"">
                <xs:complexType>
                  <xs:sequence>
                    <xs:element name=""MuseumName"" type=""xs:string"" />
                    <xs:element name=""VisitDate"" type=""xs:date"" />
                    <xs:element name=""Cost"" type=""xs:decimal"" />
                    <xs:element name=""CityRef"" type=""xs:string"" />
                    <xs:element name=""RegisteredMembers"">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element name=""MemberRef"" type=""xs:string"" minOccurs=""0"" maxOccurs=""unbounded"" />
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                  </xs:sequence>
                  <xs:attribute name=""Id"" type=""xs:string"" use=""required"" />
                </xs:complexType>
              </xs:element>
            </xs:sequence>
          </xs:complexType>
        </xs:element>
        <xs:element name=""Members"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""Member"" minOccurs=""0"" maxOccurs=""unbounded"">
                <xs:complexType>
                  <xs:sequence>
                    <xs:element name=""Name"" type=""xs:string"" />
                    <xs:element name=""BookingNumber"" type=""xs:string"" />
                    <xs:element name=""TourRef"" type=""xs:string"" />
                    <xs:element name=""MuseumVisits"">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element name=""MuseumVisitRef"" type=""xs:string"" minOccurs=""0"" maxOccurs=""unbounded"" />
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                  </xs:sequence>
                  <xs:attribute name=""Id"" type=""xs:string"" use=""required"" />
                </xs:complexType>
              </xs:element>
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";

            File.WriteAllText(_schemaFilePath, schema);
        }

        /// <summary>
        /// Validates an XML file against a schema
        /// </summary>
        /// <param name="xmlFilePath">Path to the XML file</param>
        /// <param name="schemaFilePath">Path to the schema file</param>
        private void ValidateXmlAgainstSchema(string xmlFilePath, string schemaFilePath)
        {
            var validationErrors = new List<string>();

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema
            };

            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += (sender, args) =>
            {
                validationErrors.Add($"{args.Severity}: {args.Message}");
            };

            settings.Schemas.Add("", schemaFilePath);

            using (var reader = XmlReader.Create(xmlFilePath, settings))
            {
                while (reader.Read()) { }
            }

            if (validationErrors.Any())
            {
                throw new ApplicationException($"XML validation failed: {string.Join("; ", validationErrors)}");
            }
        }

        #endregion

    }
}
