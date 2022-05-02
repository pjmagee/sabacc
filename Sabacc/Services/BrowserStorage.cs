using Blazored.LocalStorage;
using Blazored.SessionStorage;

namespace Sabacc.Services;

public class BrowserStorage
{
    private readonly ISessionStorageService _sessionStorageService;
    private readonly ILocalStorageService _localStorageService;

    private string PlayerId = nameof(PlayerId);

    public BrowserStorage(ISessionStorageService sessionStorageService, ILocalStorageService localStorageService)
    {
        _sessionStorageService = sessionStorageService;
        _localStorageService = localStorageService;
    }

    public Task<Guid> GetPlayerId()
    {
        return GetSessionStorageId();
    }

    private async Task<Guid> GetSessionStorageId()
    {
        if (!await _sessionStorageService.ContainKeyAsync(PlayerId))
        {
            await _sessionStorageService.SetItemAsync(PlayerId, Guid.NewGuid());
        }

        return await _sessionStorageService.GetItemAsync<Guid>(PlayerId);
    }

    private async Task<Guid> GetLocalStorageId()
    {
        if (!await _localStorageService.ContainKeyAsync(PlayerId))
        {
            await _localStorageService.SetItemAsync(PlayerId, Guid.NewGuid());
        }

        return await _localStorageService.GetItemAsync<Guid>(PlayerId);
    }
}