using System.Net;
using Microsoft.AspNetCore.Mvc;
using PKolos.Models;

namespace PKolos.Controllers;

[ApiController]
[Route("api/prescriptions")]
public class PrescriptionController : ControllerBase {
    private readonly IConfiguration configuration;
    private IPrescriptionService prescriptionService;

    public PrescriptionController(IConfiguration configuration, IPrescriptionService prescriptionService) {
        this.configuration = configuration;
        this.prescriptionService = prescriptionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPrescriptions() {
        var results = await prescriptionService.GetPrescriptions();
        if (!results.Any())
            return NotFound("No prescriptions");

        return Ok(results);
    }
    
    [HttpGet("doctorsLastName::string")]
    public async Task<IActionResult> GetPrescriptionWithDoctorsLastName(string doctorsLastName) {
        var results = await prescriptionService.GetPrescriptions(doctorsLastName);
        if (!results.Any())
            return NotFound("No prescriptions for doctor with such last name");
        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody]AddPrescription prescription) {
        var idPrescription = await prescriptionService.AddPrescription(prescription);
        if(idPrescription == -1)
            return BadRequest("DueDate has to be after Date");
        if (idPrescription == -2)
            return NotFound("IdDoctor or IdPatient not in database");

        var result = await prescriptionService.GetPrescription(idPrescription);
        return StatusCode(201,result);
    }
}
