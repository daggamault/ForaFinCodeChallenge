﻿using System.Net.Http.Headers;
using AutoMapper;
using ForaFin.Api.Dtos;
using ForaFin.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ForaFin.Api.Repositories;

public interface IEdgarRepository
{
    /// <summary>
    ///     Returns edgar companies (from SEC API)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<EdgarCompanyDto>> FindEdgarCompanies(
        CancellationToken cancellationToken
    );

    /// <summary>
    ///     Returns companies (staged/precomputed)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<CompanyDto>> FindCompanies(
        string? name,
        CancellationToken cancellationToken
    );

    /// <summary>
    ///     Replaces all companies in DB
    /// </summary>
    /// <param name="companies"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SaveCompanies(
        IEnumerable<CompanyDto> companies,
        CancellationToken cancellationToken
    );
}

public class EdgarRepository(AppDbContext dbContext, IMapper mapper)
    : IEdgarRepository
{
    //hacked in here to make api requests easier on whomever is reviewing this demo/app
    private readonly int[] _ciks =
    [
        18926, 892553, 1510524, 1858912, 1828248, 1819493, 60086, 1853630, 1761312, 1851182,
        1034665, 927628, 1125259, 1547660, 1393311, 1757143, 1958217, 312070, 310522, 1861841,
        1037868, 1696355, 1166834, 915912, 1085277, 831259, 882291, 1521036, 1824502, 1015647,
        884624, 1501103, 1397183, 1552797, 1894630, 823277, 21175, 1439124, 52827, 1730773,
        1867287, 1685428, 1007587, 92103, 1641751, 6845, 1231457, 947263, 895421, 1988979,
        1848898, 844790, 1541309, 1858007, 1729944, 726958, 1691221, 730272, 1308106, 884144,
        1108134, 1849058, 1435617, 1857518, 64803, 1912498, 1447380, 1232384, 1141788, 1549922,
        914475, 1498382, 1400897, 314808, 1323885, 1526520, 1550695, 1634293, 1756708,
        1540159, 1076691, 1980088, 1532346, 923796, 1849635, 1872292, 1227857, 1046311, 1710350,
        1476150, 1844642, 1967078, 14272, 933267, 1157557, 1560293, 217410, 1798562, 1038074, 1843370
    ];

    public async Task<IEnumerable<EdgarCompanyDto>> FindEdgarCompanies(
        CancellationToken cancellationToken = default
    )
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent
            .ParseAdd("PostmanRuntime/7.34.0");
        client.DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue("*/*"));
        var companies = new List<EdgarCompanyDto>();
        foreach (var i in _ciks)
        {
            var cik = i.ToString().PadLeft(10, '0');
            var response = await client
                .GetAsync($"https://data.sec.gov/api/xbrl/companyfacts/CIK{cik}.json", cancellationToken);
            if (!response.IsSuccessStatusCode) continue; //should fail here?
            var body = await response.Content
                .ReadAsStringAsync(cancellationToken);
            var edgarCompanyInfo = JsonConvert
                .DeserializeObject<EdgarCompanyDto>(body);
            if (edgarCompanyInfo != null)
                companies.Add(edgarCompanyInfo);
        }
        return companies;
    }

    public async Task<IEnumerable<CompanyDto>> FindCompanies(
        string? name,
        CancellationToken cancellationToken
    )
    {
        name = (name ?? string.Empty)
            .ToLower()
            .Trim();
        var companyEntities = await dbContext.CompanyInfos
            .Where(x => x.Name.ToLower().StartsWith(name))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
        var companies = mapper
            .Map<IEnumerable<CompanyDto>>(companyEntities);
        return companies;
    }

    public async Task SaveCompanies(
        IEnumerable<CompanyDto> companies,
        CancellationToken cancellationToken
    )
    {
        var existingCompanyEntities = await dbContext.CompanyInfos
            .ToListAsync(cancellationToken); //could be improved w/raw sql command
        dbContext.CompanyInfos
            .RemoveRange(existingCompanyEntities);
        var companyEntities = mapper
            .Map<IEnumerable<CompanyEntity>>(companies);
        await dbContext.CompanyInfos
            .AddRangeAsync(companyEntities, cancellationToken);
        await dbContext
            .SaveChangesAsync(cancellationToken);
    }
}