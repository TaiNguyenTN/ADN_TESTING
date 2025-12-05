using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string? Message { get; private set; }
        public List<string> Errors { get; private set; } = new();
        public static Result Success(string? message = null)
            => new() { IsSuccess = true, Message = message };

        public static Result Failure(params string[] errors)
            => new() { IsSuccess = false, Errors = errors.ToList() };
    }
}
