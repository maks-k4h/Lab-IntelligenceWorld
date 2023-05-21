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
    public class OperationConductionController : Controller
    {
        private MySqlConnection _connection;

        public OperationConductionController(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: OperationConduction
        public ActionResult Index()
        {
             _connection.Open();
            if (_connection.State != ConnectionState.Open)
                return NotFound();

            // retrieve records
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM OperationConduction";
            using var reader = command.ExecuteReader();
            var res = new List<OperationConduction>();
            while (reader.Read())
            {
                res.Add(new OperationConduction()
                {
                    AgencyId = (int)reader["AgencyId"],
                    CountryId = (int)reader["CountryId"],
                    OperationId = (int)reader["OperationId"],
                });
            }
            reader.Close();
            
            // retrieve countries
            foreach (var record in res)
            {
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM Countries WHERE Id = {record.CountryId}";
                var r = command.ExecuteReader();
                r.Read();
                record.Country = new Country()
                {
                    Id = (int)r["Id"],
                    Name = (string)r["Name"]
                };
                r.Close();
            }
            
            // retrieve agencies
            foreach (var record in res)
            {
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM Agencies WHERE Id = {record.AgencyId}";
                var r = command.ExecuteReader();
                r.Read();
                record.Agency = new Agency()
                {
                    Id = (int)r["Id"],
                    Name = (string)r["Name"]
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

            return View(res);
        }

        // GET: OperationConduction/Create
        public ActionResult Create(string? error, int? countryId, int? agencyId, int? operationId)
        {
            try
            {
                _connection.Open();
                
                // retrieve countries
                var getCountriesCommand = _connection.CreateCommand();
                getCountriesCommand.CommandText = "SELECT * FROM Countries";
                var countriesReader = getCountriesCommand.ExecuteReader();
                var countries = new List<Country>();
                while (countriesReader.Read())
                {
                    countries.Add(new Country()
                    {
                        Id = (int)countriesReader["Id"],
                        Name = (string)countriesReader["Name"]
                    });
                }
                countriesReader.Close();
                ViewBag.Countries = new SelectList(countries, "Id", "Name");
                
                // retrieve agencies
                var getAgenciesCommand = _connection.CreateCommand();
                getAgenciesCommand.CommandText = "SELECT * FROM Agencies";
                var agenciesReader = getAgenciesCommand.ExecuteReader();
                var agencies = new List<Agency>();
                while (agenciesReader.Read())
                {
                    agencies.Add(new Agency
                    {
                        Id = (int)agenciesReader["Id"],
                        Name = (string)agenciesReader["Name"],
                    });
                }
                agenciesReader.Close();
                ViewBag.Agencies = new SelectList(agencies, "Id", "Name");
                
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
                operationsReader.Close();
                ViewBag.Operations = new SelectList(operations, "Id", "Name");

                // error
                ViewBag.ErrorMessage = error;
                
                // defaults
                ViewBag.CountryId = countryId;
                ViewBag.AgencyId = agencyId;
                ViewBag.OperationId = operationId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            return View();
        }

        // POST: OperationConduction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OperationConduction record)
        {
            try
            {
                _connection.Open();
                
                // check if such record already exists
                var checkCommand = _connection.CreateCommand();
                checkCommand.CommandText = $"SELECT COUNT(*) FROM OperationConduction " +
                                           $"WHERE CountryId = {record.CountryId} " +
                                           $"AND AgencyId = {record.AgencyId} " +
                                           $"AND OperationId = {record.OperationId};";
                if ((Int64)checkCommand.ExecuteScalar() > 0)
                    return RedirectToAction("Create", new
                    {
                        error = "Такий запис уже існує",
                        countryId = record.CountryId,
                        agencyId = record.AgencyId,
                        operationId = record.OperationId,
                    });
                
                var command = _connection.CreateCommand();
                command.CommandText = $"INSERT INTO OperationConduction (CountryId, AgencyId, OperationId) " +
                                      $"VALUES ({record.CountryId}, {record.AgencyId}, {record.OperationId});";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OperationConduction/Delete
        public ActionResult Delete(int countryId, int agencyId, int operationId, string? error)
        {
            try
            {
                _connection.Open();

                var record = new OperationConduction
                {
                    AgencyId = agencyId,
                    CountryId = countryId,
                    OperationId = operationId,
                };

                // retrieve the country
                var getCountryCommand = _connection.CreateCommand();
                getCountryCommand.CommandText = $"SELECT * FROM Countries WHERE Id = {countryId}";
                var countryReader = getCountryCommand.ExecuteReader();
                countryReader.Read();
                record.Country = new Country
                {
                    Id = countryId,
                    Name = (string)countryReader["Name"]
                };
                countryReader.Close();
                
                // retrieve the agency
                var getAgencyCommand = _connection.CreateCommand();
                getAgencyCommand.CommandText = $"SELECT * FROM Agencies WHERE Id = {agencyId}";
                var agencyReader = getAgencyCommand.ExecuteReader();
                agencyReader.Read();
                record.Agency = new Agency
                {
                    Id = agencyId,
                    Name = (string)agencyReader["Name"]
                };
                agencyReader.Close();
                
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

        // POST: OperationConduction/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int countryId, int agencyId, int operationId, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM OperationConduction " +
                                               $"WHERE CountryId = {countryId} " +
                                               $"AND AgencyId = {agencyId} " +
                                               $"AND OperationId = {operationId};", _connection);
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
                    countryId = countryId,
                    agencyId = agencyId,
                    operationId = operationId
                });
            }
        }
    }
}