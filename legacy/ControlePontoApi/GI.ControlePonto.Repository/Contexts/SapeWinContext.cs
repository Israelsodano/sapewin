using GI.ControlePonto.Domain.Entities;
using GI.ControlePonto.Domain.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GI.ControlePonto.Repository.Contexts
{
    public class SapeWinContext : ContextBase<SapeWinContext>
    {
        public SapeWinContext(DbContextOptionsBuilder<SapeWinContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PermissoesdeDepartamentos>().HasKey(x => new { x.IDUsuario, x.IDEmpresa, x.IDDepartamento });
            modelBuilder.Entity<PermissoesdeFuncionarios>().HasKey(x => new { x.IDUsuario, x.IDEmpresa, x.IDFuncionario });
            modelBuilder.Entity<PermissoesdeSetores>().HasKey(x => new { x.IDUsuario, x.IDEmpresa, x.IDSetor });            
            modelBuilder.Entity<PermissoesdeEmpresas>().HasKey(x => new { x.IDUsuario, x.IDEmpresa });            
            modelBuilder.Entity<PermissoesdeTelas>().HasKey(x => new { x.IDUsuario, x.IDEmpresa, x.IDFuncaoTela });
            modelBuilder.Entity<Cargos>().HasKey(x => new { x.IDCargo });
            modelBuilder.Entity<Departamentos>().HasKey(x => new { x.IDDepartamento, x.IDEmpresa });
            modelBuilder.Entity<Empresas>().HasKey(x => new { x.IDEmpresa });
            modelBuilder.Entity<Funcionarios>().HasKey(x => new { x.IDFuncionario, x.IDEmpresa });
            modelBuilder.Entity<Funcoes>().HasKey(x => new { x.IDFuncao });
            modelBuilder.Entity<FuncoesdeTelas>().HasKey(x => new { x.IDFuncaoTela });
            modelBuilder.Entity<Setores>().HasKey(x => new { x.IDSetor, x.IDEmpresa });
            modelBuilder.Entity<Telas>().HasKey(x => new { x.IDTela });
            modelBuilder.Entity<FeriadosGerais>().HasKey(x => new { x.IDFeriado });
            modelBuilder.Entity<GrupodeFeriados>().HasKey(x => new { x.IDFeriado });
            modelBuilder.Entity<FeriadosEspecificos>().HasKey(x=> new { x.IDFeriado, x.Dia, x.Mes, x.Ano});
            modelBuilder.Entity<MotivosdeAbono>().HasKey(x => new { x.IDEmpresa, x.Abreviacao });
            modelBuilder.Entity<Horarios>().HasKey(x => new { x.IDHorario });
            modelBuilder.Entity<Escalas>().HasKey(x => new { x.IDEmpresa, x.IDEscala });
            modelBuilder.Entity<EscalasHorarios>().HasKey(x => new { x.IDEmpresa, x.IDEscala, x.IDHorario, x.Ordem });
            modelBuilder.Entity<IntervalosAuxiliares>().HasKey(x => new { x.IDEmpresa, x.IDHorario, x.IDIntervalo });
            modelBuilder.Entity<Parametros>().HasKey(x => new {  x.IDParametro, x.IDEmpresa });
            modelBuilder.Entity<EscalonamentodeHoraExtra>().HasKey(x => new { x.IDParametro, x.IDEmpresa, x.Tipo, x.Horas, x.Porcentagem });
            modelBuilder.Entity<CartaoProximidade>().HasKey(x => new { x.IDCartao, x.IDFuncionario, x.IDEmpresa });
            modelBuilder.Entity<Mensagem>().HasKey(x => new { x.IDMensagem });
            modelBuilder.Entity<MensagensFuncionarios>().HasKey(x => new { x.IDMensagem, x.IDFuncionario, x.IDEmpresa });
            modelBuilder.Entity<Afastamentos>().HasKey(x => new { x.IDAfastamento, x.IDFuncionario, x.IDEmpresa, x.Abreviacao, x.DataInicial });
            modelBuilder.Entity<Folgas>().HasKey(x => new { x.IDFolga, x.IDFuncionario, x.Data, x.IDEmpresa });
            modelBuilder.Entity<HorariosOcasionais>().HasKey(x => new { x.IDHorario, x.IDFuncionario, x.Data, x.IDHorarioOcasional, x.IDEmpresa });
            modelBuilder.Entity<LogSistema>().HasKey(x => new { x.IDLog });
            modelBuilder.Entity<Ponto>().HasKey(x=>x.IDPonto);
            modelBuilder.Entity<Marcacoes>().HasKey(x=>x.IDMarcacao);
            modelBuilder.Entity<PontoPares>().HasKey(x=>x.IDPontoPares);
            modelBuilder.Entity<FilaProcesso>().HasKey(x=>x.IDFilaProcesso);

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<LogSistema> LogSistema { get; set; }

        public virtual DbSet<HorariosOcasionais> HorariosOcasionais { get; set; }

        public virtual DbSet<Folgas> Folgas { get; set; }

        public virtual DbSet<Afastamentos> Afastamentos { get; set; }

        public virtual DbSet<MensagensFuncionarios> MensagensFuncionarios { get; set; }

        public virtual DbSet<Mensagem> Mensagem { get; set; }

        public virtual DbSet<CartaoProximidade> CartaoProximidade { get; set; }

        public virtual DbSet<EscalonamentodeHoraExtra> EscalonamentodeHoraExtra { get; set; }

        public virtual DbSet<Parametros> Parametros { get; set; }

        public virtual DbSet<IntervalosAuxiliares> IntervalosAuxiliares { get; set; }

        public virtual DbSet<EscalasHorarios> EscalasHorarios { get; set; }

        public virtual DbSet<Escalas> Escalas { get; set; }

        public virtual DbSet<Horarios> Horarios { get; set; }

        public virtual DbSet<MotivosdeAbono> MotivosdeAbono { get; set; }

        public virtual DbSet<Cargos> Cargos { get; set; }

        public virtual DbSet<Departamentos> Departamentos { get; set; }

        public virtual DbSet<Empresas> Empresas { get; set; }

        public virtual DbSet<Funcionarios> Funcionarios { get; set; }

        public virtual DbSet<Funcoes> Funcoes { get; set; }

        public virtual DbSet<FuncoesdeTelas> FuncoesdeTelas { get; set; }

        public virtual DbSet<PermissoesdeDepartamentos> PermissoesdeDepartamentos { get; set; }

        public virtual DbSet<PermissoesdeEmpresas> PermissoesdeEmpresas { get; set; }

        public virtual DbSet<PermissoesdeFuncionarios> PermissoesdeFuncionarios { get; set; }

        public virtual DbSet<PermissoesdeSetores> PermissoesdeSetores { get; set; }

        public virtual DbSet<PermissoesdeTelas> PermissoesdeTelas { get; set; }

        public virtual DbSet<Setores> Setores { get; set; }

        public virtual DbSet<Telas> Telas { get; set; }

        public virtual DbSet<FeriadosGerais> FeriadosGerais { get; set; }

        public virtual DbSet<GrupodeFeriados> GrupodeFeriados { get; set; }

        public virtual DbSet<FeriadosEspecificos> FeriadosEspecificos { get; set; }

        public virtual DbSet<FilaProcesso> FilaProcesso { get; set; }
    }
}