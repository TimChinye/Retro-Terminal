namespace RetroTerminal.Logic.Models;

/// <summary>
/// Represents a student with a first name and a form number.
/// Using a record for immutability and value-based equality.
/// </summary>
public record Student(string FirstName, int Form);