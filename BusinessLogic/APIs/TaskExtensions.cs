namespace ProjectHellsParadise.BusinessLogic.APIs;
/// <summary>
/// A way to use async functionality in non-async methods
/// </summary>
/// <author>Brume Ako</author>
public static class TaskExtensions
{
    public static async void FireAndForget(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}