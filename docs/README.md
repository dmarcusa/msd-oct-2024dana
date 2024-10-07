# Microservice Development

## Project

We are going to create an API for the Help Desk.

The frontend developers are working on an app where employees can submit incidents to the help desk for software that the company supports.

Employees will be able to create incidents related to particular software. They will describe their issue with the software.

A tier one help desk support person will monitor the incidents.

- They will contact the user that submits the incident.
    - If the the tier one support person can resolve the incident, it will be closed.
    - If the tier one support person cannot resolve the incident: 
        - They will determine if it is a "high priority" incident (e.g. production outage, it's the CEO, whatever)
        - They will assign it to a tech.

Once it is assigned to a tech it is now considered an "Issue".

Tier 2 techs can see a list of their assigned issues.

The software center wants to be notified of any issues for any piece of supported software.

Employees can see a list of all their incidents and issues.

An issue can be resolved, but needs approval from both the tech and the employee that originally created the incident.

An issue can also be resolved in the event that the software center retires support for a piece of software.

This does not require approval from the tech nor the employee that created the incident.