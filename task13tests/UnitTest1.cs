using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

public class UnitTest1
{
    
    private readonly Student _testStudent = new Student
    {
        FirstName = "Ivan",
        LastName = "Chvalyuk",
        BirthDate = new DateTime(2006, 3, 11),
        Grades = new List<Subject>
        {
            new Subject { Name = "Algebra", Grade = 4 }
        }
    };

    private readonly string _expectJson = File.ReadAllText(@"../../../expectJson.txt").Replace("\r\n", "\n");

    [Fact]
    public void Serializer_ShouldReturnCorrectlySerializedStudent()
    {
        var serializer = new StudentSerializer();
        var actualJson = serializer.Serialize(_testStudent).Replace("\r\n", "\n");

        Assert.Equal(_expectJson, actualJson);
    }

    [Fact]
    public void Deserializer_ShouldReturnCorrectlyDeserializedStudent()
    {
        var serializer = new StudentSerializer();
        var deserialized = serializer.Deserialize(_expectJson);

        Assert.Equivalent(_testStudent, deserialized);
    }
}
