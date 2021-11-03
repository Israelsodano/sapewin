using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;

using System;
using GI.ControlePonto.Domain.Entities;

namespace GI.ControlePonto.Business
{
    public class HelperPares
    {
        public Marcacoes marcacaoEntrada;

        public Marcacoes marcacaoSaida;

        private Horarios _horarioReferencia;

        public readonly Ponto _ponto;

        public static Func<Marcacoes, Marcacoes, bool> IsEqual = (x, y) =>
        {
            return x == null || y == null ? false : x.datahora == y.datahora;
        };

        private bool IsEntradaemHorarioReferencia(Marcacoes marcacao)
        {

            const string entrada = "Entrada";
            const string saida = "Saida";

            var horamarcacao = Calculadora.PegaHoras(marcacao.datahora);
            var horarios = new Dictionary<string, string>{
                {  _horarioReferencia.Entrada, entrada },
                { _horarioReferencia.EntradaIntervalo, saida},
                {  _horarioReferencia.SaidaIntervalo, entrada },
                {  _horarioReferencia.Saida, saida }
            };

            return horarios.Where(x => x.Key != null).Select(x => new
            {
                diferenca = Calculadora
                              .CalculadoradeHoras(horamarcacao,
                                                  Calculadora.Operacao.subtracao,
                                                  x.Key),
                isEntrada = x.Value == entrada
            }).OrderBy(x => x.diferenca).First().isEntrada;
        }

        public HelperPares()
        {

        }
        public HelperPares(Marcacoes marcacao1, Marcacoes marcacao2, Horarios horario, Ponto ponto)
        {
            _horarioReferencia = horario;
            _ponto = ponto;

            var marcacoes = new Marcacoes[]
            {
                    marcacao1,
                    marcacao2
            }.OrderBy(x => x?.datahora);
            
            if(marcacao1 != null || marcacao2 != null)
                if (marcacao1 != null && marcacao2 != null)
                {
                    if (marcacao1.datahora != marcacao2.datahora)
                    {
                        marcacaoEntrada = marcacoes.ElementAt(0);
                        marcacaoSaida = marcacoes.ElementAt(1);
                    }
                    else{
                        int minMarc1 = Calculadora.HorasparaMinuto(Calculadora.PegaHoras(marcacao1.datahora));
                        if(Math.Abs(minMarc1 - Calculadora.HorasparaMinuto(_horarioReferencia.Entrada)) > Math.Abs(minMarc1 - Calculadora.HorasparaMinuto(_horarioReferencia.Saida)))
                            marcacaoSaida = marcacao1;  
                        else 
                            marcacaoEntrada = marcacao1;
                    } 
                }
                else
                {
                    var marcacaonnNula = marcacoes.First(x => x != null);

                    var isEntrada = _horarioReferencia.Tipo == Horarios.tipo.Carga ? false : IsEntradaemHorarioReferencia(marcacaonnNula);

                    if (isEntrada)
                        marcacaoEntrada = marcacaonnNula;
                    else
                        marcacaoSaida = marcacaonnNula;
                }
        }

        public bool IsMaiorQueDataHora(DateTime data){
            var result = this.GetMenorMarcacao().datahora > data;
            return result;
        }
        public Marcacoes GetMenorMarcacao() =>
            marcacaoEntrada == null ? marcacaoSaida : marcacaoEntrada;


        public Marcacoes GetMaiorMarcacao() =>
            marcacaoSaida == null ? marcacaoEntrada : marcacaoSaida;

        public bool IsEntreasMarcacoes(Marcacoes marcacao) =>
            marcacaoEntrada == null || marcacaoSaida == null ? false : marcacaoEntrada.datahora <= marcacao.datahora && marcacao.datahora <= marcacaoSaida.datahora;

        public HelperPares AddMarcacao(Marcacoes marcacao)
        {
            var marcacoes = new Marcacoes[]
            {
                marcacaoEntrada,
                marcacaoSaida,
                marcacao
            }.OrderBy(x => x?.datahora).ToList();

            marcacaoEntrada = marcacoes.ElementAt(0);
            marcacaoSaida = marcacoes.ElementAt(1);

            marcacoes.Remove(marcacaoEntrada);
            marcacoes.Remove(marcacaoSaida);

            Marcacoes marcacaoRestante = marcacoes.FirstOrDefault();

            return marcacaoRestante == null ? null : new HelperPares(null, marcacaoRestante, _horarioReferencia, _ponto);
        }

        public int PegaMinutosTrabalhados() =>
            IsParNulo() ? 0 : (int)marcacaoSaida.datahora.Subtract(marcacaoEntrada.datahora).TotalMinutes;
       

        public bool IsParNulo() =>
            (marcacaoEntrada == null || marcacaoSaida == null) || (string.IsNullOrEmpty(marcacaoEntrada.chaveUniqueMarc) || string.IsNullOrEmpty(marcacaoSaida.chaveUniqueMarc));

