namespace E_Commerce.Core.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"لم يتم العثور على '{name}' بالمعرف '{key}'.") { }

        public NotFoundException(string message) : base(message) { }
    }

    public class ValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public ValidationException(IEnumerable<string> enumerable)
            : base("حدثت واحدة أو أكثر من أخطاء التحقق.")
        {
            Errors = new Dictionary<string, string[]>
            {
                ["Errors"] = enumerable.ToArray()
            };
        }

        public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }

        public ValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();

        }

        public ValidationException()
            : base("حدثت واحدة أو أكثر من أخطاء التحقق.")
        {
            Errors = new Dictionary<string, string[]>();
        }
        
        
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "غير مصرح.") : base(message) { }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "تم رفض الوصول.") : base(message) { }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    } 

    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }

}
