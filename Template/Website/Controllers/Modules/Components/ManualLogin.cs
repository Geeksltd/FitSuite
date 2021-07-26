using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Olive;
using Olive.Mvc;
using PeopleService;
using vm = ViewModel;

namespace ViewComponents
{

    public partial class ManualLogin : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(vm.ManualLogin info)
        {
            return View(await Bind<vm.ManualLogin>(info));
        }
    }
}

namespace Controllers
{

#pragma warning disable
    public partial class ManualLoginController : BaseController
    {
        [HttpPost("ManualLogin/SimulateLogIn")]
        public async Task<ActionResult> SimulateLogIn(vm.ManualLogin info)
        {
            if (!(info.IsVisible))
                return new UnauthorizedResult();
            info.Item = await Database.FirstOrDefault<PeopleService.UserInfo>(x => x.Email == info.Email);

            info.Item.Roles = info.RoleNames;

            await info.Item.LogOn();

            return Redirect(Request.Param("returnUrl"));
        }

        [NonAction, OnPreBinding]
        public async Task OnPreBinding(vm.ManualLogin info)
        {
            if (Request.IsGet())
            {
                // Set default roles
                info.RoleNames = Config.Get("Authentication:SimulateLogin:Roles");
            }
        }

        [NonAction, OnBound]
        public async Task OnBound(vm.ManualLogin info)
        {
            info.Item = info.Item ?? new PeopleService.UserInfo();

            info.IsVisible = info.AllowManual;

            if (Request.IsGet()) await info.Item.CopyDataTo(info);

            info.DisplayName = Config.Get("Authentication:SimulateLogin:DisplayName");

            info.DisplayName_Visible = info.IsVisible;
            info.Email_Visible = info.IsVisible;
            info.RoleNames_Visible = info.IsVisible;

            TryValidateModel(info);

            if (Request.IsGet())
            {
                // Set email
                info.Email = Config.Get("Authentication:SimulateLogin:Email");
            }
        }
    }
}

namespace ViewModel
{
#pragma warning disable
    [BindingController(typeof(Controllers.ManualLoginController))]
    public partial class ManualLogin : IViewModel
    {
        [ReadOnly(true)]
        public bool IsVisible { get; set; }

        public bool AllowManual => Config.Get("Authentication:AllowManual", defaultValue: false);

        [ValidateNever]
        public UserInfo Item { get; set; }

        [CustomBound]
        public string DisplayName { get; set; }

        [StringLengthWhen(nameof(Email_Visible), 100, ErrorMessage = "Email should not exceed 100 characters.")]
        public string Email { get; set; }

        public string RoleNames { get; set; }

        [ReadOnly(true)]
        public bool DisplayName_Visible { get; set; }

        [ReadOnly(true)]
        public bool Email_Visible { get; set; }

        [ReadOnly(true)]
        public bool RoleNames_Visible { get; set; }
    }
}