package com.technosudo.test;

import org.springframework.web.bind.annotation.*;

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

    @GetMapping("/header")
    public Object requestHeader(@RequestHeader Object header) {
        return header;
    }
}
