using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using Vasko.Models;

namespace Vasko.Controllers
{
    public class HomeController : Controller
    {
        //[Authorize(Policy = "admin")]
        public IActionResult Index()
        {
            
            ViewData["text"] = "Лабораторная работа №2";
            // Создаём список объектов ListDemo
            List<ListDemo> _listData = new List<ListDemo>
            {
                new ListDemo { Id = 1, Name = "Item 1" },
                new ListDemo { Id = 2, Name = "Item 2" },
                new ListDemo { Id = 3, Name = "Item 3" }
            };

            // Создаём SelectList: источник, поле для Value, поле для Text
            SelectList data = new SelectList(_listData, "Id", "Name");

            // Передаём SelectList как модель в представление
            return View(data);
        }
    }
}