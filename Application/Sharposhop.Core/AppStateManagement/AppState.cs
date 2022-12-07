namespace Sharposhop.Core.AppStateManagement;

public class AppState : IAppStateProvider, IAppStateManager
{
    public bool IsCurrentlySaving { get; private set; }

    public void UpdateSavingState(bool isSaving)
    {
        IsCurrentlySaving = isSaving;
    }
}