#pragma checksum "C:\Users\Radi\Documents\Projekte\Softwareprojekt\tutorium-c-team-11\Abgaben\Einzelabgaben\Achkik\Blatt03\Views\Booking\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "bd1389603122f15f6491b460bd35e7d49e3f7ddd"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Booking_Index), @"mvc.1.0.view", @"/Views/Booking/Index.cshtml")]
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
#line 1 "C:\Users\Radi\Documents\Projekte\Softwareprojekt\tutorium-c-team-11\Abgaben\Einzelabgaben\Achkik\Blatt03\Views\_ViewImports.cshtml"
using Softwareprojekt;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Radi\Documents\Projekte\Softwareprojekt\tutorium-c-team-11\Abgaben\Einzelabgaben\Achkik\Blatt03\Views\_ViewImports.cshtml"
using Softwareprojekt.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"bd1389603122f15f6491b460bd35e7d49e3f7ddd", @"/Views/Booking/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2ef050e7f06deede050415797d5757ea62e38271", @"/Views/_ViewImports.cshtml")]
    public class Views_Booking_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 2 "C:\Users\Radi\Documents\Projekte\Softwareprojekt\tutorium-c-team-11\Abgaben\Einzelabgaben\Achkik\Blatt03\Views\Booking\Index.cshtml"
  
    ViewData["Title"] = "Index";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<h1>Index</h1>

<table class=""table table-striped table-borderless"" id=""bookingsTable"">
    <thead>
        <tr>
            <th scope=""col"">State of Charge</th>
            <th scope=""col"">Needed Distance</th>
            <th scope=""col"">Start</th>
            <th scope=""col"">End</th>
        </tr>
    </thead>
    <tbody>
");
#nullable restore
#line 18 "C:\Users\Radi\Documents\Projekte\Softwareprojekt\tutorium-c-team-11\Abgaben\Einzelabgaben\Achkik\Blatt03\Views\Booking\Index.cshtml"
         foreach (Booking b in ViewData["bookings"] as List<Booking>)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <tr>\r\n            <td>");
#nullable restore
#line 21 "C:\Users\Radi\Documents\Projekte\Softwareprojekt\tutorium-c-team-11\Abgaben\Einzelabgaben\Achkik\Blatt03\Views\Booking\Index.cshtml"
           Write(b.SoC);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            <td>");
#nullable restore
#line 22 "C:\Users\Radi\Documents\Projekte\Softwareprojekt\tutorium-c-team-11\Abgaben\Einzelabgaben\Achkik\Blatt03\Views\Booking\Index.cshtml"
           Write(b.NeededDistance);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            <td>");
#nullable restore
#line 23 "C:\Users\Radi\Documents\Projekte\Softwareprojekt\tutorium-c-team-11\Abgaben\Einzelabgaben\Achkik\Blatt03\Views\Booking\Index.cshtml"
           Write(b.Start);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            <td>");
#nullable restore
#line 24 "C:\Users\Radi\Documents\Projekte\Softwareprojekt\tutorium-c-team-11\Abgaben\Einzelabgaben\Achkik\Blatt03\Views\Booking\Index.cshtml"
           Write(b.End);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n        </tr>\r\n");
#nullable restore
#line 26 "C:\Users\Radi\Documents\Projekte\Softwareprojekt\tutorium-c-team-11\Abgaben\Einzelabgaben\Achkik\Blatt03\Views\Booking\Index.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </tbody>\r\n</table>\r\n\r\n<style>\r\n    #bookingsTable tr:hover, #bookingsTable tr:focus {\r\n        text-decoration: underline;\r\n    }\r\n</style>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591