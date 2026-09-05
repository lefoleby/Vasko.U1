using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vasko.API.Data;
using Vasko.U1.Domain.Entities;
using Vasko.U1.Domain.Models;

namespace Vasko.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;  // ← добавлено

        public DishesController(AppDbContext context, IWebHostEnvironment env)  // ← добавлен env
        {
            _context = context;
            _env = env;  // ← добавлено
        }

        // GET: api/dishes?category=soups&pageNo=1&pageSize=3
        [HttpGet]
        public async Task<ActionResult<ResponseData<ListModel<Dish>>>> GetDishes(
            string? category,
            int pageNo = 1,
            int pageSize = 3)
        {
            var query = _context.Dishes
                .Include(d => d.Category)
                .Where(d => string.IsNullOrEmpty(category) || d.Category != null && d.Category.NormalizedName == category)
                .AsNoTracking();

            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (pageNo > totalPages && totalPages > 0)
                pageNo = totalPages;
            if (pageNo < 1)
                pageNo = 1;

            var items = await query
                .OrderBy(d => d.Id)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new ResponseData<ListModel<Dish>>
            {
                Data = new ListModel<Dish>
                {
                    Items = items,
                    CurrentPage = pageNo,
                    TotalPages = totalPages
                },
                Success = true
            };

            if (totalCount == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Нет объектов в выбранной категории";
            }

            return result;
        }

        // GET: api/dishes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseData<Dish>>> GetDish(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            var result = new ResponseData<Dish>();

            if (dish == null)
            {
                result.Success = false;
                result.ErrorMessage = "Объект не найден";
                return NotFound(result);
            }

            result.Data = dish;
            result.Success = true;
            return result;
        }

        // POST: api/dishes
        [HttpPost]
        public async Task<ActionResult<Dish>> PostDish(Dish dish)
        {
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDish), new { id = dish.Id }, dish);
        }

        // PUT: api/dishes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDish(int id, Dish dish)
        {
            if (id != dish.Id)
            {
                return BadRequest();
            }

            _context.Entry(dish).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DishExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/dishes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound();
            }

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/dishes/{id} — сохранение изображения
        [HttpPost("{id}")]
        public async Task<IActionResult> SaveImage(int id, IFormFile image)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound();
            }

            var imagesPath = Path.Combine(_env.WebRootPath, "Images");
            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            var randomName = Path.GetRandomFileName();
            var extension = Path.GetExtension(image.FileName);
            var fileName = Path.ChangeExtension(randomName, extension);
            var filePath = Path.Combine(imagesPath, fileName);

            using (var stream = System.IO.File.OpenWrite(filePath))
            {
                await image.CopyToAsync(stream);
            }

            var host = $"https://{Request.Host}";
            var url = $"{host}/Images/{fileName}";

            dish.Image = url;
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.Id == id);
        }
    }
}