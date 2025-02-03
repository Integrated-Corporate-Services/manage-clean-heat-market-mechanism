using FluentValidation;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetSchemeYearQueryValidator : AbstractValidator<GetSchemeYearQuery>
    {
        public GetSchemeYearQueryValidator()
        {
            RuleFor(c => c.SchemeYearId).NotEmpty();
        }
    }
}
