FROM ubuntu:25.04 AS base

RUN apt update && apt install -y libssl-dev ca-certificates && rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 8000

FROM rust:1.86.0 AS build
RUN apt update && apt install -y lld clang pkg-config libssl-dev

WORKDIR /src

COPY . .

RUN cargo build --release

FROM base AS final
COPY config.release.yaml /app/config.yaml
COPY --from=build /src/target/release/reels_microservice /app/app

ENV RUST_BACKTRACE=1
ENTRYPOINT [ "./app" ]



