using Microsoft.AspNetCore.Mvc;
using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        string constr = "Data Source=DESKTOP-VSACAM;Initial Catalog=WPFCrud; integrated security=True;MultipleActiveResultSets=True;";

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetAll()
        {
            List<Person> persons = new List<Person>();
            string query = "SELECT * FROM Person";
            using (var con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            persons.Add(new Person
                            {
                                Id = Convert.ToInt32(sdr["Id"]),
                                Name = Convert.ToString(sdr["Name"]),
                                Age = Convert.ToString(sdr["Age"])
                            });
                        }
                    }
                    con.Close();
                }
            }

            return persons;
        }

        [HttpGet("{id}")]
        public ActionResult<Person> Get(int id)
        {

            Person obj = new Person();
            string query = "SELECT * FROM Person where Id=" + id;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            obj = new Person
                            {
                                Id = Convert.ToInt32(sdr["Id"]),
                                Name = Convert.ToString(sdr["Name"]),
                                Age = Convert.ToString(sdr["Age"])
                            };
                        }
                    }
                    con.Close();
                }
            }
            if (obj == null)
            {
                return NotFound();
            }
            return obj;
        }

        [HttpPost]
        public async Task<ActionResult<Person>> Post(Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (SqlConnection con = new SqlConnection(constr))
            {
                //inserting Patient data into database
                string query = "insert into Person values (@Name, @Age)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Name", person.Name);
                    cmd.Parameters.AddWithValue("@Age", person.Age);
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return Ok();
                    }
                    con.Close();
                }
            }
            return BadRequest();

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                string query = "UPDATE Person SET Name = @Name, Age = @Age Where Id =@Id";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Name", person.Name);
                        cmd.Parameters.AddWithValue("@Age", person.Age);
                        cmd.Parameters.AddWithValue("@Id", person.Id);
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            return NoContent();
                        }
                        con.Close();
                    }
                }

            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "Delete FROM Person where Id='" + id + "'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return NoContent();
                    }
                    con.Close();
                }
            }
            return BadRequest();
        }
    }

}
