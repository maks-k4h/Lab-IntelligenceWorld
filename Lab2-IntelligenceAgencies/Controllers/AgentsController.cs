using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Lab2_IntelligenceAgencies.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;

namespace Lab2_IntelligenceAgencies.Controllers
{
    public class AgentsController : Controller
    {
        private MySqlConnection _connection;

        public AgentsController (MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: Agents
        public ActionResult Index()
        {
            _connection.Open();
            if (_connection.State != ConnectionState.Open)
                return NotFound();

            // retrieve department heads
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM Agents";
            using var reader = command.ExecuteReader();
            var res = new List<Agent>();
            while (reader.Read())
            {
                res.Add(new Agent
                {
                    Id = (int)reader["Id"], 
                });
            }
            reader.Close();
            
            // retrieve agency workers
            foreach (var head in res)
            {
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {head.Id}";
                var r = command.ExecuteReader();
                r.Read();
                head.AgencyWorker = new AgencyWorker
                {
                    Id = (int)r["Id"],
                    FullName = (string)r["FullName"]
                };
                r.Close();
            }

            return View(res);
        }

        // GET: Agents/Create
        public ActionResult Create()
        {
            try
            {
                _connection.Open();

                // retrieve agency workers
                var getWorkersCommand = _connection.CreateCommand();
                getWorkersCommand.CommandText = "SELECT * FROM AgencyWorkers";
                var workersReader = getWorkersCommand.ExecuteReader();
                var workers = new List<AgencyWorker>();
                while (workersReader.Read())
                {
                    workers.Add(new AgencyWorker
                    {
                        Id = (int)workersReader["Id"],
                        FullName = (string)workersReader["FullName"],
                    });
                }
                ViewBag.AgencyWorkers = new SelectList(workers, "Id", "FullName");
                workersReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return View();
        }

        // POST: Agents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Agent agent)
        {
            try
            {
                _connection.Open();

                var command = _connection.CreateCommand();
                command.CommandText = $"INSERT INTO Agents (Id) " +
                                      $"VALUES ({agent.Id});";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Create");
            }
        }

        // GET: Agents/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                _connection.Open();
                
                // retrieve department head
                var getAgentCommand = _connection.CreateCommand();
                getAgentCommand.CommandText = $"SELECT * FROM Agents WHERE Id = {id};";
                var agentReader = getAgentCommand.ExecuteReader();
                agentReader.Read();
                var agent = new Agent
                {
                    Id = id,
                };
                agentReader.Close();
                
                // retrieve agency worker
                var getAgencyWorkerCommand = _connection.CreateCommand();
                getAgencyWorkerCommand.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {agent.Id}";
                var workerReader = getAgencyWorkerCommand.ExecuteReader();
                workerReader.Read();
                agent.AgencyWorker = new AgencyWorker
                {
                    Id = agent.Id,
                    FullName = (string)workerReader["FullName"]
                };
                workerReader.Close();

                return View(agent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: Agents/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM Agents WHERE Id = {id}", _connection);

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
        

        public JsonResult CheckId(int id)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText =
                $"SELECT COUNT(*) FROM Agents WHERE Id = {id};";
            var count = command.ExecuteScalar() as Int64?;
            if (count == null)
                return Json("Не вдається перевірити валідність.");
            if (count > 0)
                return Json("Даний співробітник уже є агентом.");
            return Json(true);
        }
    }
}