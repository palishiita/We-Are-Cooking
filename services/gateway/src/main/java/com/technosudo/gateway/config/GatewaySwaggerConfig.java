package com.technosudo.gateway.config;

import org.springdoc.core.models.GroupedOpenApi;
import org.springdoc.core.properties.AbstractSwaggerUiConfigProperties;
import org.springdoc.core.properties.SwaggerUiConfigParameters;
import org.springframework.cloud.gateway.route.RouteDefinition;
import org.springframework.cloud.gateway.route.RouteDefinitionLocator;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Lazy;
import org.springframework.http.server.reactive.ServerHttpRequest;
import org.springframework.web.bind.annotation.GetMapping;
import reactor.core.publisher.Flux;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.*;

@Configuration
public class GatewaySwaggerConfig {

    @Bean
    @Lazy(false) // Ensures this bean is created early during startup
    public GroupedOpenApi swaggerUrls(
            RouteDefinitionLocator locator,
            SwaggerUiConfigParameters swaggerUiConfigParameters) {

        Set<SwaggerUiConfigParameters.SwaggerUrl> urls = new HashSet<>();

        Flux<RouteDefinition> routeDefinitions = locator.getRouteDefinitions();
        routeDefinitions.collectList().block().stream()
                .filter(routeDefinition -> routeDefinition.getId().startsWith("test") ||
                        routeDefinition.getId().startsWith("userinfo") ||
                        routeDefinition.getId().startsWith("recipes") ||
                        routeDefinition.getId().startsWith("reels"))
                .forEach(routeDefinition -> {
                    String serviceName = routeDefinition.getId();

                    String gatewayPathPrefix = routeDefinition.getPredicates().stream()
                            .filter(p -> p.getName().equals("Path"))
                            .map(p -> p.getArgs().get("_genkey_0").replace("/**", ""))
                            .findFirst()
                            .orElse(null);

//                    if (gatewayPathPrefix != null) {
//                        String openApiDocsUrl = gatewayPathPrefix + "/v3/api-docs";
//                        urls.add(new SwaggerUiConfigParameters.SwaggerUrl(serviceName, openApiDocsUrl, serviceName));
//                    }
                });

        swaggerUiConfigParameters.setUrls(urls);
        return GroupedOpenApi.builder()
                .group("api-gateway")
                .pathsToMatch("/api/**")
                .build();
    }
}
