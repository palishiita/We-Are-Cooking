#!/bin/bash

home="$(dirname "$0")"/..

# Initialize flags
full=false
rebuild=false
quiet=false

# Set env variables
set -ex


run() {
  if $full; then sh "$home"/scripts/start-infra.sh; fi
  docker compose -f "$home"/infrastructure/compose.dev.db.yaml up --force-recreate -d
#  sudo chown -R "$USER":"$USER" "$home"/infrastructure/db-volumes/wrc
#  sudo chmod -R 777 "$home"/infrastructure/db-volumes/wrc
  docker compose -f "$home"/infrastructure/compose.dev.services.yaml build wrc-gateway
  docker compose -f "$home"/infrastructure/compose.dev.services.yaml up --force-recreate -d
}

# Parse command-line arguments
while [[ "$#" -gt 0 ]]; do
    case $1 in
        --full) full=true ;;
		    --rebuild) rebuild=true  ;;
		    --quiet) quiet=true ;;
        *) echo "Unknown option: $1" ;;
    esac
    shift
done

run
