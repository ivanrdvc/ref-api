using System.Text.Json.Serialization;

namespace RefApi.Features.Chat.Models;

public record ChatRequestOverrides
{
    [JsonPropertyName("top")]
    public int Top { get; init; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; init; }

    [JsonPropertyName("minimum_reranker_score")]
    public int MinimumRerankerScore { get; init; }

    [JsonPropertyName("minimum_search_score")]
    public int MinimumSearchScore { get; init; }

    [JsonPropertyName("retrieval_mode")]
    public RetrievalMode RetrievalMode { get; init; } = RetrievalMode.Vectors;

    [JsonPropertyName("semantic_ranker")]
    public bool SemanticRanker { get; init; }

    [JsonPropertyName("semantic_captions")]
    public bool SemanticCaptions { get; init; }

    [JsonPropertyName("include_category")]
    public string? IncludeCategory { get; init; }

    [JsonPropertyName("exclude_category")]
    public string? ExcludeCategory { get; init; }

    [JsonPropertyName("seed")]
    public int? Seed { get; init; }

    [JsonPropertyName("prompt_template")]
    public string? PromptTemplate { get; init; }

    [JsonPropertyName("prompt_template_prefix")]
    public string? PromptTemplatePrefix { get; init; }

    [JsonPropertyName("prompt_template_suffix")]
    public string? PromptTemplateSuffix { get; init; }

    [JsonPropertyName("suggest_followup_questions")]
    public bool SuggestFollowupQuestions { get; init; }

    [JsonPropertyName("use_oid_security_filter")]
    public bool UseOidSecurityFilter { get; init; }

    [JsonPropertyName("use_groups_security_filter")]
    public bool UseGroupsSecurityFilter { get; init; }

    [JsonPropertyName("vector_fields")]
    public List<VectorFieldOptions> VectorFields { get; init; } = [VectorFieldOptions.Embedding];

    [JsonPropertyName("use_gpt4v")]
    public bool UseGpt4V { get; init; } = false;

    [JsonPropertyName("gpt4v_input")]
    public Gpt4VInput? Gpt4VInput { get; init; }

    [JsonPropertyName("language")]
    public string Language { get; init; } = "en";
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RetrievalMode
{
    [JsonPropertyName("hybrid")]
    Hybrid,

    [JsonPropertyName("vectors")]
    Vectors,

    [JsonPropertyName("text")]
    Text
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Gpt4VInput
{
    [JsonPropertyName("textAndImages")]
    TextAndImages,

    [JsonPropertyName("images")]
    Images,

    [JsonPropertyName("texts")]
    Texts
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VectorFieldOptions
{
    [JsonPropertyName("embedding")]
    Embedding,

    [JsonPropertyName("imageEmbedding")]
    ImageEmbedding,

    [JsonPropertyName("both")]
    Both
}