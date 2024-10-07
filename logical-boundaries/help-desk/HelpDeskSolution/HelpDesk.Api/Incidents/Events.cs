namespace HelpDesk.Api.Incidents;

//Employee Logs a New Incident: Id, Employee Id, Software Id, Description

public record EmployeeLoggedIncident(Guid Id, Guid EmployeeId, Guid SoftwareId, string Description);

public record EmployeeCancelledIncident(Guid Id);

