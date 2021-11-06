using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Turns : Entity
    {
        public int? Saturday { get; set; }
        public int? Sunday { get; set; }
        public int? Holiday { get; set; }
        public int? DayOff { get; set; }
        public int? Leave { get; set; }
        public DayOfWeek WeekTurn { get; set; }
    }
    
    public enum ScalesWeekTurn
    {
        Segunda = 1, Terca = 2, Quarta = 3, Quinta = 4, Sexta = 5, Sabado = 6, Domingo = 7
    };
}