#!/bin/bash
# Example usage:
# TODO

# Initialize flags
rebuild=true
soft_rebuild=false
pkl=false
run_linter=false
quiet=false

# Set env variables

set -ex

# Parse command-line arguments
while [[ "$#" -gt 0 ]]; do
    case $1 in
        --rebuild) rebuild=true ;;
		--soft-rebuild) soft_rebuild=true  ;;
		--sr) soft_rebuild=true  ;;
		--lint) run_linter=true ;;
		--pkl) pkl=true ;;
		--quiet) quiet=true ;;
        *) echo "Unknown option: $1" ;;
    esac
    shift
done



run_compose() {
    cd infrastructure
    if $rebuild; then
        docker compose -f compose.yaml up -d --force-recreate
    elif $soft_rebuild; then # don't restart db
        docker compose up -d --no-deps --force-recreate keycloak
        docker compose up -d --no-deps --force-recreate keycloak-config-cli

    fi

    cd ..

    watch -n 1 docker compose -f infrastructure/compose.yaml ps
}






run_compose

set +ex
