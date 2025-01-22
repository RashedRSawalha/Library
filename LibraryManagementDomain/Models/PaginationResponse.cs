namespace LibraryManagementDomain.Models
{
    public class PaginationResponse<T>
    {
        public int TotalRecords { get; set; } // Total number of records
        public int PageIndex { get; set; } // Current page (1-based index)
        public int PageSize { get; set; } // Number of items per page
        public List<T> Data { get; set; } // Paginated data
    }

}
