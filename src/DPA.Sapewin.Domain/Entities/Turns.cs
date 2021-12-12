using System;
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
<<<<<<< HEAD
    public static class DayOfWeekExtensions
    {
        public static DayOfWeek Next(this DayOfWeek dayOfWeek)
        => dayOfWeek == DayOfWeek.Saturday ? DayOfWeek.Sunday : dayOfWeek + 1;
=======

    public static class DayOfWeekExtensions
    {
        public static DayOfWeek Next(this DayOfWeek day) => day == DayOfWeek.Saturday ? DayOfWeek.Sunday : day + 1;
>>>>>>> main
    }
}