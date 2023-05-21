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
    public class AgenciesController : Controller
    {
        private MySqlConnection _connection;

        public AgenciesController(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: Agencies
        public ActionResult Index()
        {
            _connection.Open();
            if (_connection.State != ConnectionState.Open)
                return NotFound();

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM Agencies";

            using var reader = command.ExecuteReader();
            var res = new List<Agency>();
            
            while (reader.Read())
                res.Add(new Agency
                {
                    Id = (int)reader["Id"], 
                    Name = (string)reader["Name"],
                    Headquarters = (string?)reader["Headquarters"],
                    Description = (string?)reader["Description"]
                });

            return View(res);
        }

        // GET: Agencies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Agencies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Agency agency)
        {
            try
            {
                _connection.Open();

                var command = _connection.CreateCommand();
                command.CommandText = $"INSERT INTO Agencies (Name, Headquarters, Description) " +
                                      $"VALUES (\"{agency.Name}\", \"{agency.Headquarters}\", \"{agency.Description}\");";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Create");
            }
        }

        // GET: Agencies/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM Agencies WHERE Id = {id};";
                var reader = command.ExecuteReader();
                if (!reader.Read())
                    throw new Exception();

                var agency = new Agency
                {
                    Id = id,
                    Name = (string)reader["Name"],
                    Headquarters = (string?)reader["Headquarters"],
                    Description = (string?)reader["Description"]
                };
                return View(agency);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: Agencies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Agency agency)
        {
            try
            {
                if (id != agency.Id)
                    throw new Exception();
                
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"UPDATE Agencies SET " +
                                      $"Name = \"{agency.Name}\"," +
                                      $"Headquarters = \"{agency.Headquarters}\"," +
                                      $"Description = \"{agency.Description}\" " +
                                      $"WHERE Id = {agency.Id};";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Edit");
            }
        }

        // GET: Agencies/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM Agencies WHERE Id = {id};";
                var reader = command.ExecuteReader();
                if (!reader.Read())
                    throw new Exception();

                var agency = new Agency
                {
                    Id = id,
                    Name = (string)reader["Name"],
                    Headquarters = (string?)reader["Headquarters"],
                    Description = (string?)reader["Description"]
                };
                return View(agency);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: Agencies/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM Agencies WHERE Id = {id}", _connection);

                var res = command.ExecuteNonQuery();
                if (res == 0)
                    throw new Exception();
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Delete");
            }
        }
    }
}