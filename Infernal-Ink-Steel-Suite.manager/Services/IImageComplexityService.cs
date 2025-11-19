using System.IO;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Services
{
    public interface IImageComplexityService
    {
        ImageComplexityResult Analyze(Stream imageStream);
    }
}
