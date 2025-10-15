using System.Collections;

namespace Shared.ResultPattern
{
    public class Errors : IEnumerable<Error>
    {
        private readonly List<Error> _errors;

        public Errors(IEnumerable<Error> errors)
        {
            _errors = [.. errors]; // полная копия
        }

        public IEnumerator<Error> GetEnumerator()
        {
            return _errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator Errors(Error[] errors) => new Errors(errors);

        public static implicit operator Errors(Error error) => new Errors([error]);
    }
}
