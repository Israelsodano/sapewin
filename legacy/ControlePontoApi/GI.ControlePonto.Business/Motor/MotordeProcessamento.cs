using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GI.ControlePonto.Domain.Repository.Interfaces;
using GI.ControlePonto.Domain.Entities;
using GI.ControlePonto.Domain.Entities.CommonModels;
using Microsoft.EntityFrameworkCore;
using GI.ControlePonto.Repository;
using GI.ControlePonto.Business.Motor;

namespace GI.ControlePonto.Business
{
    public class MotordeProcessamento
    {
        private readonly IRepository<ApontamentoSujo> _repositoryApontamentoSujo;
        private readonly IRepository<Marcacoes> _repositoryMarcacoes;
        private readonly IRepository<FilaProcesso> _repositoryFilaProcesso;
        private readonly IRepository<Ponto> _repositoryPonto;
        private readonly IRepository<PontoPares> _repositoryPontoPares;
        private readonly IRepository<Horarios> _repositoryHorarios;
        private readonly IRepository<Afastamentos> _repositoryAfastamentos;
        private readonly IRepository<FeriadosGerais> _repositoryFeriadosGerais;
        private List<Marcacoes> MarcacoesnaBase;
        private List<Ponto> PontosnaBase;
        private List<Horarios> HorariosnaBase;
        //private List<PontoPares> PontoParesnaBase;
        private IList<Funcionarios> FuncionarioProcessados;
        private IList<FeriadosGerais> FeriadosGerais;
        //private List<CalculosHelperPonto> ResultPares;

        private bool CompareFunc(Funcionarios x, Funcionarios y) =>
            x.IDFuncionario == y.IDFuncionario &&
            x.IDEmpresa == y.IDEmpresa;

        private bool CompareFunc(Funcionarios x, Ponto y) =>
            x.IDFuncionario == y.IDFuncionario &&
            x.IDEmpresa == y.IDEmpresa;

        public MotordeProcessamento(IRepository<ApontamentoSujo> repositoryApontamentoSujo,
            IRepository<Marcacoes> repositoryApontamentoSemLimpo,
            IRepository<FilaProcesso> repositoryFilaProcesso,
            IRepository<Ponto> repositoryPonto,
            IRepository<PontoPares> repositoryPontoPares,
            IRepository<Horarios> repositoryHorarios,
            IRepository<Afastamentos> repositoryAfastamentos,
            IRepository<FeriadosGerais> repositoryFeriadosGerais)
        {
            _repositoryApontamentoSujo = repositoryApontamentoSujo.Clone();
            _repositoryMarcacoes = repositoryApontamentoSemLimpo.Clone();
            _repositoryFilaProcesso = repositoryFilaProcesso.Clone();
            _repositoryPonto = repositoryPonto.Clone();
            _repositoryPontoPares = repositoryPontoPares.Clone();
            _repositoryHorarios = repositoryHorarios.Clone();
            _repositoryAfastamentos = repositoryAfastamentos.Clone();
            _repositoryFeriadosGerais = repositoryFeriadosGerais.Clone();
        }

        private IList<CalendarioFuncionario> CalendarioFuncionario;
        private async Task MontaCalendarioFuncionario(IList<Funcionarios> funcionarios, DateTime dataini, DateTime datafim)
        {
            IRepository<Horarios> repHorarios = _repositoryHorarios.Clone();
            IRepository<Afastamentos> repAfastamentos = _repositoryAfastamentos.Clone();


            CalendarioFuncionario = new List<CalendarioFuncionario>();

            dataini = dataini.AddDays(-10);
            datafim = datafim.AddDays(10);

            foreach (var funcionario in funcionarios)
            {
                int _out;
                var IDsHorario = funcionario.Escala.EscalasHorarios
                                    .Where(x => int.TryParse(x.IDHorario, out _out))
                                    .Select(x => new { IDHorario = int.Parse(x.IDHorario), x.IDEmpresa });

                Task<List<Horarios>> horarios = repHorarios.GetAll()
                                            .Where(x => IDsHorario.Any(y => y.IDHorario == x.IDHorario
                                             && y.IDEmpresa == x.IDEmpresa))
                                            .Include(x => x.IntervalosAuxiliares)
                                            .ToListAsync();

                Task<List<Afastamentos>> taskAfastamentosFuncionario = repAfastamentos.GetAll()
                                                                            .Where(x => x.IDFuncionario == funcionario.IDFuncionario)
                                                                            .ToListAsync();


                CalendarioFuncionario calendario = new CalendarioFuncionario()
                {
                    Funcionario = funcionario,
                    Calendario = new List<Calendario>()
                };
                
                HorariosnaBase = await horarios;
                IList<Afastamentos> afastamentosFuncionario =
                                                await taskAfastamentosFuncionario;

                while (dataini <= datafim)
                {
                    Horarios horario = funcionario.HorariosOcasionais.FirstOrDefault(x => x.Data.Date == dataini.Date)?.Horario;

                    int days = (dataini - funcionario.Escala.DataInicio).Days;
                    int resto = Math.Abs(days)
                                        % funcionario.Escala.EscalasHorarios.Count;

                    IList<EscalasHorarios> escalasHorarios = days > 0 ?
                                                        funcionario.Escala.EscalasHorarios.OrderBy(x => x.Ordem).ToArray()
                                                        : funcionario.Escala.EscalasHorarios.OrderByDescending(x => x.Ordem).ToArray();

                    EscalasHorarios escaladodia = escalasHorarios[resto];

                    var afastamento = afastamentosFuncionario.FirstOrDefault(x => x.DataInicial <= dataini && dataini <= x.DataFinal);

                    horario = afastamento != null ? null : horario != null ? horario : HorariosnaBase
                                                .FirstOrDefault(x => x.IDHorario.ToString()
                                                == escaladodia.IDHorario && x.IDEmpresa
                                                == escaladodia.IDEmpresa);

                    for (int i = 0; i < (funcionario.HorariosOcasionais
                                                            .Any(x => x.Data.Date == dataini.Date) ? 1
                                                            : escaladodia.QuantidadedeDias); i++)
                    {
                        calendario.Calendario.Add(new Calendario(dataini,
                                                                 horario,
                                                                 afastamento,
                                                                 funcionario,
                                                                 FeriadosGerais,
                                                                 escaladodia));
                        dataini = dataini.AddDays(1);
                    }

                    horario = null;
                }

                CalendarioFuncionario.Add(calendario);
            }
        }
        private Horarios PegaHorariodoDia(Funcionarios funcionario, DateTime data) =>
            CalendarioFuncionario
                    .First(x => CompareFunc(x.Funcionario, funcionario))
                    .Calendario.FirstOrDefault(x => x.Data.Date == data.Date)
                    .Horario;

        private Calendario PegaCalendariodoDia(Funcionarios funcionario, DateTime data) =>
            CalendarioFuncionario
                .First(x => CompareFunc(x.Funcionario, funcionario))
                .Calendario.FirstOrDefault(x => x.Data.Date == data.Date);

        private Calendario PegaCalendarioMaisProximo(Funcionarios funcionario, DateTime data)
        {
            var calendario = CalendarioFuncionario
                    .First(x => CompareFunc(x.Funcionario, funcionario)).Calendario;

            var menorHorario = calendario.Where(x => x.Horario != null)
            .OrderBy(x => x.Data.Date)
            .Select(x => new
            {
                x.Horario,
                diferenca = Math.Abs((x.Data.Date - data).TotalMinutes),
                x.Data
            }).OrderBy(x => x.diferenca)
                .FirstOrDefault();

            return new Calendario(menorHorario.Data, menorHorario.Horario);
        }

