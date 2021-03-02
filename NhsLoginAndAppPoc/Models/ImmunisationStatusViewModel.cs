using System;
using System.Linq;

namespace NhsLoginAndAppPoc.Models
{
    public class ImmunisationStatusViewModel
    {
        private string _name;

        public bool? CovidEligible { get; set; }

        public string CovidVaccine { get; set; }

        public DateTime? FluVaccinationDate { get; set; }

        public DateTime? FirstCovidVaccinationDate { get; set; }

        public DateTime? SecondCovidVaccinationDate { get; set; }

        public string Name
        {
            get => string.IsNullOrEmpty(_name) ? _name : ToTitleCase(_name);
            set => _name = value;
        }

        public DateTime? DateOfBirth { get; set; }

        public string Postcode { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string ManageLoginUrl { get; set; }

        private static string ToTitleCase(string name) =>
            name.First().ToString().ToUpper() + name.ToLower().Substring(1);
    }
}