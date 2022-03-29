using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestApi.Data;
using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        private readonly IDbContextFactory<Data.PersonaContext> _contextFactory;
        string constr = "Data Source=DESKTOP-VSACAM;Initial Catalog=WPFCrud; integrated security=True;MultipleActiveResultSets=True;";

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Models.Persona>>> cargarData()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://rickandmortyapi.com/api/character/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var rnd = new Random();
            var rndNumbersToUrl = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                if (i == 100)
                {
                    rndNumbersToUrl.Append(rnd.Next(0, 670));
                }
                else
                {
                    rndNumbersToUrl.Append(rnd.Next(0, 670));
                    rndNumbersToUrl.Append(',');
                }
            }
            var url = rndNumbersToUrl.ToString();
            var response = await client.GetAsync(url);
            string json;
            using (var content = response.Content)
            {
                if (response.IsSuccessStatusCode)
                {
                    json = await content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            List<CharacterModel>  lista = JsonConvert.DeserializeObject<List<CharacterModel>>(json);
            int result = saveDB(lista);
            if (result > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }

        private int saveDB(List<CharacterModel> lista)
        {
          
            foreach (var item in lista)
            {
                using (var context = new PersonaContext())
                {
                    using (var db = new PersonaContext())
                    {
                        var person = new Persona {
                            id = item.Id,
                            name = item.Name,
                            image = item.Image,
                            status = item.Status,
                            species = item.Species,
                            cantApariciones = item.episode.Count
                        };
                        db.Persona.Add(person);
                        db.SaveChanges();
                    }
                }


                /*using (SqlConnection con = new SqlConnection(constr))
                {
                    //inserting Patient data into database
                    string query = "insert into Persona values (@id, @name, @image, @status , @species, @cantApariciones)";
                    using SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@id", item.Id);
                    cmd.Parameters.AddWithValue("@name", item.Name);
                    cmd.Parameters.AddWithValue("@image", item.Image);
                    cmd.Parameters.AddWithValue("@status", item.Status);
                    cmd.Parameters.AddWithValue("@species", item.Species);
                    cmd.Parameters.AddWithValue("@cantApariciones", item.episode.Count);
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    
                    con.Close();
                }*/
            }

            return 1;
            
        }

        [HttpGet]
        public async Task<ActionResult<ListPersona>> GetList()
        {
            ListPersona listPersona = new ListPersona();
            List<Models.Persona> personsV = new List<Models.Persona>();
            List<Models.Persona> personsM = new List<Models.Persona>();
            int cantV = 0;
            int cantM = 0;
            string query = "SELECT * FROM Persona";
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
                            
                            if (Convert.ToString(sdr["status"]).Equals("Alive")) {
                                
                                personsV.Add(new Models.Persona
                                {
                                    id = Convert.ToString(sdr["id"]),
                                    name = Convert.ToString(sdr["name"]),
                                    image = Convert.ToString(sdr["image"]),
                                    status = Convert.ToString(sdr["status"]),
                                    species = Convert.ToString(sdr["species"]),
                                    cantApariciones = Convert.ToInt32(sdr["cantApariciones"]),
                                });

                                listPersona.listaVivos = personsV;

                                cantV++;
                            } else if (Convert.ToString(sdr["status"]).Equals("Dead")) {
                                
                                personsM.Add(new Models.Persona
                                {
                                    id = Convert.ToString(sdr["id"]),
                                    name = Convert.ToString(sdr["name"]),
                                    image = Convert.ToString(sdr["image"]),
                                    status = Convert.ToString(sdr["status"]),
                                    species = Convert.ToString(sdr["species"]),
                                    cantApariciones = Convert.ToInt32(sdr["cantApariciones"]),
                                });

                                listPersona.listaMuertos = personsM;
                                cantM++;
                            }
                        }

                    }

                    listPersona.cantidadM = cantM;
                    listPersona.cantidadV = cantV;

                    con.Close();
                }
            }

            return listPersona;
        }
    }
}
