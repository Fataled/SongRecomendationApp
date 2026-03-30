namespace ProjectHellsParadise.BusinessLogic.ExtraStuff;
/// <summary>
/// A way to use display alearts in viewmodels
/// </summary>
/// <author>Brume Ako</author>
public class DialogService : IDialogService
{
    public async Task ShowAlertAsync(string title, string message, string cancel)
    {
        await Shell.Current.DisplayAlertAsync(title, message, cancel);
    }

    public async Task<bool> ShowConfirmAsync(string title, string message, string accept, string cancel)
    {
        return await Shell.Current.DisplayAlertAsync(title, message, accept, cancel);
    }
}