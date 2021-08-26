using ChartTest.Data;
using ChartTest.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartTest.Controllers
{
    public class ChartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Chart()
        {
            return View();
        }

        public ContentResult AjaxMethod([FromBody] Country country)
        {
            var orders = _context.Order
                            .Where(x => x.ShipCountry == country.Name)
                            .GroupBy(x => x.ShipCity)
                            .Select(g => new
                            {
                                City = g.Key,
                                Count = g.Count()
                            });
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (var order in orders)
            {
                sb.Append("{");
                System.Threading.Thread.Sleep(50);
                string color = String.Format("#{0:X6}", new Random().Next(0x1000000));
                sb.Append(string.Format("text :'{0}', value:{1}, color: '{2}'", order.City, order.Count, color));
                sb.Append("},");
            }
            sb = sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            return Content(sb.ToString());
        }
        public IActionResult BarChart()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            var names = _context.Employee.Select(x => x.Manager).ToList();
            var numbers = _context.Employee.Select(x => x.Number).ToList();

            //dataPoints.AddRange(from name in names
            //                    from number in numbers
            //                    select new DataPoint(name, number));

            var dictionary = names.Zip(numbers, (k, v) => new { names = k, numbers = v })
                     .ToDictionary(x => x.names, x => x.numbers);
            var count = names.Count();


            foreach (var name in names)
            {
                foreach (var number in numbers)
                {
                    if (names.IndexOf(name) == numbers.IndexOf(number))
                    {
                        dataPoints.Add(new DataPoint(name, number));
                    }                                    
                }
            }





            //foreach (var pair in names.Zip(numbers, (a, b) => new { A = a, B = b }))
            //{
            //    dataPoints.Add(new DataPoint(pair.A, pair.B));
            //}



            //dataPoints.Add(new DataPoint("Albert", 10));
            //dataPoints.Add(new DataPoint("Tim", 30));
            //dataPoints.Add(new DataPoint("Wilson", 17));
            //dataPoints.Add(new DataPoint("Joseph", 39));
            //dataPoints.Add(new DataPoint("Robert", 30));
            //dataPoints.Add(new DataPoint("Sophia", 25));
            //dataPoints.Add(new DataPoint("Emma", 15));

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            return View();
        }
    }
}
