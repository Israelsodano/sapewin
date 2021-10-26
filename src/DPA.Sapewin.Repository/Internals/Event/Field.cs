using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DPA.Sapewin.Repository.Internals.Event
{
    public class Field
    {
        [Key]
        public Guid Id { get; set; }
        public Guid HeadEventId { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public HeadEvent HeadEvent { get; set; }

        public static IEnumerable<Field> BuildFields(Repository.Event e)
        {
            var properties = e?.GetType()
                               .GetProperties();

            for (int i = 0; i < properties?.Length; i++)
                yield return new Field
                {
                    Name = properties[i].Name,
                    Value = properties[i].GetValue(e)
                };
        }
    }
}