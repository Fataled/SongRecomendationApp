namespace ProjectHellsParadise.BusinessLogic.ExtraStuff;

public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string cancel);
    Task<bool> ShowConfirmAsync(string title, string message, string accept, string cancel);
}