using Sharposhop.Core.Model;

namespace Sharposhop.Core.GammaConfiguration;

public interface IGammaProvider
{
    Gamma InitialGamma { get; }
    Gamma GammaValue { get; }
}