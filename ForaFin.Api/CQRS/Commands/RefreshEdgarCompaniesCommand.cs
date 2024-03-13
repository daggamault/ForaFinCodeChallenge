using AutoMapper;
using ForaFin.Api.Dtos;
using ForaFin.Api.Repositories;
using MediatR;

namespace ForaFin.Api.CQRS.Commands;

public record RefreshEdgarCompaniesCommand
    : IRequest;

public class RefreshEdgarCompaniesCommandHandler(
    IEdgarRepository edgarRepository,
    IMapper mapper
)
    : IRequestHandler<RefreshEdgarCompaniesCommand>
{
    public async Task Handle(
        RefreshEdgarCompaniesCommand request,
        CancellationToken cancellationToken
    )
    {
        var edgarCompanies = await edgarRepository
            .FindEdgarCompanies(cancellationToken);
        var companies = edgarCompanies
            .Select(Update)
            .ToList();
        await edgarRepository
            .SaveCompanies(companies, cancellationToken);
    }

    private CompanyDto Update(EdgarCompanyDto edgarCompany)
    {
        var company = mapper
            .Map<CompanyDto>(edgarCompany);
        company.StandardFundableAmount = CalculateStandardFundableAmount(edgarCompany);
        company.SpecialFundableAmount = CalculateSpecialFundableAmount(edgarCompany, company.StandardFundableAmount);
        return company;
    }

    private static decimal CalculateStandardFundableAmount(EdgarCompanyDto edgarCompany)
    {
        if (edgarCompany.Facts?.UsGaap?.NetIncomeLoss?.Units?.Usd == null) return 0;
        var relevantYears = new[] { "CY2018", "CY2019", "CY2020", "CY2021", "CY2022" };
        var incomeData = edgarCompany.Facts.UsGaap.NetIncomeLoss.Units.Usd
            .Where(x => x.Form != null && x.Form.Equals("10-k", StringComparison.OrdinalIgnoreCase) &&
                        relevantYears.Any(year =>
                            x.Frame != null && x.Frame.Equals(year, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        if (incomeData.Count != relevantYears.Length || incomeData.Any(x =>
                x.Val <= 0 && (x.Frame?.Equals("CY2021", StringComparison.OrdinalIgnoreCase) == true ||
                               x.Frame?.Equals("CY2022", StringComparison.OrdinalIgnoreCase) == true)))
            return 0;
        var maxIncome = incomeData.Max(x => x.Val) ?? 0;
        var percentage = maxIncome >= 10000000000m ? 0.1233m : 0.2151m;
        return maxIncome * percentage;
    }

    private static decimal CalculateSpecialFundableAmount(EdgarCompanyDto edgarCompany, decimal standardFundableAmount)
    {
        if (edgarCompany.Facts?.UsGaap?.NetIncomeLoss?.Units?.Usd == null || edgarCompany.EntityName == null)
            return 0;
        var specialFundableAmount = standardFundableAmount;
        var vowels = new[] { 'a', 'e', 'i', 'o', 'u' };
        if (vowels.Any(v => edgarCompany.EntityName.StartsWith(v.ToString(), StringComparison.OrdinalIgnoreCase)))
            specialFundableAmount += standardFundableAmount * 0.15m;
        var income2022 = edgarCompany.Facts.UsGaap.NetIncomeLoss.Units.Usd
            .FirstOrDefault(x => x.Frame != null && x.Frame.Equals("CY2022", StringComparison.OrdinalIgnoreCase))
            ?.Val ?? 0;
        var income2021 = edgarCompany.Facts.UsGaap.NetIncomeLoss.Units.Usd
            .FirstOrDefault(x => x.Frame != null && x.Frame.Equals("CY2021", StringComparison.OrdinalIgnoreCase))
            ?.Val ?? 0;
        if (income2022 < income2021)
            specialFundableAmount -= standardFundableAmount * 0.25m;
        return specialFundableAmount;
    }
}