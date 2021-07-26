using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Olive;
using Olive.Microservices.Hub;
using Olive.Mvc;

namespace Controllers
{
    public class LoginController : BaseLoginController
    {
        Microsoft.AspNetCore.Hosting.IWebHostEnvironment Environment;
        public LoginController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment) : base(environment)
        {
            Environment = environment;
        }
        public override async Task<IActionResult> OnLoggedOut()
        {
            await HttpContext.SignOutAsync();
            return Redirect(Microservice.Of("Dashboard").Url("/login/logout.aspx"));
        }

        [HttpPost("LoginForm/Login")]
        public Task<ActionResult> Login(ViewModel.LoginForm info)
        {
            return LoginWithEmail(info);
        }
        [Route("login/{item:Guid?}")]
        public async Task<ActionResult> Index(ViewModel.LoginForm loginForm)
        {
            if (Request.Param("returnUrl").IsEmpty())
            {
                return Redirect(Url.Index("Login", new { ReturnUrl = "/" }));
            }

            // Remove initial validation messages as well as unintended injected data
            ModelState.Clear();

            //ViewBag.ManualLogin = manualLogin;

            return View(loginForm);
        }
        [NonAction, OnBound]
        public async Task OnBound(ViewModel.LoginForm info)
        {
            info.Item = info.Item ?? new User();

            // Clear cookies
            var alreadyDead = new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = LocalTime.Today.AddDays(-1)
            };

            foreach (var c in Request.Cookies)
                Response.Cookies.Append(c.Key, string.Empty, alreadyDead);

            if (Request.IsGet()) await info.Item.CopyDataTo(info);
        }
        #region Custom
        async Task TryLogin(string email)
        {
            var user = await Database.FirstOrDefault<PeopleService.UserInfo>(p => p.Email == email);

            if (user == null)
            {
                Log.Error(new Exception("Null user!"), "****** User is null for email " + email);

                throw new Exception(@"<li>Google did not supply us your email address (due to security restrictions you have set with them).</li>
                        <li>The email address you logged in with [to Google] is not registered in our database.</li>");
            }

            if (!user.IsActive)
                throw new Exception("<li>Your account is currently deactivated. It might be due to security concerns on your account. Please contact the system administrator to resolve this issue. We apologise for the inconvenience.</li>");

            await user.LogOn();
        }

        [HttpGet, Route("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            try
            {
                if (remoteError.HasValue())
                    return await Error($"Error from external provider: {remoteError}");

                var info = await HttpContext.AuthenticateAsync();

                if (info == null || !info.Succeeded)
                    return Redirect("/login");

                var email = info.Principal.GetEmail();

                if (email.IsEmpty())
                {
                    return await Error("Google did not return your email to us.");
                }

                try
                {
                    var user = await Database.FirstOrDefault<PeopleService.UserInfo>(f => f.Email == email);
                    if (user == null) return await Error("Unregistered email: " + email);
                    if (!user.IsActive) return await Error("Inactive account: " + email);

                    await TryLogin(email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("********************* ERROR: " + email);
                    return await Error(ex.Message);
                }

                return Redirect("/SSO");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }



        async Task<ActionResult> Error(string message)
        {
            // throw new Exception();
            //var manual = new vm.ManualLogin();
            //await TryUpdateModelAsync(manual);

            var login = new ViewModel.LoginForm { ErrorMessage = message };
            await TryUpdateModelAsync(login);

            return await Login(login);
        }

        async Task<ActionResult> LoginWithEmail(ViewModel.LoginForm info)
        {
            if (info.Email.IsEmpty())
                return await Error("Please type your email.");

            var user = await Database.FirstOrDefault<PeopleService.UserInfo>(f => f.Email == info.Email);
            if (user == null) return await Error("Unregistered email: " + info.Email);
            if (!user.IsActive) return await Error("Account is not active: " + info.Email);

            await Database.DeleteAll<EmailClaim>(x => x.Email == user.Email);
            var newClaim = await Database.Save(new EmailClaim { Email = user.Email, Token = Guid.NewGuid().ToString().Remove("-") });

            var loginUrl = Request.RootUrl() + "claim/" + newClaim.Token;

            var email = new EmailService.SendEmailCommand
            {
                To = info.Email,
                Html = true,
                Subject = "Login link - valid until " + LocalTime.Now.AddMinutes(10).ToString("HH:mm"),
                Body = "Dear user,<br/><br/>" +
                "The following one-time-use link will log into Geeks Hub:<br>" +
                $"<a href='{loginUrl}'>{loginUrl}</a><br/><br/><hr/><br/>" +
              "If you did not request this, please ignore this email.\n" +
             "For any questions, please contact Geeks Ltd at support@geeks.ltd."
            };

            try
            {
                await EventBus.Queue<EmailService.SendEmailCommand>().Publish(email);
                await "https://email.app.geeks.ltd/flush".AsUri().Download();
                return Content("Please check your email.");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Content("There is an issue in the system. Please try again later, or contact support@geeks.ltd.uk.");
            }
        }

        [HttpGet("claim/{token}")]
        public async Task<IActionResult> Claim(string token)
        {
            var claim = await EmailClaim.FindByToken(token);

            if (claim == null)
                return Content("This link is no longer valid.");

            if (claim.Created < LocalTime.UtcNow.AddMinutes(-10))
                return Content("This link expired at " + claim.Created.AddMinutes(10).ToString("MMM dd @ HH:mm:ss"));

            await Database.Delete(claim);
            await TryLogin(claim.Email);
            return Redirect("/SSO");
        }
        #endregion

    }
}
