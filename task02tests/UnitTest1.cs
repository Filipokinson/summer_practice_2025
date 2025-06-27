using Xunit;
public class StudentServiceTests
{
    private List<Student> _testStudents;
    private StudentService _service;

    public StudentServiceTests()
    {
        _testStudents = new List<Student>
        {
            new() { Name = "Иван", Faculty = "ФИТ", Grades = new List<int> { 5, 4, 5 } },
            new() { Name = "Анна", Faculty = "ФИТ", Grades = new List<int> { 3, 4, 3 } },
            new() { Name = "Петр", Faculty = "Экономика", Grades = new List<int> { 5, 5, 5 } }
        };
        _service = new StudentService(_testStudents);
    }

    [Fact]
    public void GetStudentsByFaculty_ReturnsCorrectStudents()
    {
        var result = _service.GetStudentsByFaculty("ФИТ").ToList();
        Assert.Equal(2, result.Count);
        Assert.True(result.All(s => s.Faculty == "ФИТ"));
    }

    [Fact]
    public void GetFacultyWithHighestAverageGrade_ReturnsCorrectFaculty()
    {
        var result = _service.GetFacultyWithHighestAverageGrade();
        Assert.Equal("Экономика", result);
    }

    [Fact]
    public void GetStudentWithMinAverageGrade_ReturnsCorrectStudents()
    {
        var result = _service.GetStudentWithMinAverageGrade(4).ToList();
        Assert.True(result.All(s => s.Grades.Average() >= 4));
    }

    [Fact]
    public void GetStudentsOrderedByName_ReturnsCorrectOrder()
    {
        var result = _service.GetStudentsOrderedByName().ToList();
        var expected = _testStudents.OrderBy(s => s.Name).ToList();
        Assert.Equal(expected.Select(s => s.Name), result.Select(s => s.Name));
    }

    [Fact]
    public void GroupsStudentsByFaculty_ReturnsCorrectGroups()
    {
        var result = _service.GroupsStudentsByFaculty().ToList();
        var expected = _testStudents.ToLookup(s => s.Faculty);
        Assert.Equal(expected.Count, result.Count);
        foreach (var group in expected)
        {
            var actualGroup = result.FirstOrDefault(g => g.Key == group.Key);
            Assert.NotNull(actualGroup);
            Assert.Equal(group.Count(), actualGroup.Count());
        }
    }
}
