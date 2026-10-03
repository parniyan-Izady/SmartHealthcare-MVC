using SmartHealthcare.Application.Common.CQRS;

namespace SmartHealthcare.Application.Features.Patients.Queries.GetPatientsCount;

public record GetPatientsCountQuery : IQuery<int>;
