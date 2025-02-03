using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CommonValidation
{
    public class CustomValidator<T> {
        public required Func<T, bool> ValidationFunction { get; init; }
        public required Func<T, ActionResult> FailureAction { get; init; }
    }
}
