// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

#pragma warning disable S103 // Lines should not be too long

[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>", Scope = "member", Target = "~M:Atata.WebDriverSetup.HttpRequestExecutor.CreateHttpClient~System.Net.Http.HttpClient")]
[assembly: SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "<Pending>", Scope = "member", Target = "~F:Atata.WebDriverSetup.ChromeDriverSetupStrategy.BaseUrl")]
[assembly: SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "<Pending>", Scope = "member", Target = "~F:Atata.WebDriverSetup.EdgeDriverSetupStrategy.BaseUrl")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "<Pending>", Scope = "member", Target = "~M:Atata.WebDriverSetup.DriverSetupExecutor.AddToEnvironmentPathVariable(System.String)")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "<Pending>", Scope = "member", Target = "~M:Atata.WebDriverSetup.DriverSetupExecutor.ExtractDownloadedFile(System.String,System.String)")]
[assembly: SuppressMessage("Major Code Smell", "S2971:\"IEnumerable\" LINQs should be simplified", Justification = "<Pending>", Scope = "member", Target = "~M:Atata.WebDriverSetup.DriverSetup.SetUpPendingConfigurationsAsync~System.Threading.Tasks.Task{Atata.WebDriverSetup.DriverSetupResult[]}")]

#pragma warning restore S103 // Lines should not be too long
