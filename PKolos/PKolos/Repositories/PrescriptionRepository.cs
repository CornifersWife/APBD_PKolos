using Microsoft.Data.SqlClient;
using PKolos.Models;

namespace PKolos;

public interface IPrescriptionRepository {
    public Task<Prescription> GetPrescription(int idPrescription);
    public Task<IEnumerable<GottenPrescription>> GetPrescriptions();
    public Task<IEnumerable<GottenPrescription>> GetPrescriptions(string doctorsLastName);
    public Task<int> AddPrescription(AddPrescription addPrescription);
}

public class PrescriptionRepository : IPrescriptionRepository {
    private readonly IConfiguration configuration;

    public PrescriptionRepository(IConfiguration configuration) {
        this.configuration = configuration;
    }

    public async Task<Prescription> GetPrescription(int idPrescription) {
        await using var con = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText =
            "SELECT IdPrescription,Date,DueDate,IdPatient,IdDoctor FROM Prescription WHERE IdPrescription = @1";
        cmd.Parameters.AddWithValue("@1", idPrescription);


        var dr = await cmd.ExecuteReaderAsync();
        if (!await dr.ReadAsync()) return null;
        var prescription = new Prescription() {
            IdPrescription = (int)dr["IdPrescription"],
            Date = (DateTime)dr["Date"],
            DueDate = (DateTime)dr["DueDate"],
            IdPatient = (int)dr["IdPatient"],
            IdDoctor = (int)dr["IdDoctor"]
        };
        return prescription;
    }

    public async Task<IEnumerable<GottenPrescription>> GetPrescriptions() {
        await using var con = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText =
            "SELECT " +
            "Prescription.IdPrescription, " +
            "Prescription.Date, " +
            "Prescription.DueDate, " +
            "Patient.LastName AS PatientLastName, " +
            "Doctor.LastName As DoctorLastName " +
            "FROM Prescription " +
            "JOIN Patient ON Prescription.IdPatient = Patient.IdPatient " +
            "JOIN Doctor ON Prescription.IdDoctor = Doctor.IdDoctor " +
            "ORDER BY Prescription.Date DESC; ";

        var dr = await cmd.ExecuteReaderAsync();
        var results = new List<GottenPrescription>();
        while (await dr.ReadAsync()) {
            results.Add(new GottenPrescription() {
                IdPrescription = (int)dr["IdPrescription"],
                Date = (DateTime)dr["Date"],
                DueDate = (DateTime)dr["DueDate"],
                PatientLastName = dr["PatientLastName"].ToString(),
                DoctorLastName = dr["DoctorLastName"].ToString()
            });
        }

        return results;
    }


    public async Task<IEnumerable<GottenPrescription>> GetPrescriptions(string doctorsLastName) {
        await using var con = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText =
            "SELECT " +
            "Prescription.IdPrescription, " +
            "Prescription.Date, " +
            "Prescription.DueDate, " +
            "Patient.LastName AS PatientLastName, " +
            "Doctor.LastName As DoctorLastName " +
            "FROM Prescription " +
            "JOIN Patient ON Prescription.IdPatient = Patient.IdPatient " +
            "JOIN Doctor ON Prescription.IdDoctor = Doctor.IdDoctor " +
            "WHERE Doctor.LastName = @1 " +
            "ORDER BY Prescription.Date DESC; ";
        cmd.Parameters.AddWithValue("@1", doctorsLastName);

        var dr = await cmd.ExecuteReaderAsync();
        var results = new List<GottenPrescription>();
        while (await dr.ReadAsync()) {
            results.Add(new GottenPrescription() {
                IdPrescription = (int)dr["IdPrescription"],
                Date = (DateTime)dr["Date"],
                DueDate = (DateTime)dr["DueDate"],
                PatientLastName = dr["PatientLastName"].ToString(),
                DoctorLastName = dr["DoctorLastName"].ToString()
            });
        }

        return results;
    }

    public async Task<int> AddPrescription(AddPrescription addPrescription) {
        await using var con = new SqlConnection(configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText =
            "INSERT INTO Prescription(Date, DueDate, IdPatient, IdDoctor) " +
            "OUTPUT INSERTED.IdPrescription " +
            "VALUES (@1, @2, @3, @4)";
        cmd.Parameters.AddWithValue("@1", addPrescription.Date);
        cmd.Parameters.AddWithValue("@2", addPrescription.DueDate);
        cmd.Parameters.AddWithValue("@3", addPrescription.IdPatient);
        cmd.Parameters.AddWithValue("@4", addPrescription.IdDoctor);


        var idPrescription = (int)await cmd.ExecuteScalarAsync();
        
        return idPrescription;
    }
}