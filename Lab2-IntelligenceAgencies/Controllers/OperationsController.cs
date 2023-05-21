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
    public class OperationsController : Controller
    {
        private MySqlConnection _connection;

        public OperationsController(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: Operations
        public ActionResult Index()
        {
            _connection.Open();
            if (_connection.State != ConnectionState.Open)
                return NotFound();

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM Operations";

            using var reader = command.ExecuteReader();
            var res = new List<Operation>();
            
            while (reader.Read())
            {
                res.Add(new Operation
                {
                    Id = (int)reader["Id"], 
                    Name = (string)reader["Name"],
                    Status = (string?)reader["Status"],
                    Description = (string?)reader["Description"]
                });
            }

            return View(res);
        }

        // GET: Operations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Operations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Operation operation)
        {
            try
            {
                _connection.Open();

                var command = _connection.CreateCommand();
                command.CommandText = $"INSERT INTO Operations (Name, Status, Description) " +
                                      $"VALUES (\"{operation.Name}\", \"{operation.Status}\", \"{operation.Description}\");";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Create");
            }
        }

        // GET: Operations/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM Operations WHERE Id = {id};";
                var reader = command.ExecuteReader();
                if (!reader.Read())
                    throw new Exception();

                var operation = new Operation
                {
                    Id = id,
                    Name = (string)reader["Name"],
                    Status = (string?)reader["Status"],
                    Description = (string?)reader["Description"]
                };
                return View(operation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: Operations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Operation operation)
        {
            try
            {
                if (id != operation.Id)
                    throw new Exception();
                
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"UPDATE Operations SET " +
                                      $"Name = \"{operation.Name}\"," +
                                      $"Status = \"{operation.Status}\"," +
                                      $"Description = \"{operation.Description}\"" +
                                      $"WHERE Id = {operation.Id};";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Edit");
            }
        }

        // GET: Operations/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM Operations WHERE Id = {id};";
                var reader = command.ExecuteReader();
                if (!reader.Read())
                    throw new Exception();

                var operation = new Operation
                {
                    Id = id,
                    Name = (string)reader["Name"],
                    Status = (string)reader["Status"],
                    Description = (string?)reader["Description"]
                };
                return View(operation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: Operations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM Operations WHERE Id = {id}", _connection);

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