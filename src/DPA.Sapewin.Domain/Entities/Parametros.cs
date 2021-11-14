using System;
using System.Collections.Generic;
using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Parametros : Entity
    {
        public Guid CompanyId { get; set; }

        public string Name { get; set; }

        public Tipodetolerancia TipodeTolerancia { get; set; }

        public Tipodeatraso? TipodeAtraso { get; set; }

        public int ArrearToleratedInFirstPeriod { get; set; }

        public int ArrearToleratedInSecondPeriod { get; set; }

        public int ArrearJourney { get; set; }

        public Tipodesaida? TipodeSaida { get; set; }

        public string SaidaTotal1P { get; set; }

        public string SaidaTotal2P { get; set; }

        public string SaidaJornada { get; set; }

        public Tipoextra? TipoExtra { get; set; }

        public string ExtraTotal1P { get; set; }

        public string ExtraTotalIntervalo { get; set; }

        public string ExtraTotal2P { get; set; }

        public string ExtraJornada { get; set; }

        public ToleranciaGeraltipo? ToleranciaGeralTipo { get; set; }

        public string ToleranciaGeral1P { get; set; }

        public string ToleranciaGeral2P { get; set; }

        public string ToleranciaGeralJornada { get; set; }

        public string ToleranciaGeralIntervalo { get; set; }

        public bool PgExtraAdcn { get; set; }

        public string AdicionalNoturnoInicio { get; set; }

        public string AdicionalNoturnoFim { get; set; }

        public double CalculoAdicional { get; set; }

        public bool ExtraAdicionalMaisAdicional { get; set; }

        public bool ExtraAdicionalAcresNormais { get; set; }

        public bool PagaAdcAbono { get; set; }

        public string ReduzidoaCada { get; set; }

        public string ReduzidoAdiciona { get; set; }

        public string DsrSabado { get; set; }

        public string DsrDomingo { get; set; }

        public string DsrFeriado { get; set; }

        public string DsrFolga { get; set; }

        public bool ControleAutomaticoDsr { get; set; }

        public bool? DsrProporcionalHoras { get; set; }

        public bool DescontarDsrSemana { get; set; }

        public bool? DescDsrAnterioraFalta { get; set; }

        public string OcorrenciaSemanalDsr { get; set; }

        public Tipodetabela TipodeTabela { get; set; }

        public bool PgDiautilvirada { get; set; }

        public bool? DescIntervalo { get; set; }

        public bool SabadoUtil { get; set; }

        public bool DomingoUtil { get; set; }

        public bool FerAgregaDomingo { get; set; }

        public bool FolAgregaDomingo { get; set; }

        public int AcimadexhorasDomingo { get; set; }

        public int AcimaHEDomingo { get; set; }

        public int AcimaHEFeriado { get; set; }

        public int AcimadexhorasFeriado { get; set; }

        public int AcimaHEFolga { get; set; }

        public int AcimadexhorasFolga { get; set; }

        public bool MostrarIntervaloSeparado { get; set; }

        public bool AdcnFinaldoexpediente { get; set; }

        public bool SomaExtraAdicional { get; set; }


        public enum Tipodetolerancia
        {
            Individual = 1, Geral = 2
        }

        public enum Tipodeatraso
        {
            Período = 1, Diario = 2
        }

        public enum Tipodesaida
        {
            Período = 1, Diario = 2
        }

        public enum Tipoextra
        {
            Período = 1, Diario = 2
        }

        public enum ToleranciaGeraltipo
        {
            Período = 1, Diario = 2
        }

        public enum Tipodetabela
        {
            Diario = 1, Semanal = 2, Mensal = 3
        }

        public const string ValorPadraoDsr = "07:20";

        public IList<OvertimeScheduling> EscalonamentodeHoraExtra { get; set; }

        public IList<Employee> Funcionarios { get; set; }

    }
}