using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FinanceApp;

public class CustomDateTimeConverter : IYamlTypeConverter
{
    private readonly string format = "dd.MM.yyyy HH:mm:ss";

    public bool Accepts(Type type)
    {
        return type == typeof(DateTime);
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var scalar = parser.Expect<Scalar>();
        if (DateTime.TryParseExact(scalar.Value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
        {
            return dt;
        }
        throw new InvalidOperationException($"Не удалось преобразовать '{scalar.Value}' в DateTime с форматом '{format}'.");
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        DateTime dt = (DateTime)value;
        emitter.Emit(new Scalar(dt.ToString(format)));
    }
}