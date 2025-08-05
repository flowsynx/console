using Blazored.LocalStorage;
using Console.Settings;

namespace Console.Preferences;

public class PreferenceManager : IPreferenceManager
{
    private readonly ILocalStorageService _localStorageService;

    public PreferenceManager(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task<bool> ToggleDarkModeAsync()
    {
        var preference = await GetPreference();
        preference.IsDarkMode = !preference.IsDarkMode;
        await SetPreference(preference);
        return !preference.IsDarkMode;
    }

    public async Task<bool> IsDarkTheme()
    {
        var preference = await GetPreference();
        return preference.IsDarkMode == true;
    }

    public async Task<bool> IsDrawerOpenAsync()
    {
        var preference = await GetPreference();
        return preference.IsDrawerOpen;
    }

    public async Task<bool> ChangeDrawerStateAsync()
    {
        var preference = await GetPreference();
        preference.IsDrawerOpen = !preference.IsDrawerOpen;
        await SetPreference(preference);
        return preference.IsDrawerOpen;
    }

    private async Task<ClientPreference> GetPreference()
    {
        return await _localStorageService.GetItemAsync<ClientPreference>(StorageConstants.Local.Preference) ?? new ClientPreference();
    }

    private async Task SetPreference(ClientPreference preference)
    {
        await _localStorageService.SetItemAsync(StorageConstants.Local.Preference, preference);
    }
}