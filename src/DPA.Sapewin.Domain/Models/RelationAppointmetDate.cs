using System;
using DPA.Sapewin.Domain.Entities;

namespace DPA.Sapewin.Domain.Models
{
    public record RelationAppointmetDate(Appointment appointment, DateTime rdate);
}