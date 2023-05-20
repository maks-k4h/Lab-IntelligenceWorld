using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Lab2_IntelligenceAgencies.Controllers;

public class TestController : Controller
{
    private MySqlConnection _connection;
    
    public TestController(MySqlConnection connection)
    {
        _connection = connection;
    }
    
    public IActionResult Index()
    {
        string content = "";
        
        _connection.Open();
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM Agencies;";
        using MySqlDataReader dr = command.ExecuteReader();
        content += "Agencies:\n\n";
        while (dr.Read())
            content += dr["Name"] + "\n";

        return Content(content);
    }
}
