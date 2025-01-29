using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lab12.Data;
using Lab12.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Lab12.Controllers
{
    [ApiController]
    [Route("api/articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly StoreDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public ArticlesController(StoreDBContext context, UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                };

                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])); // Pobierz klucz z konfiguracji
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { Token = jwt });
            }

            return Unauthorized("Invalid username or password");
        }

        [HttpGet("articles")]
        public ActionResult<IEnumerable<Article>> GetArticles()
        {
            var articles = _context.Articles.ToList();
            return Ok(articles);
        }

        [HttpGet("articlesPage")]
        public ActionResult<IEnumerable<object>> GetArticles([FromQuery] int offset = 0, [FromQuery] int limit = 10)
        {
            var articles = _context.Articles
                .Skip(offset)
                .Take(limit)
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Price,
                    a.ImagePath,
                    CategoryName = _context.Categories.FirstOrDefault(c => c.Id == a.CategoryId).Name
                })
                .ToList();

            return Ok(articles);
        }


        [HttpGet("articles/{id}")]
        public ActionResult<Article> GetArticle(int id)
        {
            var article = _context.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
                return NotFound();

            return Ok(article);
        }

        //[Authorize(Policy = "CanManageArticles")]
        [HttpPost("articles")]
        public ActionResult AddArticle([FromBody] Article newArticle)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Articles.Add(newArticle);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetArticle), new { id = newArticle.Id }, newArticle);
        }

        //[Authorize(Policy = "CanManageArticles")]
        [HttpPut("articles/{id}")]
        public ActionResult UpdateArticle(int id, [FromBody] Article updatedArticle)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var article = _context.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
                return NotFound();

            article.Name = updatedArticle.Name;
            article.Price = updatedArticle.Price;
            article.ImagePath = updatedArticle.ImagePath;
            article.CategoryId = updatedArticle.CategoryId;

            _context.SaveChanges();
            return NoContent();
        }

        //[Authorize(Policy = "CanManageArticles")]
        [HttpDelete("articles/{id}")]
        public ActionResult DeleteArticle(int id)
        {
            var article = _context.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
                return NotFound();

            _context.Articles.Remove(article);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("categories")]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            var categories = _context.Categories.ToList();
            return Ok(categories);
        }

        [HttpGet("categories/{id}")]
        public ActionResult<Category> GetCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        //[Authorize(Policy = "CanManageCategories")]
        [HttpPost("categories")]
        public ActionResult AddCategory([FromBody] Category newCategory)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetCategory), new { id = newCategory.Id }, newCategory);
        }

        //[Authorize(Policy = "CanManageCategories")]
        [HttpPut("categories/{id}")]
        public ActionResult UpdateCategory(int id, [FromBody] Category updatedCategory)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            category.Name = updatedCategory.Name;

            _context.SaveChanges();
            return NoContent();
        }

        //[Authorize(Policy = "CanManageCategories")]
        [HttpDelete("categories/{id}")]
        public ActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