        private InformacoesPonto PegaMarcacoesPonto(Ponto ponto)
        {

            Funcionarios funcionario = FuncionarioProcessados.First(x => CompareFunc(x, ponto));

            IRepository<Ponto> repPonto = _repositoryPonto.Clone();

            Horarios horarioAnterior = null,
                            horarioPosterior = null,
                            horarioReferencia = null;

            Calendario calendarioMaisProximo = PegaCalendariodoDia(funcionario, ponto.Data);

            string viradaManual = null;

            if (calendarioMaisProximo.Horario == null)
            {
                switch (calendarioMaisProximo.ReferenciaSemHorario)
                {
                    case Ponto.Domingo:
                        viradaManual = funcionario.Escala.Virada_Dom;
                        break;
                    case Ponto.Feriado:
                        viradaManual = funcionario.Escala.Virada_Fer;
                        break;
                    case Ponto.Folga:
                        viradaManual = funcionario.Escala.Virada_Fol;
                        break;
                    case Ponto.Sabado:
                        viradaManual = funcionario.Escala.Virada_Sab;
                        break;
                    default:
                        viradaManual = null;
                        break;
                }
            }

            if (viradaManual == null)
            {
                calendarioMaisProximo = PegaCalendarioMaisProximo(funcionario, ponto.Data.Date);

                if (calendarioMaisProximo.Data.Date == ponto.Data.Date)
                {
                    horarioAnterior = PegaHorariodoDia(funcionario, ponto.Data.Date.AddDays(-1));
                    horarioPosterior = PegaHorariodoDia(funcionario, ponto.Data.Date.AddDays(1));
                }
            }

            horarioReferencia = calendarioMaisProximo.Horario;

            DateTime viradaanterior, viradaposterior;
            string horariovirada = string.Empty;

            horarioAnterior = horarioAnterior == null ? horarioReferencia : horarioAnterior;
            horarioPosterior = horarioPosterior == null ? horarioReferencia : horarioPosterior;

            horariovirada = viradaManual != null ? viradaManual : horarioReferencia.ViradadoDia != null ? horarioReferencia.ViradadoDia : Calculadora.Convertepara24Horas(
                                Calculadora.CalculadoradeHoras(
                                    Calculadora.CalculadoradeHoras(
                                            Calculadora.CalculadoradeHoras(
                                                horarioAnterior.Entrada,
                                                Calculadora.Operacao.subtracao,
                                                horarioReferencia.Saida),
                                                Calculadora.Operacao.divisao, 2), Calculadora.Operacao.soma, horarioAnterior.Saida));

            viradaanterior = Calculadora.HoraEmDateTime(horariovirada, ponto.Data.Date.AddDays(Calculadora.HorasparaMinuto(horariovirada) < Calculadora.HorasparaMinuto(horarioReferencia.Entrada) ? 0 : -1));

            horariovirada = viradaManual != null ? viradaManual : horarioReferencia.ViradadoDia != null ? horarioReferencia.ViradadoDia : Calculadora.Convertepara24Horas(
                                Calculadora.CalculadoradeHoras(
                                    Calculadora.CalculadoradeHoras(
                                            Calculadora.CalculadoradeHoras(
                                                horarioReferencia.Entrada,
                                                Calculadora.Operacao.subtracao,
                                                horarioPosterior.Saida),
                                                Calculadora.Operacao.divisao, 2), Calculadora.Operacao.soma, horarioReferencia.Saida));

            viradaposterior = Calculadora.HoraEmDateTime(horariovirada, ponto.Data.Date.AddDays(Calculadora.HorasparaMinuto(horarioReferencia.Saida) < Calculadora.HorasparaMinuto(horariovirada) ? 0 : 1));

            viradaposterior = horarioReferencia.VintqHoras ? viradaposterior.AddDays(1) : viradaposterior;
            return new InformacoesPonto
            {
                Horario = horarioReferencia,
                Marcacoes = MarcacoesnaBase
                            .Where(x => x.datahora >= viradaanterior
                            && x.datahora <= viradaposterior
                            && x.IDFuncionario == ponto.IDFuncionario
                            && x.IDEmpresa == ponto.IDEmpresa).OrderBy(x => x.datahora).ToList(),
                Ponto = ponto,
                Funcionario = funcionario
            };
        }

        private async Task<Marcacoes> PegaMarcacao(HorarioDataHora horarioData, IList<Marcacoes> marcacoes, string marcacao, bool verificamelhormarcacao, IEnumerable<string> horasMarcacoes)
        {
            return await Task<Marcacoes>.Run(() =>
            {
                IDictionary<string, DateTime> minutosHorario = new Dictionary<string, DateTime>();

                foreach (var hora in horasMarcacoes)
                {
                    minutosHorario.Add(hora, horarioData.DatasHorario[hora]);
                }

                return marcacoes.FirstOrDefault((marc) =>
                {
                    DateTime datahoraHorario = minutosHorario[marcacao];

                    var melhorMarcacaoHorario = marcacoes.Select(x => new
                    {
                        diferenca = Math.Abs((x.datahora - datahoraHorario).TotalMinutes),
                        marcacao = x
                    }).OrderBy(x => x.diferenca).First().marcacao;

                    DateTime datahoraMelhorHorarioMarcacao = minutosHorario
                                                .Select(x => new
                                                {
                                                    diferenca = Math.Abs((x.Value - marc.datahora).TotalMinutes),
                                                    data = x.Value
                                                }).OrderBy(x => x.diferenca)
                                                .First().data;

                    return !verificamelhormarcacao ? (HelperPares.IsEqual(marc, melhorMarcacaoHorario)) : (HelperPares.IsEqual(marc, melhorMarcacaoHorario)) && (datahoraHorario == datahoraMelhorHorarioMarcacao);
                });
            });
        }
        

        private bool VerificaOutroDia(string entrada, string saida) =>
            Calculadora.HorasparaMinuto(saida) < Calculadora.HorasparaMinuto(entrada);
            
