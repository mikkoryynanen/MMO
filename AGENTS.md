# MMO Project Rules

## AI scope (client only)

You MAY create or modify files in:
- `MMO.Client/**`
- `MMO.DummyClient/**`
- `MMO.slnx` (only to add or update the `MMO.Client` project entry)

You MUST NOT modify files in:
- `MMO.WorldServer/**`
- `MMO.Auth/**`
- `MMO.AppHost/**`
- `MMO.Shared/**`
- `MMO.Database.Shared/**`
- `MMO.Database.Migrations/**`
- `MMO.Dev.DevAPI/**`

## Behaviour

- `opencode.json` enforces these via hard `deny` rules on the `edit` permission. Attempts to edit forbidden paths will be blocked at the tool layer.
- You may still *read* forbidden paths to understand the wire protocol, packet shapes, server behaviour, etc. — context-gathering is fine.
- If a task genuinely requires a server change, **stop and tell the user**. Do not silently skip, stub it out, or create a duplicate copy elsewhere to work around the constraint.
- Do not add new files outside `MMO.Client/` or `MMO.DummyClient/` unless explicitly asked.

## Conventions

- Read the project's existing code before writing new code. Match style, naming, and patterns.
- Run `dotnet build MMO.slnx` after non-trivial changes and fix any new warnings/errors before considering work done.
- Do not commit unless explicitly asked.
