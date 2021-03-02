using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using NhsLoginAndAppPoc.AutoMapper;
using NhsLoginAndAppPoc.Models;
using NUnit.Framework;

namespace NhsLoginAndAppPoc.Tests.AutoMapper
{
    [TestFixture]
    public class AppProfileTests
    {
        [Test]
        public void Map_ImmunisationStatusResponseToViewModel_ReturnsExpected()
        {
            var mockConfigurationSection = new Mock<IConfigurationSection>();
            mockConfigurationSection.Setup(cfg => cfg["39326911000001101"])
                .Returns("Moderna");

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(cfg => cfg.GetSection("CovidVaccineTypes"))
                .Returns(mockConfigurationSection.Object);

            var mapper = new Mapper(
                new MapperConfiguration(
                    cfg => cfg.AddProfile(new AppProfile(mockConfiguration.Object))));

            var immunisationStatusResponse = new ImmunisationStatusResponse
            {
                CovidEligible = true,
                CovidVaccineTypes = new[]
                {
                    "39114911000001105",
                    "39115611000001103",
                    "39326911000001101"
                },
                Immunisations = new Dictionary<string, string>
                {
                    {"mostRecentFluVaccineAdministeredDate", "2020-11-15"},
                    {"mostRecentCovidVaccineDose1AdministeredDate", "2021-01-08"},
                    {"mostRecentCovidVaccineDose1AdministeredType", "39326911000001101"},
                    {"mostRecentCovidVaccineDose2AdministeredDate", "2021-01-31"}
                }
            };

            var immunisationStatusViewModel = mapper.Map<ImmunisationStatusViewModel>(immunisationStatusResponse);

            Assert.Pass();
        }
    }
}
