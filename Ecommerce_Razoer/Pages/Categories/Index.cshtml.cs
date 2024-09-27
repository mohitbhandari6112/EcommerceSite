using Ecommerce_Razoer.Data;
using Ecommerce_Razoer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce_Razoer.Pages.Categoris
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public List<Category> CategoryList {  get; set; }
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;  
        }

        public void OnGet()
        {
            CategoryList=_context.Categories.ToList();
        }
    
    }
}
