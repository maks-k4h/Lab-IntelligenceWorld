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
    public class StationaryWorkersToOperationsController : Controller
    {
        private MySqlConnection _connection;

        public StationaryWorkersToOperationsController(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: StationaryWorkersToOperations
        public ActionResult Index()
        {
            _connection.Open();
            if (_connection.State != ConnectionState.Open)
                return NotFound();

            // retrieve records
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM StationaryWorkersToOperations";
            using var reader = command.ExecuteReader();
            var res = new List<StationaryWorkerToOperation>();
            while (reader.Read())
            {
                res.Add(new StationaryWorkerToOperation
                {
                    StationaryWorkerId = (int)reader["StationaryWorkerId"], 
                    OperationId = (int)reader["OperationId"],
                    AccessLevelId = (int)reader["AccessLevelId"]
                });
            }
            reader.Close();
            
            // retrieve agency workers
            foreach (var record in res)
            {
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {record.StationaryWorkerId}";
                var r = command.ExecuteReader();
                r.Read();
                record.AgencyWorker = new AgencyWorker
                {
                    Id = (int)r["Id"],
                    FullName = (string)r["FullName"]
                };
                r.Close();
            }
            
            // retrieve operations
            foreach (var record in res)
            {
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM Operations WHERE Id = {record.OperationId}";
                var r = command.ExecuteReader();
                r.Read();
                record.Operation = new Operation
                {
                    Id = (int)r["Id"],
                    Name = (string)r["Name"]
                };
                r.Close();
            }
            
            // retrieve access levels
            foreach (var record in res)
            {
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM AccessLevels WHERE Id = {record.AccessLevelId}";
                var r = command.ExecuteReader();
                r.Read();
                record.AccessLevel = new AccessLevel
                {
                    Id = (int)r["Id"],
                    Name = (string)r["Name"]
                };
                r.Close();
            }

            return View(res);
        }

        // GET: StationaryWorkersToOperations/Create
        public ActionResult Create(string? error, int? workerId, int? operationId, int? accessLevelId)
        {
            try
            {
                _connection.Open();
                
                // retrieve stationary workers
                var getStationaryWorkersCommand = _connection.CreateCommand();
                getStationaryWorkersCommand.CommandText = "SELECT * FROM StationaryWorkers";
                var stationaryWorkersReader = getStationaryWorkersCommand.ExecuteReader();
                var stationaryWorkers = new List<StationaryWorker>();
                while (stationaryWorkersReader.Read())
                {
                    stationaryWorkers.Add(new StationaryWorker
                    {
                        Id = (int)stationaryWorkersReader["Id"],
                    });
                }
                stationaryWorkersReader.Close();

                // retrieve agency workers
                var agencyWorkers = new List<AgencyWorker>();
                foreach (var stationaryWorker in stationaryWorkers)
                {
                    var getAgencyWorkerCommand = _connection.CreateCommand();
                    getAgencyWorkerCommand.CommandText = $"SELECT * FROM AgencyWorkers Where Id = {stationaryWorker.Id}";
                    var agencyWorkersReader = getAgencyWorkerCommand.ExecuteReader();
                    agencyWorkersReader.Read();
                    agencyWorkers.Add(new AgencyWorker
                    {
                        Id = stationaryWorker.Id,
                        FullName = (string)agencyWorkersReader["FullName"]
                    });
                    agencyWorkersReader.Close();
                }
                ViewBag.StationaryWorkers = new SelectList(agencyWorkers, "Id", "FullName");
                
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
                
                // retrieve access levels
                var getAccessLevelsCommand = _connection.CreateCommand();
                getAccessLevelsCommand.CommandText = "SELECT * FROM AccessLevels";
                var levelsReader = getAccessLevelsCommand.ExecuteReader();
                var accessLevels = new List<AccessLevel>();
                while (levelsReader.Read())
                {
                    accessLevels.Add(new AccessLevel()
                    {
                        Id = (int)levelsReader["Id"],
                        Name = (string)levelsReader["Name"],
                    });
                }
                ViewBag.AccessLevels = new SelectList(accessLevels, "Id", "Name");
                levelsReader.Close();
                
                // error
                ViewBag.ErrorMessage = error;
                
                // defaults 
                ViewBag.OperationId = operationId;
                ViewBag.WorkerId = workerId;
                ViewBag.AccessLevelId = accessLevelId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            return View();
        }

        // POST: StationaryWorkersToOperations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StationaryWorkerToOperation record)
        {
            try
            {
                _connection.Open();

                var checkCommand = _connection.CreateCommand();
                checkCommand.CommandText = $"SELECT COUNT(*) " +
                                           $"FROM StationaryWorkersToOperations " +
                                           $"WHERE StationaryWorkerId = {record.StationaryWorkerId} " +
                                           $"AND OperationId = {record.OperationId};";
                if ((Int64)checkCommand.ExecuteScalar() > 0)
                    return RedirectToAction("Create", new
                    {
                        error = "Цей співробітник уже бере участь у даній операцї.",
                        workerId = record.StationaryWorkerId,
                        operationId = record.OperationId,
                        accessLevelId = record.AccessLevelId
                    });
                
                var command = _connection.CreateCommand();
                command.CommandText = $"INSERT INTO StationaryWorkersToOperations (StationaryWorkerId, OperationId, AccessLevelId) " +
                                      $"VALUES ({record.StationaryWorkerId}, {record.OperationId}, {record.AccessLevelId});";
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

        // GET: StationaryWorkersToOperations/Edit?operationId=1&stationaryWorkerId=2
        public ActionResult Edit(int operationId, int stationaryWorkerId)
        {
            try
            {
                _connection.Open();
                
                // retrieve the record
                var getRecordCommand = _connection.CreateCommand();
                getRecordCommand.CommandText = $"SELECT * FROM StationaryWorkersToOperations " +
                                               $"WHERE StationaryWorkerId = {stationaryWorkerId} " +
                                               $"AND OperationId = {operationId}";
                var recordReader = getRecordCommand.ExecuteReader();
                recordReader.Read();
                var record = new StationaryWorkerToOperation
                {
                    StationaryWorkerId = stationaryWorkerId,
                    OperationId = operationId,
                    AccessLevelId = (int)recordReader["AccessLevelId"]
                };
                recordReader.Close();

                // retrieve access levels
                var getAccessLevelsCommand = _connection.CreateCommand();
                getAccessLevelsCommand.CommandText = "SELECT * FROM AccessLevels";
                var levelsReader = getAccessLevelsCommand.ExecuteReader();
                var accessLevels = new List<AccessLevel>();
                while (levelsReader.Read())
                {
                    accessLevels.Add(new AccessLevel()
                    {
                        Id = (int)levelsReader["Id"],
                        Name = (string)levelsReader["Name"],
                    });
                }
                levelsReader.Close();
                ViewBag.AccessLevels = new SelectList(accessLevels, "Id", "Name");
                
                // retrieve agency worker
                var getAgencyWorkerCommand = _connection.CreateCommand();
                getAgencyWorkerCommand.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {stationaryWorkerId}";
                var workerReader = getAgencyWorkerCommand.ExecuteReader();
                workerReader.Read();
                record.AgencyWorker = new AgencyWorker
                {
                    Id = stationaryWorkerId,
                    FullName = (string)workerReader["FullName"]
                };
                workerReader.Close();
                
                // retrieve operation
                var getOperationCommand = _connection.CreateCommand();
                getOperationCommand.CommandText = $"SELECT * FROM Operations WHERE Id = {operationId}";
                var operationReader = getOperationCommand.ExecuteReader();
                operationReader.Read();
                record.Operation = new Operation
                {
                    Id = operationId,
                    Name = (string)operationReader["Name"]
                };
                operationReader.Close();

                return View(record);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: StationaryWorkersToOperations/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StationaryWorkerToOperation record)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"UPDATE StationaryWorkersToOperations " +
                                      $"SET AccessLevelId = {record.AccessLevelId} " +
                                      $"WHERE StationaryWorkerId = {record.StationaryWorkerId} " +
                                      $"AND OperationId = {record.OperationId};";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Edit");
            }
        }

        // GET: StationaryWorkersToOperations/Delete?workerId=1&operationId=2
        [HttpGet]
        public ActionResult Delete(int stationaryWorkerId, int operationId)
        {
            try
            {
                _connection.Open();
                
                // retrieve the record
                var getRecordCommand = _connection.CreateCommand();
                getRecordCommand.CommandText = $"SELECT * FROM StationaryWorkersToOperations " +
                                               $"WHERE StationaryWorkerId = {stationaryWorkerId} " +
                                               $"AND OperationId = {operationId}";
                var recordReader = getRecordCommand.ExecuteReader();
                recordReader.Read();
                var record = new StationaryWorkerToOperation
                {
                    StationaryWorkerId = stationaryWorkerId,
                    OperationId = operationId,
                    AccessLevelId = (int)recordReader["AccessLevelId"]
                };
                recordReader.Close();

                // retrieve access level
                var getLevelCommand = _connection.CreateCommand();
                getLevelCommand.CommandText = $"SELECT * FROM AccessLevels WHERE Id = {record.AccessLevelId};";
                var levelReader = getLevelCommand.ExecuteReader();
                levelReader.Read();
                record.AccessLevel = new AccessLevel
                {
                    Id = record.AccessLevelId,
                    Name = (string)levelReader["Name"]
                };
                levelReader.Close();
                
                // retrieve agency worker
                var getAgencyWorkerCommand = _connection.CreateCommand();
                getAgencyWorkerCommand.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {stationaryWorkerId}";
                var workerReader = getAgencyWorkerCommand.ExecuteReader();
                workerReader.Read();
                record.AgencyWorker = new AgencyWorker
                {
                    Id = stationaryWorkerId,
                    FullName = (string)workerReader["FullName"]
                };
                workerReader.Close();
                
                // retrieve operation
                var getOperationCommand = _connection.CreateCommand();
                getOperationCommand.CommandText = $"SELECT * FROM Operations WHERE Id = {operationId}";
                var operationReader = getOperationCommand.ExecuteReader();
                operationReader.Read();
                record.Operation = new Operation
                {
                    Id = operationId,
                    Name = (string)operationReader["Name"]
                };
                operationReader.Close();

                return View(record);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: StationaryWorkersToOperations/Delete?workerId=1&operationId=2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int stationaryWorkerId, int operationId, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM StationaryWorkersToOperations " +
                                               $"WHERE StationaryWorkerId = {stationaryWorkerId} " +
                                               $"AND OperationId = {operationId}", _connection);

                var res = command.ExecuteNonQuery();
                if (res == 0)
                    throw new Exception();
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Delete", new
                {
                    stationaryWorkerId = stationaryWorkerId,
                    operationId = operationId
                });
            }
        }
    }
}