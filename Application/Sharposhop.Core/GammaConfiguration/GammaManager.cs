using Sharposhop.Core.Model;
using Sharposhop.Core.PictureManagement;

namespace Sharposhop.Core.GammaConfiguration;

public class GammaManager : IGammaProvider, IGammaUpdater
{
    private readonly IPictureParametersUpdateObserver _updateObserver;

    public GammaManager(IPictureParametersUpdateObserver updateObserver, Gamma initialGamma)
    {
        _updateObserver = updateObserver;
        GammaValue = Gamma.DefaultGamma;
        InitialGamma = initialGamma;
    }

    public Gamma GammaValue { get; private set; }

    public Gamma InitialGamma { get; set; }

    public ValueTask Update(Gamma value)
    {
        if (GammaValue.Equals(value))
            return ValueTask.CompletedTask;

        GammaValue = value;
        return _updateObserver.OnPictureParametersUpdated();
    }
}