        private async Task EncaixaMarcacoes(List<Marcacoes> marcacoes,
                                            Horarios horario,
                                            Ponto ponto,
                                            Funcionarios funcionario)
        {

            Marcacoes marcacaoEntrada,
                             marcacaoEntradaIntervalo = null,
                             marcacaoSaidaIntervalo = null,
                             marcacaoSaida;


            var horarioDataHora = new HorarioDataHora(horario, ponto.Data.Date);            

            var horasMarcacoes = new [] 
            {
                horario.Entrada,
                horario.EntradaIntervalo,
                horario.SaidaIntervalo,
                horario.Saida
            };

            List<Marcacoes> marcacoesUsadas = new List<Marcacoes>();

            marcacaoEntrada = await PegaMarcacao(horarioDataHora, marcacoes, horario.Entrada, false, horasMarcacoes);

            marcacoesUsadas.Add(marcacaoEntrada);
            if (horario.EntradaIntervalo != null)
            {
                marcacaoEntradaIntervalo = await PegaMarcacao(horarioDataHora, marcacoes.Where(x=> !marcacoesUsadas.Any(y=> y.datahora == x.datahora)).ToArray(), horario.EntradaIntervalo, true, horasMarcacoes);

                marcacaoEntradaIntervalo = funcionario.Intervalo == Funcionarios.intervalo.Pre_Assinalado ? 
                                            marcacaoEntradaIntervalo == null ?
                                            new Marcacoes { IDEmpresa = ponto.IDEmpresa, IDFuncionario = funcionario.IDFuncionario, datahora = Calculadora.HoraEmDateTime(horario.EntradaIntervalo, ponto.Data.AddDays(VerificaOutroDia(horario.Entrada, horario.EntradaIntervalo) ? 1 : 0)) }
                                            : marcacaoEntradaIntervalo
                                            : marcacaoEntradaIntervalo;

                marcacoesUsadas.Add(marcacaoEntradaIntervalo);

                marcacaoSaidaIntervalo = await PegaMarcacao(horarioDataHora, marcacoes.Where(x=> !marcacoesUsadas.Any(y=> y.datahora == x.datahora)).ToArray(), horario.SaidaIntervalo, true, horasMarcacoes);

                marcacaoSaidaIntervalo = funcionario.Intervalo == Funcionarios.intervalo.Pre_Assinalado ? 
                                            marcacaoSaidaIntervalo == null ?
                                            new Marcacoes { IDEmpresa = ponto.IDEmpresa, IDFuncionario = funcionario.IDFuncionario, datahora = Calculadora.HoraEmDateTime(horario.SaidaIntervalo, ponto.Data.AddDays(VerificaOutroDia(horario.Entrada, horario.SaidaIntervalo) ? 1 : 0)) }
                                            : marcacaoSaidaIntervalo
                                            : marcacaoSaidaIntervalo;

                marcacoesUsadas.Add(marcacaoSaidaIntervalo);

            }

            marcacaoSaida = await PegaMarcacao(horarioDataHora, marcacoes.Where(x=> !marcacoesUsadas.Any(y=> y.datahora == x.datahora)).ToArray(), horario.Saida, false, horasMarcacoes);

            var marcacoesReferencia = marcacoes;

            marcacoes = marcacoes.Where(x => !HelperPares.IsEqual(x, marcacaoEntrada)
                                           && !HelperPares.IsEqual(x, marcacaoEntradaIntervalo)
                                           && !HelperPares.IsEqual(x, marcacaoSaidaIntervalo)
                                           && !HelperPares.IsEqual(x, marcacaoSaida)).ToList();

            List<HelperPares> helper = (marcacaoEntradaIntervalo != null || marcacaoSaidaIntervalo != null) ? new List<HelperPares>
                {
                    new HelperPares(marcacaoEntrada, marcacaoEntradaIntervalo, horario, ponto),
                    new HelperPares(marcacaoSaidaIntervalo, marcacaoSaida, horario, ponto)
                } :
            new List<HelperPares>
            {
                    new HelperPares(marcacaoEntrada, marcacaoSaida, horario, ponto),
            };

            helper.RemoveAll(x=> x.marcacaoEntrada == null && x.marcacaoSaida == null);

            for (int i = 0; i < marcacoes.Count; i++)
            {
                HelperPares menorHelper = helper.FirstOrDefault(x => x.IsEntreasMarcacoes(marcacoes[i]));

                if (menorHelper == null)
                    menorHelper = helper.Select(x => new
                    {
                        diferenca = (x.GetMenorMarcacao().datahora - marcacoes[i].datahora).TotalMinutes,
                        valueHelper = x
                    }).OrderBy(x => x.diferenca).First().valueHelper;

                HelperPares novoPar = menorHelper.AddMarcacao(marcacoes[i]);

                helper.Add(novoPar);
            }

            var marcacoesPerfeitasTotais = new List<string> { horario.Entrada, horario.EntradaIntervalo, horario.Saida, horario.SaidaIntervalo };

            foreach (var intervalo in horario.IntervalosAuxiliares)
            {
                marcacoesPerfeitasTotais.AddRange(new[] { intervalo.Inicio, intervalo.Fim });
            }

            if(marcacaoSaidaIntervalo == null)
                marcacaoSaidaIntervalo = new Marcacoes{ datahora = horarioDataHora.DatasHorario[horario.SaidaIntervalo] };    

            if(marcacaoEntradaIntervalo == null){
                var diffHorarioCadastrado = (horarioDataHora.DatasHorario[horario.SaidaIntervalo] - horarioDataHora.DatasHorario[horario.EntradaIntervalo]).TotalMinutes;
                marcacaoEntradaIntervalo = new Marcacoes { datahora = marcacaoSaidaIntervalo.datahora.AddMinutes(-diffHorarioCadastrado) };
            }

            var calendario = CalendarioFuncionario.FirstOrDefault(x=> CompareFunc(x.Funcionario, funcionario)).Calendario.FirstOrDefault(x=> x.Data.Date == ponto.Data.Date);

            ponto = await CalculaAtrasos(helper, horarioDataHora, horario, ponto, funcionario, marcacoes, marcacaoEntradaIntervalo, marcacaoSaidaIntervalo, calendario);
            ponto = await CalculaSaidasAntecipadas(helper, horarioDataHora, horario, ponto, funcionario, marcacoes, marcacaoEntradaIntervalo, calendario);
            ponto = await CalculaHorasTrabalhadas(helper, horarioDataHora, horario, ponto, marcacoes, marcacoesPerfeitasTotais,  marcacaoEntradaIntervalo, marcacaoSaidaIntervalo, calendario);
            ponto = await CalculaFaltas(helper, horarioDataHora, horario, ponto, marcacoes, marcacoesPerfeitasTotais,  marcacaoEntradaIntervalo, calendario);
            ponto = await CalculaHorasExtra(helper, horarioDataHora, horario, ponto, funcionario, marcacoes, marcacoesPerfeitasTotais, marcacaoEntradaIntervalo, marcacaoSaidaIntervalo);
            ponto = await CalculaAdicionalNoturno(helper, horarioDataHora, horario, ponto, funcionario, marcacaoEntradaIntervalo, marcacaoSaidaIntervalo);

            var repositorioPonto = _repositoryPonto.Clone();
            var repositorioPontoPares = _repositoryPontoPares.Clone();

            await repositorioPonto.UpdateAsync(ponto);
            await repositorioPontoPares.InsertAsync(helper.ToPontoPares());
        }

