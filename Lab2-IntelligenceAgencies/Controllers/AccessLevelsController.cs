using Lab2_IntelligenceAgencies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;

namespace Lab2_IntelligenceAgencies.Controllers
{
    public class AccessLevelsController : Controller
    {
        private MySqlConnection _connection;

        public AccessLevelsController(MySqlConnection connection)
        {
            _connection = connection;
        }
        
        
        // GET: AccessLevels
        public ActionResult Index()
        {
            try
            {
                _connection.Open();

                var command = _connection.CreateCommand();
                command.CommandText = "SELECT * FROM AccessLevels";

                using var reader = command.ExecuteReader();
                var res = new List<AccessLevel>();

                while (reader.Read())
                {
                    var a = new AccessLevel
                    {
                        Id = (int)reader["Id"],
                        CountryId = (int)reader["CountryId"],
                        Name = (string)reader["Name"],
                        Description = (string)reader["Description"]
                    };
                    
                    res.Add(a);
                }
                
                reader.Close();
        
                foreach (var al in res)
                {
                    command = _connection.CreateCommand();
                    command.CommandText = $"SELECT * From Countries WHERE Countries.Id = {al.CountryId};";
                    var cReader = command.ExecuteReader();
                    cReader.Read();
                    al.Country = new Country
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

        // GET: AccessLevels/Create
        public ActionResult Create()
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = "SELECT * FROM Countries;";
                var reader = command.ExecuteReader();
                var countries = new List<Country>();
                while (reader.Read())
                {
                    countries.Add(new Country
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    });
                }

                ViewBag.CountriesId = new SelectList(countries, "Id", "Name");
                
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: AccessLevels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccessLevel accessLevel)
        {
            try
            {
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText =
                    $"INSERT INTO AccessLevels (CountryId, Name, Description) " +
                    $"VALUES ({accessLevel.CountryId}, \"{accessLevel.Name}\", \"{accessLevel.Description}\");";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Create");
            }
        }

        // GET: AccessLevels/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                // retrieve access level
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM AccessLevels WHERE Id = {id}";
                var reader = command.ExecuteReader();
                reader.Read();
                var accessLevel = new AccessLevel
                {
                    Id = (int)reader["Id"],
                    CountryId = (int)reader["CountryId"],
                    Name = (string)reader["Name"],
                    Description = (string)reader["Description"]
                };
                reader.Close();
                
                // retrieve all countries
                command = _connection.CreateCommand();
                command.CommandText = "SELECT * FROM Countries;";
                reader = command.ExecuteReader();
                var countries = new List<Country>();
                while (reader.Read())
                {
                    countries.Add(new Country
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    });
                }

                ViewBag.CountriesId = new SelectList(countries, "Id", "Name");
                
                return View(accessLevel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: AccessLevels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AccessLevel accessLevel)
        {
            try
            {
                if (id != accessLevel.Id)
                    throw new Exception();
                
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"UPDATE AccessLevels SET " +
                                      $"Name = \"{accessLevel.Name}\", " +
                                      $"Description = \"{accessLevel.Description}\", " +
                                      $"CountryId = {accessLevel.CountryId} " +
                                      $"WHERE Id = {accessLevel.Id};";
                if (command.ExecuteNonQuery() == 0)
                    throw new Exception();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Edit");
            }
        }

        // GET: AccessLevels/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                // retrieve access level
                _connection.Open();
                var command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM AccessLevels WHERE Id = {id}";
                var reader = command.ExecuteReader();
                reader.Read();
                var accessLevel = new AccessLevel
                {
                    Id = (int)reader["Id"],
                    CountryId = (int)reader["CountryId"],
                    Name = (string)reader["Name"],
                    Description = (string)reader["Description"]
                };
                reader.Close();
                
                // retrieve the country
                command = _connection.CreateCommand();
                command.CommandText = $"SELECT * FROM Countries WHERE Id = {accessLevel.CountryId};";
                reader = command.ExecuteReader();
                reader.Read();
                accessLevel.Country = new Country
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"]
                };
                
                return View(accessLevel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // POST: AccessLevels/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _connection.Open();
                var command = new MySqlCommand($"DELETE FROM AccessLevels WHERE Id = {id}", _connection);

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