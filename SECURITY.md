# Security Policy

## Reporting a vulnerability

Please **do not open a public issue or pull request** for security problems.

Report privately to **[SECURITY CONTACT EMAIL]** with:

- a description of the issue and its impact,
- the affected endpoint, file or feature,
- steps to reproduce (requests/responses, role used, sample data),
- any suggested fix.

You can expect an acknowledgement within **[N] business days** and a status update
within **[N] business days**. Please give us a reasonable time to fix the issue before
any disclosure.

## Scope

This repository contains the SylviaNG Community backend service (.NET). In scope:
authentication and authorization, multi-tenancy isolation, data exposure, file upload
and storage, injection, and misconfiguration.

## Supported versions

Only the latest state of the `dev` branch and the current release branch
(`master`) receive security fixes. [CONFIRM RELEASE POLICY]

## Handling secrets

- Never commit real credentials (database passwords, Keycloak client secrets, JWT
  signing keys, SMTP passwords). Use environment variables or a secrets manager, and
  see `SylviaNG.Community/appsettings.Example.json` for the expected configuration keys.
- If a secret is committed by mistake, treat it as compromised: rotate it first, then
  remove it from the repository.
- The `DevHeader` authentication scheme (`X-Dev-Employee-Id` / `X-Dev-Role`) exists for
  local development only and must never be enabled outside the `Development` environment.
