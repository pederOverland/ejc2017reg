using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImageSharp;
using ImageSharp.Processing;
using System.IO;
using ecreg.Data;
using OfficeOpenXml;

namespace ecreg.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly EcRegDb _db;
        public ApiController(EcRegDb db)
        {
            _db = db;
        }
        [HttpGet("picture/{name}/{size}")]
        public IActionResult Picture(string name, int size, [FromQuery] bool greyscale = true)
        {
            try
            {
                var image = Image.Load(@"wwwroot/profiles/" + name);
                image.Resize(new ResizeOptions { Size = new Size(size, size), Mode = ResizeMode.Max });
                if (greyscale)
                {
                    image.Grayscale();
                }
                var stream = new MemoryStream();
                image.SaveAsPng(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return this.File(stream, "image/png");
            }
            catch
            {
                return this.NotFound();
            }
        }
        [HttpGet("contestants")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        public IActionResult Contestants([FromQuery] string pw)
        {
            var contestants = _db.Contestants.ToList();
            var stream = new MemoryStream();
            using (var pkg = new ExcelPackage())
            {
                var cells = pkg.Workbook.Worksheets.Add("Contestants").Cells;
                cells["A1"].Value = "Id";
                cells["B1"].Value = "Nation";
                cells["C1"].Value = "Role";
                cells["D1"].Value = "Name";
                cells["E1"].Value = "Passport #";
                cells["F1"].Value = "BirthDate";
                cells["G1"].Value = "Image";
                cells["H1"].Value = "Last Changed";
                var line = 2;
                contestants.ForEach(c =>
                {
                    cells["A" + line].Value = c.ContestantId;
                    cells["B" + line].Value = c.Nation;
                    cells["C" + line].Value = c.Role;
                    cells["D" + line].Value = c.Name;
                    cells["E" + line].Value = c.PassportNumber;
                    cells["F" + line].Value = c.BirthDate.ToString("dd/MM/yyyy");
                    cells["G" + line].Value = "http://registration.ejc2017bergen.no/profiles/" + c.ContestantId + "_" + c.Nation + ".jpg";
                    cells["H" + line].Value = c.Modified.ToString();
                    line++;
                });
                pkg.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return this.File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "contestants.xlsx");
            }
        }
        [HttpGet("contestants.csv")]
        public IActionResult ContestantsToCSV()
        {
            var contestants = _db.Contestants.ToList();
            var response = "Id;Nation;Role;Name;PN;BD;Image\n";
            contestants.ForEach(c =>
            {
                response += String.Format("{0};{1};{2};{3};{4};{5};{6}\n",
            c.ContestantId,
            c.Nation, c.Role, c.Name, c.PassportNumber, c.BirthDate.ToString("dd/MM/yyyy"), "http://registration.ejc2017bergen.no/profiles/" + c.ContestantId + "_" + c.Nation + ".jpg");
            });
            return this.Ok(response);
        }
    }

}