using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Services.Database.Servicies;
using URLShortener.Services.Interfaces;
using URLShortener.Services.JWT;

namespace URLShortener.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
    }
}
