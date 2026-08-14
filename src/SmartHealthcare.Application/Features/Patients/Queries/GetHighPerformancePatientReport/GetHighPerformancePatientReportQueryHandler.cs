using System.Data;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Patients.Queries.GetHighPerformancePatientReport;

/// <summary>
/// CQRS Read Model Query Handler executing optimized SQL query via ADO.NET connection factory.
/// </summary>
public class GetHighPerformancePatientReportQueryHandler : IQueryHandler<GetHighPerformancePatientReportQuery, IReadOnlyList<PatientReportDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetHighPerformancePatientReportQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<IReadOnlyList<PatientReportDto>> Handle(GetHighPerformancePatientReportQuery request, CancellationToken cancellationToken)
    {
        var reports = new List<PatientReportDto>();

        using var connection = _sqlConnectionFactory.CreateConnection();
        using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT 
                p.Id AS PatientId,
                u.FirstName + ' ' + u.LastName AS FullName,
                p.NationalCode,
                p.PhoneNumber,
                COUNT(a.Id) AS TotalAppointmentsCount,
                MAX(a.AppointmentStartUtc) AS LastAppointmentDateUtc
            FROM Patients p
            INNER JOIN Users u ON p.UserId = u.Id
            LEFT JOIN Appointments a ON a.PatientId = p.Id
            WHERE p.IsDeleted = 0
            GROUP BY p.Id, u.FirstName, u.LastName, p.NationalCode, p.PhoneNumber
            ORDER BY TotalAppointmentsCount DESC";

        if (connection.State != ConnectionState.Open)
            connection.Open();

        using var reader = await ((Microsoft.Data.SqlClient.SqlCommand)command).ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            reports.Add(new PatientReportDto(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetInt32(4),
                reader.IsDBNull(5) ? null : reader.GetDateTime(5)
            ));
        }

        return reports;
    }
}
