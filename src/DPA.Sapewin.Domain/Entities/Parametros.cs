using System;
using System.Collections.Generic;

namespace GI.ControlePonto.Domain.Entities
{
    public class Parametros
    {
        public virtual int IDParametro { get; set; }

        public virtual int IDEmpresa { get; set; }

        public virtual String Nome { get; set; }

        public virtual Tipodetolerancia TipodeTolerancia { get; set; }

        public virtual Tipodeatraso? TipodeAtraso { get; set; }

        public virtual String AtrasoTotal1P { get; set; }

        public virtual String AtrasoTotal2P { get; set; }

        public virtual String AtrasoJornada { get; set; }

        public virtual Tipodesaida? TipodeSaida { get; set; }

        public virtual String SaidaTotal1P { get; set; }

        public virtual String SaidaTotal2P { get; set; }

        public virtual String SaidaJornada { get; set; }

        public virtual Tipoextra? TipoExtra { get; set; }

        public virtual String ExtraTotal1P { get; set; }

        public virtual String ExtraTotalIntervalo { get; set; }

        public virtual String ExtraTotal2P { get; set; }

        public virtual String ExtraJornada { get; set; }

        public virtual ToleranciaGeraltipo? ToleranciaGeralTipo { get; set; }

        public virtual String ToleranciaGeral1P { get; set; }

        public virtual String ToleranciaGeral2P { get; set; }

        public virtual String ToleranciaGeralJornada { get; set; }

        public virtual String ToleranciaGeralIntervalo { get; set; }

        public virtual bool PgExtraAdcn { get; set; }

        public virtual String AdicionalNoturnoInicio { get; set; }

        public virtual String AdicionalNoturnoFim { get; set; }

        public virtual double CalculoAdicional { get; set; }

        public virtual bool ExtraAdicionalMaisAdicional { get; set; }

        public virtual bool ExtraAdicionalAcresNormais { get; set; }

        public virtual bool PagaAdcAbono { get; set; }

        public virtual String ReduzidoaCada { get; set; }

        public virtual String ReduzidoAdiciona { get; set; }

        public virtual String DsrSabado { get; set; }

        public virtual String DsrDomingo { get; set; }

        public virtual String DsrFeriado { get; set; }

        public virtual String DsrFolga { get; set; }

        public virtual bool ControleAutomaticoDsr { get; set; }

        public virtual bool? DsrProporcionalHoras { get; set; }

        public virtual bool DescontarDsrSemana { get; set; }

        public virtual bool? DescDsrAnterioraFalta { get; set; }

        public virtual String OcorrenciaSemanalDsr { get; set; }

        public Tipodetabela TipodeTabela { get; set; }

        public virtual bool PgDiautilvirada { get; set; }

        public virtual bool? DescIntervalo { get; set; }

        public virtual bool SabadoUtil { get; set; }

        public virtual bool DomingoUtil { get; set; }

        public virtual bool FerAgregaDomingo { get; set; }

        public virtual bool FolAgregaDomingo { get; set; }

        public virtual int AcimadexhorasDomingo { get; set; }

        public virtual int AcimaHEDomingo { get; set; }

        public virtual int AcimaHEFeriado { get; set; }

        public virtual int AcimadexhorasFeriado { get; set; }

        public virtual int AcimaHEFolga { get; set; }

        public virtual int AcimadexhorasFolga { get; set; }

        public virtual bool MostrarIntervaloSeparado { get; set; }

        public virtual bool AdcnFinaldoexpediente { get; set; }

        public virtual bool SomaExtraAdicional { get; set; }


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

        public virtual IList<EscalonamentodeHoraExtra> EscalonamentodeHoraExtra { get; set; }

        public virtual IList<Funcionarios> Funcionarios { get; set; }

    }
}