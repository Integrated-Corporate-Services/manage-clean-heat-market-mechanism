using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;

namespace Desnz.Chmm.Identity.Api.Infrastructure.Setup
{
    public static class Users
    {
        public static List<(string, string)> Admins = new ()
        {
            ("Andrei Negrea", "andrei.negrea@triad.co.uk"),
            ("Nathan Gradidge", "nathan.gradidge@triad.co.uk"),
            ("Richard Priddy", "richard.priddy@triad.co.uk"),
            ("Radoslav Semerdjiev", "radoslav.semerdjiev@triad.co.uk")
        };

        public static List<List<CreateManufacturerUserDto>> Manufacturers = new ()
        {
            new()
            {
                new() 
                {
                    Name = "Andrei Negrea M",
                    Email = "andrei.negrea.manufacturer@triad.co.uk",
                    JobTitle = "Software Developer",
                    TelephoneNumber = "01908278450",
                    IsResponsibleOfficer = true,
                },
            },
            new()
            {
                new()
                {
                    Name = "Nathan Gradidge M",
                    Email = "nathan.gradidge.manufacturer@triad.co.uk",
                    JobTitle = "Software Developer",
                    TelephoneNumber = "01908278450",
                    IsResponsibleOfficer = true,
                },
            },
            new()
            {
                new()
                {
                    Name = "Richard Priddy M",
                    Email = "richard.priddy.manufacturer@triad.co.uk",
                    JobTitle = "Software Developer",
                    TelephoneNumber = "01908278450",
                    IsResponsibleOfficer = true,
                },
            },
            new()
            {
                new()
                {
                    Name = "Radoslav Semerdjiev M",
                    Email = "radoslav.semerdjiev.manufacturer@triad.co.uk",
                    JobTitle = "Software Developer",
                    TelephoneNumber = "01908278450",
                    IsResponsibleOfficer = true,
                },
            }
        };
    }
}
