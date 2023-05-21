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
    public class AgencyWorkersController : Controller
    {
        private MySqlConnection _connection;

        public AgencyWorkersController(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: AgencyWorkers
        public ActionResult Index()
        {
            _connection.Open();
            if (_connection.State != ConnectionState.Open)
                return NotFound();

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM AgencyWorkers";

            using var reader = command.ExecuteReader();
            var res = new List<AgencyWorker>();
            
            while (reader.Read())
            {
                res.Add(new AgencyWorker{Id = (int)reader["Id"], FullName = (string)reader["FullName"]});
            }

            return View(res);
        }

        // GET: AgencyWorkers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AgencyWorkers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AgencyWorker worker)
        {
            try
            {
                _connection.Open();

                var command = _connection.CreateCommand();
                command.CommandText = $"INSERT INTO AgencyWorkers (FullName) VALUES (\"{worker.FullName}\");";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Create");
            }
        }

        // GET: AgencyWorkers/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT AgencyWorkers.FullName FROM AgencyWorkers WHERE Id = {id};";
                var reader = command.ExecuteReader();
                if (!reader.Read())
                    throw new Exception();

                var worker = new AgencyWorker
                {
                    Id = id,
                    FullName = (string)reader["FullName"]
                };
                return View(worker);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: AgencyWorkers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AgencyWorker worker)
        {
            try
            {
                if (id != worker.Id)
                    throw new Exception();
                
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"UPDATE AgencyWorkers SET FullName = \"{worker.FullName}\" WHERE Id = {worker.Id};";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Edit");
            }
        }

        // GET: AgencyWorkers/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT AgencyWorkers.FullName FROM AgencyWorkers WHERE Id = {id};";
                var reader = command.ExecuteReader();
                if (!reader.Read())
                    throw new Exception();

                var worker = new AgencyWorker
                {
                    Id = id,
                    FullName = (string)reader["FullName"]
                };
                return View(worker);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: AgencyWorkers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM AgencyWorkers WHERE Id = {id}", _connection);

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