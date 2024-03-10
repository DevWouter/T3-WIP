namespace T3.Accounts.Api.Dto;

public record CreateAccountDto(string Username, string Password);
public record LoginDto(string Username, string Password);
