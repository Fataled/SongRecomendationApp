namespace ProjectHellsParadise.BusinessLogic.ExtraStuff;
/// <summary>
/// A way to use display alearts in viewmodels
/// </summary>
/// <author>Brume Ako</author>
public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string cancel);
    Task<bool> ShowConfirmAsync(string title, string message, string accept, string cancel);
}