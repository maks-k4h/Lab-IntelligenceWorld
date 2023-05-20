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
    public class CoverEntitiesController : Controller
    {
        private MySqlConnection _connection;

        public CoverEntitiesController(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: CoverEntities
        public ActionResult Index()
        {
            _connection.Open();
            if (_connection.State != ConnectionState.Open)
                return NotFound();

            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM CoverEntities";

            using var reader = command.ExecuteReader();
            var res = new List<CoverEntity>();
            
            while (reader.Read())
            {
                res.Add(new CoverEntity
                {
                    Id = (int)reader["Id"], 
                    FullName = (string)reader["FullName"],
                    Legend = (string?)reader["Legend"]
                });
            }

            return View(res);
        }

        // GET: CoverEntities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CoverEntities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CoverEntity entity)
        {
            try
            {
                _connection.Open();

                var command = _connection.CreateCommand();
                command.CommandText = $"INSERT INTO CoverEntities (FullName, Legend) " +
                                      $"VALUES (\"{entity.FullName}\", \"{entity.Legend}\");";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CoverEntities/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM CoverEntities WHERE Id = {id};";
                var reader = command.ExecuteReader();
                if (!reader.Read())
                    throw new Exception();

                var entity = new CoverEntity
                {
                    Id = id,
                    FullName = (string)reader["FullName"],
                    Legend = (string?)reader["Legend"]
                };
                return View(entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: CoverEntities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CoverEntity entity)
        {
            try
            {
                if (id != entity.Id)
                    throw new Exception();
                
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"UPDATE CoverEntities SET " +
                                      $"FullName = \"{entity.FullName}\"," +
                                      $"Legend = \"{entity.Legend}\"" +
                                      $"WHERE Id = {entity.Id};";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CoverEntities/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM CoverEntities WHERE Id = {id};";
                var reader = command.ExecuteReader();
                if (!reader.Read())
                    throw new Exception();

                var entity = new CoverEntity
                {
                    Id = id,
                    FullName = (string)reader["FullName"],
                    Legend = (string?)reader["Legend"]
                };
                return View(entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: CoverEntities/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM CoverEntities WHERE Id = {id}", _connection);

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
    }
}