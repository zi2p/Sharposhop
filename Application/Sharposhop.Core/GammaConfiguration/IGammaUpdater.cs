using Sharposhop.Core.Model;

namespace Sharposhop.Core.GammaConfiguration;

public interface IGammaUpdater
{
    Gamma InitialGamma { get; set; }
    ValueTask Update(Gamma value);
}