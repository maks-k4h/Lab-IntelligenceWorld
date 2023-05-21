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
    public class StationaryWorkersController : Controller
    {
        private MySqlConnection _connection;

        public StationaryWorkersController (MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: StationaryWorkers
        public ActionResult Index()
        {
            _connection.Open();
            if (_connection.State != ConnectionState.Open)
                return NotFound();

            // retrieve department heads
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM StationaryWorkers";
            using var reader = command.ExecuteReader();
            var res = new List<StationaryWorker>();
            while (reader.Read())
            {
                res.Add(new StationaryWorker
                {
                    Id = (int)reader["Id"], 
                    DepartmentId = (int)reader["DepartmentId"]
                });
            }
            reader.Close();
            
            // retrieve agency workers
            foreach (var worker in res)
            {
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {worker.Id}";
                var r = command.ExecuteReader();
                r.Read();
                worker.AgencyWorker = new AgencyWorker
                {
                    Id = (int)r["Id"],
                    FullName = (string)r["FullName"]
                };
                r.Close();
            }
            
            // retrieve departments
            foreach (var commander in res)
            {
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM Departments WHERE Id = {commander.DepartmentId}";
                var r = command.ExecuteReader();
                r.Read();
                commander.Department = new Department
                {
                    Id = (int)r["Id"],
                    Name = (string)r["Name"]
                };
                r.Close();
            }

            return View(res);
        }

        // GET: StationaryWorkers/Create
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
                
                // retrieve departments
                var getDepartmentsCommand = _connection.CreateCommand();
                getDepartmentsCommand.CommandText = "SELECT * FROM Departments";
                var departmentsReader = getDepartmentsCommand.ExecuteReader();
                var departments = new List<Department>();
                while (departmentsReader.Read())
                {
                    departments.Add(new Department
                    {
                        Id = (int)departmentsReader["Id"],
                        Name = (string)departmentsReader["Name"],
                    });
                }
                ViewBag.Departments = new SelectList(departments, "Id", "Name");
                departmentsReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return View();
        }

        // POST: StationaryWorkers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StationaryWorker worker)
        {
            try
            {
                _connection.Open();

                var command = _connection.CreateCommand();
                command.CommandText = $"INSERT INTO StationaryWorkers (Id, DepartmentId) " +
                                      $"VALUES ({worker.Id}, {worker.DepartmentId});";
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

        // GET: StationaryWorkers/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                _connection.Open();
                
                // retrieve the stationary worker
                var getWorkerCommand = _connection.CreateCommand();
                getWorkerCommand.CommandText = $"SELECT * FROM StationaryWorkers WHERE Id = {id};";
                var stationaryWorkerReader = getWorkerCommand.ExecuteReader();
                stationaryWorkerReader.Read();
                var stationaryWorker = new StationaryWorker
                {
                    Id = id,
                    DepartmentId = (int)stationaryWorkerReader["DepartmentId"]
                };
                stationaryWorkerReader.Close();
                
                // retrieve agency worker
                var getAgencyWorkerCommand = _connection.CreateCommand();
                getAgencyWorkerCommand.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {stationaryWorker.Id}";
                var workerReader = getAgencyWorkerCommand.ExecuteReader();
                workerReader.Read();
                stationaryWorker.AgencyWorker = new AgencyWorker
                {
                    Id = stationaryWorker.Id,
                    FullName = (string)workerReader["FullName"]
                };
                workerReader.Close();
                
                // retrieve department
                var getDepartmentCommand = _connection.CreateCommand();
                getDepartmentCommand.CommandText = $"SELECT * FROM Departments WHERE Id = {stationaryWorker.DepartmentId}";
                var departmentReader = getDepartmentCommand.ExecuteReader();
                departmentReader.Read();
                stationaryWorker.Department = new Department
                {
                    Id = stationaryWorker.DepartmentId,
                    Name = (string)departmentReader["Name"],
                };
                departmentReader.Close();

                return View(stationaryWorker);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: StationaryWorkers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM StationaryWorkers WHERE Id = {id}", _connection);

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
                $"SELECT COUNT(*) FROM StationaryWorkers WHERE Id = {id};";
            var count = command.ExecuteScalar() as Int64?;
            if (count == null)
                return Json("Не вдається перевірити валідність.");
            if (count > 0)
                return Json("Даний співробітник працює в іншому департаменті.");
            return Json(true);
        }
    }
}