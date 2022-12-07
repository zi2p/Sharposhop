namespace Sharposhop.Core.AppStateManagement;

public interface IAppStateManager
{
    void UpdateSavingState(bool isSaving);
}