        private async Task<Ponto> CalculaAtrasos(IList<HelperPares> pares, HorarioDataHora horarioDataHora, Horarios horario, Ponto ponto, Funcionarios funcionario, List<Marcacoes> marcacoes,  Marcacoes marcacaoIntervaloEntrada, Marcacoes marcacaoIntervaloSaida, Calendario calendario)
        {
            int atrasosPer1 = 0;
            int atrasosPer2 = 0;
            int tryparse;

            if(int.TryParse(calendario.ReferenciaSemHorario, out tryparse))
            {
                var marcacoesEntrada = pares.Where(x=> !x.IsParNulo()).Select(x=> x.marcacaoEntrada).ToList();

                marcacoes.AddRange(marcacoesEntrada);
                marcacoes.AddRange(pares.Where(x=> x.marcacaoSaida != null)
                            .Select(x=> x.marcacaoSaida));

                var marcacoesPerfeitas = new List<string>
                {
                    horario.Entrada,     
                };

                var intervalosCarga = new List<string>();

                foreach (var intervalo in horario.IntervalosAuxiliares.Where(x=> x.DescontarIntervalo))
                {
                    if(intervalo.Tipo == IntervalosAuxiliares.tipo.Fixo)
                        marcacoesPerfeitas.Add(intervalo.Inicio);
                    else    
                        intervalosCarga.Add(intervalo.Carga);
                }

                var atribuidor = new Dictionary<DateTime, DateTime>();
                var marcacoesUsadas = new List<Marcacoes>();

                foreach (var marcacaoPerfeita in marcacoesPerfeitas)
                {
                    var result = await PegaMarcacao(horarioDataHora, marcacoesEntrada, marcacaoPerfeita, true, marcacoesPerfeitas);
                    if(result !=  null)
                    {
                        marcacoesUsadas.Add(result);
                        atribuidor.Add(horarioDataHora.DatasHorario[marcacaoPerfeita], result.datahora);
                    }
                }

                int minutosIntervalo = Calculadora.HorasparaMinuto(Calculadora.PegaHoras(marcacaoIntervaloEntrada.datahora));

                var periodo1 = atribuidor.Keys.Where(x=> x <= marcacaoIntervaloEntrada.datahora).ToArray();

                foreach (var marcacao in periodo1)
                {
                    int diff = (int)(atribuidor[marcacao] - marcacao).TotalMinutes;

                    diff = diff > 0 ? diff : 0;

                    atrasosPer1 += diff;
                }

                int diffIntervalo = 0;
                var horarioEntradaIntervalo = Calculadora.HoraEmDateTime(horario.EntradaIntervalo, ponto.Data.Date.AddDays(VerificaOutroDia(horario.Entrada, horario.EntradaIntervalo) ? 1 : 0));
                
                if(!funcionario.IntervaloFixo)
                {
                    var horariosIntervalo = new[]{ horario.EntradaIntervalo, horario.SaidaIntervalo };

                    var horarioSaidaIntervalo = horarioDataHora.DatasHorario[horario.SaidaIntervalo];

                    diffIntervalo = (int)((marcacaoIntervaloSaida.datahora - marcacaoIntervaloEntrada.datahora).TotalMinutes - (horarioSaidaIntervalo - horarioEntradaIntervalo).TotalMinutes);
                }else
                {
                    diffIntervalo = (int)(marcacaoIntervaloEntrada.datahora - horarioEntradaIntervalo).TotalMinutes;
                }

                diffIntervalo = diffIntervalo > 0 ? diffIntervalo : 0;

                atrasosPer1 += diffIntervalo;

                var periodo2 = atribuidor.Keys.Where(x=> x > marcacaoIntervaloEntrada.datahora).ToArray();

                foreach (var marcacao in periodo2)
                {
                    int diff = (int)(atribuidor[marcacao] - marcacao).TotalMinutes;

                    diff = diff > 0 ? diff : 0;

                    atrasosPer2 += diff;
                }

                foreach (var intervalo in intervalosCarga)
                {
                    var par = pares.PegaParPorDiferencadeIntervalos(intervalo, marcacoesUsadas);

                    if(par != null)
                    {
                        var result = (par.DiferencaEmMinutos() - Calculadora.HorasparaMinuto(intervalo));

                        if(par.GetMaiorMarcacao().datahora <= marcacaoIntervaloEntrada.datahora)
                            atrasosPer1 += result > 0 ? result : 0;
                        else    
                            atrasosPer2 += result > 0 ? result : 0;
                    }
                }
            }

            var totalParametros = Calculadora.HorasparaMinuto(funcionario.Parametro.AtrasoJornada);

            atrasosPer1 = atrasosPer1 + atrasosPer2 > totalParametros ? atrasosPer1 : 0;
            atrasosPer2 = atrasosPer1 + atrasosPer2 > totalParametros ? atrasosPer2 : 0;

            atrasosPer1 = atrasosPer1 > Calculadora.HorasparaMinuto(funcionario.Parametro.AtrasoTotal1P) ? atrasosPer1 : 0;
            atrasosPer2 = atrasosPer2 > Calculadora.HorasparaMinuto(funcionario.Parametro.AtrasoTotal2P) ? atrasosPer2 : 0;
            
            ponto.AtrasoDesPer1 = Calculadora.MinutosparaHora(atrasosPer1);
            ponto.AtrasoDesPer2 = Calculadora.MinutosparaHora(atrasosPer2);

            return ponto;
        }

        private async Task<Ponto> CalculaSaidasAntecipadas(IList<HelperPares> pares, HorarioDataHora horarioDataHora, Horarios horario, Ponto ponto, Funcionarios funcionario, List<Marcacoes> marcacoes,  Marcacoes marcacaoIntervaloEntrada, Calendario calendario)
        {
            int saidaAntecipadaPer1 = 0;
            int saidaAntecipadaPer2 = 0;

            int tryparse;

            if(int.TryParse(calendario.ReferenciaSemHorario, out tryparse))
            {
                var marcacoesSaida = pares.Where(x=> !x.IsParNulo()).Select(x=> x.marcacaoSaida).ToList();

                marcacoes.AddRange(marcacoesSaida);
                marcacoes.AddRange(pares.Where(x=> x.marcacaoEntrada != null)
                            .Select(x=> x.marcacaoEntrada));

                var marcacoesPerfeitas = new List<string>
                {
                    horario.Saida,     
                    horario.EntradaIntervalo
                };

                foreach (var intervalo in horario.IntervalosAuxiliares.Where(x=> x.DescontarIntervalo))
                {
                    if(intervalo.Tipo == IntervalosAuxiliares.tipo.Fixo)
                        marcacoesPerfeitas.Add(intervalo.Fim);
                }

                var atribuidor = new Dictionary<DateTime, DateTime>();

                foreach (var marcacaoPerfeita in marcacoesPerfeitas)
                {
                    var result = await PegaMarcacao(horarioDataHora, marcacoesSaida, marcacaoPerfeita, true, marcacoesPerfeitas);
                    if(result != null)
                        atribuidor.Add(horarioDataHora.DatasHorario[marcacaoPerfeita], result.datahora);
                }

                var periodo1 = atribuidor.Keys.Where(x=> x <= marcacaoIntervaloEntrada.datahora).ToArray();
                
                if(funcionario.IntervaloFixo)
                    foreach (var marcacao in periodo1)
                    {
                        int diff = (int)(marcacao - atribuidor[marcacao]).TotalMinutes;

                        diff = diff > 0 ? diff : 0;

                        saidaAntecipadaPer1 += diff;
                    }
                    

                var periodo2 = atribuidor.Keys.Where(x=> x > marcacaoIntervaloEntrada.datahora).ToArray();

                foreach (var marcacao in periodo2)
                {
                    int diff = (int)(marcacao - atribuidor[marcacao]).TotalMinutes;

                    diff = diff > 0 ? diff : 0;

                    saidaAntecipadaPer2 += diff;
                }
            }

            var totalParametros = Calculadora.HorasparaMinuto(funcionario.Parametro.SaidaJornada);

            saidaAntecipadaPer1 = saidaAntecipadaPer1 + saidaAntecipadaPer2 > totalParametros ? saidaAntecipadaPer1 : 0;
            saidaAntecipadaPer2 = saidaAntecipadaPer1 + saidaAntecipadaPer2 > totalParametros ? saidaAntecipadaPer2 : 0;

            saidaAntecipadaPer1 = saidaAntecipadaPer1 > Calculadora.HorasparaMinuto(funcionario.Parametro.SaidaTotal1P) ? saidaAntecipadaPer1 : 0;
            saidaAntecipadaPer2 = saidaAntecipadaPer2 > Calculadora.HorasparaMinuto(funcionario.Parametro.SaidaTotal2P) ? saidaAntecipadaPer2 : 0;

            ponto.SaidaDesPer1 = Calculadora.MinutosparaHora(saidaAntecipadaPer1);
            ponto.SaidaDesPer2 = Calculadora.MinutosparaHora(saidaAntecipadaPer2);

            return ponto;
        }

