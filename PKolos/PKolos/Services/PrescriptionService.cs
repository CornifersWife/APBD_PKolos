using Microsoft.AspNetCore.Http.HttpResults;
using PKolos.Models;

namespace PKolos;

public interface IPrescriptionService {
    public Task<Prescription> GetPrescription(int idPrescription);
    public Task<IEnumerable<GottenPrescription>> GetPrescriptions();
    public Task<IEnumerable<GottenPrescription>> GetPrescriptions(string doctorsLastName);
    public Task<int> AddPrescription(AddPrescription addPrescription);
}

public class PrescriptionService : IPrescriptionService {
    private readonly IPrescriptionRepository prescriptionRepository;

    public PrescriptionService(IPrescriptionRepository prescriptionRepository) {
        this.prescriptionRepository = prescriptionRepository;
    }

    public async Task<Prescription> GetPrescription(int idPrescription) {
        var result = await prescriptionRepository.GetPrescription(idPrescription);
        return result;
    }

    public async Task<IEnumerable<GottenPrescription>> GetPrescriptions() {
        var results = await prescriptionRepository.GetPrescriptions();
        return results;
    }

    public async Task<IEnumerable<GottenPrescription>> GetPrescriptions(string doctorsLastName) {
        var results = await prescriptionRepository.GetPrescriptions(doctorsLastName);
        return results;
    }

    public async Task<int> AddPrescription(AddPrescription addPrescription) {
        if (addPrescription.DueDate <= addPrescription.Date)
            return -1;
        var idPrescription = await prescriptionRepository.AddPrescription(addPrescription);
        if (idPrescription == -1) {
            return -2;
        }

        var newPrescription = await prescriptionRepository.GetPrescription(idPrescription);
        
        return newPrescription.IdPrescription;
    }
}