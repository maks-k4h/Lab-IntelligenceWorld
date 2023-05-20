using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Lab2_IntelligenceAgencies.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Lab2_IntelligenceAgencies.Controllers
{
    public class CountriesController : Controller
    {
        private MySqlConnection _connection;

        public CountriesController(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: Countries
        public ActionResult Index()
        {
            _connection.Open();
            if (_connection.State != ConnectionState.Open)
                return NotFound();

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM Countries";

            using var reader = command.ExecuteReader();
            var res = new List<Country>();
            
            while (reader.Read())
            {
                
                res.Add(new Country{Id = (int)reader["Id"], Name = (string)reader["Name"]});
            }

            return View(res);
        }

        // GET: Countries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Country country)
        {
            try
            {
                _connection.Open();

                var command = _connection.CreateCommand();
                command.CommandText = $"INSERT INTO Countries (Name) VALUES (\"{country.Name}\");";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Countries/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT Countries.Name FROM Countries WHERE Id = {id};";
                var reader = command.ExecuteReader();
                if (!reader.Read())
                    throw new Exception();

                var country = new Country
                {
                    Id = id,
                    Name = (string)reader["Name"]
                };
                return View(country);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: Countries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Country country)
        {
            try
            {
                if (id != country.Id)
                    throw new Exception();
                
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"UPDATE Countries SET Name = \"{country.Name}\" WHERE Id = {country.Id};";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Countries/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT Countries.Name FROM Countries WHERE Id = {id};";
                var reader = command.ExecuteReader();
                if (!reader.Read())
                    throw new Exception();

                var country = new Country
                {
                    Id = id,
                    Name = (string)reader["Name"]
                };
                return View(country);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: Countries/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM Countries WHERE Id = {id}", _connection);

                var res = command.ExecuteNonQuery();
                if (res == 0)
                    throw new Exception();
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public JsonResult CheckName(string name)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText =
                $"SELECT COUNT(*) FROM Countries WHERE LOWER(Countries.Name) = \"{name.ToLower().Trim()}\";";
            var count = command.ExecuteScalar() as Int64?;
            if (count == null)
                return Json("Не вдається перевірити валідність.");
            if (count > 0)
                return Json("Назва повинна бути унікальною.");
            return Json(true);
        }
    }
}