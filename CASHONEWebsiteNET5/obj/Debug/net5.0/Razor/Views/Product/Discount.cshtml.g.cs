#pragma checksum "D:\projects\softwares\pos\PointOfSale\CASHONEWebsiteNET5\Views\Product\Discount.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "69911e5b572194cb0e49276b1ca2498744a39c78"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Product_Discount), @"mvc.1.0.view", @"/Views/Product/Discount.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\projects\softwares\pos\PointOfSale\CASHONEWebsiteNET5\Views\_ViewImports.cshtml"
using Application;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\projects\softwares\pos\PointOfSale\CASHONEWebsiteNET5\Views\_ViewImports.cshtml"
using Application.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"69911e5b572194cb0e49276b1ca2498744a39c78", @"/Views/Product/Discount.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"03ea476b5db410d4405042b0a5ed991b4e6710c5", @"/Views/_ViewImports.cshtml")]
    public class Views_Product_Discount : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Application.Models.DiscountViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\projects\softwares\pos\PointOfSale\CASHONEWebsiteNET5\Views\Product\Discount.cshtml"
  
    ViewData["Title"] = "Discounted Products Page";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            DefineSection("ColumnContentTitle", async() => {
                WriteLiteral("\r\n    <h1 class=\"my-4\">Discount</h1>\r\n");
            }
            );
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Application.Models.DiscountViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
