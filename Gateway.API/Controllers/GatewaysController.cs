using Microsoft.AspNetCore.Mvc;
using Gateways.API.Models;

namespace Gateways.API.Controllers
{
    [HttpPost]
    public async Task<ActionResult<Gateway>> PostPatient(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetPatient", new { id = patient.Id }, patient);
    }
}
