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
    public class DepartmentHeadsController : Controller
    {
        private MySqlConnection _connection;

        public DepartmentHeadsController (MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: DepartmentHeads
        public ActionResult Index()
        {
            _connection.Open();
            if (_connection.State != ConnectionState.Open)
                return NotFound();

            // retrieve department heads
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM DepartmentHeads";
            using var reader = command.ExecuteReader();
            var res = new List<DepartmentHead>();
            while (reader.Read())
            {
                res.Add(new DepartmentHead
                {
                    Id = (int)reader["Id"], 
                    DepartmentId = (int)reader["DepartmentId"]
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

        // GET: DepartmentHeads/Create
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

        // POST: DepartmentHeads/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DepartmentHead head)
        {
            try
            {
                _connection.Open();

                var command = _connection.CreateCommand();
                command.CommandText = $"INSERT INTO DepartmentHeads (Id, DepartmentId) " +
                                      $"VALUES ({head.Id}, {head.DepartmentId});";
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

        // GET: DepartmentHeads/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                _connection.Open();
                
                // retrieve department head
                var getHeadCommand = _connection.CreateCommand();
                getHeadCommand.CommandText = $"SELECT * FROM DepartmentHeads WHERE Id = {id};";
                var headReader = getHeadCommand.ExecuteReader();
                headReader.Read();
                var head = new DepartmentHead
                {
                    Id = id,
                    DepartmentId = (int)headReader["DepartmentId"]
                };
                headReader.Close();
                
                // retrieve agency worker
                var getAgencyWorkerCommand = _connection.CreateCommand();
                getAgencyWorkerCommand.CommandText = $"SELECT * FROM AgencyWorkers WHERE Id = {head.Id}";
                var workerReader = getAgencyWorkerCommand.ExecuteReader();
                workerReader.Read();
                head.AgencyWorker = new AgencyWorker
                {
                    Id = head.Id,
                    FullName = (string)workerReader["FullName"]
                };
                workerReader.Close();
                
                // retrieve department
                var getDepartmentCommand = _connection.CreateCommand();
                getDepartmentCommand.CommandText = $"SELECT * FROM Departments WHERE Id = {head.DepartmentId}";
                var departmentReader = getDepartmentCommand.ExecuteReader();
                departmentReader.Read();
                head.Department = new Department
                {
                    Id = head.DepartmentId,
                    Name = (string)departmentReader["Name"],
                };
                departmentReader.Close();

                return View(head);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: DepartmentHeads/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM DepartmentHeads WHERE Id = {id}", _connection);

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
                $"SELECT COUNT(*) FROM DepartmentHeads WHERE Id = {id};";
            var count = command.ExecuteScalar() as Int64?;
            if (count == null)
                return Json("Не вдається перевірити валідність.");
            if (count > 0)
                return Json("Даний співробітник уже є головою департаменту.");
            return Json(true);
        }

        public JsonResult CheckDepartmentId(int departmentId)
        {
            _connection.Open();
            var command = _connection.CreateCommand();
            command.CommandText =
                $"SELECT COUNT(*) FROM DepartmentHeads WHERE DepartmentId = {departmentId};";
            var count = command.ExecuteScalar() as Int64?;
            if (count == null)
                return Json("Не вдається перевірити валідність.");
            if (count > 0)
                return Json("Даний департамент уже має головнокомандувача.");
            return Json(true);
        }
    }
}