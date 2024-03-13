using FluentValidation;
using ForaFin.Api.Dtos;
using ForaFin.Api.Repositories;
using MediatR;

namespace ForaFin.Api.CQRS.Queries;

public record FindCompaniesQuery(string? Name = null)
    : IRequest<IEnumerable<CompanyDto>>;

public class FindCompaniesQueryValidator
    : AbstractValidator<FindCompaniesQuery>
{
    public FindCompaniesQueryValidator()
    {
        RuleFor(x => x.Name).MaximumLength(255);
    }
}

public class FindCompaniesQueryHandler(
    FindCompaniesQueryValidator validator,
    IEdgarRepository edgarRepository
)
    : IRequestHandler<FindCompaniesQuery, IEnumerable<CompanyDto>>
{
    public async Task<IEnumerable<CompanyDto>> Handle(
        FindCompaniesQuery request,
        CancellationToken cancellationToken
    )
    {
        var validation = await validator
            .ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        var companies = await edgarRepository
            .FindCompanies(request.Name, cancellationToken);
        return companies;
    }
}