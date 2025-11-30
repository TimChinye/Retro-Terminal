using RetroTerminal.Logic.Interfaces;
using RetroTerminal.Logic.Models;

namespace RetroTerminal.Logic.Services;

public class SchoolRoster : ISchoolRoster
{
    private readonly Dictionary<string, Student> _students = new();

    public bool AddStudent(string name, int form)
    {
        if (string.IsNullOrWhiteSpace(name) || _students.ContainsKey(name)) return false;
        _students.Add(name, new Student(name, form));
        return true;
    }

    public bool GetStudent(string name, out Student? student)
    {
        if (_students.TryGetValue(name, out var s))
        {
            student = s;
            return true;
        }
        student = null;
        return false;
    }

    public IEnumerable<Student> GetStudentsByForm(int form)
    {
        // FIX: Added the .OrderBy() clause to sort students alphabetically by first name.
        return _students.Values.Where(s => s.Form == form).OrderBy(s => s.FirstName).ToList();
    }

    public IEnumerable<Student> GetAllStudentsSorted()
    {
        return _students.Values.OrderBy(s => s.Form).ThenBy(s => s.FirstName).ToList();
    }
}