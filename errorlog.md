Dec 04 16:48:36 Infernal-Server dotnet[35031]:    at Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider.Load(Stream stream)
Dec 04 16:48:36 Infernal-Server dotnet[35031]:    at Microsoft.Extensions.Configuration.FileConfigurationProvider.Load(Boolean reload)
Dec 04 16:48:36 Infernal-Server dotnet[35031]:    --- End of inner exception stack trace ---
Dec 04 16:48:36 Infernal-Server dotnet[35031]:    at Microsoft.Extensions.Configuration.FileConfigurationProvider.Load(Boolean reload)
Dec 04 16:48:36 Infernal-Server dotnet[35031]:    at Microsoft.Extensions.Configuration.ConfigurationManager.AddSource(IConfigurationSource source)
Dec 04 16:48:36 Infernal-Server dotnet[35031]:    at Microsoft.Extensions.Configuration.ConfigurationManager.Microsoft.Extensions.Configuration.IConfigurationBuilder.Add(IConfigurationSource source)
Dec 04 16:48:36 Infernal-Server dotnet[35031]:    at Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ApplyDefaultAppConfiguration(HostBuilderContext hostingContext, IConfigurationBuilder appConfigBuilder, String[] args)
Dec 04 16:48:36 Infernal-Server dotnet[35031]:    at Microsoft.Extensions.Hosting.HostApplicationBuilder..ctor(HostApplicationBuilderSettings settings)
Dec 04 16:48:36 Infernal-Server dotnet[35031]:    at Microsoft.AspNetCore.Builder.WebApplicationBuilder..ctor(WebApplicationOptions options, Action`1 configureDefaults)
Dec 04 16:48:36 Infernal-Server dotnet[35031]:    at Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(String[] args)
Dec 04 16:48:36 Infernal-Server dotnet[35031]:    at Program.<Main>$(String[] args) in /home/jake-johnson/github/Infernal-Ink-Steel-Suite/InfernalInkSteelSuite.Api/Program.cs:line 17
Dec 04 16:48:36 Infernal-Server systemd[1]: infernal-api.service: Main process exited, code=dumped, status=6/ABRT
Dec 04 16:48:36 Infernal-Server systemd[1]: infernal-api.service: Failed with result 'core-dump'.
Dec 04 16:48:46 Infernal-Server systemd[1]: infernal-api.service: Scheduled restart job, restart counter is at 630.
Dec 04 16:48:46 Infernal-Server systemd[1]: Started infernal-api.service - Infernal Ink & Steel Suite API.
Dec 04 16:48:46 Infernal-Server dotnet[35095]: Unhandled exception. System.IO.InvalidDataException: Failed to load configuration from file '/opt/infernal-publish/Api/appsettings.json'.
Dec 04 16:48:46 Infernal-Server dotnet[35095]:  ---> System.FormatException: Could not parse the JSON file.
Dec 04 16:48:46 Infernal-Server dotnet[35095]:  ---> System.Text.Json.JsonReaderException: '}' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 2.
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at System.Text.Json.ThrowHelper.ThrowJsonReaderException(Utf8JsonReader& json, ExceptionResource resource, Byte nextByte, ReadOnlySpan`1 bytes)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at System.Text.Json.Utf8JsonReader.ConsumeValue(Byte marker)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at System.Text.Json.Utf8JsonReader.ReadFirstToken(Byte first)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at System.Text.Json.Utf8JsonReader.ReadSingleSegment()
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at System.Text.Json.Utf8JsonReader.Read()
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at System.Text.Json.JsonDocument.Parse(ReadOnlySpan`1 utf8JsonSpan, JsonReaderOptions readerOptions, MetadataDb& database, StackRowStack& stack)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at System.Text.Json.JsonDocument.Parse(ReadOnlyMemory`1 utf8Json, JsonReaderOptions readerOptions, Byte[] extraRentedArrayPoolBytes, PooledByteBufferWriter extraPooledByteBufferWriter)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at System.Text.Json.JsonDocument.Parse(ReadOnlyMemory`1 json, JsonDocumentOptions options)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at Microsoft.Extensions.Configuration.Json.JsonConfigurationFileParser.ParseStream(Stream input)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider.Load(Stream stream)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    --- End of inner exception stack trace ---
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider.Load(Stream stream)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at Microsoft.Extensions.Configuration.FileConfigurationProvider.Load(Boolean reload)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    --- End of inner exception stack trace ---
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at Microsoft.Extensions.Configuration.FileConfigurationProvider.Load(Boolean reload)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at Microsoft.Extensions.Configuration.ConfigurationManager.AddSource(IConfigurationSource source)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at Microsoft.Extensions.Configuration.ConfigurationManager.Microsoft.Extensions.Configuration.IConfigurationBuilder.Add(IConfigurationSource source)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ApplyDefaultAppConfiguration(HostBuilderContext hostingContext, IConfigurationBuilder appConfigBuilder, String[] args)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at Microsoft.Extensions.Hosting.HostApplicationBuilder..ctor(HostApplicationBuilderSettings settings)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at Microsoft.AspNetCore.Builder.WebApplicationBuilder..ctor(WebApplicationOptions options, Action`1 configureDefaults)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(String[] args)
Dec 04 16:48:46 Infernal-Server dotnet[35095]:    at Program.<Main>$(String[] args) in /home/jake-johnson/github/Infernal-Ink-Steel-Suite/InfernalInkSteelSuite.Api/Program.cs:line 17
Dec 04 16:48:46 Infernal-Server systemd[1]: infernal-api.service: Main process exited, code=dumped, status=6/ABRT
Dec 04 16:48:46 Infernal-Server systemd[1]: infernal-api.service: Failed with result 'core-dump'.
Dec 04 16:48:56 Infernal-Server systemd[1]: infernal-api.service: Scheduled restart job, restart counter is at 631.
Dec 04 16:48:57 Infernal-Server systemd[1]: Started infernal-api.service - Infernal Ink & Steel Suite API.
Dec 04 16:48:57 Infernal-Server dotnet[35129]: Unhandled exception. System.IO.InvalidDataException: Failed to load configuration from file '/opt/infernal-publish/Api/appsettings.json'.
Dec 04 16:48:57 Infernal-Server dotnet[35129]:  ---> System.FormatException: Could not parse the JSON file.
Dec 04 16:48:57 Infernal-Server dotnet[35129]:  ---> System.Text.Json.JsonReaderException: '}' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 2.
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at System.Text.Json.ThrowHelper.ThrowJsonReaderException(Utf8JsonReader& json, ExceptionResource resource, Byte nextByte, ReadOnlySpan`1 bytes)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at System.Text.Json.Utf8JsonReader.ConsumeValue(Byte marker)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at System.Text.Json.Utf8JsonReader.ReadFirstToken(Byte first)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at System.Text.Json.Utf8JsonReader.ReadSingleSegment()
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at System.Text.Json.Utf8JsonReader.Read()
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at System.Text.Json.JsonDocument.Parse(ReadOnlySpan`1 utf8JsonSpan, JsonReaderOptions readerOptions, MetadataDb& database, StackRowStack& stack)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at System.Text.Json.JsonDocument.Parse(ReadOnlyMemory`1 utf8Json, JsonReaderOptions readerOptions, Byte[] extraRentedArrayPoolBytes, PooledByteBufferWriter extraPooledByteBufferWriter)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at System.Text.Json.JsonDocument.Parse(ReadOnlyMemory`1 json, JsonDocumentOptions options)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at Microsoft.Extensions.Configuration.Json.JsonConfigurationFileParser.ParseStream(Stream input)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider.Load(Stream stream)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    --- End of inner exception stack trace ---
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider.Load(Stream stream)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at Microsoft.Extensions.Configuration.FileConfigurationProvider.Load(Boolean reload)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    --- End of inner exception stack trace ---
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at Microsoft.Extensions.Configuration.FileConfigurationProvider.Load(Boolean reload)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at Microsoft.Extensions.Configuration.ConfigurationManager.AddSource(IConfigurationSource source)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at Microsoft.Extensions.Configuration.ConfigurationManager.Microsoft.Extensions.Configuration.IConfigurationBuilder.Add(IConfigurationSource source)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ApplyDefaultAppConfiguration(HostBuilderContext hostingContext, IConfigurationBuilder appConfigBuilder, String[] args)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at Microsoft.Extensions.Hosting.HostApplicationBuilder..ctor(HostApplicationBuilderSettings settings)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at Microsoft.AspNetCore.Builder.WebApplicationBuilder..ctor(WebApplicationOptions options, Action`1 configureDefaults)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(String[] args)
Dec 04 16:48:57 Infernal-Server dotnet[35129]:    at Program.<Main>$(String[] args) in /home/jake-johnson/github/Infernal-Ink-Steel-Suite/InfernalInkSteelSuite.Api/Program.cs:line 17
Dec 04 16:48:57 Infernal-Server systemd[1]: infernal-api.service: Main process exited, code=dumped, status=6/ABRT
Dec 04 16:48:57 Infernal-Server systemd[1]: infernal-api.service: Failed with result 'core-dump'.
Dec 04 16:49:07 Infernal-Server systemd[1]: infernal-api.service: Scheduled restart job, restart counter is at 632.
Dec 04 16:49:07 Infernal-Server systemd[1]: Started infernal-api.service - Infernal Ink & Steel Suite API.
Dec 04 16:49:07 Infernal-Server dotnet[35173]: Unhandled exception. System.IO.InvalidDataException: Failed to load configuration from file '/opt/infernal-publish/Api/appsettings.json'.
Dec 04 16:49:07 Infernal-Server dotnet[35173]:  ---> System.FormatException: Could not parse the JSON file.
Dec 04 16:49:07 Infernal-Server dotnet[35173]:  ---> System.Text.Json.JsonReaderException: '}' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 2.
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at System.Text.Json.ThrowHelper.ThrowJsonReaderException(Utf8JsonReader& json, ExceptionResource resource, Byte nextByte, ReadOnlySpan`1 bytes)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at System.Text.Json.Utf8JsonReader.ConsumeValue(Byte marker)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at System.Text.Json.Utf8JsonReader.ReadFirstToken(Byte first)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at System.Text.Json.Utf8JsonReader.ReadSingleSegment()
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at System.Text.Json.Utf8JsonReader.Read()
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at System.Text.Json.JsonDocument.Parse(ReadOnlySpan`1 utf8JsonSpan, JsonReaderOptions readerOptions, MetadataDb& database, StackRowStack& stack)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at System.Text.Json.JsonDocument.Parse(ReadOnlyMemory`1 utf8Json, JsonReaderOptions readerOptions, Byte[] extraRentedArrayPoolBytes, PooledByteBufferWriter extraPooledByteBufferWriter)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at System.Text.Json.JsonDocument.Parse(ReadOnlyMemory`1 json, JsonDocumentOptions options)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at Microsoft.Extensions.Configuration.Json.JsonConfigurationFileParser.ParseStream(Stream input)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider.Load(Stream stream)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    --- End of inner exception stack trace ---
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider.Load(Stream stream)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at Microsoft.Extensions.Configuration.FileConfigurationProvider.Load(Boolean reload)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    --- End of inner exception stack trace ---
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at Microsoft.Extensions.Configuration.FileConfigurationProvider.Load(Boolean reload)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at Microsoft.Extensions.Configuration.ConfigurationManager.AddSource(IConfigurationSource source)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at Microsoft.Extensions.Configuration.ConfigurationManager.Microsoft.Extensions.Configuration.IConfigurationBuilder.Add(IConfigurationSource source)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ApplyDefaultAppConfiguration(HostBuilderContext hostingContext, IConfigurationBuilder appConfigBuilder, String[] args)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at Microsoft.Extensions.Hosting.HostApplicationBuilder..ctor(HostApplicationBuilderSettings settings)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at Microsoft.AspNetCore.Builder.WebApplicationBuilder..ctor(WebApplicationOptions options, Action`1 configureDefaults)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(String[] args)
Dec 04 16:49:07 Infernal-Server dotnet[35173]:    at Program.<Main>$(String[] args) in /home/jake-johnson/github/Infernal-Ink-Steel-Suite/InfernalInkSteelSuite.Api/Program.cs:line 17
Dec 04 16:49:07 Infernal-Server systemd[1]: infernal-api.service: Main process exited, code=dumped, status=6/ABRT
Dec 04 16:49:07 Infernal-Server systemd[1]: infernal-api.service: Failed with result 'core-dump'.

