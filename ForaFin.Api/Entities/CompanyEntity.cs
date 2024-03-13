using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForaFin.Api.Entities;

[Table("Companies")]
public class CompanyEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] [MaxLength(255)] public string Name { get; set; }

    [Column(TypeName = "decimal(18,2)")] public decimal StandardFundableAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")] public decimal SpecialFundableAmount { get; set; }
}