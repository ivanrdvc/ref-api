# TODOs
- update scalar & add it to readme (also update it), readme comment about .md
- (scalar too) add user-jwts & BearerSecurityScheme to open api
- remove mediatr
- sort common fodler and its content 

- telemetry both config & activity (test it?)
- pass over apis and see if it makes sense and if returns match and add v to expriment
- handle error from chatcompletion service
- can improve Messagerole string?
- logging? too much stuff also what about general logging
- session state to guid? but have to check with UI
- followup

forscalar
https://github.com/dotnet/eShop/blob/main/src/eShop.ServiceDefaults/OpenApi.Extensions.cs
https://github.com/dotnet/eShop/pull/786/files


for telemetry
https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/Step07_Telemetry.cs

new pr
- remove fluent
- Azure RAG Specificity: The AzureOpenAIPromptExecutionSettings and its AzureChatDataSource are tied specifically to Azure OpenAI's integration with Azure AI Search. If you wanted to implement RAG with OpenAI (e.g., by manually retrieving context from a vector database and adding it to the prompt) or use a different data source with Azure OpenAI, this structure would need modification. The IAIProviderSettings interface doesn't explicitly abstract the concept of a "data source" for RAG, it's implicitly handled only within the Azure path of the concrete implementation.
- add auth0/test