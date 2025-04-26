# FROM liquibase/liquibase
# ADD changelog /liquibase/changelog
# RUN pwd && ls && ls -la /liquibase/changelog
# USER root
# WORKDIR /liquibase/changelog
FROM liquibase/liquibase
ADD changelog /liquibase/changelog
WORKDIR /liquibase/changelog

CMD ["liquibase", "--url=jdbc:postgresql://wrc-db:5432/wrc", "--username=admin", "--password=admin", "--changeLogFile=db.changelog.yaml", "update"]
