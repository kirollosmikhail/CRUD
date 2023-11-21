using Demo.DAL.Models;
using Demo.PL.Helper;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
			_userManager = userManager;
			_signInManager = signInManager;
		}

		#region Register

		// BaseURL/Account/Register
		[HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid) // Server Side Validation
            {
                var User = new ApplicationUser()
                {
                    UserName = model.Email.Split('@')[0],
                    Email = model.Email,
                    IsAgree = model.IsAgree,
                    FName = model.FName,
                    LName = model.LName

                };
                var result = await _userManager.CreateAsync(User, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
		#endregion

		#region Login
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid) // server side validation
			{
				var User = await _userManager.FindByEmailAsync(model.Email);
				if (User is not null)
				{

					var Result = await _userManager.CheckPasswordAsync(User, model.Password);
					if (Result)
					{
						// LOGIN
						var LoginResult = await _signInManager.PasswordSignInAsync(User, model.Password, model.RememberMe, false);
						if (LoginResult.Succeeded)
							return RedirectToAction("Index", "Home");

					}
					else
						ModelState.AddModelError(string.Empty, "Password is Incorrect");

				}
				else
					ModelState.AddModelError(string.Empty, "Email is not Exists");


			}
			return View(model);

		}

		#endregion

		#region SignOut
		public new async Task<IActionResult> SignOut()
		{
			await _signInManager.SignOutAsync();

			return RedirectToAction(nameof(Login));
		}

        #endregion


        // Forget Password
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendEmail(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByEmailAsync(model.Email);
                if (User is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(User);
                    // valid for only one time for this user
                    // https://localhost:5001/Account/ResetPassword?email=kiko766@yahoo.com?Token=asfasdnjklasndjkasb
                    var ResetPasswordLink = Url.Action("ResetPassword", "Account", new {email = User.Email, Token = token},Request.Scheme);
                    // send Email
                    var email = new Email()
                    {
                        Subject = "Reset Password",
                        To = model.Email,
                        Body = ResetPasswordLink
                    };
                    EmailSettings.SendEmail(email);
                    return RedirectToAction(nameof(CheckYourInbox));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email is not Exists");
                }
            }
            return View("ForgetPassword", model);

        }

        public IActionResult CheckYourInbox()
        {
            return View();
        }


        // Reset Password

        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string email = TempData["email"] as string;
                string token = TempData["token"] as string;
                var User = await _userManager.FindByEmailAsync(email);
                var Result = await _userManager.ResetPasswordAsync(User, token, model.NewPassword);
                if (Result.Succeeded)
                   return RedirectToAction(nameof(Login));

                else
                    foreach (var error in Result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

            }
            return View(model);

        }


    }
}
