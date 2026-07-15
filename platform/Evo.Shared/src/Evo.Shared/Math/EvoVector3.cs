using System.Text.Json;
using System.Text.Json.Serialization;

namespace Evo.Shared.Math;

[JsonConverter(typeof(EvoVector3Converter))]
public readonly struct EvoVector3 : IEquatable<EvoVector3>
{
    public readonly float X;
    public readonly float Y;
    public readonly float Z;

    public EvoVector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static readonly EvoVector3 Zero = new(0f, 0f, 0f);
    public static readonly EvoVector3 One = new(1f, 1f, 1f);
    public static readonly EvoVector3 Up = new(0f, 1f, 0f);
    public static readonly EvoVector3 Forward = new(0f, 0f, 1f);

    public float Magnitude() => MathF.Sqrt(X * X + Y * Y + Z * Z);
    public float SqrMagnitude() => X * X + Y * Y + Z * Z;

    public EvoVector3 Normalized()
    {
        var mag = Magnitude();
        return mag < float.Epsilon ? Zero : new EvoVector3(X / mag, Y / mag, Z / mag);
    }

    public float Dot(EvoVector3 other) =>
        X * other.X + Y * other.Y + Z * other.Z;

    public EvoVector3 Cross(EvoVector3 other) =>
        new(
            Y * other.Z - Z * other.Y,
            Z * other.X - X * other.Z,
            X * other.Y - Y * other.X
        );

    public static float Distance(EvoVector3 a, EvoVector3 b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z - b.Z;
        return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public override bool Equals(object? obj) =>
        obj is EvoVector3 other && Equals(other);

    public bool Equals(EvoVector3 other) =>
        X == other.X && Y == other.Y && Z == other.Z;

    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public override string ToString() =>
        $"({X:0.#####}, {Y:0.#####}, {Z:0.#####})";

    public static EvoVector3 operator +(EvoVector3 a, EvoVector3 b) =>
        new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static EvoVector3 operator -(EvoVector3 a, EvoVector3 b) =>
        new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static EvoVector3 operator *(EvoVector3 v, float s) =>
        new(v.X * s, v.Y * s, v.Z * s);

    public static EvoVector3 operator *(float s, EvoVector3 v) =>
        new(v.X * s, v.Y * s, v.Z * s);

    public static EvoVector3 operator /(EvoVector3 v, float s) =>
        new(v.X / s, v.Y / s, v.Z / s);

    public static bool operator ==(EvoVector3 left, EvoVector3 right) =>
        left.Equals(right);

    public static bool operator !=(EvoVector3 left, EvoVector3 right) =>
        !left.Equals(right);
}

public sealed class EvoVector3Converter : JsonConverter<EvoVector3>
{
    public override EvoVector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var doc = JsonDocument.ParseValue(ref reader);
        var x = doc.RootElement.GetProperty("X").GetSingle();
        var y = doc.RootElement.GetProperty("Y").GetSingle();
        var z = doc.RootElement.GetProperty("Z").GetSingle();
        return new EvoVector3(x, y, z);
    }

    public override void Write(Utf8JsonWriter writer, EvoVector3 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteNumber("Z", value.Z);
        writer.WriteEndObject();
    }
}
