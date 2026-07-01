# CaBootstrap.CLI

**ca-bootstrap** is a generic .NET 8 global CLI tool that bootstraps Clean Architecture solutions with a single command.

## Installation

```bash
cd InspectPro.CLI/InspectPro.CLI
dotnet pack -c Release
dotnet tool install --global --add-source ./nupkg CaBootstrap.CLI
```

Update: `dotnet tool update --global CaBootstrap.CLI`  
Uninstall: `dotnet tool uninstall --global CaBootstrap.CLI`

## Usage (Kaise use karein)

### Step 1 — Solution folder mein jao

```bash
cd "D:\my projects\SmartInventoryApp"
```

### Step 2 — Bootstrap chalao

```bash
ca-bootstrap init
```

Ya explicitly path do:

```bash
ca-bootstrap setup --path "D:\my projects\SmartInventoryApp"
```

### Step 3 — Custom config (optional)

Solution root mein `cabootstrap.json` rakho, phir:

```bash
ca-bootstrap init --config ./cabootstrap.json
```

### Bina install ke (development)

```bash
cd InspectPro.CLI
dotnet run --project InspectPro.CLI -- init --path "D:\my projects\SmartInventoryApp"
```

## Commands

| Command | Kaam |
|---------|------|
| `ca-bootstrap init` | Full bootstrap |
| `ca-bootstrap setup` | Same as init |
| `--path`, `-p` | Solution folder |
| `--config`, `-c` | `cabootstrap.json` path |
| `ca-bootstrap --help` | Help |

## MediatR pattern — HandlerResult

Handlers `ServiceResult` nahi, **`HandlerResult<T>`** return karte hain:

```csharp
// Handler example
public async Task<HandlerResult<UserDto>> Handle(GetUserQuery request, CancellationToken ct)
{
    var user = await _repo.GetByIdAsync(request.Id, ct);
    if (user is null)
        return HandlerResult<UserDto>.Failure("User not found");

    return HandlerResult<UserDto>.Success(user.ToDto());
}
```

Tool automatically generates `Common/HandlerResult.cs` in Application project.

## Configuration — `cabootstrap.json`

```json
{
  "projectNames": {
    "Api": "SmartInventory.Presentation"
  },
  "packages": { ... },
  "folders": { ... },
  "references": [ ... ],
  "features": {
    "installPackages": true,
    "build": true
  }
}
```

## Project location

```
D:\my projects\SmartInventoryApp\InspectPro.CLI\
```

(Folder name abhi `InspectPro.CLI` hai; tool command **`ca-bootstrap`** hai.)
