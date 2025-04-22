namespace RefApi.Configuration;

public class AzureAISearchOptions
{
    public string ApiKey { get; init; } = string.Empty;

    public string IndexName { get; init; } = string.Empty;

    public string Endpoint { get; init; } = string.Empty;

    public string QueryType { get; init; } = string.Empty;

    public string EmbeddingDeploymentName { get; init; } = string.Empty;

    public string SemanticConfiguration { get; init; } = "default";

    public int Strictness { get; init; } = 4;

    public int TopNDocuments { get; init; } = 10;
}