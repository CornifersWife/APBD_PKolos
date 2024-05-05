using System.ComponentModel.DataAnnotations;

namespace PKolos.Models;

public class GottenPrescription {
    [Required] public int IdPrescription { get; set; }
    [Required] public DateTime Date { get; set; }
    [Required] public DateTime DueDate { get; set; }
    [Required] public string PatientLastName { get; set; }
    [Required] public string DoctorLastName { get; set; }
}
