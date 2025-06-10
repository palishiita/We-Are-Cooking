<#import "template.ftl" as layout>
<@layout.registrationLayout; section>
    <#if section = "header">
        ${msg("registerTitle")}
    <#elseif section = "form">
        <form id="kc-register-form" class="${properties.kcFormClass!}" action="${url.registrationAction}" method="post">
            <#if !realm.registrationEmailAsUsername>
            <div class="${properties.kcFormGroupClass!} ${messagesPerField.printIfExists('username',properties.kcFormGroupErrorClass!)}">

                <div class="${properties.kcInputWrapperClass!}">
                    <input type="text" id="username" placeholder="${msg('username')}" class="${properties.kcInputClass!}" name="username" value="${(register.formData.username!'')}" autocomplete="username" />
                </div>
            </div>
            </#if>
            
            
            <div class="${properties.kcFormGroupClass!} ${messagesPerField.printIfExists('email',properties.kcFormGroupErrorClass!)}">

                <div class="${properties.kcInputWrapperClass!}">
                    <input type="text" id="email" placeholder="${msg('email')}" class="${properties.kcInputClass!}" name="email" value="${(register.formData.email!'')}" autocomplete="email" />
                </div>
            </div>

          

            <#if passwordRequired>
            <div class="${properties.kcFormGroupClass!} ${messagesPerField.printIfExists('password',properties.kcFormGroupErrorClass!)}">

                <div class="${properties.kcInputWrapperClass!}">
                    <input type="password" id="password" placeholder="${msg('password')}" class="${properties.kcInputClass!}" name="password" autocomplete="new-password"/>
                </div>
            </div>

            <div class="${properties.kcFormGroupClass!} ${messagesPerField.printIfExists('password-confirm',properties.kcFormGroupErrorClass!)}">

                <div class="${properties.kcInputWrapperClass!}">
                    <input type="password" id="password-confirm" placeholder="${msg('confirmPassword')}" class="${properties.kcInputClass!}" name="password-confirm" />
                </div>
            </div>
            </#if>

            <#if recaptchaRequired??>
            <div class="form-group">
                <div class="${properties.kcInputWrapperClass!}">
                    <div class="g-recaptcha" data-size="compact" data-sitekey="${recaptchaSiteKey}"></div>
                </div>
            </div>
            </#if>

            <div class='privacy-policy'>
                <label class='policy-label'>
                    <input style="margin-bottom: 8px;" type="checkbox" id="exl-agree-tos" name="exl-agree-tos" required />
                    <span> I agree to the <a class="get_me_out" href="https://exlinc.com/legal#privacy-statement" rel="noopener" target="_blank">privacy policy</a> and <a class="get_me_out" href="https://exlinc.com/en/legal/#terms-of-service" rel="noopener" target="_blank">terms of service</a>.</span>
                </label>
            </div>

            <div class="${properties.kcFormGroupClass!}">
                

                <div id="kc-form-buttons" class="${properties.kcFormButtonsClass!}">
                    <input class="${properties.kcButtonClass!} ${properties.kcButtonPrimaryClass!} ${properties.kcButtonBlockClass!} ${properties.kcButtonLargeClass!}" type="submit" value="${msg("doRegister")}"/>
                </div>

                <div id="kc-form-options" class="${properties.kcFormOptionsClass!}">
                    <div class="${properties.kcFormOptionsWrapperClass!}">
                        <span><a class="get_me_out" href="${url.loginUrl}">${msg("backToLogin")?no_esc}</a></span>
                    </div>
                </div>
            </div>
        </form>
    </#if>
</@layout.registrationLayout>
