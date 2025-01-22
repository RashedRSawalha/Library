namespace LibraryManagementDomain.Models
{
    public class PaginationRequest
    {
        public int PageIndex {get; set;}
        public int PageSize {get; set;}
        public string ?Search { get; set; } // Optional search term
    }
}
