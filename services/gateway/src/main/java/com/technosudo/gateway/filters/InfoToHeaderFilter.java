package com.technosudo.gateway.filters;


import org.springframework.cloud.gateway.filter.GatewayFilterChain;
import org.springframework.cloud.gateway.filter.GlobalFilter;
import org.springframework.http.server.reactive.ServerHttpRequest;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.ReactiveSecurityContextHolder;
import org.springframework.security.core.context.SecurityContext;
import org.springframework.security.oauth2.client.authentication.OAuth2AuthenticationToken;
import org.springframework.security.oauth2.core.oidc.user.OidcUser;
import org.springframework.security.oauth2.core.user.OAuth2User;
import org.springframework.security.oauth2.server.resource.authentication.JwtAuthenticationToken;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

@Component
public class InfoToHeaderFilter implements GlobalFilter {

    @Override
    public Mono<Void> filter(ServerWebExchange exchange, GatewayFilterChain chain) {
        return ReactiveSecurityContextHolder.getContext()
                .map(SecurityContext::getAuthentication)
                .flatMap(authentication -> {
                    if (authentication instanceof OAuth2AuthenticationToken token) {
                        var oidcUser = (OidcUser) token.getPrincipal();

                        String username = oidcUser.getPreferredUsername(); // or oidcUser.getName()
                        String email = oidcUser.getEmail(); // available if email scope is enabled
                        String uuid = oidcUser.getSubject(); // this is the "sub" claim — the UUID

                        ServerHttpRequest mutatedRequest = exchange.getRequest().mutate()
                                .header("X-Username", username)
                                .header("X-Email", email)
                                .header("X-Uuid", uuid)
                                .build();

                        return chain.filter(exchange.mutate().request(mutatedRequest).build());
                    } else {
                        return chain.filter(exchange); // fallback
                    }
                })
                .switchIfEmpty(chain.filter(exchange));
    }

    private String extractUsername(Authentication authentication) {
        Object principal = authentication.getPrincipal();
        if (principal instanceof OAuth2User) {
            return ((OAuth2User) principal).getAttribute("preferred_username");
        } else {
            return authentication.getName(); // Fallback to name if not OAuth2User
        }
    }
}