# CQRS Code Organization Pattern for C# Solutions

## Context and Problem Statement

When implementing CQRS (Command Query Responsibility Segregation) pattern, we need to decide on a
file/folder organization pattern for commands, queries, and their respective handlers. The
organization should promote maintainability, and make it easy for developers to locate and modify
related code.

## Considered Options

### 1. Command/Query Separation with Individual Files

```
Orders/
├── Commands/
│   ├── CreateOrderCommand.cs
│   ├── CreateOrderCommandHandler.cs
│   ├── CancelOrderCommand.cs
│   └── CancelOrderCommandHandler.cs
└── Queries/
    ├── GetOrderByIdQuery.cs
    ├── GetOrderByIdQueryHandler.cs
    └── GetOrdersListQuery.cs
```

* Follows standard C# convention of one class per file
* Provides granular source control history of command and handler changes
* Clear type discovery through consistent file naming pattern

### 2. Nested Class Approach

```
Orders/
├── Commands/
│   ├── CreateOrder.cs
│   │   - CreateOrderCommand
│   │   - CreateOrderCommandHandler
│   │   or
│   │   - Command
│   │   - Handler
│   └── CancelOrder.cs
│       - CancelOrderCommand
│       - CancelOrderCommandHandler
└── Queries/
    └── GetOrderById.cs
        - GetOrderByIdQuery
        - GetOrderByIdQueryHandler
```

* Groups related command/handler code together in single file
* Reduces total number of files while maintaining feature cohesion
* Simpler navigation between related command and handler code

### 3. Combined Files Without Nesting

```
Orders/
├── Commands/
│   ├── CreateOrder.cs        // Contains Command and Handler classes
│   └── CancelOrder.cs        // Contains Command and Handler classes
└── Queries/
    ├── GetOrderById.cs       // Contains Query and Handler classes
    └── GetOrdersList.cs      // Contains Query and Handler classes
```

* Maintains command/query separation while keeping related code together
* Simplifies file management without using nested classes
* Clear file naming that maps directly to business operations

### 4. Feature-Based Organization

```
Orders/
├── CreateOrder/
│   ├── CreateOrderCommand.cs
│   └── CreateOrderCommandHandler.cs
├── CancelOrder/
│   ├── CancelOrderCommand.cs
│   └── CancelOrderCommandHandler.cs
└── GetOrderById/
    ├── GetOrderByIdQuery.cs
    └── GetOrderByIdQueryHandler.cs
```

* Natural fit for domain-driven design approaches
* Supports independent feature development and deployment
* Makes feature-specific modifications and testing more focused

## Additional Considerations

Chosen option: "Combined Files Without Nesting", because:

1. Easy to find and navigate related code for each operation
2. Maintains clear separation between Commands and Queries
3. Avoids complexity of nested classes
4. Reduced file count without sacrificing code organization

Supporting classes like CreateOrderCommandValidator, DTOs, and exceptions can be organized in the
same manner as commands and handlers in each approach - either at the same level as the
command/handler files, as nested classes within the command file, or in separate type-specific
subfolders (e.g., Validators/, Dtos/).

### Consequences

Good:

* One file per operation (command/query)
* Clean folder structure
* No nested class complexity
* Easy to locate related code

Bad:

* May need to split files if complexity grows significantly
* No physical separation between command and handler code
* Deviates from C# convention of one class per file (this is a common C# convention where each
  public class should be in its own file, though not a strict requirement)