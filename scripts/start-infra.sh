
home="$(pwd)"/"$(dirname "$0")"..

docker build -f "$home"/services/gateway/Dockerfile . -t wrc-gateway:latest

docker compose \
-f "$home"/infrastructure/keycloak/compose.dev.yaml \
--env-file "$home"/infrastructure/.env \
up -d


sudo chown -R "$USER":"$USER" "$home"/../infrastructure/keycloak/authentication-DB-data
sudo chmod -R 777 "$home"/../infrastructure/keycloak/authentication-DB-data