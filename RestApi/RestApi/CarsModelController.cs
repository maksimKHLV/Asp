using Microsoft.AspNetCore.Mvc;
namespace RestWebApi.Controllers;
    

{
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class CarsModelController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok("Hello from CardModelController");
        }

        [HttpGet]
        public IActionResult GetUser([FromHeader] string id)
        {
            return Ok($"Hello from GetCar {id}\n");
        }

        [HttpPost]
        public IActionResult Post([FromBody] CarsModelController user)
        {
            return Ok($"Hello from GetUser {CarsModel.Id}\n{user.Name}\n{user.Age}");
        }


    }
}
