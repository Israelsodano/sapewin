﻿using System;

namespace DPA.Sapewin.Domain.Entities
{
    public class HorariosOcasionais
    {
        public virtual int IDHorarioOcasional { get; set; }

        public virtual long IDFuncionario { get; set; }

        public virtual int IDHorario { get; set; }

        public virtual int IDEmpresa { get; set; }

        public virtual DateTime Data { get; set; }

        public virtual Employees Funcionario { get; set; }

        public virtual Horarios Horario { get; set; }
    }
}