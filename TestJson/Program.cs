using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public record UpdatePatientCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Address,
    string? Gender);

class Program
{
    static void Main()
    {
        string json = @"{""id"":""11111111-1111-1111-1111-111111111111"",""firstName"":""Sike"",""lastName"":""Kulati"",""email"":""sike@gmail.com"",""phoneNumber"":""555-0101"",""address"":""20 Anderson Street"",""dateOfBirth"":""1980-01-15T00:00:00"",""gender"":""female""}";
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var cmd = JsonSerializer.Deserialize<UpdatePatientCommand>(json, options);
        Console.WriteLine($"Gender: {cmd.Gender ?? "NULL"}");
    }
}
