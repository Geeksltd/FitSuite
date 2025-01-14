﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Olive;
using Olive.Entities;
using Olive.Mvc;
using Olive.Web;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using vm = ViewModel;
using Olive.Microservices.Hub;

namespace ViewComponents
{
    
#pragma warning disable
    public partial class BoardView : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(vm.BoardView info)
        {
            return View(await Bind<vm.BoardView>(info));
        }
    }
}

namespace Controllers
{
    
#pragma warning disable
    public partial class BoardViewController : BaseController
    {
    }
}

namespace ViewModel
{
    
#pragma warning disable
    [BindingController(typeof(Controllers.BoardViewController))]
    public partial class BoardView : IViewModel
    {
        public string FeatureId { get; set; }

        [ValidateNever]
        public Board Item { get; set; }
    }
}