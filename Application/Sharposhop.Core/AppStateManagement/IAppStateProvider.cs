namespace Sharposhop.Core.AppStateManagement;

public interface IAppStateProvider
{
    bool IsCurrentlySaving { get; }
}