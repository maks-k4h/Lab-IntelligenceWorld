using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab2_IntelligenceAgencies.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;

namespace Lab2_IntelligenceAgencies.Controllers
{
    public class QueriesController : Controller
    {
        private MySqlConnection _connection;

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
                            "FROM AgencyWorkers " +
                            "WHERE (" +
                            "   SELECT COUNT(DISTINCT OperationId) " +
                            "   FROM StationaryWorkersToOperations " +
                            "   WHERE StationaryWorkerId = AgencyWorkers.Id" +
                            $") >= {num};",
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
    }
}