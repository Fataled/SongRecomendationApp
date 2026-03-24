namespace ProjectHellsParadise.BusinessLogic.Models;

public record UserLoginModel(string Email, string Password, string? TotpCode);