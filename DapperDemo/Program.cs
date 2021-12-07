using Dapper;
using DapperDemo.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DapperDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cs = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DapperCars;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            using var connection = new SqlConnection(cs);
            connection.Open();
            // verze
            var version = connection.ExecuteScalar<string>("SELECT @@VERSION");
            Console.WriteLine(version);

            // SELECT: ziskani zaznamu Car
            var cars = connection.Query<Car>("SELECT * FROM cars").ToList();
            cars.ForEach(car => Console.WriteLine(car));

            // UPDATE: prikaz k provedeni
            int nOfRows = connection.Execute("UPDATE dbo.[cars] SET [price] = 52000 WHERE [id] = 1");
            Console.WriteLine("'UPDATE' affected rows: {0}", nOfRows);
            cars = connection.Query<Car>("SELECT * FROM cars").ToList();
            cars.ForEach(car => Console.WriteLine(car));

            // DELETE
            /*
            int delRows = connection.Execute(@"DELETE FROM [cars] WHERE Id = @Id", new { Id = 4 });
            if (delRows > 0)
            {
                Console.WriteLine("car deleted");
            }*/

            // INSERT
            var query = "INSERT INTO cars(name, price) VALUES('Honda', 3000)";
            int res = connection.Execute(query);
            if (res > 0)
            {
                Console.WriteLine("Row inserted");
            }

            // parametrizovaný SELECT
            var car = connection.QueryFirst<Car>("SELECT * FROM cars WHERE id=@id", new { id = 3 });
            Console.WriteLine(car);

            // INSERT s parametry
            var query2 = "INSERT INTO cars(name, price) VALUES(@name, @value)";
            var dp = new DynamicParameters();
            dp.Add("@name","BMW", System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 255);
            dp.Add("@value", 36000);
            int res2 = connection.Execute(query2, dp);
            if (res2 > 0)
            {
                Console.WriteLine("Row inserted");
            }

            var rows = connection.Query("SELECT * FROM cars").AsList();
            foreach(var row in rows)
            {
                IDictionary<string, object> r = (IDictionary<string, object>)row;
                Console.WriteLine(r["id"]);
            }

        }
    }
}

// dotnet add package dapper
// dotnet add package System.Data.SqlClient

/*
CREATE TABLE cars (
    id INT identity(1,1) PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    price INT
)

INSERT INTO cars(name, price) VALUES('Audi', 52642);
INSERT INTO cars(name, price) VALUES('Mercedes', 57127);
INSERT INTO cars(name, price) VALUES('Skoda', 9000);
INSERT INTO cars(name, price) VALUES('Volvo', 29000);
INSERT INTO cars(name, price) VALUES('Bentley', 350000);
INSERT INTO cars(name, price) VALUES('Citroen', 21000);
INSERT INTO cars(name, price) VALUES('Hummer', 41400);
INSERT INTO cars(name, price) VALUES('Volkswagen', 21600);

*/

// https://github.com/DapperLib/Dapper