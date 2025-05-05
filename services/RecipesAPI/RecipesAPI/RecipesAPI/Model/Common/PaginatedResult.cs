namespace RecipesAPI.Model.Common
{
    public class PaginatedResult<T> where T : class
    {
        public T Data { get; set; }
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public int Page {  get; set; }
        public int PageSize { get; set; }
    }
}
