FROM maven:3.9.9-eclipse-temurin-23-alpine AS builder
LABEL authors="technosudo"

WORKDIR /app
ENV MAVEN_OPTS="-XX:+TieredCompilation -XX:TieredStopAtLevel=1"

COPY services/test/pom.xml .
RUN mvn dependency:go-offline

COPY services/test/src ./src
RUN mvn clean install -Dmaven.test.skip=true -X


FROM openjdk:23-slim

WORKDIR /app

COPY --from=builder /app/target/*.jar /app/app.jar

CMD java -jar /app/app.jar
