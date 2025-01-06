using Blazored.LocalStorage;
using Blazored.SessionStorage;

namespace Sabacc.Services;

public class BrowserStorage(ISessionStorageService sessionStorageService, ILocalStorageService localStorageService)
{
    private string PlayerId = nameof(PlayerId);

    public Task<Guid> GetPlayerId()
    {
        return GetSessionStorageId();
    }

    private async Task<Guid> GetSessionStorageId()
    {
        if (!await sessionStorageService.ContainKeyAsync(PlayerId))
        {
            await sessionStorageService.SetItemAsync(PlayerId, Guid.NewGuid());
        }

        return await sessionStorageService.GetItemAsync<Guid>(PlayerId);
    }

    private async Task<Guid> GetLocalStorageId()
    {
        if (!await localStorageService.ContainKeyAsync(PlayerId))
        {
            await localStorageService.SetItemAsync(PlayerId, Guid.NewGuid());
        }

        return await localStorageService.GetItemAsync<Guid>(PlayerId);
    }
}