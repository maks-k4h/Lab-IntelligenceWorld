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
    public class OperationCommandersController : Controller
    {
        private MySqlConnection _connection;

        public OperationCommandersController(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: OperationCommanders
        public ActionResult Index()
        {
            _connection.Open();
            if (_connection.State != ConnectionState.Open)
                return NotFound();

            // retrieve operations
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM OperationCommanders";
            using var reader = command.ExecuteReader();
            var res = new List<OperationCommander>();
            while (reader.Read())
            {
                res.Add(new OperationCommander
                {
                    Id = (int)reader["Id"], 
                    OperationId = (int)reader["OperationId"]
                });
            }
            reader.Close();
            
            // retrieve agency workers
            foreach (var commander in res)
            {
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {commander.Id}";
                var r = command.ExecuteReader();
                r.Read();
                commander.AgencyWorker = new AgencyWorker
                {
                    Id = (int)r["Id"],
                    FullName = (string)r["FullName"]
                };
                r.Close();
            }
            
            // retrieve operation
            foreach (var commander in res)
            {
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM Operations WHERE Id = {commander.OperationId}";
                var r = command.ExecuteReader();
                r.Read();
                commander.Operation = new Operation
                {
                    Id = (int)r["Id"],
                    Name = (string)r["Name"]
                };
                r.Close();
            }

            return View(res);
        }

        // GET: OperationCommanders/Create
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
                        FullName = (string)workersReader["FullName"]
                    });
                }
                ViewBag.AgencyWorkers = new SelectList(workers, "Id", "FullName");
                workersReader.Close();
                
                // retrieve operations
                var getOperationsCommand = _connection.CreateCommand();
                getOperationsCommand.CommandText = "SELECT * FROM Operations";
                var operationsReader = getOperationsCommand.ExecuteReader();
                var operations = new List<Operation>();
                while (operationsReader.Read())
                {
                    operations.Add(new Operation
                    {
                        Id = (int)operationsReader["Id"],
                        Name = (string)operationsReader["Name"],
                        Status = (string)operationsReader["Status"],
                        Description = (string?)operationsReader["Description"]
                    });
                }
                ViewBag.Operations = new SelectList(operations, "Id", "Name");
                operationsReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            return View();
        }

        // POST: OperationCommanders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OperationCommander commander)
        {
            try
            {
                _connection.Open();

                var command = _connection.CreateCommand();
                command.CommandText = $"INSERT INTO OperationCommanders (Id, OperationId) " +
                                      $"VALUES ({commander.Id}, {commander.OperationId});";
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

        // GET: OperationCommanders/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                _connection.Open();
                
                // retrieve operation commander
                var getCommanderCommand = _connection.CreateCommand();
                getCommanderCommand.CommandText = $"SELECT * FROM OperationCommanders WHERE Id = {id};";
                var commanderReader = getCommanderCommand.ExecuteReader();
                commanderReader.Read();
                var commander = new OperationCommander
                {
                    Id = id,
                    OperationId = (int)commanderReader["OperationId"]
                };
                commanderReader.Close();
                
                // retrieve agency worker
                var getAgencyWorkerCommand = _connection.CreateCommand();
                getAgencyWorkerCommand.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {commander.Id}";
                var workerReader = getAgencyWorkerCommand.ExecuteReader();
                workerReader.Read();
                commander.AgencyWorker = new AgencyWorker
                {
                    Id = commander.Id,
                    FullName = (string)workerReader["FullName"]
                };
                workerReader.Close();
                
                // retrieve operation
                var getOperationCommand = _connection.CreateCommand();
                getOperationCommand.CommandText = $"SELECT * FROM Operations WHERE Id = {commander.OperationId}";
                var operationReader = getOperationCommand.ExecuteReader();
                operationReader.Read();
                commander.Operation = new Operation
                {
                    Id = commander.OperationId,
                    Name = (string)operationReader["Name"],
                    Status = (string)operationReader["Status"],
                    Description = (string?)operationReader["Description"]
                };
                operationReader.Close();

                return View(commander);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: OperationCommanders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM OperationCommanders WHERE Id = {id}", _connection);

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
                $"SELECT COUNT(*) FROM OperationCommanders WHERE Id = {id};";
            var count = command.ExecuteScalar() as Int64?;
            if (count == null)
                return Json("Не вдається перевірити валідність.");
            if (count > 0)
                return Json("Даний співробітник уже є командиром операції.");
            return Json(true);
        }

        public JsonResult CheckOperationId(int operationId)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText =
                $"SELECT COUNT(*) FROM OperationCommanders WHERE OperationId = {operationId};";
            var count = command.ExecuteScalar() as Int64?;
            if (count == null)
                return Json("Не вдається перевірити валідність.");
            if (count > 0)
                return Json("Дана операція уже має командира.");
            return Json(true);
        }
    }
}