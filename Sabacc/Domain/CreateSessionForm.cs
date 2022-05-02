using System.ComponentModel.DataAnnotations;

namespace Sabacc.Domain;

public class CreateSessionForm
{
    [Required]
    [Range(minimum: 1, maximum: 8)]
    public int Slots { get; set; }

    public SabaccVariantType SabaccVariant { get; set; }
}