        private async Task<Ponto> CalculaHorasTrabalhadas(IList<HelperPares> pares, HorarioDataHora horarioDataHora, Horarios horario, Ponto ponto, List<Marcacoes> marcacoes, List<string> marcacoesPerfeitas, Marcacoes marcacaoIntervaloEntrada, Marcacoes marcacaoIntervaloSaida, Calendario calendario)
        {
           return await Task<Ponto>.Run(() => 
           {
                var horasTrabalhadasPer1 = 0;
                var horasTrabalhadasPer2 = 0;

                int tryparse;

            if(int.TryParse(calendario.ReferenciaSemHorario, out tryparse))
            {
                    pares = pares.Where(x=> !x.IsParNulo() && (x.GetMenorMarcacao().datahora >= horarioDataHora.DatasHorario[horario.Entrada] && x.GetMaiorMarcacao().datahora <= horarioDataHora.DatasHorario[horario.Saida])).ToList();
                    horasTrabalhadasPer1 = pares.Where(x=> x.GetMaiorMarcacao().datahora <= marcacaoIntervaloEntrada.datahora ).Select(x=>x.DiferencaEmMinutos()).Sum();
                    horasTrabalhadasPer2 = pares.Where(x=>x.GetMenorMarcacao().datahora > marcacaoIntervaloEntrada.datahora).Select(x=>x.DiferencaEmMinutos()).Sum();

                    if(horario.NDescontarIntervalo)
                    {
                        var diffIntervalos = (int)(marcacaoIntervaloSaida.datahora - marcacaoIntervaloEntrada.datahora).TotalMinutes;

                        if((diffIntervalos % 2) != 0)
                        {
                            horasTrabalhadasPer2 += 1;
                            diffIntervalos -= 1;
                        }

                        horasTrabalhadasPer1 += diffIntervalos / 2;
                        horasTrabalhadasPer2 += diffIntervalos / 2;
                    }
            }

                ponto.HoraPagPer1 = Calculadora.MinutosparaHora(horasTrabalhadasPer1);
                ponto.HoraPagPer2 = Calculadora.MinutosparaHora(horasTrabalhadasPer2);

                return ponto;
            });
        }

        private async Task<Ponto> CalculaFaltas(IList<HelperPares> pares, HorarioDataHora horarioDataHora, Horarios horario, Ponto ponto, List<Marcacoes> marcacoes, List<string> marcacoesPerfeitas, Marcacoes marcacaoIntervalo, Calendario calendario)
        {   
            return await Task<Ponto>.Run(() => 
            {
                int faltasPer1 = 0;
                int faltasPer2 = 0;

                int tryparse;

            if(int.TryParse(calendario.ReferenciaSemHorario, out tryparse))
                {
                    pares = pares.Where(x=>x.GetMenorMarcacao().datahora >= horarioDataHora.DatasHorario[horario.Entrada]).OrderBy(x=>x.GetMenorMarcacao().datahora).ToList();
                    var paresnulos = pares.Where(x=> x.IsParNulo()).OrderBy(x=>x.GetMenorMarcacao().datahora);


                    foreach (var par in paresnulos)
                    {
                        var parvetor = par;

                        while (parvetor != null && parvetor.IsParNulo())
                        {
                            parvetor = pares.PegaProximoPar(parvetor);
                        }

                        int diff = (int)(par.GetMaiorMarcacao().datahora - parvetor.GetMenorMarcacao().datahora).TotalMinutes;
                        
                        if(par.GetMaiorMarcacao().datahora <= marcacaoIntervalo.datahora) 
                            faltasPer1 += diff;
                        else faltasPer2 += diff;
                    }

                    if(!pares.Any(x=> x.GetMenorMarcacao().datahora <= marcacaoIntervalo.datahora))
                        faltasPer1 = (int)(horarioDataHora.DatasHorario[horario.EntradaIntervalo] - horarioDataHora.DatasHorario[horario.Entrada]).TotalMinutes;

                    if(!pares.Any(x=> x.GetMaiorMarcacao().datahora > marcacaoIntervalo.datahora))
                        faltasPer2 = (int)(horarioDataHora.DatasHorario[horario.EntradaIntervalo] - horarioDataHora.DatasHorario[horario.Entrada]).TotalMinutes;
                }

                ponto.FaltaDesPer1 = Calculadora.MinutosparaHora(faltasPer1);
                ponto.FaltaDesPer2 = Calculadora.MinutosparaHora(faltasPer2);

                return ponto;
            });
        }

        private async Task<Ponto> CalculaHorasExtra(IList<HelperPares> pares, HorarioDataHora horarioDataHora, Horarios horario, Ponto ponto, Funcionarios funcionario, List<Marcacoes> marcacoes, List<string> marcacoesPerfeitas, Marcacoes marcacaoIntervaloEntrada, Marcacoes marcacaoIntervaloSaida)
        {
            return await Task<Ponto>.Run(() => 
            {

                var dataini = horarioDataHora.DatasHorario[horario.Entrada];
                var datafim = horarioDataHora.DatasHorario[horario.Saida];

                var paresExtra = pares.Where(x=> !x.IsParNulo()).Where(x=> x.GetMenorMarcacao().datahora < dataini || x.GetMaiorMarcacao().datahora > datafim || (x.GetMenorMarcacao().datahora >= marcacaoIntervaloEntrada.datahora && x.GetMaiorMarcacao().datahora <= marcacaoIntervaloSaida.datahora));
                
                int extraPer1 = paresExtra.Where(x=> x.GetMenorMarcacao().datahora < dataini).Sum(x=> x.DiferencaEmMinutosHoraMaxima(horarioDataHora.DatasHorario[horario.Entrada]));
                int extraPer2 = paresExtra.Where(x=> x.GetMaiorMarcacao().datahora > datafim).Sum(x=> x.DiferencaEmMinutosHoraMinima(horarioDataHora.DatasHorario[horario.Saida]));
                int extraInter = paresExtra.Where(x=> x.GetMenorMarcacao().datahora >= marcacaoIntervaloEntrada.datahora && x.GetMaiorMarcacao().datahora <= marcacaoIntervaloSaida.datahora).Sum(x=> x.DiferencaEmMinutos());

                if((extraInter + extraPer2 + extraPer1) < Calculadora.HorasparaMinuto(funcionario.Parametro.ExtraJornada)) 
                    extraInter = extraPer2 = extraPer1 = 0;

                extraPer1 = extraPer1 < Calculadora.HorasparaMinuto(funcionario.Parametro.ExtraTotal1P) ? 0 : extraPer1;
                extraPer2 = extraPer2 < Calculadora.HorasparaMinuto(funcionario.Parametro.ExtraTotal2P) ? 0 : extraPer2;
                extraInter = extraInter < Calculadora.HorasparaMinuto(funcionario.Parametro.ExtraTotalIntervalo) ? 0 : extraInter;


                ponto.PagExtrapPer1 = Calculadora.MinutosparaHora(extraPer1);
                ponto.PagExtrapPer2 = Calculadora.MinutosparaHora(extraPer2);
                ponto.PagExtrapIntervalo = Calculadora.MinutosparaHora(extraInter);

                return ponto;
            });
        }

