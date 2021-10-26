using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GI.ControlePonto.Domain.Entities
{
    [Table("ponto")]
    public class Ponto
    {
        public int IDPonto { get; set; }
        public int IDFilaProcesso { get; set; }
        public long IDFuncionario { get; set; }
        public int IDEmpresa { get; set; }
        public string DsrPago { get; set; }
        public string DsrDescontado { get; set; }
        public string PagExtrapPer1 { get; set; }
        public string PagExtrapPer2 { get; set; }
        public string PagExtrapIntervalo { get; set; }
        public string DesExtraInter { get; set; }
        public string DesExtrapPer1 { get; set; }
        public string DesExtrapPer2 { get; set; }
        public int? IDHorario { get; set; }
        public DateTime Data { get; set; }
        public bool Tratado { get; set; }
        public string HoraPagPer1 { get; set; }
        public string HoraPagPer2 { get; set; }
        public string FaltaPagPer1 { get; set; }
        public string FaltaPagPer2 { get; set; }
        public string FaltaDesPer1 { get; set; }
        public string FaltaDesPer2 { get; set; }
        public string AtrasoPagPer1 { get; set; }
        public string AtrasoPagPer2 { get; set; }
        public string AtrasoDesPer2 { get; set; }
        public string AtrasoDesPer1 { get; set; }
        public string SaidaPagPer1 { get; set; }
        public string SaidaPagPer2 { get; set; }
        public string SaidaDesPer1 { get; set; }
        public string SaidaDesPer2 { get; set; }
        public string AdicionalPagPer1 { get; set; }
        public string AdicionalPagPer2 { get; set; }
        public string ExtraAdicPagPer1 { get; set; }
        public string ExtraAdicPagPer2 { get; set; }
        public string ExtraAdicPagInter { get; set; }
        public string ExtraAdicDesPer1 { get; set; }
        public string ExtraAdicDesPer2 { get; set; }
        public string ExtraAdicDesInter { get; set; }
        public string ReferenciaSemHorario { get; set; }
        public virtual IEnumerable<PontoPares> Pares { get; set; }

        public const string Folga = "Folga";
        public const string Sabado = "Sabado";
        public const string Domingo = "Domingo";
        public const string Feriado = "Feriado";
    }
}