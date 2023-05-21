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
    public class DepartmentsController : Controller
    {
        private MySqlConnection _connection;

        public DepartmentsController(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        // GET: Departments
        public ActionResult Index()
        {
            try
            {
                _connection.Open();

                var command = _connection.CreateCommand();
                command.CommandText = "SELECT * FROM Departments";

                using var reader = command.ExecuteReader();
                var res = new List<Department>();

                while (reader.Read())
                {
                    var a = new Department()
                    {
                        Id = (int)reader["Id"],
                        AgencyId = (int)reader["AgencyId"],
                        Name = (string)reader["Name"],
                    };
                    
                    res.Add(a);
                }
                
                reader.Close();
        
                foreach (var dep in res)
                {
                    command = _connection.CreateCommand();
                    command.CommandText = $"SELECT Id, Name From Agencies WHERE Id = {dep.AgencyId};";
                    var cReader = command.ExecuteReader();
                    cReader.Read();
                    dep.Agency = new Agency()
                    {
                        Id = (int)cReader["Id"],
                        Name = (string)cReader["Name"]
                    };
                    cReader.Close();
                }

                return View(res);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            try
            {
                // retrieve agencies
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = "SELECT * FROM Agencies;";
                var reader = command.ExecuteReader();
                var agencies = new List<Agency>();
                while (reader.Read())
                {
                    agencies.Add(new Agency
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    });
                }

                ViewBag.AgencyId = new SelectList(agencies, "Id", "Name");
                
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText =
                    $"INSERT INTO Departments (AgencyId, Name) " +
                    $"VALUES ({department.AgencyId}, \"{department.Name}\");";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Create");
            }
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                // retrieve department
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM Departments WHERE Id = {id}";
                var reader = command.ExecuteReader();
                reader.Read();
                var department = new Department
                {
                    Id = (int)reader["Id"],
                    AgencyId = (int)reader["AgencyId"],
                    Name = (string)reader["Name"],
                };
                reader.Close();
                
                // retrieve all agencies
                command = _connection.CreateCommand();
                command.CommandText = "SELECT * FROM Agencies;";
                reader = command.ExecuteReader();
                var agencies = new List<Agency>();
                while (reader.Read())
                {
                    agencies.Add(new Agency()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    });
                }

                ViewBag.AgencyId = new SelectList(agencies, "Id", "Name");
                
                return View(department);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Department department)
        {
            try
            {
                if (id != department.Id)
                    throw new Exception();
                
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"UPDATE Departments SET " +
                                      $"Name = \"{department.Name}\", " +
                                      $"AgencyId = {department.AgencyId} " +
                                      $"WHERE Id = {department.Id};";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Edit");
            }
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                // retrieve the department
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM Departments WHERE Id = {id}";
                var reader = command.ExecuteReader();
                reader.Read();
                var department = new Department
                {
                    Id = (int)reader["Id"],
                    AgencyId = (int)reader["AgencyId"],
                    Name = (string)reader["Name"]
                };
                reader.Close();
                
                // retrieve department's agency
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT Id, Name FROM Agencies WHERE Id = {department.AgencyId};";
                reader = command.ExecuteReader();
                reader.Read();
                department.Agency = new Agency
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"]
                };
                
                return View(department);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: Departments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM Departments WHERE Id = {id}", _connection);

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
    }
}