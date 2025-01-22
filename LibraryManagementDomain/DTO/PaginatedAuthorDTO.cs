namespace LibraryManagementDomain.DTO
{
    public class PaginatedAuthorDTO
    {
        public int TotalRecords { get; set; } // Total number of records
        public int PageIndex { get; set; } // Current page index
        public int PageSize { get; set; } // Number of items per page
        public List<AuthorDTO> Authors { get; set; } // Paginated data
    }

}
