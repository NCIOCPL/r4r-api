# Copilot Instructions

## General Guidelines

- If you don't know how to do something, say you don't know. Do not make assumptions.
- If an instruction is unclear, ask for clarification BEFORE proceeding.
- Write clear and concise code comments where necessary.
- Follow the existing coding style and conventions used in the repository.
- Ensure proper error handling and input validation.

## Elasticsearch

### Client Usage
- Use the modern **Elastic.Clients.Elasticsearch** client (version 8.x or higher).
- DO NOT use the older NEST client.
- Target Elasticsearch 8.x compatibility.

### Query Patterns
- Use **object initializers (Query DSL style)** instead of the fluent API with lambdas.
- This approach is more explicit and aligns better with Elasticsearch JSON documentation.

#### Preferred Pattern (Object Initializers):
```csharp
var response = await client.SearchAsync<MyDoc>(new SearchRequest("my-index")
{
    Query = new MatchQuery(new Field("name"))
    {
        Query = "search term"
    },
    Size = 10,
    From = 0
});
```

#### Avoid (Fluent API):
```csharp
// Don't use this pattern
var response = await client.SearchAsync<MyDoc>(s => s
    .Index("my-index")
    .Query(q => q.Match(m => m.Field(f => f.Name).Query("search term")))
);
```

### Elasticsearch 7 → 8 Migration
- Be aware of breaking changes: type removal, mapping changes, and deprecation removals.
- Update index creation code to remove type references (types were removed in ES 8).
- Replace any `_type` field references in queries.
- Update authentication patterns if using API keys or security features.
- Check for and remove deprecated API usage.

### Error Handling
- Always check `response.IsValidResponse` before using results.
- Log Elasticsearch errors with sufficient context for debugging.
- Handle network failures and timeouts gracefully.
- Example:
```csharp
var response = await client.SearchAsync<MyDoc>(request);
if (!response.IsValidResponse)
{
    // Log response.DebugInformation or response.ElasticsearchServerError
    throw new Exception($"Elasticsearch error: {response.ElasticsearchServerError?.Error}");
}
```

## JSON Serialization

- Use **System.Text.Json** instead of Newtonsoft.Json.
- Follow modern .NET serialization patterns.

## Code Organization

### Using Statements
- Remove any unneeded Using statements.
- Using statements should be split into groups in this order:
  1. "Out of the box" .NET packages (System.*)
  2. Third-party packages (e.g., Elastic.Clients.Elasticsearch)
  3. Packages which are part of this solution
- Within a group, Using statements should be in alphabetical order.
- Leave a blank line between groups.

#### Example:
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

using MyProject.Models;
using MyProject.Services;
```

### Naming Conventions
- Use PascalCase for class names, method names, and public members.
- Use camelCase for local variables and method parameters.
- Prefix private fields with underscore (e.g., `_fieldName`).
- Use meaningful, descriptive names; avoid abbreviations unless widely understood.
- Suffix async method names with "Async".

## Async Patterns

- Use async/await consistently; avoid blocking calls with `.Result` or `.Wait()`.
- Suffix async method names with "Async".
- Use `ConfigureAwait(false)` in library code to avoid deadlocks.
- Example:
```csharp
public async Task<SearchResponse<MyDoc>> SearchAsync(string query)
{
    var response = await client.SearchAsync<MyDoc>(new SearchRequest("my-index")
    {
        Query = new MatchQuery(new Field("name")) { Query = query }
    }).ConfigureAwait(false);

    return response;
}
```

## Testing

- Include unit tests for any new or modified code.
- Ensure integration tests pass against Elasticsearch 8.x.
- Test backwards compatibility considerations during migration.
- Mock Elasticsearch client appropriately in unit tests.

## Version Control

### Commit Messages
- Include a brief summary of what was changed and why.
- Use present tense ("Add feature" not "Added feature").
- Reference issue numbers when applicable.
- Example: "Update search query to use object initializers for ES8 compatibility"

## Documentation

- Update XML documentation comments for public APIs.
- Document any breaking changes in migration notes.
- Add code comments for complex business logic or non-obvious implementations.
- Keep README files up to date with dependency changes.