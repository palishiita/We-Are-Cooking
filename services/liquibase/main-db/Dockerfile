FROM liquibase/liquibase
ADD changelog /liquibase/changelog
RUN pwd && ls && ls -la /liquibase/changelog
USER root
WORKDIR /liquibase/changelog
CMD ["sh", "-c", "docker-entrypoint.sh --url=${DB_URL} --username=${DB_USERNAME} --password=${DB_PASSWORD} --changeLogFile=db.changelog.yaml update"]
