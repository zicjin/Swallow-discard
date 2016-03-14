using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Swallow.Entity {
    public class EntityValidator {
        public static bool TryValidate(object @object, string eclude_props, out string errors) {
            errors = string.Empty;
            var context = new ValidationContext(@object, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            bool fine = Validator.TryValidateObject(
                @object, context, results,
                validateAllProperties: true
            );

            if (!string.IsNullOrEmpty(eclude_props)) {
                IList<string> props = eclude_props.Split(',');
                results.RemoveAll(d => props.Contains(d.MemberNames.First()));
                if (results.Count == 0)
                    fine = true;
            }

            if (!fine)
                errors = string.Join("\n", Array.ConvertAll(results.ToArray(), i => i.ErrorMessage));
            return fine;
        }
    }
}
