using RetroTerminal.Logic.Models;

namespace RetroTerminal.Logic.Interfaces;

/// <summary>
/// Defines the contract for managing a school roster.
/// </summary>
public interface ISchoolRoster
{
    /// <summary>
    /// Adds a new student to a form. Does not add if a student with the same name already exists in that form.
    /// </summary>
    /// <param name="name">The name of the student.</param>
    /// <param name="form">The form number.</param>
    /// <returns>True if the student was added, false otherwise.</returns>
    bool AddStudent(string name, int form);
    bool GetStudent(string name, out Student? student);

    /// <summary>
    /// Gets a list of all students in a specific form, sorted alphabetically by name.
    /// </summary>
    /// <param name="form">The form number.</param>
    /// <returns>An enumerable collection of students in the specified form.</returns>
    IEnumerable<Student> GetStudentsByForm(int form);

    /// <summary>
    /// Gets a list of all students in the school, sorted by form number, then alphabetically by name.
    /// </summary>
    /// <returns>An enumerable collection of all students.</returns>
    IEnumerable<Student> GetAllStudentsSorted();
}