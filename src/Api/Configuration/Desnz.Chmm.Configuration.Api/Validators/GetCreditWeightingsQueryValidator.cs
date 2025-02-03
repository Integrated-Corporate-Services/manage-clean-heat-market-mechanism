using FluentValidation;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetCreditWeightingsQueryValidator : AbstractValidator<GetCreditWeightingsQuery>
    {
        public GetCreditWeightingsQueryValidator()
        {
            RuleFor(c => c.SchemeYearId).NotEmpty();
        }
    }
}
