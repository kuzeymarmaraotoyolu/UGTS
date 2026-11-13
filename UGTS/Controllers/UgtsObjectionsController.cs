using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using UGTS.Models;

[Route("api/v1/objections")]
[ApiController]
public class UgtsObjectionsController : ControllerBase
{
    private readonly string _connectionString;

    public UgtsObjectionsController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    [HttpPost]
    [Authorize]
    public IActionResult ReceiveObjection([FromBody] ObjectionNoficationModel model)
    {
        var externalRefNo = Guid.NewGuid();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                string query = @"INSERT INTO objections (
                                    id, objection_id, ugts_reference_no, transaction_reference_no, 
                                    customer_name, email, telephone_number, plate, 
                                    objection_status, objection_date, description, company_id
                                 ) VALUES (
                                    @id, @objection_id, @ugts_reference_no, @transaction_reference_no, 
                                    @customer_name, @email, @telephone_number, @plate, 
                                    @objection_status, @objection_date, @description, @company_id)";

                using var cmd = new NpgsqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Uuid, externalRefNo);
                cmd.Parameters.AddWithValue("@objection_id", NpgsqlDbType.Uuid, Guid.Parse(model.objection_id));
                cmd.Parameters.AddWithValue("@ugts_reference_no", NpgsqlDbType.Uuid, Guid.Parse(model.ugts_reference_no));
                cmd.Parameters.AddWithValue("@transaction_reference_no", NpgsqlDbType.Varchar, model.transaction_reference_no);
                cmd.Parameters.AddWithValue("@customer_name", NpgsqlDbType.Varchar, model.customer_name);
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, model.email);
                cmd.Parameters.AddWithValue("@telephone_number", NpgsqlDbType.Varchar, model.telephone_number);
                cmd.Parameters.AddWithValue("@plate", NpgsqlDbType.Varchar, model.plate);
                cmd.Parameters.AddWithValue("@objection_status", NpgsqlDbType.Varchar, model.objection_status);
                cmd.Parameters.AddWithValue("@objection_date", NpgsqlDbType.Timestamp, DateTime.Parse(model.objection_date));
                cmd.Parameters.AddWithValue("@description", NpgsqlDbType.Varchar, model.description);

                var clientIdFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                int companyId = clientIdFromToken == "ugts_client_asia" ? 216 : 212;

                cmd.Parameters.AddWithValue("@company_id", NpgsqlDbType.Integer, companyId);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Veritabanı kayıt hatası oluştu.", error = ex.Message });
            }
        }

        return Accepted(new { externalReferenceNo = externalRefNo });
    }
}