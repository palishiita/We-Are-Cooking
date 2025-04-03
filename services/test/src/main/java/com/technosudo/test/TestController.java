package com.technosudo.test;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/test")
public class TestController {

    @GetMapping
    public String publicResource() {
        return "public";
    }

    @GetMapping("/private")
    public String privateResource() {
        return "private";
    }

    @GetMapping("/body")
    public Object requestBody(@RequestBody Object body) {
        return body;
    }
}
