using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Lab2_IntelligenceAgencies.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using NuGet.Protocol;

namespace Lab2_IntelligenceAgencies.Controllers
{
    public class QueriesController : Controller
    {
        private readonly MySqlConnection _connection;

        public QueriesController(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        
        // GET: Queries/A3
        public ActionResult Index(string query = "A1")
        {
            return RedirectToAction(query);
        }

        // GET: Queries/A1?id=2
        public ActionResult A1(int? id)
        {
            try
            {
                _connection.Open();

                // retrieve operations
                var getOperationsCommand = new MySqlCommand("SELECT * FROM Operations", _connection);
                var operationsReader = getOperationsCommand.ExecuteReader();
                var operations = new List<Operation>();
                while (operationsReader.Read())
                {
                    operations.Add(new Operation
                    {
                        Id = (int)operationsReader["Id"],
                        Name = (string)operationsReader["Name"]
                    });
                }
                ViewBag.Operations = new SelectList(operations, "Id", "Name");
                operationsReader.Close();
                
                // retrieve agency workers
                ViewBag.AgencyWorkers = new List<AgencyWorker>();
                if (id != null)
                {
                    var getAgencyWorkersCommand = new MySqlCommand(
                        "SELECT * " +
                               "FROM AgencyWorkers " +
                               "WHERE AgencyWorkers.Id IN " +
                                   "(SELECT AgentIntelligenceActivities.AgentId " +
                                   "FROM AgentIntelligenceActivities " +
                                    $"WHERE OperationId = {id});", 
                        _connection);
                    Console.WriteLine(getAgencyWorkersCommand.CommandText);
                    var agencyWorkersReader = getAgencyWorkersCommand.ExecuteReader();
                    while (agencyWorkersReader.Read())
                    {
                        ViewBag.AgencyWorkers.Add(new AgencyWorker
                        {
                            Id = (int)agencyWorkersReader["Id"],
                            FullName = (string)agencyWorkersReader["FullName"]
                        });
                    }
                    agencyWorkersReader.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
            
            return View();
        }
        
        // GET: Queries/A2?param=2
        public ActionResult A2(string? param)
        {
            try
            {
                _connection.Open();

                // retrieve operations
                var getOperationsCommand = new MySqlCommand("SELECT * FROM Operations", _connection);
                var operationsReader = getOperationsCommand.ExecuteReader();
                var operations = new List<Operation>();
                while (operationsReader.Read())
                {
                    operations.Add(new Operation
                    {
                        Id = (int)operationsReader["Id"],
                        Name = (string)operationsReader["Name"]
                    });
                }
                ViewBag.Operations = new SelectList(operations, "Id", "Name");
                operationsReader.Close();
                
                // retrieve agency workers
                var workers = new List<AgencyWorker>();
                if (param != null)
                {
                    try
                    {
                        int num = int.Parse(param);

                        var getAgencyWorkersCommand = new MySqlCommand(
                            "SELECT * " +
                            "FROM AgencyWorkers A INNER JOIN StationaryWorkers S ON A.Id = S.Id " +
                            "WHERE (" +
                            "   SELECT COUNT(DISTINCT OperationId) " +
                            "   FROM StationaryWorkersToOperations" +
                            "   WHERE StationaryWorkerId = A.Id" +
                            $"   ) >= {num};",
                            _connection);
                        Console.WriteLine(getAgencyWorkersCommand.CommandText);
                        var agencyWorkersReader = getAgencyWorkersCommand.ExecuteReader();
                        while (agencyWorkersReader.Read())
                        {
                            workers.Add(new AgencyWorker
                            {
                                Id = (int)agencyWorkersReader["Id"],
                                FullName = (string)agencyWorkersReader["FullName"]
                            });
                        }

                        agencyWorkersReader.Close();

                        ViewBag.Num = num;
                    }
                    catch
                    {
                        ViewBag.ErrorMessage = "Необхідно ввести ціле число";
                    }
                }

                return View(workers);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }
        
        // GET: Queries/A3?id=2
        public ActionResult A3(int? id)
        {
            try
            {
                _connection.Open();
                
                // retrieve countries
                var getCountriesCommand = new MySqlCommand("SELECT * FROM Countries;", _connection);
                var countriesReader = getCountriesCommand.ExecuteReader();
                var countries = new List<Country>();
                while (countriesReader.Read())
                {
                    countries.Add(new Country
                    {
                        Id = (int)countriesReader["Id"],
                        Name = (string)countriesReader["Name"]
                    });
                }
                countriesReader.Close();
                ViewBag.Countries = new SelectList(countries, "Id", "Name");

                // retrieve agencies
                ViewBag.Agencies = new List<Agency>();
                if (id != null)
                {
                    var getAgenciesCommand = new MySqlCommand(
                        "SELECT * " +
                        "FROM Agencies " +
                        "WHERE Id IN (" +
                        "   SELECT OperationConduction.AgencyId  " +
                        "   FROM OperationConduction" +
                        $"   WHERE OperationConduction.CountryId = {id}" +
                        ")", _connection);
                    var agenciesReader = getAgenciesCommand.ExecuteReader();
                    while (agenciesReader.Read())
                    {
                        ViewBag.Agencies.Add(new Agency
                        {
                            Id = (int)agenciesReader["Id"],
                            Name = (string)agenciesReader["Name"],
                            Headquarters = (string?)agenciesReader["Headquarters"]
                        });
                    }
                    agenciesReader.Close();
                }
                
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }
        
        // GET: Queries/A4?id=2
        public ActionResult A4(int? id)
        {
            try
            {
                _connection.Open();
                
                // retrieve countries
                var getCountriesCommand = new MySqlCommand("SELECT * FROM Countries;", _connection);
                var countriesReader = getCountriesCommand.ExecuteReader();
                var countries = new List<Country>();
                while (countriesReader.Read())
                {
                    countries.Add(new Country
                    {
                        Id = (int)countriesReader["Id"],
                        Name = (string)countriesReader["Name"]
                    });
                }
                countriesReader.Close();
                ViewBag.Countries = new SelectList(countries, "Id", "Name");

                // retrieve cover entities
                ViewBag.CoverEntities = new List<CoverEntity>();
                if (id != null)
                {
                    var getCoverEntitiesCommand = new MySqlCommand(
                        "SELECT Id, FullName " +
                        "FROM CoverEntities " +
                        "WHERE Id IN (" +
                        "   SELECT CoverEntityId " +
                        "   FROM AgentIntelligenceActivities " +
                        "   WHERE OperationId IN (" +
                        "       SELECT OperationId " +
                        "       FROM OperationConduction " +
                        $"       WHERE CountryId = {id}" +
                        "   )" +
                        ");", _connection);
                    var coverEntitiesReader = getCoverEntitiesCommand.ExecuteReader();
                    while (coverEntitiesReader.Read())
                    {
                        ViewBag.CoverEntities.Add(new CoverEntity()
                        {
                            Id = (int)coverEntitiesReader["Id"],
                            FullName = (string)coverEntitiesReader["FullName"],
                        });
                    }
                    coverEntitiesReader.Close();
                }
                
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }
        
        // GET: Queries/A5?id=2
        public ActionResult A5(int? id)
        {
            try
            {
                _connection.Open();
                
                // retrieve countries
                var getCountriesCommand = new MySqlCommand("SELECT * FROM Countries;", _connection);
                var countriesReader = getCountriesCommand.ExecuteReader();
                var countries = new List<Country>();
                while (countriesReader.Read())
                {
                    countries.Add(new Country
                    {
                        Id = (int)countriesReader["Id"],
                        Name = (string)countriesReader["Name"]
                    });
                }
                countriesReader.Close();
                ViewBag.Countries = new SelectList(countries, "Id", "Name");

                // retrieve agency workers
                ViewBag.AgencyWorkers = new List<AgencyWorker>();
                if (id != null)
                {
                    var getAgencyWorkersCommand = new MySqlCommand(
                        "SELECT Id, FullName " +
                        "FROM AgencyWorkers " +
                        "WHERE Id IN (" +
                        "   SELECT StationaryWorkerId " +
                        "   FROM StationaryWorkersToOperations " +
                        "   WHERE AccessLevelId IN (" +
                        "       SELECT Id " +
                        "       FROM AccessLevels " +
                        $"       WHERE CountryId = {id}" +
                        "   )" +
                        ");", _connection);
                    var agencyWorkersReader = getAgencyWorkersCommand.ExecuteReader();
                    while (agencyWorkersReader.Read())
                    {
                        ViewBag.AgencyWorkers.Add(new AgencyWorker
                        {
                            Id = (int)agencyWorkersReader["Id"],
                            FullName = (string)agencyWorkersReader["FullName"],
                        });
                    }
                    agencyWorkersReader.Close();
                }
                
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }
        
        // GET: Queries/B1?param=2
        public ActionResult B1(string? param)
        {
            var workers = new List<Tuple<AgencyWorker,AgencyWorker>>();
            if (param == null)
                return View(workers);
            
            ViewBag.Num = param;
            
            // try to retrieve the number
            int num;
            try
            {
                num = int.Parse(param);
            }
            catch
            {
                ViewBag.ErrorMessage = "Необхідно ввести ціле число";
                return View(workers);
            }
            
            // retrieve agency workers
            try
            {
                _connection.Open();
                var getAgencyWorkersCommand = new MySqlCommand(
                    "SELECT W1.Id, W1.FullName, W2.Id, W2.FullName " +
                    "FROM AgencyWorkers W1, AgencyWorkers W2 " +
                    "WHERE W1.Id < W2.Id " +
                    "AND W1.Id IN (" +
                    "   SELECT Id" +
                    "   FROM Agents" +
                    "   ) " +
                    "AND W2.Id IN (" +
                    "   SELECT Id " +
                    "   FROM Agents" +
                    "   ) " +
                    "AND NOT EXISTS(" +
                    "   SELECT * " +
                    "   FROM AgentIntelligenceActivities " +
                    "   WHERE AgentId = W1.Id" +
                    "   AND OperationId NOT IN (" +
                    "       SELECT OperationId " +
                    "       FROM AgentIntelligenceActivities " +
                    "       WHERE AgentId = W2.Id" +
                    "       )" +
                    "   )" +
                    "AND NOT EXISTS(" +
                    "   SELECT *" +
                    "   FROM AgentIntelligenceActivities " +
                    "   WHERE AgentId = W2.Id" +
                    "   AND OperationId NOT IN (" +
                    "       SELECT OperationId " +
                    "       FROM AgentIntelligenceActivities " +
                    "       WHERE AgentId = W1.Id" +
                    "       )" +
                    "   )" +
                    "AND (" +
                    "   SELECT COUNT(DISTINCT OperationId)" +
                    "   FROM AgentIntelligenceActivities " +
                    "   WHERE AgentId = W1.Id" +
                    $") >= {num};",
                    _connection);
                var agencyWorkersReader = getAgencyWorkersCommand.ExecuteReader();
                while (agencyWorkersReader.Read())
                {
                    workers.Add(new Tuple<AgencyWorker, AgencyWorker>
                    (
                        new AgencyWorker
                        {
                            Id = (int)agencyWorkersReader.GetValue(0),
                            FullName = (string)agencyWorkersReader.GetValue(1)
                        },
                        new AgencyWorker
                        {
                            Id = (int)agencyWorkersReader.GetValue(2),
                            FullName = (string)agencyWorkersReader.GetValue(3)
                        }
                    )); 
                }
                agencyWorkersReader.Close();
                
                return View(workers);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }
        
        // GET: Queries/B2?id=2
        public ActionResult B2(int? id)
        {
            try
            {
                _connection.Open();

                // retrieve agency workers
                var getAgencyWorkersCommand = new MySqlCommand("SELECT * FROM AgencyWorkers;", _connection);
                var agencyWorkersReader = getAgencyWorkersCommand.ExecuteReader();
                var agencyWorkers = new List<AgencyWorker>();
                while (agencyWorkersReader.Read())
                {
                    agencyWorkers.Add(new AgencyWorker()
                    {
                        Id = (int)agencyWorkersReader["Id"],
                        FullName = (string)agencyWorkersReader["FullName"]
                    });
                }

                agencyWorkersReader.Close();
                ViewBag.AgencyWorkers = new SelectList(agencyWorkers, "Id", "FullName");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }

            ViewBag.StationaryWorkers = new List<AgencyWorker>();
            
            if (id == null)
                return View();
            
            try
            {
                // retrieve stationary workers (as agency workers)
                var getAgencyWorkersCommand = new MySqlCommand(
                    "SELECT Id, FullName " +
                    "FROM AgencyWorkers " +
                    "WHERE NOT EXISTS(" +
                    "   SELECT * " +
                    "   FROM StationaryWorkersToOperations R1" +
                    $"   WHERE StationaryWorkerId = {id} " +
                    "   AND NOT EXISTS(" +
                    "       SELECT * " +
                    "       FROM StationaryWorkersToOperations " +
                    "       WHERE StationaryWorkerId = AgencyWorkers.Id " +
                    "       AND AccessLevelId = R1.AccessLevelId" +
                    "       )" +
                    "   )", _connection);
                var agencyWorkersReader = getAgencyWorkersCommand.ExecuteReader();
                while (agencyWorkersReader.Read())
                {
                    ViewBag.StationaryWorkers.Add(new AgencyWorker
                    {
                        Id = (int)agencyWorkersReader["Id"],
                        FullName = (string)agencyWorkersReader["FullName"],
                    });
                }
                agencyWorkersReader.Close();
                
                
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }
        
        public ActionResult B3(int? id)
        {
            try
            {
                _connection.Open();

                // retrieve operations
                var getOperationsCommand = new MySqlCommand("SELECT * FROM Operations", _connection);
                var operationsReader = getOperationsCommand.ExecuteReader();
                var operations = new List<Operation>();
                while (operationsReader.Read())
                {
                    operations.Add(new Operation
                    {
                        Id = (int)operationsReader["Id"],
                        Name = (string)operationsReader["Name"]
                    });
                }
                ViewBag.Operations = new SelectList(operations, "Id", "Name");
                operationsReader.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }

            ViewBag.Agencies = new List<Agency>();

            if (id == null)
                return View();
            
            // retrieve agencies
            try
            {
                var getAgenciesCommand = new MySqlCommand(
                    "SELECT Id, Name " +
                    "FROM Agencies A " +
                    "WHERE NOT EXISTS(" +
                    "   SELECT * " +
                    "   FROM OperationConduction C1 " +
                    $"   WHERE OperationId = {id} " +
                    "   AND NOT EXISTS(" +
                    "       SELECT * " +
                    "       FROM OperationConduction C2 " +
                    "       WHERE C2.CountryId = C1.CountryId" +
                    "       AND C2.AgencyId = A.Id" +
                    "       )" +
                    "   )", _connection);
                var agenciesReader = getAgenciesCommand.ExecuteReader();
                while (agenciesReader.Read())
                {
                    ViewBag.Agencies.Add(new Agency
                    {
                        Id = (int)agenciesReader["Id"],
                        Name = (string)agenciesReader["Name"]
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            return View();
        }
    }
}