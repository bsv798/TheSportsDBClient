string openapiV1YamlName = "InternalOpenapiV1";
string projectPath = ""; // "../../../";
string openapiV1ClassPath = $"{projectPath}../TheSportsDBClient/{openapiV1YamlName}Client.cs";

File.Move($"{projectPath}obj/{openapiV1YamlName}Client.cs", openapiV1ClassPath, true);

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
ReplaceInFile(openapiV1ClassPath, "System.Threading.Tasks.Task<EventsResponse> GetEventByTitleAsync(string e, string s, System.DateTimeOffset? d, string f", "System.Threading.Tasks.Task<EventResponse> GetEventByTitleAsync(string e, string s, System.DateTimeOffset? d, string f");
ReplaceNextInFile(openapiV1ClassPath, "urlBuilder_.Append(\"searchevents.php\");", "var objectResponse_ = await ReadObjectResponseAsync<EventsResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);", "var objectResponse_ = await ReadObjectResponseAsync<EventResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);");
ReplaceInFile(openapiV1ClassPath, "System.Threading.Tasks.Task<EventResponse> GetEventByIdAsync(int id", "System.Threading.Tasks.Task<EventsResponse> GetEventByIdAsync(int id");
ReplaceNextInFile(openapiV1ClassPath, "urlBuilder_.Append(\"lookupevent.php\");", "var objectResponse_ = await ReadObjectResponseAsync<EventResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);", "var objectResponse_ = await ReadObjectResponseAsync<EventsResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);");
ReplaceInFile(openapiV1ClassPath, "System.Threading.Tasks.Task<PlayersResponse> GetPlayerByNameAsync(string p", "System.Threading.Tasks.Task<PlayerResponse> GetPlayerByNameAsync(string p");
ReplaceNextInFile(openapiV1ClassPath, "urlBuilder_.Append(\"searchplayers.php\");", "var objectResponse_ = await ReadObjectResponseAsync<PlayersResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);", "var objectResponse_ = await ReadObjectResponseAsync<PlayerResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);");
ReplaceNextInFile(openapiV1ClassPath, "public virtual async System.Threading.Tasks.Task<EventResponse> GetEventByTitleAsync(string e, string s, System.DateTimeOffset? d, string f, System.Threading.CancellationToken cancellationToken)", "throw new System.ArgumentNullException(\"e\");", ";");
ReplaceInFile(openapiV1ClassPath, "urlBuilder_.Append(System.Uri.EscapeDataString(\"e\")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(e, System.Globalization.CultureInfo.InvariantCulture))).Append('&');", "if (e != null) urlBuilder_.Append(System.Uri.EscapeDataString(\"e\")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(e, System.Globalization.CultureInfo.InvariantCulture))).Append('&');");

static void ReplaceInFile(string filePath, string oldText, string newText)
{
    string fileContent = File.ReadAllText(filePath);
    string modifiedContent = fileContent.Replace(oldText, newText);

    File.WriteAllText(filePath, modifiedContent);
}

static void ReplaceNextInFile(string filePath, string findText, string oldText, string newText)
{
    string fileContent = File.ReadAllText(filePath);
    int index1 = fileContent.IndexOf(findText);
    string firstPart = fileContent.Substring(0, index1);
    int index2 = fileContent.IndexOf(oldText, index1) + oldText.Length;
    string secondPart = fileContent.Substring(index1, index2 - index1);
    string thirdPart = fileContent.Substring(index2 + 1);
    string modifiedContent = firstPart + secondPart.Replace(oldText, newText) + thirdPart;

    File.WriteAllText(filePath, modifiedContent);
}
