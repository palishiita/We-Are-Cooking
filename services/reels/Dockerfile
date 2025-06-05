FROM ubuntu:25.04 AS base

RUN apt update && apt install -y libssl-dev ca-certificates && rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 7000

FROM rust:1.86.0-slim AS build
RUN apt update && apt install -y lld clang pkg-config libssl-dev curl

WORKDIR /src


COPY Cargo.toml Cargo.lock ./

RUN mkdir src && echo "fn main() {}" > src/main.rs

RUN CARGO_TERM_COLOR=always cargo build --release --verbose

RUN rm -rf src/

COPY . .

RUN mkdir -p /app/upload/

RUN CARGO_TERM_COLOR=always cargo build --release --verbose

FROM base AS final
RUN mkdir -p /app/upload
COPY config.release.yaml /app/config.yaml
COPY --from=build /src/target/release/reels_microservice /app/app

ENV RUST_BACKTRACE=1
ENTRYPOINT [ "./app" ]



