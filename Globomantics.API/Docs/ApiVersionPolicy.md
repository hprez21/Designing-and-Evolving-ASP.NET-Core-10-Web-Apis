# Product Catalog API — Versioning Policy

## Version Format
Versions use sequential integers: `v1`, `v2`, `v3`.
Preview versions use the suffix `-preview.N`: `v2-preview.1`, `v2-preview.2`.

## Stability Guarantees

### GA Versions
- No breaking changes within a GA version.
- Additive changes (new optional fields, new endpoints) may be added at any time.
- Clients SHOULD implement the Tolerant Reader pattern.
- Clients SHOULD ignore unknown fields in responses.

### Preview Versions
- Breaking changes are expected between preview releases.
- Do not build production systems on preview versions.
- Previews may be retired with 90 days notice.
- Previews must graduate to GA or be removed within 12 months.

## Deprecation Policy
- Deprecated versions continue to function but are no longer recommended.
- Deprecation is announced via:
  - `Deprecation` response header on all responses
  - API documentation updates
  - Direct communication to registered API consumers
- Minimum deprecation period: 12 months before sunset.

## Sunset Policy
- Sunset date is communicated via `Sunset` response header (RFC 8594).
- After sunset, the version returns `410 Gone` for all requests.
- Minimum notice: 6 months between sunset announcement and removal.

## What Constitutes a Breaking Change
- Removing or renaming a response field
- Adding a required request field
- Changing a field's data type
- Changing HTTP status codes for existing conditions
- Removing an endpoint
- Changing the response structure (e.g., array to object)
- Changing error response format

## What Is NOT a Breaking Change
- Adding an optional response field
- Adding an optional request field
- Adding a new endpoint
- Adding new values to extensible enumerations
- Widening validation constraints
- Improving error detail messages (not structure)

## Multi-Version Support
- Maximum 2 GA versions supported simultaneously.
- When v3 reaches GA, v1 enters deprecation.
- Support cost grows linearly with active versions — minimize active versions.

