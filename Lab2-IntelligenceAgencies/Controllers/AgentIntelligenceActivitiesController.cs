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
    public class AgentIntelligenceActivitiesController : Controller
    {
        private MySqlConnection _connection;

        public AgentIntelligenceActivitiesController(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: AgentIntelligenceActivities
        public ActionResult Index()
        {
            _connection.Open();
            if (_connection.State != ConnectionState.Open)
                return NotFound();

            // retrieve records
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM AgentIntelligenceActivities";
            using var reader = command.ExecuteReader();
            var res = new List<AgentIntelligenceActivity>();
            while (reader.Read())
            {
                res.Add(new AgentIntelligenceActivity
                {
                    Id = (int)reader["Id"], 
                    OperationId = (int)reader["OperationId"],
                    AgentId = (int)reader["AgentId"],
                    CoverEntityId = (int)reader["CoverEntityId"],
                    Description = (string?)reader["Description"]
                });
            }
            reader.Close();
            
            // retrieve agency workers
            foreach (var record in res)
            {
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {record.AgentId}";
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
            
            // retrieve cover entities
            foreach (var record in res)
            {
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM CoverEntities WHERE Id = {record.CoverEntityId}";
                var r = command.ExecuteReader();
                r.Read();
                record.CoverEntity = new CoverEntity
                {
                    Id = (int)r["Id"],
                    FullName = (string)r["FullName"]
                };
                r.Close();
            }

            return View(res);
        }

        // GET: AgentIntelligenceActivities/Create
        public ActionResult Create(string? error, int? agentId, int? operationId, int? coverEntityId, string? description)
        {
            try
            {
                _connection.Open();
                
                // retrieve agents
                var getAgentsCommand = _connection.CreateCommand();
                getAgentsCommand.CommandText = "SELECT * FROM Agents";
                var agentsReader = getAgentsCommand.ExecuteReader();
                var agents = new List<Agent>();
                while (agentsReader.Read())
                {
                    agents.Add(new Agent
                    {
                        Id = (int)agentsReader["Id"],
                    });
                }
                agentsReader.Close();

                // retrieve agency workers
                var agencyWorkers = new List<AgencyWorker>();
                foreach (var agent in agents)
                {
                    var getAgencyWorkerCommand = _connection.CreateCommand();
                    getAgencyWorkerCommand.CommandText = $"SELECT * FROM AgencyWorkers Where Id = {agent.Id}";
                    var agencyWorkersReader = getAgencyWorkerCommand.ExecuteReader();
                    agencyWorkersReader.Read();
                    agencyWorkers.Add(new AgencyWorker
                    {
                        Id = agent.Id,
                        FullName = (string)agencyWorkersReader["FullName"]
                    });
                    agencyWorkersReader.Close();
                }
                ViewBag.Agents = new SelectList(agencyWorkers, "Id", "FullName");
                
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
                
                // retrieve cover entities
                var getCoverEntitiesCommand = _connection.CreateCommand();
                getCoverEntitiesCommand.CommandText = "SELECT * FROM CoverEntities";
                var coverEntitiesReader = getCoverEntitiesCommand.ExecuteReader();
                var coverEntities = new List<CoverEntity>();
                while (coverEntitiesReader.Read())
                {
                    coverEntities.Add(new CoverEntity
                    {
                        Id = (int)coverEntitiesReader["Id"],
                        FullName = (string)coverEntitiesReader["FullName"],
                    });
                }
                ViewBag.CoverEntities = new SelectList(coverEntities, "Id", "FullName");
                coverEntitiesReader.Close();
                
                // error
                ViewBag.ErrorMessage = error;
                
                // defaults
                ViewBag.OperationId = operationId;
                ViewBag.CoverEntityId = coverEntityId;
                ViewBag.AgentId = agentId;
                ViewBag.Description = description;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            return View();
        }

        // POST: AgentIntelligenceActivities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AgentIntelligenceActivity activity)
        {
            try
            {
                _connection.Open();
                
                // check if the agent already takes part in the operation
                var checkAgentCommand = _connection.CreateCommand();
                checkAgentCommand.CommandText = $"SELECT COUNT(*) " +
                                                $"FROM AgentIntelligenceActivities " +
                                                $"WHERE AgentId = {activity.AgentId} " +
                                                $"AND OperationId = {activity.OperationId};";
                if ((Int64)checkAgentCommand.ExecuteScalar() > 0)
                    return RedirectToAction("Create", new
                    {
                        error = "Цей агент уже бере участь у даній операцї.",
                        agentId = activity.AgentId,
                        operationId = activity.OperationId,
                        coverEntityId = activity.CoverEntityId,
                        description = activity.Description
                    });
                
                // check if the cover entity is already used in the operation
                var checkCoverEntityCommand = _connection.CreateCommand();
                checkCoverEntityCommand.CommandText = $"SELECT COUNT(*) " +
                                                      $"FROM AgentIntelligenceActivities " +
                                                      $"WHERE CoverEntityId = {activity.CoverEntityId} " +
                                                      $"AND OperationId = {activity.OperationId};";
                if ((Int64)checkCoverEntityCommand.ExecuteScalar() > 0)
                    return RedirectToAction("Create", new
                    {
                        error = "Цей персонаж уже бере участь у даній операцї.",
                        agentId = activity.AgentId,
                        operationId = activity.OperationId,
                        coverEntityId = activity.CoverEntityId,
                        description = activity.Description
                    });
                
                var command = _connection.CreateCommand();
                command.CommandText = $"INSERT INTO AgentIntelligenceActivities (AgentId, OperationId, CoverEntityId, Description) " +
                                      $"VALUES ({activity.AgentId}, {activity.OperationId}, {activity.CoverEntityId}, \"{activity.Description}\");";
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

        // GET: AgentIntelligenceActivities/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                _connection.Open();
                
                // retrieve the record
                var getRecordCommand = _connection.CreateCommand();
                getRecordCommand.CommandText = $"SELECT * FROM AgentIntelligenceActivities WHERE Id = {id};";
                var recordReader = getRecordCommand.ExecuteReader();
                recordReader.Read();
                var activity = new AgentIntelligenceActivity
                {
                    AgentId = (int)recordReader["AgentId"],
                    OperationId = (int)recordReader["OperationId"],
                    CoverEntityId = (int)recordReader["CoverEntityId"],
                    Description = (string?)recordReader["Description"]
                };
                recordReader.Close();

                // retrieve the cover entity
                var getCoverEntityCommand = _connection.CreateCommand();
                getCoverEntityCommand.CommandText = $"SELECT * FROM CoverEntities WHERE Id = {activity.CoverEntityId}";
                var coverEntityReader = getCoverEntityCommand.ExecuteReader();
                coverEntityReader.Read();
                activity.CoverEntity = new CoverEntity
                {
                    Id = activity.CoverEntityId,
                    FullName = (string)coverEntityReader["FullName"]
                };
                coverEntityReader.Close();
                
                // retrieve agency worker
                var getAgencyWorkerCommand = _connection.CreateCommand();
                getAgencyWorkerCommand.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {activity.AgentId}";
                var workerReader = getAgencyWorkerCommand.ExecuteReader();
                workerReader.Read();
                activity.AgencyWorker = new AgencyWorker
                {
                    Id = activity.AgentId,
                    FullName = (string)workerReader["FullName"]
                };
                workerReader.Close();
                
                // retrieve operation
                var getOperationCommand = _connection.CreateCommand();
                getOperationCommand.CommandText = $"SELECT * FROM Operations WHERE Id = {activity.OperationId}";
                var operationReader = getOperationCommand.ExecuteReader();
                operationReader.Read();
                activity.Operation = new Operation
                {
                    Id = activity.OperationId,
                    Name = (string)operationReader["Name"]
                };
                operationReader.Close();

                return View(activity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: AgentIntelligenceActivities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AgentIntelligenceActivity activity)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"UPDATE AgentIntelligenceActivities " +
                                      $"SET Description = \"{activity.Description}\" " +
                                      $"WHERE Id = {id};";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Edit");
            }
        }

        // GET: AgentIntelligenceActivities/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                _connection.Open();
                
                // retrieve the record
                var getRecordCommand = _connection.CreateCommand();
                getRecordCommand.CommandText = $"SELECT * FROM AgentIntelligenceActivities WHERE Id = {id};";
                var recordReader = getRecordCommand.ExecuteReader();
                recordReader.Read();
                var activity = new AgentIntelligenceActivity
                {
                    AgentId = (int)recordReader["AgentId"],
                    OperationId = (int)recordReader["OperationId"],
                    CoverEntityId = (int)recordReader["CoverEntityId"],
                    Description = (string?)recordReader["Description"]
                };
                recordReader.Close();

                // retrieve the cover entity
                var getCoverEntityCommand = _connection.CreateCommand();
                getCoverEntityCommand.CommandText = $"SELECT * FROM CoverEntities WHERE Id = {activity.CoverEntityId}";
                var coverEntityReader = getCoverEntityCommand.ExecuteReader();
                coverEntityReader.Read();
                activity.CoverEntity = new CoverEntity
                {
                    Id = activity.CoverEntityId,
                    FullName = (string)coverEntityReader["FullName"]
                };
                coverEntityReader.Close();
                
                // retrieve agency worker
                var getAgencyWorkerCommand = _connection.CreateCommand();
                getAgencyWorkerCommand.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {activity.AgentId}";
                var workerReader = getAgencyWorkerCommand.ExecuteReader();
                workerReader.Read();
                activity.AgencyWorker = new AgencyWorker
                {
                    Id = activity.AgentId,
                    FullName = (string)workerReader["FullName"]
                };
                workerReader.Close();
                
                // retrieve operation
                var getOperationCommand = _connection.CreateCommand();
                getOperationCommand.CommandText = $"SELECT * FROM Operations WHERE Id = {activity.OperationId}";
                var operationReader = getOperationCommand.ExecuteReader();
                operationReader.Read();
                activity.Operation = new Operation
                {
                    Id = activity.OperationId,
                    Name = (string)operationReader["Name"]
                };
                operationReader.Close();

                return View(activity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: AgentIntelligenceActivities/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM AgentIntelligenceActivities WHERE Id = {id};", _connection);
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
                    id = id
                });
            }
        }
    }
}