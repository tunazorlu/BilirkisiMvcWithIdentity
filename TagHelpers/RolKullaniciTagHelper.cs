using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BilirkisiMvc.TagHelpers
{
    [HtmlTargetElement("td", Attributes = "asp-role-users")]
    public class RolKullaniciTagHelper(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager) : TagHelper
    {
        [HtmlAttributeName("asp-role-users")]
        public string RolId { get; set; } = null!;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var userNames = new List<string>();
            var role = await roleManager.FindByIdAsync(RolId);

            if (role != null && role.Name != null)
            {
                var users = userManager.Users.ToList();
                foreach (var user in users)
                {
                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        userNames.Add(user.UserName ?? "");
                    }
                }
                output.Content.SetHtmlContent(userNames.Count == 0 ? "Kullanıcı yok": string.Join(", ", setHtml(userNames)));
            }
        }

        private string setHtml(List<string> userNames)
        {
            var sortedUserNames = userNames.OrderBy(name => name.Substring(0, 1)).ToList(); //Kullanıcı adlarını sıralar

            var html = "<ul>";
            foreach (var userName in sortedUserNames)
            {
                html += $"<li>{userName}</li>";
            }
            html += "</ul>";
            return html;
        }

    }
}

