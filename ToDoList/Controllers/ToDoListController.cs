using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Globalization;
using ToDoListAPI.Data;
using ToDoListAPI.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ToDoListAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoListController : ControllerBase
    {
        private readonly ToDoListDBContext _context;

        public ToDoListController(ToDoListDBContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<ToDoList>> Get() => await _context.ToDoList.ToListAsync();

        [HttpGet("id")]
        [ProducesResponseType(typeof(ToDoList), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getByID(int id)
        {
            var toDoList = await _context.ToDoList.FindAsync(id);

            return toDoList == null ? NotFound() : Ok(toDoList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> create (ToDoList toDoList)
        {
            await _context.ToDoList.AddAsync(toDoList);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(getByID), new {id = toDoList.id}, toDoList);
        }

        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> update(ToDoList toDoList)
        {
            _context.Entry(toDoList).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> delete(int id)
        {
            var toDoList = await _context.ToDoList.FindAsync(id);

            if (toDoList == null)
            {
                return NotFound();
            }

            _context.ToDoList.Remove(toDoList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("sorting")]
        [ProducesResponseType(typeof(ToDoList), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IEnumerable<ToDoList>> sort(string sorting)
        {
            var toDoList = from t in _context.ToDoList
                           select t;

            if (sorting == "Date Asc")
            {
                toDoList = toDoList.OrderBy(t => t.dueDate);
            }
            else if (sorting == "Date Des")
            {
                toDoList = toDoList.OrderByDescending(t => t.dueDate);
            }
            else if (sorting == "Status Asc")
            {
                toDoList = toDoList.OrderBy(t => t.status);
            }
            else if (sorting == "Status Des")
            {
                toDoList = toDoList.OrderByDescending(t => t.status);
            }
            else if (sorting == "Name Asc")
            {
                toDoList = toDoList.OrderBy(t => t.toDoName);
            }
            else if (sorting == "Name Des")
            {
                toDoList = toDoList.OrderByDescending(s => s.toDoName);
            }

            return await toDoList.AsNoTracking().ToListAsync();
        }

        [HttpGet("status")]
        [ProducesResponseType(typeof(ToDoList), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IEnumerable<ToDoList>> filter(string status)
        {
            var toDoList = from t in _context.ToDoList
                           select t;

            if (!string.IsNullOrEmpty(status))
            {
                toDoList = toDoList.Where(t => t.status.ToUpper().Contains(status.ToUpper()));
            }

            return await toDoList.AsNoTracking().ToListAsync();
        }

        [HttpGet("fromDate, toDate")]
        [ProducesResponseType(typeof(ToDoList), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IEnumerable<ToDoList>> filter(string fromDate, string toDate)
        {
            var toDoList = from t in _context.ToDoList
                           select t;

            DateTime from, to;

            if (DateTime.TryParse(fromDate, out from) && DateTime.TryParse(toDate, out to))
            {
                toDoList = toDoList.Where(t => t.dueDate >= from && t.dueDate <= to);
            }

            return await toDoList.AsNoTracking().ToListAsync();
        }
    }
}
