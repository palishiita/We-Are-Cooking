package com.technosudo.gateway;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class TestController {

    @GetMapping("/test/gateway")
    public String test() {
        return "gateway";
    }
}
