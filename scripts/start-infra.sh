
home="$(dirname "$0")"/..

docker build -f "$home"/services/gateway/Dockerfile . -t wrc-gateway:latest

docker compose -f "$home"/infrastructure/compose.dev.infra.yaml down --remove-orphans

docker compose \
-f "$home"/infrastructure/compose.dev.infra.yaml \
--env-file "$home"/infrastructure/.env \
up --force-recreate -d


#sudo chown -R "$USER":"$USER" "$home"/infrastructure/db-volumes/auth
#sudo chmod -R 777 "$home"/infrastructure/db-volumes/auth