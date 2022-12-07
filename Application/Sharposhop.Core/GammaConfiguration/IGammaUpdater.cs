using Sharposhop.Core.Model;

namespace Sharposhop.Core.GammaConfiguration;

public interface IGammaUpdater
{
    ValueTask Update(Gamma value);
}