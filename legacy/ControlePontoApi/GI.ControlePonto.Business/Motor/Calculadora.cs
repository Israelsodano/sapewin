using System;

namespace GI.ControlePonto.Business
{
    public class Calculadora
    {
        public const int minutospordia = 1440;

        public enum Operacao
        {
            soma, subtracao, multiplicacao, divisao
        }

        private static bool ValidaHorario(string str)
        {
            int _out;
            return string.IsNullOrEmpty(str) ? false : str.Length == 5 && str.Contains(":") && int.TryParse(str.Split(':')[1], out _out) && _out <= 60;
        }

         public static string PegaHoras(DateTime? data) =>
             data.HasValue ? $"{data.Value.Hour.ToString("00")}:{data.Value.Minute.ToString("00")}" : null;

        public static string CalculadoradeHoras(string primeiro, Operacao operacao, string segundo)
        {

            if (!ValidaHorario(primeiro) || !ValidaHorario(segundo))
            {
                throw new Exception("Horario possui formato incorreto");
            }

            var minutosTotais = 0;

            switch (operacao)
            {
                case Operacao.soma:
                    
                    minutosTotais = HorasparaMinuto(primeiro) + HorasparaMinuto(segundo);
                    break;
                case Operacao.subtracao:

                    var minPrimeiro = HorasparaMinuto(primeiro);
                    var minSegundo = HorasparaMinuto(segundo);

                    if(minPrimeiro < minSegundo)
                        minPrimeiro += minutospordia;

                    minutosTotais = minPrimeiro - minSegundo;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return MinutosparaHora(minutosTotais);
        }

        public static string CalculadoradeHoras(string hora, Operacao operacao, int valor)
        {
            if (!ValidaHorario(hora))
            {
                throw new Exception("Horario possui formato incorreto");
            }

            int minutos = HorasparaMinuto(hora);

            switch (operacao)
            {
                case Operacao.multiplicacao:

                    minutos = minutos * valor;
                    break;
                case Operacao.divisao:

                    minutos = minutos / valor;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return MinutosparaHora(minutos);
        }

        public static string DiferencaHoras(string de, string ate)
        {

            string[] arrayde = de.Split(':');
            string[] arrayate = ate.Split(':');

            int minutosde = HorasparaMinuto(de);
            int minutosate = HorasparaMinuto(ate);

            if (minutosde > minutospordia || minutosate > minutospordia)
            {
                throw new Exception("Horario possui mais que 24 horas");
            }

            if (minutosde > minutosate)
                minutosate += minutospordia;

            int minutostotais = minutosate - minutosde;

            return MinutosparaHora(minutostotais);
        }

        public static string MinutosparaHora(int minutos)
        {
            int horasresult = (int)Math.Floor((decimal)minutos / 60);
            int minutosresult = minutos % 60;

            return $"{Math.Abs(horasresult).ToString("00")}:{minutosresult.ToString("00")}";
        }

        public static int HorasparaMinuto(string hora)
        {
            if(string.IsNullOrEmpty(hora))
                hora = "00:00";
                
            if (!ValidaHorario(hora))
            {
                throw new Exception("Horario possui formato incorreto");
            }

            string[] arrayhora = hora.Split(':');

            return (int.Parse(arrayhora[0]) * 60) + int.Parse(arrayhora[1]);
        }

        public static string Convertepara24Horas(string hora)
        {
            int minutos = HorasparaMinuto(hora);

            while (minutos >= minutospordia)
            {
                minutos = minutos - minutospordia;
            }

            return MinutosparaHora(minutos);
        }

        public static DateTime HoraEmDateTime(string hora, DateTime data)
        {
            if (ValidaHorario(hora))
                return Convert.ToDateTime(data.ToShortDateString() + " " + hora);
            else
                throw new Exception("Horario não válido");
        }
    }
}