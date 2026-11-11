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
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    [HttpPost]
    [Authorize] 
    public IActionResult ReceiveObjection([FromBody] ObjectionNoficationModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.ObjectionId) || string.IsNullOrEmpty(model.Plate))
        {
            return BadRequest(new { message = "Zorunlu alanlardan biri veya birkaçı eksik." });
        }

        var externalRefNo = $"EXT-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO objections (objection_id, ugts_reference_no, transaction_reference_no, customer_name, email, telephone_number, plate, objection_status, objection_date, description) 
                                 VALUES (@objection_id, @ugts_reference_no, @transaction_reference_no, @customer_name, @email, @telephone_number, @plate, @objection_status, @objection_date, @description)";


                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@objection_id", NpgsqlDbType.Uuid, Guid.TryParse(model.ObjectionId, out var parsedObjId) ? parsedObjId : Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@ugts_reference_no", NpgsqlDbType.Uuid, model.UgtsReferenceNo != null && Guid.TryParse(model.UgtsReferenceNo, out var parsedUgtsRef) ? parsedUgtsRef : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@transaction_reference_no", NpgsqlDbType.Varchar, (object)model.TransactionReferenceNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@customer_name", NpgsqlDbType.Varchar, (object)model.CustomerName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, (object)model.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@telephone_number", NpgsqlDbType.Varchar, (object)model.TelephoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@plate", NpgsqlDbType.Varchar, model.Plate);
                    cmd.Parameters.AddWithValue("@objection_status", NpgsqlDbType.Varchar, (object)model.ObjectionStatus ?? "RECEIVED");

                    DateTime objectionDateVal = DateTime.UtcNow;

                    if (!string.IsNullOrEmpty(model.ObjectionDate) && DateTime.TryParse(model.ObjectionDate, out var parsedDate))
                    {
                        objectionDateVal = parsedDate;
                    }
                    cmd.Parameters.AddWithValue("@objection_date", NpgsqlDbType.Timestamp, objectionDateVal);
                    cmd.Parameters.AddWithValue("@description", NpgsqlDbType.Varchar, (object)model.Description ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Veritabanı kayıt hatası oluştu.", error = ex.Message });
        }

        var response = new
        {
            externalReferenceNo = externalRefNo
        };

        return Accepted(response);
    }
}