        private async Task<Ponto> CalculaAdicionalNoturno(IList<HelperPares> pares, HorarioDataHora horarioDataHora, Horarios horario, Ponto ponto, Funcionarios funcionario, Marcacoes marcacaoIntervaloEntrada, Marcacoes marcacaoIntervaloSaida)
        {
            return await Task<Ponto>.Run(() => 
            {
                DateTime inicioAdicional, fimAdicional;

                inicioAdicional = Calculadora.HoraEmDateTime(funcionario.Parametro.AdicionalNoturnoInicio, ponto.Data.AddDays(VerificaOutroDia(horario.Entrada, funcionario.Parametro.AdicionalNoturnoInicio) ? 1 : 0));
                fimAdicional = Calculadora.HoraEmDateTime(funcionario.Parametro.AdicionalNoturnoFim, ponto.Data.AddDays(VerificaOutroDia(horario.Entrada, funcionario.Parametro.AdicionalNoturnoFim) ? 1 : 0));

                var paresAdicionalNoturnoTotais = pares.Where(x=> !x.IsParNulo() && (x.GetMenorMarcacao().datahora >= inicioAdicional && (funcionario.Parametro.AdcnFinaldoexpediente || x.GetMaiorMarcacao().datahora <= fimAdicional)));

                var paresExtraAdicional = paresAdicionalNoturnoTotais.Where(x=> x.GetMenorMarcacao().datahora < horarioDataHora.DatasHorario[horario.Entrada] || x.GetMaiorMarcacao().datahora > horarioDataHora.DatasHorario[horario.Saida] || (x.GetMenorMarcacao().datahora >= marcacaoIntervaloEntrada.datahora && x.GetMaiorMarcacao().datahora <= marcacaoIntervaloSaida.datahora));
                
                var extraAdicionalPer1 = paresExtraAdicional.Where(x=> x.GetMaiorMarcacao().datahora <= marcacaoIntervaloEntrada.datahora).Sum(x=>x.DiferencaEmMinutosHoraMaxima(horarioDataHora.DatasHorario[horario.Entrada]));

                var extraAdicionalPer2 = paresExtraAdicional.Where(x=> x.GetMenorMarcacao().datahora >= marcacaoIntervaloSaida.datahora).Sum(x=> x.DiferencaEmMinutosHoraMinima(horarioDataHora.DatasHorario[horario.Saida]));

                var extraAdicionalInter = paresExtraAdicional.Where(x=> x.GetMenorMarcacao().datahora >= marcacaoIntervaloEntrada.datahora && x.GetMaiorMarcacao().datahora <= marcacaoIntervaloSaida.datahora).Sum(x=>x.DiferencaEmMinutos());

                var AdicionalPer1 = paresAdicionalNoturnoTotais.Where(x=> x.GetMaiorMarcacao().datahora >= horarioDataHora.DatasHorario[horario.Entrada] && x.GetMaiorMarcacao().datahora <= marcacaoIntervaloEntrada.datahora).Sum(x=>x.DiferencaEmMinutos());
                
                var AdicionalPer2 = paresAdicionalNoturnoTotais.Where(x=> x.GetMaiorMarcacao().datahora >= marcacaoIntervaloSaida.datahora && x.GetMaiorMarcacao().datahora <= horarioDataHora.DatasHorario[horario.Saida]).Sum(x=>x.DiferencaEmMinutos());
                
                if((extraAdicionalInter + extraAdicionalPer1 + extraAdicionalPer2) < Calculadora.HorasparaMinuto(funcionario.Parametro.ExtraJornada))
                    extraAdicionalInter = extraAdicionalPer1 = extraAdicionalPer2 = 0;


                ponto.AdicionalPagPer1 = Calculadora.MinutosparaHora(AdicionalPer1);
                ponto.AdicionalPagPer2 = Calculadora.MinutosparaHora(AdicionalPer2);
                ponto.ExtraAdicPagPer1 = Calculadora.MinutosparaHora(extraAdicionalPer1);
                ponto.ExtraAdicPagInter = Calculadora.MinutosparaHora(extraAdicionalInter);
                ponto.ExtraAdicPagPer2 = Calculadora.MinutosparaHora(extraAdicionalPer2);
                
                return ponto;
            });
        }

