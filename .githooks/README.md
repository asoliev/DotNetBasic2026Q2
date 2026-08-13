# Git Hooks

This folder contains tracked git hook examples for local validation.

Enable them with:

```bash
git config core.hooksPath .githooks
```

The current `pre-commit` hook runs `dotnet format Module17_WebApi/Module17_WebApi.slnx --verify-no-changes`.