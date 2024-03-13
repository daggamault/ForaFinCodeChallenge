namespace ForaFin.Api.Dtos;

public class CompanyDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal StandardFundableAmount { get; set; } = 0;
    public decimal SpecialFundableAmount { get; set; } = 0;
}