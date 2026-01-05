string openapiV1YamlName = "InternalOpenapiV1";
string openapiV1ClassPath = $"../TheSportsDB/{openapiV1YamlName}Client.cs";

File.Move($"obj/{openapiV1YamlName}Client.cs", openapiV1ClassPath, true);

ReplaceInFile(openapiV1ClassPath, "    public partial class OpenapiV1Client", "    internal partial class OpenapiV1Client");

void ReplaceInFile(string filePath, string oldText, string newText)
{
    string fileContent = File.ReadAllText(filePath);
    string modifiedContent = fileContent.Replace(oldText, newText);

    File.WriteAllText(filePath, modifiedContent);
}
