namespace T3.Accounts.Api.Dto;

public static class AccountExtension
{
    public static AccountDto ToDto(this UserDoc doc)
    {
        return new AccountDto()
        {
            Id = doc.Id.ToString(),
            Password = doc.Password,
            Username = doc.Username
        };
    }
    
    
}