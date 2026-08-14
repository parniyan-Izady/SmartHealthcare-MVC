using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Doctors.Queries.GetDoctorById;

public record GetDoctorByIdQuery(Guid Id) : IQuery<DoctorResponse?>;
