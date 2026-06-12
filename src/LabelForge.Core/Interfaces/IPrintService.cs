using LabelForge.Core.Models.Templates;
using LabelForge.Core.Models.Printing;

namespace LabelForge.Core.Interfaces;

public interface IPrintService
{
    Task<string> GeneratePrintHtmlAsync(LabelTemplate template);
    Task<string> GenerateZplAsync(LabelTemplate template);
    Task<string> GenerateEplAsync(LabelTemplate template);
    Task<string> GenerateCpclAsync(LabelTemplate template);
    Task<byte[]> GeneratePdfAsync(LabelTemplate template);
    Task PrintAsync(LabelTemplate template, PrintOptions options);
    Task PrintPreviewAsync(LabelTemplate template);
}