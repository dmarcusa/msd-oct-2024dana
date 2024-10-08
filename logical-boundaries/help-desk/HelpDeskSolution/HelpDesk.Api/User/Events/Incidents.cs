namespace HelpDesk.Api.User.Events;

public record EmployeeLoggedIncident(Guid Id, Guid EmployeeId, Guid SoftwareId, string Description);

public record EmployeeCancelledIncident(Guid Id);