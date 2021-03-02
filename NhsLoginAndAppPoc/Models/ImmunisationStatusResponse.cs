using System.Collections.Generic;

namespace NhsLoginAndAppPoc.Models
{
    public class ImmunisationStatusResponse
    {
        public bool CovidEligible { get; set; }

        public string[] CovidVaccineTypes { get; set; }

        public Dictionary<string, string> Immunisations { get; set; }
    }
}