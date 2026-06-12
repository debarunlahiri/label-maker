using LabelForge.Core.Models.Templates;

namespace LabelForge.Core.Interfaces;

public interface ILabelPrinterGenerator
{
    string GenerateZpl(LabelTemplate template);
    string GenerateEpl(LabelTemplate template);
    string GenerateCpcl(LabelTemplate template);
}