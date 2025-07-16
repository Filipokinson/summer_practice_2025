using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Subject
{
    public required string Name { get; set; }
    public int Grade { get; set; }
}

public class Student
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime BirthDate { get; set; }
    public required List<Subject> Grades { get; set; }
}

public class StudentSerializer
{
    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "dd.MM.yyyy";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string dateString = reader.GetString()!;
            return DateTime.ParseExact(dateString, Format, null);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
        
    }

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new CustomDateTimeConverter() }
    };

    public string Serialize(Student student)
    {
        return JsonSerializer.Serialize(student, _options);
    }

    public Student Deserialize(string json)
    {
        return JsonSerializer.Deserialize<Student>(json, _options)!;
    }

    public void SerializeToFile(Student student, string filePath)
    {
        File.WriteAllText(filePath, Serialize(student));
    }

    public Student DeserializeFromFile(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return Deserialize(json);
    }
}
