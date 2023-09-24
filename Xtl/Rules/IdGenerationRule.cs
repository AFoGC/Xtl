using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Xtl.Rules
{
    internal class IdGenerationRule
    {
        private Func<int, int?> _idGenerator;

        public IdGenerationRule()
        {
            _idGenerator = (counter) =>
            {
                return null;
            };
        }

        public void SetIdGenerationExpression(Func<int, int?> idGenerationExpression)
        {
            _idGenerator = idGenerationExpression;
        }

        public int? GenerateId(int counter)
        {
            return _idGenerator(counter);
        }
    }
}
