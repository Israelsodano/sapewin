using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Turns : Entity
    {
        public string Saturday { get; set; }
        public string Sunday { get; set; }
        public string Holiday { get; set; }
        public string DayOff { get; set; }
        public string Afa { get; set; }
        public ScalesWeekTurn WeekTurn { get; set; }
    }
    public enum ScalesWeekTurn
    {
        Segunda = 1, Terca = 2, Quarta = 3, Quinta = 4, Sexta = 5, Sabado = 6, Domingo = 7
    };
}