        public int DiferencaEmMinutos()
        {
            if(marcacaoEntrada == null || marcacaoSaida == null)
                return 0;
            
            return (int)(marcacaoSaida.datahora - marcacaoEntrada.datahora).TotalMinutes;
        }  

        public int DiferencaEmMinutosHoraMinima(DateTime horaMinima)
        {
            if(marcacaoEntrada == null || marcacaoSaida == null)
                return 0;
            
            var data = marcacaoEntrada.datahora < horaMinima ? horaMinima : marcacaoEntrada.datahora;
            return (int)(marcacaoSaida.datahora - data).TotalMinutes;
        }  

        public int DiferencaEmMinutosHoraMaxima(DateTime horaMaxima)
        {
            if(marcacaoEntrada == null || marcacaoSaida == null)
                return 0;
            
            var data = marcacaoSaida.datahora > horaMaxima ? horaMaxima : marcacaoSaida.datahora;
            return (int)(data - marcacaoEntrada.datahora).TotalMinutes;
        }  
    }

    public static class StaticMethodsHelper
    {
        public static IList<PontoPares> ToPontoPares(this List<HelperPares> helper)
        {
            helper.RemoveAll(x => x == null);

            helper = helper.OrderBy(x => x.GetMenorMarcacao()?.datahora).ToList();

            int index = 0;
            List<PontoPares> pontoPares = helper.Select((x) =>
            {
                return new PontoPares
                {
                    IDEmpresa = x._ponto.IDEmpresa,
                    IDPonto = x._ponto.IDPonto,
                    IDFuncionario = x._ponto.IDFuncionario,
                    Ordem = ++index,
                    IDEntradaOri = x.marcacaoEntrada?.IDMarcacao,
                    IDSaidaOri = x.marcacaoSaida?.IDMarcacao,
                    DataHoraEntrada = x.marcacaoEntrada?.datahora,
                    DataHoraSaida = x.marcacaoSaida?.datahora,
                };
            }).ToList();

            return pontoPares;
        }

        public static HelperPares PegaParMaisProximo(this IList<HelperPares> helper, string hora) 
        {
            var diffs = helper.Select(x=> {
                var diff = new 
                {
                    entradaDiff = x.marcacaoEntrada == null ? null : (int?)Calculadora.HorasparaMinuto(Calculadora.DiferencaHoras(Calculadora.PegaHoras(x.marcacaoEntrada.datahora), hora)),
                    saidaDiff = x.marcacaoSaida == null ? null : (int?)Calculadora.HorasparaMinuto(Calculadora.DiferencaHoras(Calculadora.PegaHoras(x.marcacaoSaida.datahora), hora)),
                    marcacaoEntradaId = x.marcacaoEntrada?.IDMarcacao,
                    marcacaoSaidaId = x.marcacaoSaida?.IDMarcacao
                };

                return diff;
            });

            var result = diffs.OrderBy(x=> x.entradaDiff.HasValue ? x.entradaDiff.Value : x.saidaDiff.Value).FirstOrDefault();
            return helper.FirstOrDefault(x=> x.marcacaoEntrada?.IDMarcacao == result.marcacaoEntradaId && x.marcacaoSaida.IDMarcacao == result.marcacaoSaidaId);
        }

        public static int[] PegaDiferencaemMinutosRange(this IList<HelperPares> helper, DateTime inicio, DateTime fim)
        {
            var range = helper.Where((x) => {
                return (x.marcacaoEntrada != null && x.marcacaoSaida != null) &&
                    (inicio <= x.GetMenorMarcacao().datahora || x.GetMaiorMarcacao().datahora <= fim) && !(x.GetMenorMarcacao().datahora == inicio && x.GetMaiorMarcacao().datahora == fim);
            });

            return range.Select(x=> x.DiferencaEmMinutos()).ToArray();
        }

        public static HelperPares PegaParPorDiferencadeIntervalos(this IList<HelperPares> helper, string carga, List<Marcacoes> exclusoes)
        {
            var chaves = exclusoes.Select(y=>y.chaveUniqueMarc);
            var minutoscarga = Calculadora.HorasparaMinuto(carga);
            var diffs = helper.Where(x=> !x.IsParNulo() && !(chaves.Contains(x.marcacaoEntrada.chaveUniqueMarc) || chaves.Contains(x.marcacaoSaida.chaveUniqueMarc))).Select(x=> new { diferenca = Math.Abs(x.DiferencaEmMinutos() - minutoscarga), value = x });
            return diffs.OrderBy(x=>x.diferenca).FirstOrDefault().value;
        }

        public static HelperPares PegaProximoPar(this IList<HelperPares> helper, HelperPares par)
        {
            helper = helper.OrderBy(x=> x.GetMenorMarcacao().datahora).ToList();
            
            int indexof = helper.IndexOf(par);

            return indexof <= helper.Count - 1 ? helper[indexof + 1] : null;
        }
    }
}