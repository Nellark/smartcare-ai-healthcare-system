using FluentValidation;
using SmartCare.Application.Dashboard.Queries;

namespace SmartCare.Application.Common.Validators;

public class GetDashboardStatsQueryValidator : AbstractValidator<GetDashboardStatsQuery>
{
    public GetDashboardStatsQueryValidator()
    {
    }
}
