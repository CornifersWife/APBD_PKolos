using System.ComponentModel.DataAnnotations;

namespace PKolos.Models;

public class AddPrescription {
    [Required] public DateTime Date { get; set; }
    [Required] public DateTime DueDate { get; set; }
    [Required] public int IdPatient { get; set; }
    [Required] public int IdDoctor { get; set; }
}