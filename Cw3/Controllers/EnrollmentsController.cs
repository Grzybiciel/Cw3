using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs.Requests;
using Cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _service;
        public EnrollmentsController(IStudentDbService service)
        {
            _service = service;
        }

        [Route("api/enrollments")]
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var response = _service.EnrollStudent(request);


            //...

            return Ok(response);
        }

        [Route("api/enrollments/promotions")]
        [HttpPost]
        public IActionResult PromoteStudent(PromotionRequest request)
        {
            var response = _service.PromoteStudent(request);

            return Ok(response);
        }
    }
}