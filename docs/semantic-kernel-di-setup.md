# Semantic Kernel DI Setup

## Registration Methods

### Core Services Registration
```csharp
// Registers basic Kernel and Plugin services
builder.Services.AddKernel();
```

### Manual Kernel Registration
```csharp
builder.Services.AddTransient<Kernel>(sp =>
{
    var builder = Kernel.CreateBuilder();
    builder.AddOpenAIChatCompletion("gpt-4", "your-api-key");
    return builder.Build();
});
```
Best practice is to create singletons of your AI services and reuse them in transient kernels using `builder.Services.AddTransient(sp => new Kernel(sp));`.

## Chat Completion Registration

### Built-in Registration
```csharp
// Using SK's built-in method
builder.Services.AddOpenAIChatCompletion("gpt-4", "your-api-key");
```

### Custom Registration
```csharp
// Manual registration with configuration
builder.Services.AddSingleton<IChatCompletionService>(sp =>
{
    var options = sp.GetRequiredService<IOptions<OpenAI>>().Value;
    return new OpenAIChatCompletionService(options.ChatModelId, options.ApiKey);
});
```

## Plugin Registration Methods

### Basic Plugin Registration
```csharp
builder.Services.AddTransient(sp =>
{
    var pluginCollection = new KernelPluginCollection();
    pluginCollection.Add(new MyCustomPlugin());
    pluginCollection.Add(new AnotherPlugin());
    return new Kernel(sp, pluginCollection);
});
```

### Advanced Plugin Registration
```csharp
// Register plugin instances
builder.Services.AddSingleton(() => new MyCustomPlugin());
builder.Services.AddSingleton(() => new AnotherPlugin());

// Create plugin collection
builder.Services.AddSingleton<KernelPluginCollection>((serviceProvider) => 
    [
        KernelPluginFactory.CreateFromObject(serviceProvider.GetRequiredService<MyCustomPlugin>()),
        KernelPluginFactory.CreateFromObject(serviceProvider.GetRequiredService<AnotherPlugin>())
    ]
);

// Register Kernel with plugins
builder.Services.AddTransient((serviceProvider) => {
    KernelPluginCollection pluginCollection = serviceProvider.GetRequiredService<KernelPluginCollection>();
    return new Kernel(serviceProvider, pluginCollection);
});
```

## References
- [Using Semantic Kernel with Dependency Injection](https://devblogs.microsoft.com/semantic-kernel/using-semantic-kernel-with-dependency-injection/)
- [Semantic Kernel Concepts: Kernel](https://learn.microsoft.com/en-us/semantic-kernel/concepts/kernel?pivots=programming-language-csharp)