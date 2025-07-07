// Copyright 2025 SonarBeserk
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Pages;

using Htmx;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

public class ExampleModel : PageModel
{
    private readonly ILogger<ExampleModel> _logger;

    // Post Properties
    [BindProperty]
    public string? Name { get; set; }

    public ExampleModel(ILogger<ExampleModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnPost()
    {
        Name ??= "Unknown";

        if (!Request.IsHtmx())
        {
            return Page();
        }

        _logger.LogInformation("OnPost {name}", this.Name);

        Response.Htmx(h =>
        {
            // we want to push the current url
            // into the history
            h.PushUrl(Request.GetEncodedUrl());
        });

        return Partial("Shared/_Greeting", this);
    }
}
