using Newtonsoft.Json.Linq;

class Program
{
    static void Main(string[] args)
    {
        string inputFilePath = @"your_appsettings.json";
        string outputVariableFilePath = @"your_output_variables.txt";
        string outputTemplateFilePath = @"your_output_template.txt";

        string jsonContent = File.ReadAllText(inputFilePath);
        var jsonObject = JObject.Parse(jsonContent);

        Dictionary<string, string> keyValues = [];

        var templateJson = CreateTemplate(jsonObject, keyValues, "");

        using (StreamWriter variableFile = new(outputVariableFilePath))
        {
            foreach (var kv in keyValues)
            {
                variableFile.WriteLine($"{kv.Key}=\"{kv.Value}\"");
            }
        }

        File.WriteAllText(outputTemplateFilePath, templateJson.ToString());

        Console.WriteLine("Done!");
    }

    static JObject CreateTemplate(JObject jsonObject, Dictionary<string, string> keyValues, string parentKey)
    {
        JObject templateObject = [];

        foreach (var property in jsonObject.Properties())
        {
            string currentKey = string.IsNullOrEmpty(parentKey) ? property.Name : $"{parentKey}_{property.Name}";

            if (property.Value is JObject childObject)
            {
                templateObject[property.Name] = CreateTemplate(childObject, keyValues, currentKey);
            }
            else
            {
                keyValues[currentKey] = property.Value.ToString();
                templateObject[property.Name] = $"${currentKey}";
            }
        }

        return templateObject;
    }
}
