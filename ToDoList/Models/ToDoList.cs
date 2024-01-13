using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ToDoListAPI.Models
{
    public class ToDoList
    {
        public int id { get; set; }
        [Required]

        public string toDoName { get; set; }
        [Required]

        public string description { get; set; }

        public DateTime dueDate { get; set; }

        public string status { get; set; }

        public string priority { get; set; }

        public string tags { get; set; }
    }
}