        private async Task CalculaDsr(DateTime dataini, DateTime datafim, Funcionarios funcionario)
        {
            var calendario = CalendarioFuncionario.FirstOrDefault(x=> CompareFunc(x.Funcionario, funcionario));

            var de_paraDias = new Dictionary<Escalas.viradaSemana, DayOfWeek>
            {
                { Escalas.viradaSemana.Segunda, DayOfWeek.Monday },
                { Escalas.viradaSemana.Terca, DayOfWeek.Tuesday },
                { Escalas.viradaSemana.Quarta, DayOfWeek.Wednesday },
                { Escalas.viradaSemana.Quinta, DayOfWeek.Thursday },
                { Escalas.viradaSemana.Sexta, DayOfWeek.Friday },
                { Escalas.viradaSemana.Sabado, DayOfWeek.Saturday },
                { Escalas.viradaSemana.Domingo, DayOfWeek.Sunday }
            };

            var viradaSemana = de_paraDias[funcionario.Escala.ViradaSemana];

            while((int)dataini.DayOfWeek != (viradaSemana == DayOfWeek.Saturday ? (int)DayOfWeek.Sunday : ((int)viradaSemana) + 1))
            {
                dataini = dataini.AddDays(-1);
            }

            while(datafim.DayOfWeek != viradaSemana)
            {
                datafim = datafim.AddDays(1);
            }

            int semanas = (int)(datafim - dataini).TotalDays / 6;

            var repositoryPonto = _repositoryPonto.Clone();

            var pontos = await repositoryPonto.GetAll(x=> (x.Data >= dataini && x.Data <= datafim) && x.IDFuncionario == funcionario.IDFuncionario && x.IDEmpresa == funcionario.IDEmpresa).ToListAsync();

            var SemanasPonto = new List<SemanaPonto>();

            DateTime datavetor = dataini;

            for (int i = 0; i < semanas; i++)
            {
                var semana = new SemanaPonto();
                var dataInicioSemana = datavetor;
                var dataFimSemana = dataInicioSemana.AddDays(7);

                var pontosSemana = pontos.Where(x=>x.Data >= dataInicioSemana && x.Data <= dataFimSemana);

                foreach (var ponto in pontosSemana)
                {
                    var dia = new DiaPonto(ponto, funcionario.Parametro, calendario.Calendario.FirstOrDefault(x=> x.Data == ponto.Data));
                    await dia.Calcula();
                    semana.DiasdaSemana.Add(dia);
                }

                datavetor = dataFimSemana.AddDays(1);
                SemanasPonto.Add(semana);
            }

            foreach (var semana in SemanasPonto)
            {
                int atrasoseSaidasMin = 0;
                int faltasMin = 0;
                int diasdeOcorrenciaFalta = 0;
                int diasdeOcorrenciaAtraso = 0;

                foreach (var diaSemana in semana.DiasdaSemana)
                {
                    if(!diaSemana.IsDsr)
                        if(diaSemana.IsFaltaouAtraso)
                        {
                            atrasoseSaidasMin += diaSemana.QtdAtrasoseSaidasEmMinutos;
                            faltasMin += diaSemana.QtdFaltasEmMinutos;
                            
                            if(diaSemana.QtdFaltasEmMinutos > 0)
                                diasdeOcorrenciaFalta++;

                            if(diaSemana.QtdAtrasoseSaidasEmMinutos > 0)
                                diasdeOcorrenciaAtraso++;
                        }

                    
                    var ultrapassouLimiteAtraso = Calculadora.HorasparaMinuto(diaSemana.parametro.OcorrenciaSemanalDsr) <= atrasoseSaidasMin;
                    var ultrapassouLimiteAtrasoFalta = faltasMin > 0 || ultrapassouLimiteAtraso;


                    if(diaSemana.IsDsr && !diaSemana.PontoReferencia.Tratado && ultrapassouLimiteAtrasoFalta)
                    {
                        var dsrProporcionalHoras = funcionario.Parametro.DsrProporcionalHoras.HasValue 
                                                && diaSemana.parametro.DsrProporcionalHoras.Value;
                        var dsrProporcionalDias = funcionario.Parametro.DsrProporcionalHoras.HasValue 
                                                && !diaSemana.parametro.DsrProporcionalHoras.Value;

                        if(dsrProporcionalDias || dsrProporcionalHoras)
                            if(dsrProporcionalHoras)
                                diaSemana.PontoReferencia.DsrDescontado = Calculadora.MinutosparaHora(((ultrapassouLimiteAtraso ? atrasoseSaidasMin : 0) + faltasMin));
                            else    
                                diaSemana.PontoReferencia.DsrDescontado = Calculadora.MinutosparaHora((Calculadora.HorasparaMinuto(diaSemana.ValorDsr) / semana.DiasdaSemana.Count(x=> x.IsDiaTrabalho)) * (ultrapassouLimiteAtraso ? (diasdeOcorrenciaAtraso + diasdeOcorrenciaFalta) : diasdeOcorrenciaFalta));
                        else    
                            diaSemana.PontoReferencia.DsrDescontado = diaSemana.ValorDsr;

                        if(!funcionario.Parametro.DescontarDsrSemana)
                            atrasoseSaidasMin = faltasMin = diasdeOcorrenciaFalta = 0;
                    }

                    if(diaSemana.IsDsr)
                    {
                        diaSemana.PontoReferencia.DsrDescontado = Calculadora.HorasparaMinuto(diaSemana.PontoReferencia.DsrDescontado) > Calculadora.HorasparaMinuto(diaSemana.ValorDsr) ? diaSemana.ValorDsr : diaSemana.PontoReferencia.DsrDescontado;
                        diaSemana.PontoReferencia.DsrPago = Calculadora.MinutosparaHora(Calculadora.HorasparaMinuto(diaSemana.ValorDsr) 
                                                         - Calculadora.HorasparaMinuto(diaSemana.PontoReferencia.DsrDescontado));

                    }
                                                            
                }

                if(funcionario.Parametro.DescDsrAnterioraFalta.HasValue 
                && funcionario.Parametro.DescDsrAnterioraFalta.Value) 
                {
                    var ultimoDsr = semana.PegaUltimoDsr();
                    semana.MudarTodososDsrPara(ultimoDsr.PontoReferencia.DsrDescontado, ultimoDsr.PontoReferencia.DsrPago);
                }  
            }

            foreach (var semana in SemanasPonto)
                foreach (var diaSemana in semana.DiasdaSemana)
                    repositoryPonto.Update(diaSemana.PontoReferencia); 
        }



        private async Task<IList<HelperPares>> EncaixaMarcacoesCarga(IList<Marcacoes> marcacoes,
                                            Horarios horario,
                                            Ponto ponto,
                                            Funcionarios funcionario)
        {
            var result = await Task<IList<HelperPares>>.Run(() =>
            {
                marcacoes = marcacoes
                                .OrderBy(x => x.datahora).ToList();

                List<IList<Marcacoes>> vetores =
                    new List<IList<Marcacoes>>();
                List<Marcacoes> vetor = new List<Marcacoes>();

                for (int i = 0; i < marcacoes.Count; i++)
                {
                    vetor.Add(marcacoes[i]);

                    if (vetor.Count == 2)
                    {
                        vetores.Add(vetor);
                        vetor = new List<Marcacoes>();
                    }

                    if (marcacoes.Count - 1 == i && vetor.Count > 0)
                    {
                        vetor.Add(null);
                        vetores.Add(vetor);
                    }
                }

                List<HelperPares> helpers = new List<HelperPares>();

                for (int i = 0; i < marcacoes.Count; i += 2)
                {
                    var helper = i == marcacoes.Count - 1 ? new HelperPares(marcacoes[i], null, horario, ponto) : new HelperPares(marcacoes[i], marcacoes[i + 1], horario, ponto);

                    helpers.Add(helper);
                }

                return helpers;
            });

            return result;
        }

