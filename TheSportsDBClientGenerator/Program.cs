string openapiV1YamlName = "InternalOpenapiV1";
string openapiV1ClassPath = $"../TheSportsDBClient/{openapiV1YamlName}Client.cs";

File.Move($"obj/{openapiV1YamlName}Client.cs", openapiV1ClassPath, true);

ReplaceInFile(openapiV1ClassPath, "namespace TheSportsDBClientGenerator", "namespace TheSportsDBClient");
ReplaceInFile(openapiV1ClassPath, "#pragma warning disable 1591 // Disable \"CS1591 Missing XML comment for publicly visible type or member ...\"", "#pragma warning disable 1591 // Disable \"CS1591 Missing XML comment for publicly visible type or member ...\"" + Environment.NewLine + "#pragma warning disable 1998");
ReplaceInFile(openapiV1ClassPath, "#pragma warning restore 1591", "#pragma warning restore 1591" + Environment.NewLine + "#pragma warning restore 1998");
ReplaceInFile(openapiV1ClassPath, "#pragma warning disable 612 // Disable \"CS0612 '...' is obsolete\"", "#pragma warning disable 612 // Disable \"CS0612 '...' is obsolete\"" + Environment.NewLine + "#pragma warning disable 642");
ReplaceInFile(openapiV1ClassPath, "#pragma warning restore  612", "#pragma warning restore  612" + Environment.NewLine + "#pragma warning restore  642");
ReplaceInFile(openapiV1ClassPath, "    public partial class OpenapiV1Client", "    internal partial class OpenapiV1Client");
ReplaceInFile(openapiV1ClassPath, "var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken)", "var response_ = await client_.SendAsync(request_, cancellationToken)");
ReplaceInFile(openapiV1ClassPath, "static partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings);", "static partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings); static partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings) { settings.ContractResolver = new CustomContractResolver(); } public class CustomContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver { protected override Newtonsoft.Json.Serialization.JsonProperty CreateProperty(System.Reflection.MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization) { var property = base.CreateProperty(member, memberSerialization); if (property.Required == Newtonsoft.Json.Required.DisallowNull) { property.Required = Newtonsoft.Json.Required.Default; } return property; } }");
ReplaceInFile(openapiV1ClassPath, "throw new System.ArgumentNullException(\"l\");", ";");
ReplaceInFile(openapiV1ClassPath, "urlBuilder_.Append(System.Uri.EscapeDataString(\"l\")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(l, System.Globalization.CultureInfo.InvariantCulture))).Append('&');", "if (l != null) urlBuilder_.Append(System.Uri.EscapeDataString(\"l\")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(l, System.Globalization.CultureInfo.InvariantCulture))).Append('&');");
ReplaceInFile(openapiV1ClassPath, "public System.TimeSpan? StrTime { get; set; }", "public System.DateTimeOffset? StrTime { get; set; }");
ReplaceInFile(openapiV1ClassPath, "public System.TimeSpan StrTime { get; set; }", "public System.DateTimeOffset StrTime { get; set; }");

static void ReplaceInFile(string filePath, string oldText, string newText)
{
    string fileContent = File.ReadAllText(filePath);
    string modifiedContent = fileContent.Replace(oldText, newText);

    File.WriteAllText(filePath, modifiedContent);
}
