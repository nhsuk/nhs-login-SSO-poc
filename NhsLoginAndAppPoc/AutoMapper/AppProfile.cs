using AutoMapper;
using Microsoft.Extensions.Configuration;
using NhsLoginAndAppPoc.Models;

namespace NhsLoginAndAppPoc.AutoMapper
{
    public class AppProfile : Profile
    {
        public AppProfile(IConfiguration configuration)
        {
            IConfigurationSection covidVaccineTypes = configuration.GetSection("CovidVaccineTypes");

            CreateMap<UserInfo, ImmunisationStatusViewModel>();

            CreateMap<ImmunisationStatusResponse, ImmunisationStatusViewModel>()
                .ForMember(
                    dest => dest.CovidVaccine,
                    opt => opt.MapFrom(
                        src => covidVaccineTypes[src.Immunisations["mostRecentCovidVaccineDose1AdministeredType"]]))
                .ForMember(
                    dest => dest.FluVaccinationDate,
                    opt => opt.MapFrom(
                        src => src.Immunisations["mostRecentFluVaccineAdministeredDate"]))
                .ForMember(
                    dest => dest.FirstCovidVaccinationDate,
                    opt => opt.MapFrom(
                        src => src.Immunisations["mostRecentCovidVaccineDose1AdministeredDate"]))
                .ForMember(
                    dest => dest.SecondCovidVaccinationDate,
                    opt => opt.MapFrom(
                        src => src.Immunisations["mostRecentCovidVaccineDose2AdministeredDate"]));
        }
    }
}