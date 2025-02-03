using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Identity.Common.Commands;

public class InviteManufacturerUserCommand : IRequest<ActionResult<Guid>>
{
    public InviteManufacturerUserCommand(Guid organisationId, string name, string email, string jobTitle, string telephoneNumber)
    {
        OrganisationId = organisationId;
        Name = name;
        Email = email;
        JobTitle = jobTitle;
        TelephoneNumber = telephoneNumber;
    }

    public string TelephoneNumber { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string JobTitle { get; private set; }
    public Guid OrganisationId { get; private set; }
}