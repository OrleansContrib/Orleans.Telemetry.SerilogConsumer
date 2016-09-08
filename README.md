# Orleans.Telemetry.SerilogConsumer

[![Build status](https://ci.appveyor.com/api/projects/status/cc72wuytncosdc8v?svg=true)](https://ci.appveyor.com/project/moswald/orleans-telemetry-serilogconsumer)

This is a Serilog `ILogConsumer` and `ITelemetryConsumer` for Orleans. Default-constructed instances (ie, `OrleansConfiguration.xml`)
will write incoming log messages and trace events to the default `Serilog.Log.Logger`. You can also create instances in code and
which will allow you to configure the `Serilog` logging instance.

Add a `TelemetryConsumer` element to the `OrleansConfiguration.xml` file like this:
```xml
<Defaults>
  <Telemetry>
    <TelemetryConsumer Assembly="Orleans.Telemetry.SerilogConsumer" Type="Orleans.Telemetry.SerilogConsumer.SerilogConsumer" />
  </Telemetry>
</Defaults>
```

Or register in code before initializing the Orleans Silo:
```csharp
var logger = new Serilog.LoggerConfiguration()
    .WriteTo.LiterateConsole()
    .CreateLogger();
var serilogConsumer = new SerilogConsumer(logger);

LogManager.LogConsumers.Add(serilogConsumer);
LogManager.TelemetryConsumers.Add(serilogConsumer);
```
