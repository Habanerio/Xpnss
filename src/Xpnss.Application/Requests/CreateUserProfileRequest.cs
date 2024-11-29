namespace Habanerio.Xpnss.Application.Requests;

public record CreateUserProfileRequest(string Email, string FirstName, string LastName = "", string ExtUserId = "");