namespace YazOkulu.Application.DTOs.Auth;

public class CheckPhoneResponseDto
{
    public bool IsRegistered { get; set; }
    public string Message { get; set; } = string.Empty;
}
