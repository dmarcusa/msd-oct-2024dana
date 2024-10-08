namespace HelpDesk.Api.TierOneSupport.Events;

public record IncidentContactRecorded(Guid Id, Guid TierOneTechId, string Note);