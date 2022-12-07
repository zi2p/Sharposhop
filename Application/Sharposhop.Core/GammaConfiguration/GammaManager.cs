using Sharposhop.Core.Model;
using Sharposhop.Core.PictureManagement;

namespace Sharposhop.Core.GammaConfiguration;

public class GammaManager : IGammaProvider, IGammaUpdater
{
    private readonly IPictureParametersUpdateObserver _updateObserver;

    public GammaManager(IPictureParametersUpdateObserver updateObserver)
    {
        _updateObserver = updateObserver;
        Gamma = Gamma.DefaultGamma;
    }

    public Gamma Gamma { get; private set; }

    public ValueTask Update(Gamma value)
    {
        if (Gamma.Equals(value))
            return ValueTask.CompletedTask;

        Gamma = value;
        return _updateObserver.OnPictureParametersUpdated();
    }
}