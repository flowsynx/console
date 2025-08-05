namespace Console.Preferences;

public interface IPreferenceManager
{
    Task<bool> IsDarkTheme();
    Task<bool> ToggleDarkModeAsync();
    Task<bool> ChangeDrawerStateAsync();
    Task<bool> IsDrawerOpenAsync();
}