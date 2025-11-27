using System.IO;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.UI.Services
{
    public interface IImageComplexityService
    {
        ImageComplexityResult Analyze(Stream imageStream);
    }
}
