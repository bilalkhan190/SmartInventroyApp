# Security Policy

## Reporting a vulnerability

Please report suspected vulnerabilities privately to the repository owner. Do not open a public issue containing credentials, tokens, personal data, or exploit details.

Include the affected endpoint or component, reproduction steps, impact, and any suggested mitigation. Revoke exposed credentials before sharing a report.

## Secret handling

- Never commit `appsettings.json`, `appsettings.Development.json`, `.env`, database passwords, JWT keys, or refresh tokens.
- Use environment variables or a managed secret store in deployed environments.
- Use a unique JWT signing key with at least 32 bytes of entropy.
- Do not enable the development admin seed in production.
- Rotate a secret immediately if it may have entered Git history; deleting it from the latest commit is not sufficient.

## Supported versions

Security fixes are applied to the latest version on the default branch.
