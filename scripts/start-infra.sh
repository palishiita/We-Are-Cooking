docker compose \
--env-file "$( dirname "${BASH_SOURCE[0]}" )"/../infrastructure/.env \
-f "$( dirname "${BASH_SOURCE[0]}" )"/../infrastructure/keycloak/compose.dev.yaml \
up -d