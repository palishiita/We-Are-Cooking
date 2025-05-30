FROM maven:3.9.9-eclipse-temurin-23-alpine AS builder
LABEL authors="technosudo"

WORKDIR /app
ENV MAVEN_OPTS="-XX:+TieredCompilation -XX:TieredStopAtLevel=1"

COPY ./pom.xml .
COPY ./src ./src

RUN --mount=type=cache,target=/root/.m2,rw mvn clean package -DskipTests


FROM openjdk:23-slim

WORKDIR /app

COPY --from=builder /app/target/*.jar app.jar
CMD ["java", "-jar", "app.jar"]