        public async Task IniciaProcessamento(DateTime dataini,
            DateTime datafim,
            IList<Funcionarios> funcionarios,
            TiposProcessamento tipoProcessamento,
            Processamento processamento)
        {
            dataini = dataini.Date;
            datafim = datafim.Date;
            FuncionarioProcessados = funcionarios;

            Task limpaTabelas = LimpaTabelas(dataini,
                                                datafim,
                                                funcionarios,
                                                tipoProcessamento);

            IRepository<ApontamentoSujo> repApontamenotoSujo = _repositoryApontamentoSujo.Clone();
            IRepository<Marcacoes> repMarcacoes = _repositoryMarcacoes.Clone();
            IRepository<Ponto> repPonto = _repositoryPonto.Clone();
            IRepository<FilaProcesso> repFila = _repositoryFilaProcesso.Clone();
            IRepository<PontoPares> repPontoPares = _repositoryPontoPares.Clone();
            IRepository<FeriadosGerais> repFeriadosGerais = _repositoryFeriadosGerais.Clone();


            FeriadosGerais =  await repFeriadosGerais.GetAll().ToListAsync();
            Task montacalendario = MontaCalendarioFuncionario(funcionarios, dataini, datafim);

            FilaProcesso fila = new FilaProcesso
            {
                dataInicio = DateTime.Now,
                IDUsuario = 1,
                qtdLinhas = 100,
                linhasProcessadas = 1,
                linhasErro = 0
            };

            fila = repFila.Insert(fila);
            
            using (limpaTabelas)
                await limpaTabelas;

            Task<List<Ponto>> taskpontosnabase = repPonto.GetAll().Where(x => (funcionarios
                                .Any(y => y.IDFuncionario == x.IDFuncionario && y.IDEmpresa == x.IDEmpresa))
                                && (dataini <= x.Data && x.Data <= datafim)).ToListAsync();

            // Task<ApontamentoSujo[]> taskapontamentosSujo = repApontamenotoSujo
            //                     .GetAll().Where(x => (funcionarios.Any(y => x.pis == y.Pis))
            //                     && dataini.Date <= x.data
            //                     && x.data <= datafim.Date).ToArrayAsync();

            Task<List<Marcacoes>> taskmarcacoesnabase = repMarcacoes
                                .GetAll().Where(x => (funcionarios
                                .Any(y => y.IDFuncionario == x.IDFuncionario && y.IDEmpresa == x.IDEmpresa))
                                && (dataini.AddDays(-2) <= x.datahora.Date && x.datahora.Date <= datafim.AddDays(2))).ToListAsync();

            using(taskpontosnabase)
                PontosnaBase = await taskpontosnabase;

            using(taskmarcacoesnabase)
                MarcacoesnaBase = await taskmarcacoesnabase;

            var apontamentosSujo =  new ApontamentoSujo[0];//await taskapontamentosSujo;
            //taskapontamentosSujo.Dispose();

            apontamentosSujo = apontamentosSujo
                                .Where(x => !MarcacoesnaBase
                                .Any(y => y.chaveUniqueMarc == x.chaveUniqueMarc)).ToArray();

            Marcacoes[] marcacoes = apontamentosSujo.Select((x) =>
            {
                Funcionarios funcionario = funcionarios.First(y => y.Pis == x.pis);

                return new Marcacoes
                {
                    chaveUniqueMarc = x.chaveUniqueMarc,
                    datahora = x.data,
                    nfr = x.nfr,
                    nsr = x.nsr,
                    IDEmpresa = funcionario.IDEmpresa,
                    IDFuncionario = funcionario.IDFuncionario
                };

            }).Where(x => !MarcacoesnaBase.Any(y => y.chaveUniqueMarc == x.chaveUniqueMarc)).ToArray();

            if (marcacoes.Length > 0)
            {
                repMarcacoes.Insert(marcacoes);
                MarcacoesnaBase.AddRange(marcacoes);
            }

            using (montacalendario)
                await montacalendario;

            List<Ponto> pontosmapeados = new List<Ponto>();

            for (int i = 0; i < CalendarioFuncionario.Count; i++)
            {
                pontosmapeados.AddRange(
                CalendarioFuncionario[i].Calendario
                .Where(x => x.Data.Date >= dataini.Date
                && x.Data.Date <= datafim.Date)
                .Select(x => new Ponto
                {
                    Data = x.Data.Date,
                    IDFilaProcesso = fila.IDFilaProcesso,
                    IDHorario = x.Horario?.IDHorario,
                    IDFuncionario = CalendarioFuncionario[i].Funcionario.IDFuncionario,
                    IDEmpresa = CalendarioFuncionario[i].Funcionario.IDEmpresa,
                    Tratado = false,
                    ReferenciaSemHorario = x.Horario == null ? x.ReferenciaSemHorario : null
                }).ToList());
            }

            pontosmapeados = pontosmapeados
                                .Where(x => !PontosnaBase
                                .Any(y => y.Data.Date == x.Data.Date && y.IDFuncionario == x.IDFuncionario
                                && y.IDEmpresa == x.IDEmpresa))
                                .ToList();

            if (pontosmapeados.Count > 0)
            {
                repPonto.Insert(pontosmapeados);
                PontosnaBase.AddRange(pontosmapeados);
            }

            var encaixedeMarcacoes = new List<Task>();

            for (int i = 0; i < PontosnaBase.Count; i++)
            {
                var InformacoesPonto = PegaMarcacoesPonto(PontosnaBase[i]);

                if (!PontosnaBase[i].Tratado)
                    if(InformacoesPonto.Horario.Tipo == Horarios.tipo.Carga)
                    {
                        await EncaixaMarcacoesCarga(InformacoesPonto.Marcacoes,
                                                    InformacoesPonto.Horario,
                                                    InformacoesPonto.Ponto,
                                                    InformacoesPonto.Funcionario);
                    }else{
                         await EncaixaMarcacoes(InformacoesPonto.Marcacoes,
                                                InformacoesPonto.Horario,
                                                InformacoesPonto.Ponto,
                                                InformacoesPonto.Funcionario);
                    }  
            }

            foreach (var encaixe in encaixedeMarcacoes)
                await encaixe;

            var taksDsr = new List<Task>();

            for (int i = 0; i < funcionarios.Count; i++)
            {
                await CalculaDsr(dataini, datafim, funcionarios[i]);
                //taksDsr.Add(CalculaDsr(dataini, datafim, funcionarios[i]));
            }

            foreach (var taskdsr in taksDsr)
                await taskdsr;
        }

        private async Task LimpaTabelas(DateTime dataini,
            DateTime datafim,
            IList<Funcionarios> funcionarios,
            TiposProcessamento tipoProcessamento)
        {
            IRepository<Ponto> repPonto = _repositoryPonto.Clone();
            IRepository<Marcacoes> repMarcs = _repositoryMarcacoes.Clone();
            IRepository<PontoPares> repPontoPares = _repositoryPontoPares.Clone();

            Expression<Func<Ponto, bool>> expression = null;
            bool apagamarcacoes = false;

            Task LimpaPontos = new Task(() =>
            {
                IList<Ponto> listPontosNormal = repPonto.GetAll().Include(x => x.Pares)
                        .Include("Pares.EntradaOriginal")
                        .Include("Pares.SaidaOriginal")
                        .Where(expression).ToArray();

                List<Marcacoes> marcacoes = new List<Marcacoes>();
                List<PontoPares> pontopares = new List<PontoPares>();
                for (int i = 0; i < listPontosNormal.Count; i++)
                {
                    if (listPontosNormal[i].Pares.Count() > 0)
                    {
                        pontopares.AddRange(listPontosNormal[i].Pares);
                        marcacoes.AddRange(listPontosNormal[i].Pares.Select(x => x.EntradaOriginal));
                        marcacoes.AddRange(listPontosNormal[i].Pares.Select(x => x.SaidaOriginal));
                    }

                    listPontosNormal[i].Pares = null;
                }

                try
                {
                    {
                        UnitOfWork.BeginTransactionAsync(repPontoPares).Wait();
                        repPontoPares.Delete(pontopares);
                        UnitOfWork.CommitTransactionAsync(repPontoPares).Wait();
                    }

                    {
                        UnitOfWork.BeginTransactionAsync(repPonto).Wait();
                        repPonto.Delete(listPontosNormal);
                        UnitOfWork.CommitTransactionAsync(repPonto).Wait();
                    }

                    {

                        if (apagamarcacoes)
                        {
                            UnitOfWork.BeginTransactionAsync(repMarcs).Wait();
                            marcacoes.RemoveAll(x => x == null);
                            repMarcs.Delete(marcacoes);
                            UnitOfWork.CommitTransactionAsync(repMarcs).Wait();
                        }
                    }
                }
                catch (System.Exception ex)
                {

                    UnitOfWork.RollbackTransactionAsync(repPonto).Wait();


                    UnitOfWork.RollbackTransactionAsync(repPontoPares).Wait();


                    UnitOfWork.RollbackTransactionAsync(repMarcs).Wait();


                    throw ex;
                }
            });

            switch (tipoProcessamento)
            {
                case TiposProcessamento.Normal:

                    expression = (x => (dataini <= x.Data && x.Data <= datafim)
                    && (funcionarios.Any(y => y.IDFuncionario == x.IDFuncionario && y.IDEmpresa == x.IDEmpresa))
                    && !x.Tratado);
                    break;
                case TiposProcessamento.Recalcular:

                    expression = (x => (dataini <= x.Data && x.Data <= datafim)
                    && (funcionarios.Any(y => y.IDFuncionario == x.IDFuncionario && y.IDEmpresa == x.IDEmpresa)));
                    break;
                case TiposProcessamento.Reanalizar:
                    expression = (x => (dataini <= x.Data && x.Data <= datafim)
                    && (funcionarios.Any(y => y.IDFuncionario == x.IDFuncionario && y.IDEmpresa == x.IDEmpresa)));

                    apagamarcacoes = true;
                    break;
            }

            LimpaPontos.Start();
            using(LimpaPontos)
                await LimpaPontos;
        }
    }
}
